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
            _ = Parallel.ForEach(files, file =>
            {
                try
                {
                    using VideoCapture vid = new(file.FullName);
                    var values = new double[vid.FrameCount];

                    Mat frame = new(), roi = new();
                    for (int i = 0; vid.Read(frame); i++)
                    {
                        roi = ImgProc.GetROI(frame, x, y, w, h);
                        values[i] = ImgProc.GetValue(roi, channel);
                    }

                    var outName = Tools.File.GenOutName([file]);
                    outName = ValCvt.AppendSuff(outName, channel);
                    Tools.File.OutputValues(outputType, values, outputDir, outName);
                }
                catch (Exception e)
                {
                    Tools.MsgB.OkErr($"分析文件\"{file.Name}\"时出错：{e.Message}", "出错，已跳过");
                }
            });

            Tools.MsgB.OkInfo("分析完成", "提示");
        }
    }
}
