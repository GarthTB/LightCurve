using OpenCvSharp;

namespace LightCurve.Core
{
    /// <summary> 图像处理器 </summary>
    internal static class ImgProc
    {
        /// <summary> 将一张图片中的ROI提取为一张新图 </summary>
        internal static Mat GetROI(Mat image, uint? x, uint? y, uint? w, uint? h)
        {
            if (image.Empty())
                throw new ArgumentException("无法加载图像");
            if (x is null || y is null || w is null || h is null)
                return image; // 直接返回全帧
            if (x + w > image.Cols || y + h > image.Rows)
                throw new ArgumentException("选区超出图像范围");
            Rect roi = new((int)x, (int)y, (int)w, (int)h);
            return new Mat(image, roi);
        }

        /// <summary> 计算一块ROI中指定指标的均值 </summary>
        internal static double GetValue(Mat image, int channel)
        => image.Channels() switch
        {
            1 => ValCvt.MeanValue1(image, channel), // 单色图
            3 => ValCvt.MeanValue3(image, channel), // 彩色图
            4 => ValCvt.MeanValue4(image, channel), // 含透明度的彩色图
            _ => throw new ArgumentException("不支持的通道数量"),
        };
    }
}
