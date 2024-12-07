﻿using Microsoft.Win32;
using OpenCvSharp;
using System.IO;
using System.Text;

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
        internal static bool IsImages(string[] path)
            => path.AsParallel().All(IsImage);

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
        internal static bool IsVideos(string[] path)
            => path.AsParallel().All(IsVideo);

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

        /// <summary> 生成结果文件名 </summary>
        internal static string GenOutName(List<FileInfo> files)
        => files.Count == 1
            ? Path.GetFileNameWithoutExtension(files[0].Name)
            : $"{Path.GetFileNameWithoutExtension(files[0].Name)} - {Path.GetFileNameWithoutExtension(files[^1].Name)}";

        /// <summary> 生成不重复的文件名 </summary>
        private static string DistinctPath(string dir, string name, string ext)
        {
            var path = Path.Combine(dir, $"{name}.{ext}");
            for (int i = 2; System.IO.File.Exists(path); i++)
                path = Path.Combine(dir, $"{name}_{i}.{ext}");
            return path;
        }

        /// <summary> 将结果列表输出到指定路径 </summary>
        internal static void OutputTxt(double[] values, string dir, string name)
        {
            var path = DistinctPath(dir, name, "txt");

            var sb = new StringBuilder();
            _ = sb.AppendLine("序号\t值");
            for (int i = 0; i < values.Length; i++)
                _ = sb.AppendLine($"{i + 1}\t{values[i]}");

            System.IO.File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        /// <summary> 将结果折线图输出到指定路径 </summary>
        internal static void OutputPlot(double[] values, string dir, string name)
        {
            var path = DistinctPath(dir, name, "png");

            var indexes = Enumerable.Range(1, values.Length).ToArray();
            ScottPlot.Plot plot = new();
            _ = plot.Add.Scatter(indexes, values);
            plot.XLabel("Frame Number");
            plot.YLabel("Value");
            plot.ScaleFactor = 2;
            plot.Axes.SetLimits(1, indexes[^1], 0, 1);
            plot.Axes.AntiAlias(true);

            _ = plot.SavePng(path, 3240, 2000);
        }
    }
}
