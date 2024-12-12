using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace LightCurve.Core
{
    /// <summary> 图像处理器 </summary>
    internal static class ImgProc
    {
        /// <summary> 将一张图片中的ROI提取为一张新图 </summary>
        internal static Mat GetROI(this Mat image, uint? x, uint? y, uint? w, uint? h)
        {
            if (x is null || y is null || w is null || h is null)
                return image; // 直接返回全帧
            if (x + w > image.Cols || y + h > image.Rows)
                throw new ArgumentException("选区超出图像范围");
            Rectangle roi = new((int)x, (int)y, (int)w, (int)h); // 若无法转换，应该会在上一步筛掉
            return new Mat(image, roi);
        }

        /// <summary> 计算一块ROI图的指定通道的均值 </summary>
        internal static double ChMean(this Mat image, int channel)
        => image.NumberOfChannels switch
        {
            1 => image.MeanValue1(channel), // 单色图
            3 => image.MeanValue3(channel), // 彩色图
            4 => image.MeanValue4(channel), // 含透明度的彩色图
            _ => throw new ArgumentException("不支持的通道数量"),
        };

        /// <summary> 获取单色图像指定通道的均值 </summary>
        internal static double MeanValue1(this Mat image, int channel)
            => channel is >= 0 and <= 6 ? image.Mean() : 0;

        /// <summary> 获取3通道图像指定通道的均值 </summary>
        internal static double MeanValue3(this Mat image, int channel)
        => channel switch
        {
            0 => image.MeanCIEL(),
            1 => image.MeanI(),
            2 => image.MeanL(),
            3 => image.MeanV(),
            4 => image.MeanR(),
            5 => image.MeanG(),
            6 => image.MeanB(),
            7 => image.MeanSl(),
            8 => image.MeanSv(),
            9 => image.MeanH(),
            _ => 0,
        };

        /// <summary> 获取4通道图像指定通道的均值 </summary>
        internal static double MeanValue4(this Mat image, int channel)
            => image.CvtColor(ColorConversion.Bgra2Bgr).MeanValue3(channel);

        /// <summary> 为结果文件名添加通道后缀 </summary>
        internal static string AppendCh(this string name, int channel)
        => channel switch
        {
            0 => $"{name}_CIEL",
            1 => $"{name}_I",
            2 => $"{name}_L",
            3 => $"{name}_V",
            4 => $"{name}_R",
            5 => $"{name}_G",
            6 => $"{name}_B",
            7 => $"{name}_Sl",
            8 => $"{name}_Sv",
            9 => $"{name}_H",
            _ => name,
        };
    }
}
