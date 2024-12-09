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
                        throw new Exception("无法获取视频总帧数。");

                    var values = new double[vid.FrameCount];
                    var group = Enumerable.Range(0, 64).Select(_ => new Mat()).ToArray();
                    for (int start = 0; ; start += group.Length)
                    {
                        var end = GetFrameGroup(vid, start, group);
                        _ = Parallel.For(0, group.Length, i =>
                        {
                            var index = start + i;
                            if (index < values.Length)
                                values[index] = group[i].GetROI(x, y, w, h).ChMean(channel);
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
        private static bool GetFrameGroup(VideoCapture vid, int start, Mat[] group)
        {
            vid.PosFrames = start;
            foreach (var frameRef in group)
                if (!vid.Read(frameRef))
                    return true;
            return false;
        }
    }
}
