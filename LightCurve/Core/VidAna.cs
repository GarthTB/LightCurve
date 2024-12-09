using OpenCvSharp;
using System.IO;

namespace LightCurve.Core
{
    /// <summary> 视频分析器 </summary>
    internal class VidAna(
        List<FileInfo> files,
        int channel,
        uint? x,
        uint? y,
        uint? w,
        uint? h,
        int outputType,
        string outputDir)
    {
        /// <summary> 分析一组视频 </summary>
        internal void Run()
        {
            var safe = true;

            foreach (var file in files)
            {
                try
                {
                    VideoCapture vid = new(file.FullName);
                    if (vid.FrameCount <= 0)
                        throw new Exception("无效的视频帧数。");
                    var values = new double[vid.FrameCount];

                    var groupSize = 64;
                    for (int start = 0; ; start += groupSize)
                    {
                        var end = GetFrameGroup(vid, start, groupSize, out var frames);
                        _ = Parallel.For(0, groupSize, i =>
                        {
                            var index = start + i;
                            if (index < values.Length)
                                values[index] = frames[i].GetROI(x, y, w, h).ChMean(channel);
                        });
                        GC.Collect();
                        if (end) break;
                    }

                    var outName = Tools.File.GenOutName([file]).AppendCh(channel);
                    Tools.File.OutputValues(outputType, values, outputDir, outName);
                }
                catch (Exception e)
                {
                    safe = false;
                    Tools.MsgB.OkErr($"分析文件\"{file.Name}\"时出错：{e.Message}", "出错，已跳过");
                }
            }

            Tools.MsgB.OkInfo(safe ? "全部分析成功" : "分析结束", "提示");
        }

        /// <summary> 获取一组视频帧，若已到末尾则返回true </summary>
        private static bool GetFrameGroup(VideoCapture vid, int start, int size, out Mat[] group)
        {
            vid.PosFrames = start;
            group = Enumerable.Range(0, size).Select(_ => new Mat()).ToArray();
            for (int i = 0; i < size; i++)
                if (!vid.Read(group[i]))
                    return true;
            return false;
        }
    }
}
