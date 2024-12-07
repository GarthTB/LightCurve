using OpenCvSharp;

namespace LightCurve.Core
{
    /// <summary> 颜色通道转换器 </summary>
    internal static class ValCvt
    {
        /// <summary> 获取单色图像指定通道的均值 </summary>
        internal static double MeanValue1(Mat image, int channel)
            => channel is >= 0 and <= 6 ? Cv2.Mean(image).Val0 : 0;

        /// <summary> 获取3通道图像指定通道的均值 </summary>
        internal static double MeanValue3(Mat image, int channel)
        => channel switch
        {
            0 => MeanCIEL(image),
            1 => MeanI(image),
            2 => MeanL(image),
            3 => MeanV(image),
            4 => MeanR(image),
            5 => MeanG(image),
            6 => MeanB(image),
            7 => MeanSl(image),
            8 => MeanSv(image),
            9 => MeanH(image),
            _ => 0,
        };

        /// <summary> 获取4通道图像指定通道的均值 </summary>
        internal static double MeanValue4(Mat image, int channel)
        {
            Cv2.CvtColor(image, image, ColorConversionCodes.BGRA2BGR);
            return MeanValue3(image, channel);
        }

        /// <summary> 为结果文件名添加通道后缀 </summary>
        internal static string AppendSuff(string name, int channel)
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

        /// <summary> 获取单通道图像归一化后的均值 </summary>
        private static double NormalizedMean(Mat image)
        => image.Depth() switch
        {
            MatType.CV_8U => Mean(image) / 255.0,
            MatType.CV_16U => Mean(image) / 65535.0,
            MatType.CV_32F or MatType.CV_64F => Mean(image), // 浮点图已经归一化
            _ => throw new ArgumentException("不支持的位深度！")
        };

        /// <summary> 单通道图像的均值 </summary>
        private static double Mean(Mat image) => Cv2.Mean(image).Val0;

        /// <summary> 提取R通道的均值 </summary>
        private static double MeanR(Mat image) => NormalizedMean(Cv2.Split(image)[2]);

        /// <summary> 提取G通道的均值 </summary>
        private static double MeanG(Mat image) => NormalizedMean(Cv2.Split(image)[1]);

        /// <summary> 提取B通道的均值 </summary>
        private static double MeanB(Mat image) => NormalizedMean(Cv2.Split(image)[0]);

        /// <summary> 提取CIEL通道的均值 </summary>
        private static double MeanCIEL(Mat image)
        {
            using Mat labImage = image.CvtColor(ColorConversionCodes.BGR2Lab);
            return NormalizedMean(Cv2.Split(labImage)[0]);
        }

        /// <summary> 提取I通道的均值 </summary>
        private static double MeanI(Mat image)
            => (MeanR(image) + MeanG(image) + MeanB(image)) / 3;

        /// <summary> 提取L通道的均值 </summary>
        private static double MeanL(Mat image)
        {
            using Mat hlsImage = image.CvtColor(ColorConversionCodes.BGR2HLS);
            return NormalizedMean(Cv2.Split(hlsImage)[1]);
        }

        /// <summary> 提取V通道的均值 </summary>
        private static double MeanV(Mat image)
        {
            using Mat hsvImage = image.CvtColor(ColorConversionCodes.BGR2HSV);
            return NormalizedMean(Cv2.Split(hsvImage)[2]);
        }

        /// <summary> 提取Sl通道的均值 </summary>
        private static double MeanSl(Mat image)
        {
            using Mat hlsImage = image.CvtColor(ColorConversionCodes.BGR2HLS);
            return NormalizedMean(Cv2.Split(hlsImage)[2]);
        }

        /// <summary> 提取Sv通道的均值 </summary>
        private static double MeanSv(Mat image)
        {
            using Mat hsvImage = image.CvtColor(ColorConversionCodes.BGR2HSV);
            return NormalizedMean(Cv2.Split(hsvImage)[1]);
        }

        /// <summary> 提取H通道的均值 </summary>
        private static double MeanH(Mat image)
        {
            using Mat hsvImage = image.CvtColor(ColorConversionCodes.BGR2HSV);
            return NormalizedMean(Cv2.Split(hsvImage)[0]);
        }
    }
}
