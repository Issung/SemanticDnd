using System.Diagnostics;

namespace DndTest.Services;

public class ConverterService
{
    public async Task ConvertDocxToPdf()
    {
        var inputPath = "input.docx";
        var outputPath = Guid.NewGuid().ToString();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "C:\\Program Files\\LibreOffice\\program\\soffice.exe",
                Arguments = $"--headless --convert-to pdf --outdir \"{Path.GetDirectoryName(outputPath)}\" \"{inputPath}\"",
                WorkingDirectory = Path.GetDirectoryName(inputPath),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();

    }
}
