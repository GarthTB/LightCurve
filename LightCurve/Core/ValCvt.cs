using OpenCvSharp;

namespace LightCurve.Core
{
    /// <summary> 颜色通道转换器 </summary>
    internal static class ValCvt
    {
        /// <summary> 获取单色图像指定通道的均值 </summary>
        internal static double MeanValue1(this Mat image, int channel)
            => channel is >= 0 and <= 6 ? Cv2.Mean(image).Val0 : 0;

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
            => image.CvtColor(ColorConversionCodes.BGRA2BGR).MeanValue3(channel);

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
