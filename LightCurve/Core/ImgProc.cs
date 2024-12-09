using Emgu.CV;
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
    }
}
