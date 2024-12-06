using Microsoft.Win32;
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
    }
}
