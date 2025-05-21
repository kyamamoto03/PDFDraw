using PDFtoImage;
using SkiaSharp;

try
{
    // 環境変数からファイル名を取得、設定されていない場合はデフォルト値を使用
    var fileName = Environment.GetEnvironmentVariable("PDF_FILE_NAME");
    var outputDir = Environment.GetEnvironmentVariable("OUTPUT_DIR");

    // 環境変数が設定されていない場合、コマンドライン引数をチェック
    if (string.IsNullOrEmpty(fileName) && args.Length > 0)
    {
        fileName = args[0];
    }

    // それでも指定がない場合はデフォルト値を使用
    if (string.IsNullOrEmpty(fileName))
    {
        fileName = "input.pdf";
        Console.WriteLine($"環境変数'PDF_FILE_NAME'またはコマンドライン引数が指定されていないため、デフォルト値を使用します: {fileName}");
    }
    else
    {
        Console.WriteLine($"ファイル名: {fileName}");
    }

    if (string.IsNullOrEmpty(outputDir) == true)
    {
        outputDir = ".";
    }

    /// 出力ファイル名
    string outputFile = string.Format($"{outputDir}/ToImage.png");

    // fileNameを読み込んでbyte[]に格納する
    byte[] pdfBytes = File.ReadAllBytes(fileName);

    // PDFを画像に変換する
    RenderOptions renderOptions = new RenderOptions(Dpi: 144);

    MemoryStream imageStream = new MemoryStream();
    Conversion.SavePng(imageStream, pdfBytes, options: renderOptions);

    // ストリームの位置を0に戻す
    imageStream.Position = 0;

    // ストリームからSKBitmapを作成する
    using var skBitmap = SKBitmap.Decode(imageStream);

    using var canvas = new SKCanvas(skBitmap);

    var paint = new SKPaint
    {
        Color = SKColors.Red,
        StrokeWidth = 3,
        IsAntialias = true
    };
    // 例: 左上(10,10)から右下(100,100)へ直線
    canvas.DrawLine(10, 10, 100, 100, paint);
    // skBitmapをファイルに保存する

    using (var image = SKImage.FromBitmap(skBitmap))
    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
    using (var stream = File.OpenWrite(outputFile))
    {
        data.SaveTo(stream);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"エラー:");
    Console.WriteLine(ex.ToString());
}

Console.WriteLine("Press any key to exit.");
Console.ReadLine();