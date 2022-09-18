using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using SixLabors.ImageSharp;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Processing;
using WinRT;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;

namespace MauiApp1;
public static class OcrPredictor
{
    public static OcrOutput Predict(Stream imageStream)
    {
        var img = CleanImage(imageStream);

        img.Save($"C:\\Temp\\{DateTime.Now.ToFileTime()}.gif");

        var predictionInput = CreatePredictionInput(img);



        return Predict(predictionInput);

    }

    public static Image<Rgba32> CleanImage(Stream imageStream)
    {
        var img = Image.Load<Rgba32>(imageStream);
        img.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new SixLabors.ImageSharp.Size(300, 300),
            Mode = SixLabors.ImageSharp.Processing.ResizeMode.BoxPad,
            PadColor = Color.Black,
        }));
        img.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new SixLabors.ImageSharp.Size(28, 28),
            Mode = SixLabors.ImageSharp.Processing.ResizeMode.Pad
        }));
        return img;
    }

    private static OcrInput CreatePredictionInput(Image<Rgba32> image)
    {
        var prediction = new List<float>();
        for (int i = 0; i < 28; i++)
        {
            for (int j = 0; j < 28; j++)
            {
                prediction.Add(image[i, j].B);
            }
        }
        return new OcrInput { PixelValues = prediction.ToArray() };
    }

    private static string MLNetModelPath = Path.GetFullPath(@"C:\Users\frte\source\repos\MauiApp1\OCRTrainer\bin\Debug\net6.0\ocr.zip");

    public static readonly Lazy<PredictionEngine<OcrInput, OcrOutput>> PredictEngine = new Lazy<PredictionEngine<OcrInput, OcrOutput>>(() => CreatePredictEngine(), true);

    /// <summary>
    /// Use this method to predict on <see cref="ModelInput"/>.
    /// </summary>
    /// <param name="input">model input.</param>
    /// <returns><seealso cref=" ModelOutput"/></returns>
    public static OcrOutput Predict(OcrInput input)
    {
        var predEngine = PredictEngine.Value;
        return predEngine.Predict(input);
    }

    private static PredictionEngine<OcrInput, OcrOutput> CreatePredictEngine()
    {
        var mlContext = new MLContext();
        ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
        return mlContext.Model.CreatePredictionEngine<OcrInput, OcrOutput>(mlModel);
    }
}

public class OcrInput
{
    public float Character { get; set; }

    [VectorType(784)]
    public float[] PixelValues { get; set; }
}

public class OcrOutput
{
    [ColumnName(@"Label")]
    public uint Label { get; set; }

    [ColumnName(@"PredictedLabel")]
    public uint PredictedLabel { get; set; }

    [ColumnName(@"Score")]
    public float[] Score { get; set; }
}