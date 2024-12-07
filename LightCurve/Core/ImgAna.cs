using OpenCvSharp;
using System.IO;

namespace LightCurve.Core
{
    /// <summary> 图片分析器 </summary>
    internal class ImgAna(
        List<FileInfo> files,
        int channel,
        uint? x,
        uint? y,
        uint? w,
        uint? h,
        int outputType,
        string outputDir)
    {
        /// <summary> 分析一组图片 </summary>
        internal void Run()
        {
            try
            {
                var values = new double[files.Count];
                _ = Parallel.For(0, values.Length, i =>
                {
                    using Mat image = Cv2.ImRead(files[i].FullName);
                    using Mat roi = ImgProc.GetROI(image, x, y, w, h);
                    values[i] = ImgProc.GetValue(roi, channel);
                });

                var outName = Tools.File.GenOutName(files);
                outName = ValCvt.AppendSuff(outName, channel);
                Tools.File.OutputValues(outputType, values, outputDir, outName);

                Tools.MsgB.OkInfo("分析完成", "提示");
            }
            catch (Exception e)
            {
                Tools.MsgB.OkErr($"运行出错：{e.Message}", "异常中止");
            }
        }
    }
}
