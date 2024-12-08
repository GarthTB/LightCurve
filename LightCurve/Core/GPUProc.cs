using ILGPU;
using ILGPU.Runtime;

namespace LightCurve.Core
{
    /// <summary> GPU处理 </summary>
    internal static class GPUProc
    {
        /// <summary> 计算一组数据的均值 </summary>
        internal static double Mean(double[] data)
        {
            // 创建ILGPU上下文和加速器
            using var context = Context.CreateDefault();
            using var accelerator = context.GetPreferredDevice(false).CreateAccelerator(context);

            // 将数据复制到GPU内存
            using var buffer = accelerator.Allocate1D(data);
            using var resultBuffer = accelerator.Allocate1D<double>(1);

            // 调用GPU核函数计算均值
            var kernel = accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<double>, ArrayView<double>, int>(Kernel);
            kernel(1, buffer.View, resultBuffer.View, data.Length);

            // 将结果复制到主机内存并返回
            accelerator.Synchronize();
            return resultBuffer.GetAsArray1D()[0];
        }

        /// <summary> GPU的核函数 </summary>
        private static void Kernel(Index1D index, ArrayView<double> data, ArrayView<double> result, int length)
        {
            double sum = 0.0;
            for (int i = 0; i < length; i++)
                sum += data[i];
            result[0] = sum / length;
        }
    }
}
