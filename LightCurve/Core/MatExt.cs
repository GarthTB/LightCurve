using Emgu.CV;
using Emgu.CV.CvEnum;

namespace LightCurve.Core
{
    /// <summary> Mat扩展方法 </summary>
    internal static class MatExt
    {
        /// <summary> 单通道图像的均值 </summary>
        internal static double Mean(this Mat image) => CvInvoke.Mean(image).V0;

        /// <summary> 将图像转换为指定色空间 </summary>
        internal static Mat CvtColor(this Mat image, ColorConversion code)
        {
            CvInvoke.CvtColor(image, image, code);
            return image;
        }

        /// <summary> 提取图像的指定通道 </summary>
        private static Mat GetChannel(this Mat image, int channel)
        {
            CvInvoke.ExtractChannel(image, image, channel);
            return image;
        }

        /// <summary> 获取单通道图像归一化后的均值 </summary>
        private static double NormalizedMean(this Mat image)
        => image.Depth switch
        {
            DepthType.Cv8U => image.Mean() / 255.0,
            DepthType.Cv16U => image.Mean() / 65535.0,
            DepthType.Cv32F or DepthType.Cv64F => image.Mean(), // 浮点图已经归一化
            _ => throw new ArgumentException("不支持的位深度！")
        };

        /// <summary> 提取B通道的均值 </summary>
        internal static double MeanB(this Mat image)
            => image.GetChannel(0).NormalizedMean();

        /// <summary> 提取G通道的均值 </summary>
        internal static double MeanG(this Mat image)
            => image.GetChannel(1).NormalizedMean();

        /// <summary> 提取R通道的均值 </summary>
        internal static double MeanR(this Mat image)
            => image.GetChannel(2).NormalizedMean();

        /// <summary> 提取I通道的均值 </summary>
        internal static double MeanI(this Mat image)
            => (image.MeanR() + image.MeanG() + image.MeanB()) / 3.0;

        /// <summary> 提取H通道的均值 </summary>
        internal static double MeanH(this Mat image)
            => image.CvtColor(ColorConversion.Bgr2Hsv).GetChannel(0).Mean() / 179.0;

        /// <summary> 提取CIEL通道的均值 </summary>
        internal static double MeanCIEL(this Mat image)
            => image.CvtColor(ColorConversion.Bgr2Lab).GetChannel(0).NormalizedMean();

        /// <summary> 提取Sv通道的均值 </summary>
        internal static double MeanSv(this Mat image)
            => image.CvtColor(ColorConversion.Bgr2Hsv).GetChannel(1).NormalizedMean();

        /// <summary> 提取V通道的均值 </summary>
        internal static double MeanV(this Mat image)
            => image.CvtColor(ColorConversion.Bgr2Hsv).GetChannel(2).NormalizedMean();

        /// <summary> 提取Sl通道的均值 </summary>
        internal static double MeanSl(this Mat image)
            => image.CvtColor(ColorConversion.Bgr2Hls).GetChannel(2).NormalizedMean();

        /// <summary> 提取L通道的均值 </summary>
        internal static double MeanL(this Mat image)
            => image.CvtColor(ColorConversion.Bgr2Hls).GetChannel(1).NormalizedMean();
    }
}
