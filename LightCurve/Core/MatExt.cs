using Emgu.CV;
using Emgu.CV.CvEnum;

namespace LightCurve.Core
{
    /// <summary> Mat扩展方法 </summary>
    internal static class MatExt
    {
        /// <summary> 单通道图像的均值 </summary>
        private static double Mean(Mat image) => CvInvoke.Mean(image).V0;

        /// <summary> 提取Hue通道的均值 </summary>
        internal static double MeanH(this Mat image)
        {
            Mat hsv = new(), h = new();
            CvInvoke.CvtColor(image, hsv, ColorConversion.Bgr2Hsv);
            CvInvoke.ExtractChannel(hsv, h, 0);
            return h.Depth switch
            {
                DepthType.Cv8U => Mean(h) / 179.0, // H通道范围0-179
                DepthType.Cv16U => Mean(h) / 65535.0, // 未验证
                DepthType.Cv32F or DepthType.Cv64F => Mean(h) / 360.0, // 未验证
                _ => throw new ArgumentException("不支持的位深度！")
            };
        }

        /// <summary> 获取单通道图像归一化后的均值 </summary>
        private static double NormalizedMean(this Mat image)
        => image.Depth switch
        {
            DepthType.Cv8U => Mean(image) / 255.0,
            DepthType.Cv16U => Mean(image) / 65535.0,
            DepthType.Cv32F or DepthType.Cv64F => Mean(image), // 浮点图已经归一化
            _ => throw new ArgumentException("不支持的位深度！")
        };

        /// <summary> 提取B通道的均值 </summary>
        internal static double MeanB(this Mat image)
        {
            Mat b = new();
            CvInvoke.ExtractChannel(image, b, 0);
            return b.NormalizedMean();
        }

        /// <summary> 提取G通道的均值 </summary>
        internal static double MeanG(this Mat image)
        {
            Mat g = new();
            CvInvoke.ExtractChannel(image, g, 1);
            return g.NormalizedMean();
        }

        /// <summary> 提取R通道的均值 </summary>
        internal static double MeanR(this Mat image)
        {
            Mat r = new();
            CvInvoke.ExtractChannel(image, r, 2);
            return r.NormalizedMean();
        }

        /// <summary> 提取I通道的均值 </summary>
        internal static double MeanI(this Mat image)
            => (image.MeanR() + image.MeanG() + image.MeanB()) / 3.0;

        /// <summary> 提取CIEL通道的均值 </summary>
        internal static double MeanCIEL(this Mat image)
        {
            Mat lab = new(), ciel = new();
            CvInvoke.CvtColor(image, lab, ColorConversion.Bgr2Lab);
            CvInvoke.ExtractChannel(lab, ciel, 0);
            return ciel.NormalizedMean();
        }

        /// <summary> 提取Sv通道的均值 </summary>
        internal static double MeanSv(this Mat image)
        {
            Mat hsv = new(), sv = new();
            CvInvoke.CvtColor(image, hsv, ColorConversion.Bgr2Hsv);
            CvInvoke.ExtractChannel(hsv, sv, 1);
            return sv.NormalizedMean();
        }

        /// <summary> 提取V通道的均值 </summary>
        internal static double MeanV(this Mat image)
        {
            Mat hsv = new(), v = new();
            CvInvoke.CvtColor(image, hsv, ColorConversion.Bgr2Hsv);
            CvInvoke.ExtractChannel(hsv, v, 2);
            return v.NormalizedMean();
        }

        /// <summary> 提取Sl通道的均值 </summary>
        internal static double MeanSl(this Mat image)
        {
            Mat hls = new(), sl = new();
            CvInvoke.CvtColor(image, hls, ColorConversion.Bgr2Hls);
            CvInvoke.ExtractChannel(hls, sl, 2);
            return sl.NormalizedMean();
        }

        /// <summary> 提取L通道的均值 </summary>
        internal static double MeanL(this Mat image)
        {
            Mat hls = new(), l = new();
            CvInvoke.CvtColor(image, hls, ColorConversion.Bgr2Hls);
            CvInvoke.ExtractChannel(hls, l, 1);
            return l.NormalizedMean();
        }
    }
}
