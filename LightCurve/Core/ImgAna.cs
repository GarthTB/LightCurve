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
                var groupSize = 32;
                for (int start = 0; ; start += groupSize)
                {
                    var end = GetImageGroup(files, start, groupSize, out var frames);
                    _ = Parallel.For(0, groupSize, i =>
                    {
                        var index = start + i;
                        if (index < values.Length)
                            values[index] = frames[i].GetROI(x, y, w, h).ChMean(channel);
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
        private static bool GetImageGroup(List<FileInfo> files, int start, int size, out Mat[] group)
        {
            group = Enumerable.Range(0, size).Select(_ => new Mat()).ToArray();
            for (int i = 0; i < size; i++)
            {
                var index = start + i;
                group[i] = Cv2.ImRead(files[index].FullName);
                if (index == files.Count - 1) return true;
            }
            return false;
        }
    }
}
