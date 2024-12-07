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
            foreach (var file in files)
            {
                try
                {
                    var vid = new VideoCapture(file.FullName);
                    var values = new double[vid.FrameCount];

                    Mat frame = new();
                    for (int i = 0; vid.Read(frame); i++)
                    {
                        var roi = ImgProc.GetROI(frame, x, y, w, h);
                        values[i] = ImgProc.GetValue(roi, channel);
                    }

                    var outName = Tools.File.GenOutName([file]);
                    outName = ValCvt.AppendSuff(outName, channel);
                    switch (outputType)
                    {
                        case 0:
                            Tools.File.OutputTxt(values, outputDir, outName);
                            break;
                        case 1:
                            Tools.File.OutputPlot(values, outputDir, outName);
                            break;
                        default:
                            Tools.File.OutputTxt(values, outputDir, outName);
                            Tools.File.OutputPlot(values, outputDir, outName);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Tools.MsgB.OkErr($"分析文件\"{file.Name}\"时出错：{e.Message}", "出错，已跳过");
                }
            }
            Tools.MsgB.OkInfo("分析完成", "提示");
        }
    }
}
