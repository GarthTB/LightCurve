using Microsoft.Win32;
using OpenCvSharp;
using System.IO;

namespace LightCurve.Core.Tools
{
    /// <summary> 和文件相关的工具类 </summary>
    internal static class File
    {
        /// <summary> 选择输出的文件夹 </summary>
        internal static string SelectOutputDir(int typeIndex)
        {
            var dialog = new SaveFileDialog
            {
                Title = "选择输出文件夹",
                FileName = "临时文件名，最后会根据原文件名修改",
                Filter = typeIndex switch
                {
                    1 => "png图片文件(*.png)|*.png",
                    _ => "txt文本文件(*.txt)|*.txt",
                },
            };
            if (dialog.ShowDialog() == false) return "";
            var dir = Path.GetDirectoryName(dialog.FileName);
            return string.IsNullOrEmpty(dir) ? "" : dir;
        }

        /// <summary> 选择多个待处理的文件 </summary>
        internal static string[] PickInputs()
        {
            OpenFileDialog ofd = new()
            {
                Title = "选择待处理的文件",
                Multiselect = true,
            };
            return ofd.ShowDialog() == true ? ofd.FileNames : [];
        }

        /// <summary> 判断文件是否全为图片 </summary>
        internal static bool IsImages(string[] path) => path.All(IsImage);

        /// <summary> 判断文件是否为图片 </summary>
        private static bool IsImage(string path)
        {
            try
            {
                using var image = Cv2.ImRead(path);
                return !image.Empty();
            }
            catch
            {
                return false;
            }
        }

        /// <summary> 判断文件是否全为视频 </summary>
        internal static bool IsVideos(string[] path) => path.All(IsVideo);

        /// <summary> 判断文件是否为视频 </summary>
        private static bool IsVideo(string path)
        {
            try
            {
                using var videoCapture = new VideoCapture(path);
                return videoCapture.IsOpened();
            }
            catch
            {
                return false;
            }
        }
    }
}
