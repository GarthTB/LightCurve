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

                    for (int i = 0; i < values.Length; i++)
                    {
                        Mat frame = new();
                        if (!vid.Read(frame))
                            throw new Exception("读取视频帧失败。");
                        values[i] = frame.GetROI(x, y, w, h).ChMean(channel);
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
    }
}
