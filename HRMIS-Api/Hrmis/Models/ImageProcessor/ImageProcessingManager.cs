using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Hrmis.Models.ImageProcessor
{
    public class ImageProcessingManager
    {
        public string OutputPath { get; set; }
        public long ImageQualityLevel {
            get
            {
                long level = 70;
                try
                {
                    level = long.Parse(ConfigurationManager.AppSettings["hrImageQualityLevel"]);
                }
                catch (Exception)
                {
                    // ignored
                }
                return level;
            }
        }
        public ImageProcessingManager(string path)
        {
            OutputPath = path;
        }
        public void SaveCompressedImage(Image image)
        {
            // Get a bitmap. The using statement ensures objects  
            // are automatically disposed from memory after use.  
            using (Bitmap bmp1 = new Bitmap(image))
            {

                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(Encoder.Quality, ImageQualityLevel);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bmp1.Save(OutputPath, jpgEncoder, myEncoderParameters);
                bmp1.Dispose();
            }
        }


        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}