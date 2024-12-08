# [像素值统计工具](https://github.com/GarthTB/LightCurve)

一个Windows小工具，用于生成一组图片特定区域像素值的变化曲线。

## 功能

- 统计一组图片或一段视频中，特定区域像素的被测指标的平均值。
- 将这个平均值的变化轨迹绘制为一个折线图，或输出为一个txt列表。

## 环境要求

- [.NET 9.0运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/9.0)

## 注意

- 此程序不使用GPU。在统计大文件时可能卡顿。
- 每一组照片或一个视频文件进行一次统计。照片和视频不能混合处理。
- 选择输出位置时的文件名不是最终的文件名。最终的文件根据原文件来命名。
- 折线图的高度固定为2000像素，宽度在2000至4800之间浮动。

## 快捷键

- F1：查看帮助和软件信息

## Credits

- [OpenCvSharp](https://github.com/shimat/opencvsharp)
- [ScottPlot](https://github.com/ScottPlot/ScottPlot)

## 版本日志

### [v0.2.0](https://github.com/GarthTB/LightCurve/releases/tag/v0.2.0) - 20241208

- 修复：特殊视频文件无法读取的问题
- 优化：微调界面，增加拖放功能
- 优化：折线图外观

### [v0.1.0](https://github.com/GarthTB/LightCurve/releases/tag/v0.1.0) - 20241208

- 发布