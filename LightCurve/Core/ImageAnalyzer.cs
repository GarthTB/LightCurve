using OpenCvSharp;
using System.IO;

namespace LightCurve.Core
{
    internal class ImageAnalyzer(
        List<FileInfo> files,
        int channel,
        uint? x,
        uint? y,
        uint? w,
        uint? h,
        int outputType,
        string outputPath)
    {
        internal void Run()
        {
            try
            {
                var ranges = files.Select(file => GetROI(file, x, y, w, h)).ToArray();
                var values = ranges.Select(range => GetValue(range, channel)).ToArray();
                Output(values, outputPath, outputType);
            }
            catch (Exception e)
            {
                Tools.MsgB.OkErr($"运行出错：{e.Message}", "异常中止");
            }
        }

        /// <summary> 将一个图片文件中的ROI提取为一张新图 </summary>
        private static Mat GetROI(FileInfo file, uint? x, uint? y, uint? w, uint? h)
        {
            var mat = Cv2.ImRead(file.FullName);
            if (mat.Empty())
                throw new ArgumentException("无法加载图像");
            if (x is null || y is null || w is null || h is null)
                return mat; // 直接返回全帧
            if (x + w > mat.Cols || y + h > mat.Rows)
                throw new ArgumentException("选区超出图像范围");
            Rect roi = new((int)x, (int)y, (int)w, (int)h);
            return new Mat(mat, roi);
        }

        /// <summary> 计算一块ROI中指定指标的均值 </summary>
        private static double GetValue(Mat range, int channel)
        {
            if (range.Channels() == 1) // 灰度图
                return range.Mean().Val0;
            if (range.Channels() != 3) // 无效图
                throw new ArgumentException("不支持的图像通道数量");

            var values = ConvertPixels();
            return values.Average();

            List<double> ConvertPixels()
            {
                int depth = range.Depth();
                double Normalize(double value) => depth switch
                {
                    MatType.CV_8U => value / 255.0,
                    MatType.CV_16U => value / 65535.0,
                    MatType.CV_32F => value,
                    _ => throw new NotSupportedException("不支持的图像位深")
                };

                List<double> values = new(range.Cols * range.Rows);
                if (depth == MatType.CV_8U)
                    for (int row = 0; row < range.Rows; row++)
                        for (int col = 0; col < range.Cols; col++)
                        {
                            var px = range.At<Vec3b>(row, col);
                            var b = Normalize(px.Item0);
                            var g = Normalize(px.Item1);
                            var r = Normalize(px.Item2);
                            values.Add(RGBConverter.Convert(r, g, b, channel));
                        }
                else if (depth == MatType.CV_16U)
                    for (int row = 0; row < range.Rows; row++)
                        for (int col = 0; col < range.Cols; col++)
                        {
                            var px = range.At<Vec3w>(row, col);
                            var b = Normalize(px.Item0);
                            var g = Normalize(px.Item1);
                            var r = Normalize(px.Item2);
                            values.Add(RGBConverter.Convert(r, g, b, channel));
                        }
                else if (depth == MatType.CV_32F)
                    for (int row = 0; row < range.Rows; row++)
                        for (int col = 0; col < range.Cols; col++)
                        {
                            var px = range.At<Vec3f>(row, col);
                            var b = Normalize(px.Item0);
                            var g = Normalize(px.Item1);
                            var r = Normalize(px.Item2);
                            values.Add(RGBConverter.Convert(r, g, b, channel));
                        }
                else throw new NotSupportedException("不支持的图像位深");

                return values;
            }
        }

        /// <summary> 将结果输出到指定路径 </summary>
        private static void Output(double[] values, string path, int Type)
        {

        }
    }
}
