using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BenchmarkDotNetVisualizer.Tests;

internal static class ImageComparer
{
    public static async Task<bool> IsEqualAsync(string imagePath1, string imagePath2)
    {
        using var image1 = await Image.LoadAsync<RgbaVector>(imagePath1);
        using var image2 = await Image.LoadAsync<RgbaVector>(imagePath2);

        if (image1.Width != image2.Width || image1.Height != image2.Height)
        {
            Console.WriteLine("Images must be of the same dimensions.");
            return false;
        }

        for (int x = 0; x < image1.Width; x++)
        {
            for (int y = 0; y < image1.Height; y++)
            {
                var pixel1 = image1[x, y];
                var pixel2 = image2[x, y];

                if (pixel1 != pixel2)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static async Task<bool> IsSimilarAsync(string imagePath1, string imagePath2, double ignoreDifferentColorPercentLessThan = 5.0, double thresholdDifferentPixlesPercent = 1.0)
    {
        var diff = await CalculateDiffAsync(imagePath1, imagePath2, ignoreDifferentColorPercentLessThan);
        return diff < thresholdDifferentPixlesPercent;
    }

    public static async Task<double> CalculateDiffAsync(string imagePath1, string imagePath2, double ignoreDifferentColorPercentLessThan = 10.0)
    {
        //TODO: Test with Rgba32 and PackedValue property

        using var image1 = await Image.LoadAsync<RgbaVector>(imagePath1);
        using var image2 = await Image.LoadAsync<RgbaVector>(imagePath2);

        if (image1.Width != image2.Width || image1.Height != image2.Height)
        {
            Console.WriteLine("Images must be of the same dimensions.");
            return 100;
        }

        int width = image1.Width;
        int height = image1.Height;
        int totalPixels = width * height;
        double diffSum = 0;
        //var differentPixels = new List<double>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixel1 = image1[x, y];
                var pixel2 = image2[x, y];

                var diff = GetDiff(pixel1, pixel2);
                if (diff > ignoreDifferentColorPercentLessThan)
                    diffSum += diff;
                //if (diff > 0)
                //    differentPixels.Add(diff);
            }
        }

        //Classifying diffs
        //var diff5 = differentPixels.FindAll(p => p is <= 5.0);
        //var diff10 = differentPixels.FindAll(p => p is <= 10.0);

        //Threshold[] thresholds =
        //[
        //    new(0  ,5   ,2.0 ),   //0  ~ 5      2.0 %
        //    new(5  ,10  ,1.5 ),   //5  ~ 10     1.5 %
        //    new(10 ,20  ,1.0 ),   //10 ~ 20     1.0 %
        //    new(20 ,30  ,0.7 ),   //20 ~ 30     0.7 %
        //    new(30 ,40  ,0.5 ),   //30 ~ 40     0.5 %
        //    new(40 ,60  ,0.3 ),   //40 ~ 60     0.3 %
        //    new(60 ,80  ,0.2 ),   //60 ~ 80     0.2 %
        //    new(80 ,100 ,0.1 ),   //80 ~ 100    0.1 %
        //];

        //foreach (var threshold in thresholds)
        //{
        //    var diffs = differentPixels.FindAll(p => p > threshold.ColorDiffPercentFrom && p <= threshold.ColorDiffPercentTo);
        //    threshold.RealPixlePercent = diffs.DefaultIfEmpty().Sum() / totalPixels;
        //}

        var differentPixlesPercent = diffSum / totalPixels;

        return differentPixlesPercent;

        static double GetDiff(RgbaVector pixle1, RgbaVector pixle2)
        {
            var diffR = Math.Abs(pixle1.R - pixle2.R);
            var diffG = Math.Abs(pixle1.G - pixle2.G);
            var diffB = Math.Abs(pixle1.B - pixle2.B);
            var diffA = Math.Abs(pixle1.A - pixle2.A);

            var diffRGB = ((double)diffR + diffG + diffB) / 3;

            return diffRGB * (1 - diffA) * 100;
        }
    }

    //private class Threshold(double colorDiffPercentFrom, double colorDiffPercentTo, double thresholdPixlePercent)
    //{
    //    public double ColorDiffPercentFrom => colorDiffPercentFrom;
    //    public double ColorDiffPercentTo => colorDiffPercentTo;
    //    public double ThresholdPixlePercent => thresholdPixlePercent;
    //    public double RealPixlePercent { get; internal set; }
    //}
}
