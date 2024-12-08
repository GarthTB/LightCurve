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

        /// <summary> 提取B通道的均值 </summary>
        internal static double MeanB(this Mat image)
            => Cv2.Split(image)[0].NormalizedMean();

        /// <summary> 提取G通道的均值 </summary>
        internal static double MeanG(this Mat image)
            => Cv2.Split(image)[1].NormalizedMean();

        /// <summary> 提取R通道的均值 </summary>
        internal static double MeanR(this Mat image)
            => Cv2.Split(image)[2].NormalizedMean();

        /// <summary> 提取I通道的均值 </summary>
        internal static double MeanI(this Mat image)
            => (image.MeanR() + image.MeanG() + image.MeanB()) / 3;

        /// <summary> 提取CIEL通道的均值 </summary>
        internal static double MeanCIEL(this Mat image)
            => Cv2.Split(image.CvtColor(ColorConversionCodes.BGR2Lab))[0].NormalizedMean();

        /// <summary> 提取H通道的均值 </summary>
        internal static double MeanH(this Mat image)
            => Cv2.Split(image.CvtColor(ColorConversionCodes.BGR2HSV))[0].NormalizedMean();

        /// <summary> 提取Sv通道的均值 </summary>
        internal static double MeanSv(this Mat image)
            => Cv2.Split(image.CvtColor(ColorConversionCodes.BGR2HSV))[1].NormalizedMean();

        /// <summary> 提取V通道的均值 </summary>
        internal static double MeanV(this Mat image)
            => Cv2.Split(image.CvtColor(ColorConversionCodes.BGR2HSV))[2].NormalizedMean();

        /// <summary> 提取L通道的均值 </summary>
        internal static double MeanL(this Mat image)
            => Cv2.Split(image.CvtColor(ColorConversionCodes.BGR2HLS))[1].NormalizedMean();

        /// <summary> 提取Sl通道的均值 </summary>
        internal static double MeanSl(this Mat image)
            => Cv2.Split(image.CvtColor(ColorConversionCodes.BGR2HLS))[2].NormalizedMean();
    }
}
