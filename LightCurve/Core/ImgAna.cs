using Emgu.CV;
using Emgu.CV.CvEnum;
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
                var group = new Mat[32];
                for (int start = 0; ; start += group.Length)
                {
                    var end = GetImageGroup(files, start, group);
                    _ = Parallel.For(0, group.Length, i =>
                    {
                        var index = start + i;
                        if (index < values.Length)
                            values[index] = group[i].GetROI(x, y, w, h).ChMean(channel);
                    });
                    GC.Collect();
                    if (end) break;
                }

                var outName = Tools.File.GenOutName(files).AppendCh(channel);
                Tools.File.OutputValues(outputType, values, outputDir, outName);

                Tools.MsgB.OkInfo("分析完成", "提示");
            }
            catch (Exception e) { Tools.MsgB.OkErr($"运行出错：{e.Message}", "异常中止"); }
        }

        /// <summary> 获取一组图片，若已到末尾则返回true </summary>
        private static bool GetImageGroup(List<FileInfo> files, int start, Mat[] group)
        {
            for (int i = 0; i < group.Length; i++)
            {
                var fileIdx = start + i;
                group[i] = new Mat(files[fileIdx].FullName, ImreadModes.Unchanged);
                if (fileIdx == files.Count - 1) return true;
            }
            return false;
        }
    }
}
