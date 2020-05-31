using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.Utility
{
   public static class OtherUtility
    {
        /// <summary>
        /// clamp a value between min and max
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        /// <summary>
        /// swap two items in same list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        public static void SwapItems<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public static string GetImageExtension(ImageFormat img)
        {
            //return ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == format.Guid).FilenameExtension;
            if (img.Equals(ImageFormat.Jpeg)) return ".jpg";
            else if (img.Equals(ImageFormat.Png)) return ".png";
            else if (img.Equals(ImageFormat.Gif)) return ".gif";
            else if (img.Equals(ImageFormat.Bmp)) return ".bmp";
            else if (img.Equals(ImageFormat.Tiff)) return ".tif";
            else if (img.Equals(ImageFormat.Icon)) return ".ico";
            else if (img.Equals(ImageFormat.Emf)) return ".emf";
            else if (img.Equals(ImageFormat.Wmf)) return ".wmf";
            else if (img.Equals(ImageFormat.Exif)) return ".exif";
            else if (img.Equals(ImageFormat.MemoryBmp)) return ".bmp";
            return ".unknown";
        }

    }
}
