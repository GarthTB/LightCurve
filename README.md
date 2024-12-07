# [像素值统计工具](https://github.com/GarthTB/LightCurve)

一个Windows小工具，用于生成一组图片特定区域像素值的变化曲线。

## 功能

- 统计一组图片或一段视频中，特定区域像素的被测指标的平均值。
- 将这个平均值的变化轨迹输出为一个txt列表，或绘制为一个折线图。

## 环境要求

- [.NET 9.0运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/9.0)

## 注意

- 一组照片或一个视频文件进行一次统计。照片和视频不能混合处理。多个视频可以并行处理。
- 选择输出位置时的文件名不是最终的文件名。最终文件根据原文件来命名。
- gif图属于视频，也可以分析。

## 快捷键

- F1：帮助

## Credits

- [OpenCvSharp](https://github.com/shimat/opencvsharp)
- [ScottPlot](https://github.com/ScottPlot/ScottPlot)