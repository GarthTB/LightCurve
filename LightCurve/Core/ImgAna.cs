﻿using OpenCvSharp;
using System.IO;

namespace LightCurve.Core
{
    internal class ImgAna(
        List<FileInfo> files,
        int channel,
        uint? x,
        uint? y,
        uint? w,
        uint? h,
        int outputType,
        string outputDir)
    {
        internal void Run()
        {
            try
            {
                double[] values = new double[files.Count];
                _ = Parallel.For(0, values.Length, i =>
                {
                    var range = GetROI(files[i], x, y, w, h);
                    values[i] = GetValue(range, channel);
                });

                var outName = Tools.File.GenOutName(files);
                outName = ValCvt.AppendSuff(outName, channel);
                switch (outputType)
                {
                    case 0:
                        Tools.File.OutputTxt(values, outputDir, outName);
                        break;
                    case 1:
                        Tools.File.OutputPlot(values, outputDir, outName);
                        break;
                    default:
                        Tools.File.OutputTxt(values, outputDir, outName);
                        Tools.File.OutputPlot(values, outputDir, outName);
                        break;
                }

                Tools.MsgB.OkInfo("分析完成", "提示");
            }
            catch (Exception e)
            {
                Tools.MsgB.OkErr($"运行出错：{e.Message}", "异常中止");
            }
        }

        /// <summary> 将一个图片文件中的ROI提取为一张新图 </summary>
        private static Mat GetROI(FileInfo file, uint? x, uint? y, uint? w, uint? h)
        {
            var mat = Cv2.ImRead(file.FullName);
            if (mat.Empty())
                throw new ArgumentException("无法加载图像");
            if (x is null || y is null || w is null || h is null)
                return mat; // 直接返回全帧
            if (x + w > mat.Cols || y + h > mat.Rows)
                throw new ArgumentException("选区超出图像范围");
            Rect roi = new((int)x, (int)y, (int)w, (int)h);
            return new Mat(mat, roi);
        }

        /// <summary> 计算一块ROI中指定指标的均值 </summary>
        private static double GetValue(Mat range, int channel)
        => range.Channels() switch
        {
            1 => ValCvt.MeanValue1(range, channel), // 单色图
            3 => ValCvt.MeanValue3(range, channel), // 彩色图
            4 => ValCvt.MeanValue4(range, channel), // 含透明度的彩色图
            _ => throw new ArgumentException("不支持的通道数量"),
        };
    }
}
