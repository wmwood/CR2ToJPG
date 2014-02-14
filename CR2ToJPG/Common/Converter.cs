using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CR2ToJPG
{
    public static class Converter
    {
        private static byte[] _buffer = new byte[_bufferSize];
        private static int _bufferSize = 512 * 1024;
        private static ImageCodecInfo _jpgImageCodec = GetJpegCodec();

        public static void ConvertImage(string fileName, string outputFolder)
        {
            using (FileStream fi = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, _bufferSize, FileOptions.None))
            {
                // Start address is at offset 0x62, file size at 0x7A, orientation at 0x6E
                fi.Seek(0x62, SeekOrigin.Begin);
                BinaryReader br = new BinaryReader(fi);
                UInt32 jpgStartPosition = br.ReadUInt32();  // 62
                br.ReadUInt32();  // 66
                br.ReadUInt32();  // 6A
                UInt32 orientation = br.ReadUInt32() & 0x000000FF; // 6E
                br.ReadUInt32();  // 72
                br.ReadUInt32();  // 76
                Int32 fileSize = br.ReadInt32();  // 7A

                fi.Seek(jpgStartPosition, SeekOrigin.Begin);

                string baseName = Path.GetFileNameWithoutExtension(fileName);
                string jpgName = Path.Combine(outputFolder, baseName + ".jpg");

                Bitmap bitmap = new Bitmap(new PartialStream(fi, jpgStartPosition, fileSize));

                try
                {
                    if (_jpgImageCodec != null && (orientation == 8 || orientation == 6))
                    {
                        if (orientation == 8)
                            bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        else
                            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                }
                catch(Exception ex)
                {
                    // Image Skipped
                }

                EncoderParameters ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                bitmap.Save(jpgName, _jpgImageCodec, ep);
            }
        }

        private static ImageCodecInfo GetJpegCodec()
        {
            foreach (ImageCodecInfo c in ImageCodecInfo.GetImageEncoders())
            {
                if (c.CodecName.ToLower().Contains("jpeg")
                    || c.FilenameExtension.ToLower().Contains("*.jpg")
                    || c.FormatDescription.ToLower().Contains("jpeg")
                    || c.MimeType.ToLower().Contains("image/jpeg"))
                    return c;
            }

            return null;
        }
    }
}