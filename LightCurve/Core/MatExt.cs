using OpenCvSharp;

namespace LightCurve.Core
{
    /// <summary> Mat扩展方法 </summary>
    internal static class MatExt
    {
        /// <summary> 单通道图像的均值 </summary>
        private static double Mean(Mat image) => Cv2.Mean(image).Val0;

        /// <summary> 获取单通道图像归一化后的均值 </summary>
        private static double NormalizedMean(this Mat image)
        => image.Depth() switch
        {
            MatType.CV_8U => Mean(image) / 255.0,
            MatType.CV_16U => Mean(image) / 65535.0,
            MatType.CV_32F or MatType.CV_64F => Mean(image), // 浮点图已经归一化
            _ => throw new ArgumentException("不支持的位深度！")
        };

        /// <summary> 提取H通道的均值 </summary>
        internal static double MeanH(this Mat image)
        {
            var h = image.CvtColor(ColorConversionCodes.BGR2HSV).ExtractChannel(0);
            return h.Depth() switch
            {
                MatType.CV_8U => Mean(h) / 179.0, // H通道范围0-179
                MatType.CV_16U => Mean(h) / 65535.0, // 未验证
                MatType.CV_32F or MatType.CV_64F => Mean(h) / 360.0, // 未验证
                _ => throw new ArgumentException("不支持的位深度！")
            };
        }

        /// <summary> 提取B通道的均值 </summary>
        internal static double MeanB(this Mat image)
            => image.ExtractChannel(0).NormalizedMean();

        /// <summary> 提取G通道的均值 </summary>
        internal static double MeanG(this Mat image)
            => image.ExtractChannel(1).NormalizedMean();

        /// <summary> 提取R通道的均值 </summary>
        internal static double MeanR(this Mat image)
            => image.ExtractChannel(2).NormalizedMean();

        /// <summary> 提取I通道的均值 </summary>
        internal static double MeanI(this Mat image)
            => (image.MeanR() + image.MeanG() + image.MeanB()) / 3.0;

        /// <summary> 提取CIEL通道的均值 </summary>
        internal static double MeanCIEL(this Mat image)
            => image.CvtColor(ColorConversionCodes.BGR2Lab).ExtractChannel(0).NormalizedMean();

        /// <summary> 提取Sv通道的均值 </summary>
        internal static double MeanSv(this Mat image)
            => image.CvtColor(ColorConversionCodes.BGR2HSV).ExtractChannel(1).NormalizedMean();

        /// <summary> 提取V通道的均值 </summary>
        internal static double MeanV(this Mat image)
            => image.CvtColor(ColorConversionCodes.BGR2HSV).ExtractChannel(2).NormalizedMean();

        /// <summary> 提取Sl通道的均值 </summary>
        internal static double MeanSl(this Mat image)
            => image.CvtColor(ColorConversionCodes.BGR2HLS).ExtractChannel(2).NormalizedMean();

        /// <summary> 提取L通道的均值 </summary>
        internal static double MeanL(this Mat image)
            => image.CvtColor(ColorConversionCodes.BGR2HLS).ExtractChannel(1).NormalizedMean();
    }
}
