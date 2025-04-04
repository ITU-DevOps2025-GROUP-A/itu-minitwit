using System.Diagnostics;
using ArxOne.MrAdvice.Advice;
using Serilog;

namespace Api.Services.LogDecorator;

[AttributeUsage(AttributeTargets.Method)]
public class LogTimeAttribute : Attribute, IMethodAdvice
{
    public void Advise(MethodAdviceContext context)
    {
        var methodName = $"{context.TargetType.FullName}.{context.TargetMethod.Name}";
        var stopwatch = Stopwatch.StartNew();

        try
        {
            context.Proceed();
        }
        finally
        {
            stopwatch.Stop();
            Log.Information("Finished {MethodName} (Execution Time: {ElapsedMilliseconds} ms)", methodName, stopwatch.ElapsedMilliseconds);
        }
    }
}