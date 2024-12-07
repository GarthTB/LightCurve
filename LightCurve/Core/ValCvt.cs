using OpenCvSharp;

namespace LightCurve.Core
{
    internal static class ValCvt
    {
        /// <summary> 获取单色图像指定通道的均值 </summary>
        internal static double MeanValue1(Mat image, int channel)
            => channel is >= 0 and <= 6 ? image.Mean().Val0 : 0;

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

        /// <summary> 获取Mat图允许的最大值 </summary>
        private static double Normalize(double value, int depth)
        => depth switch
        {
            MatType.CV_8U => value / 255.0,
            MatType.CV_16U => value / 65535.0,
            MatType.CV_32F or MatType.CV_64F => value, // 浮点图已经归一化
            _ => throw new ArgumentException("不支持的位深度！")
        };

        /// <summary> 提取R通道的均值 </summary>
        private static double MeanR(Mat image)
        {
            var r = Cv2.Split(image)[2];
            var mean = Cv2.Mean(r).Val0;
            return Normalize(mean, image.Depth());
        }

        /// <summary> 提取G通道的均值 </summary>
        private static double MeanG(Mat image)
        {
            var g = Cv2.Split(image)[1];
            var mean = Cv2.Mean(g).Val0;
            return Normalize(mean, image.Depth());
        }

        /// <summary> 提取B通道的均值 </summary>
        private static double MeanB(Mat image)
        {
            var b = Cv2.Split(image)[0];
            var mean = Cv2.Mean(b).Val0;
            return Normalize(mean, image.Depth());
        }

        /// <summary> 提取CIEL通道的均值 </summary>
        private static double MeanCIEL(Mat image)
        {
            Mat labImage = new();
            Cv2.CvtColor(image, labImage, ColorConversionCodes.BGR2Lab);
            var l = Cv2.Split(labImage)[0];
            var mean = Cv2.Mean(l).Val0;
            return Normalize(mean, image.Depth());
        }

        /// <summary> 提取I通道的均值 </summary>
        private static double MeanI(Mat image)
            => (MeanR(image) + MeanG(image) + MeanB(image)) / 3;

        /// <summary> 提取L通道的均值 </summary>
        private static double MeanL(Mat image)
        {
            Mat hlsImage = new();
            Cv2.CvtColor(image, hlsImage, ColorConversionCodes.BGR2HLS);
            var l = Cv2.Split(hlsImage)[1];
            var mean = Cv2.Mean(l).Val0;
            return Normalize(mean, image.Depth());
        }

        /// <summary> 提取V通道的均值 </summary>
        private static double MeanV(Mat image)
        {
            Mat hsvImage = new();
            Cv2.CvtColor(image, hsvImage, ColorConversionCodes.BGR2HSV);
            var v = Cv2.Split(hsvImage)[2];
            var mean = Cv2.Mean(v).Val0;
            return Normalize(mean, image.Depth());
        }

        /// <summary> 提取Sl通道的均值 </summary>
        private static double MeanSl(Mat image)
        {
            Mat hlsImage = new();
            Cv2.CvtColor(image, hlsImage, ColorConversionCodes.BGR2HLS);
            var s = Cv2.Split(hlsImage)[2];
            var mean = Cv2.Mean(s).Val0;
            return Normalize(mean, image.Depth());
        }

        /// <summary> 提取Sv通道的均值 </summary>
        private static double MeanSv(Mat image)
        {
            Mat hsvImage = new();
            Cv2.CvtColor(image, hsvImage, ColorConversionCodes.BGR2HSV);
            var s = Cv2.Split(hsvImage)[1];
            var mean = Cv2.Mean(s).Val0;
            return Normalize(mean, image.Depth());
        }

        /// <summary> 提取H通道的均值 </summary>
        private static double MeanH(Mat image)
        {
            Mat hsvImage = new();
            Cv2.CvtColor(image, hsvImage, ColorConversionCodes.BGR2HSV);
            var h = Cv2.Split(hsvImage)[0];
            var mean = Cv2.Mean(h).Val0;
            return Normalize(mean, image.Depth());
        }
    }
}
