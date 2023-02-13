using ImageMagick;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Mvc;
namespace UpLoadFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpLoadFilesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> OnPostUploadAsync(IFormFile file, double between_sec_image, int delay)
        {
            var filePath = "E:\\demovideo.mp4";
            System.IO.DirectoryInfo di = new DirectoryInfo("E:\\images");

            foreach (FileInfo filed in di.GetFiles())
            {
                filed.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            if (file.Length > 0)
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
            }

            using (var engine = new Engine())
            {
                var mp4 = new MediaFile { Filename = filePath };

                engine.GetMetadata(mp4);

                var i = 0.0;
                while (i < mp4.Metadata.Duration.TotalSeconds)
                {
                    var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i) };
                    var outputFile = new MediaFile { Filename = string.Format("{0}\\image-{1}.png", "E:\\images", i) };
                    engine.GetThumbnail(mp4, outputFile, options);
                    i += between_sec_image;
                }
            }

            string[] fileArray = Directory.GetFiles(@"E:\images", "*.png");
            using (var images = new MagickImageCollection())
            {
                for(int i= 0; i < fileArray.Length;i++)
                {

                    // Add first image and set the animation delay (in 1/100th of a second) 
                    images.Add(fileArray[i]);
                    images[i].AnimationDelay = delay; // in this example delay is 1000ms/1sec
                }



                // Optionally reduce colors
                var settings = new QuantizeSettings();
                settings.Colors = 256;
                images.Quantize(settings);

                // Optionally optimize the images (images should have the same size).
                images.Optimize();

                // Save gif
                images.Write("E:\\result.gif");
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            var stream1 = System.IO.File.OpenRead("E:\\result.gif");
            return new FileStreamResult(stream1, "application/octet-stream");
        }
    }
}
