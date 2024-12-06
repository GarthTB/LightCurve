﻿using System.Reflection;
using System.Text;

namespace LightCurve.Core
{
    internal static class Help
    {
        internal static void Show()
        {
            var version = Assembly.GetExecutingAssembly()
                                  .GetName()
                                  .Version?
                                  .ToString()
                                  ?? "未知";
            var sb = new StringBuilder();
            var help = sb.AppendLine("欢迎使用像素值统计工具！\n")
                         .AppendLine("本工具会统计一组照片或一段视频中")
                         .AppendLine("指定区域内被测指标的变化，")
                         .AppendLine("然后输出为一个列表，或渲染为一个折线图。")
                         .AppendLine("详见README.md。\n")
                         .AppendLine($"版本号：{version}")
                         .AppendLine("作者：GarthTB\n")
                         .ToString();
            Tools.MsgB.OkInfo(help, "帮助");
        }
    }
}