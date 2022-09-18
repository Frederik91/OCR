using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using System.Collections.ObjectModel;
using System.IO;

namespace MauiApp1.ViewModels;
public partial class MainPageViewModel : ObservableObject
{
    private int lineIndex = 0;
    private IEnumerator<string> lines;

    [ObservableProperty]
    private ObservableCollection<IDrawingLine> _lines = new();

    [ObservableProperty]
    private string _predictedCharacter;

    [ObservableProperty]
    private ImageSource _predictedImage = ImageSource.FromUri(new Uri("https://static.remove.bg/remove-bg-web/c4b29bf4b97131238fda6316e24c9b3606c18000/assets/start-1abfb4fe2980eabfbbaaa4365a0692539f7cd2725f324f904565a9a744f8e214.jpg"));

    public MainPageViewModel()
    {
        var _ = LetterRecognizer.PredictEngine.Value;
        lines = File.ReadLines(@"C:\Temp\A_Z Handwritten Data.csv").GetEnumerator();
        lines.MoveNext();
    }

    [RelayCommand]
    private async Task DrawingLineCompleted()
    {
        if (_lines.Count == 0)
        {
            return;
        }


        var imageStream = await DrawingView.GetImageStream(
                _lines,
                new Size(300, 300),
                Colors.Black);

        var image = OcrPredictor.CleanImage(imageStream);
        using MemoryStream ms = new();
        image.Save(ms, new GifEncoder());
        imageStream.CopyTo(ms);
        var result = LetterRecognizer.Predict(new LetterRecognizer.ModelInput
        {
            ImageSource = ms.ToArray()
        });
        //var result = OcrPredictor.Predict(imageStream);

        PredictedCharacter = result.PredictedLabel;
    }

    [RelayCommand]
    private void Predict()
    {
        var lineArray = lines.Current.Split(',');
        lines.MoveNext();

        var character = float.Parse(lineArray[0]);
        var values = lineArray[1..].Select(x => float.Parse(x)).ToArray();
        var prediction = OcrPredictor.Predict(new OcrInput { Character = character, PixelValues = values });

        PredictedCharacter = $"Char: {character}, Pred: {prediction.PredictedLabel}";
    }
}
