// See https://aka.ms/new-console-template for more information
using System.Drawing;
using System.Text;

var folder = $"C:\\Temp\\Handwriting\\Training\\";
for (int i = 0; i < 26; i++)
{
    var charNum = i + 65;
    Directory.CreateDirectory(folder + ((char)charNum).ToString().ToUpper());
}

var path = @"C:\Temp\A_Z Handwritten Data.csv";
var lines = File.ReadLines(path);
var count = 0;
foreach (var line in lines)
{
    var split = line.Split(",").Select(x => int.Parse(x)).ToArray();
    var charNum = split[0] + 65;
    var character = ((char)charNum).ToString().ToUpper();
    var imgValues = split[1..].Chunk(28).ToArray();

    var img = new Bitmap(28, 28);
    for (var h = 0; h < imgValues.Length; h++)
    {
        var row = imgValues[h];
        for (var w = 0; w < row.Length; w++)
        {
            var colorValue = row[w];
            img.SetPixel(w, h, Color.FromArgb(colorValue, colorValue, colorValue));
        }
    }
    img.Save($"C:\\Temp\\Handwriting\\Training\\{character}\\{count}.gif");
    count++;
}
