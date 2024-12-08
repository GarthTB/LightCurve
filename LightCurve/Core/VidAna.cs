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
                    using VideoCapture vid = new(file.FullName);
                    var values = vid.FrameCount > 0
                        ? new List<double>(vid.FrameCount) : [];

                    Mat frame = new();
                    while (vid.Read(frame))
                        values.Add(
                            ImgProc.MeanValue(
                                ImgProc.GetROI(frame, x, y, w, h), channel));

                    var outName = Tools.File.GenOutName([file]);
                    outName = ValCvt.AppendSuff(outName, channel);
                    Tools.File.OutputValues(outputType, [.. values], outputDir, outName);
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
