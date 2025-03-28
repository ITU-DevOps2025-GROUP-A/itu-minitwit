using System.Reflection;
using ArxOne.MrAdvice.Advice;
using Serilog;
using Serilog.Context;

namespace Api.Services.LogDecorator;

[AttributeUsage(AttributeTargets.Method)]
public class LogMethodParametersAttribute : Attribute, IMethodAdvice
{
    public void Advise(MethodAdviceContext context)
    {
        
        // Get the current method info
        MethodBase method = context.TargetMethod;
        var methodName = $"{context.TargetType.FullName}.{method.Name}";
        
        // Get method arguments
        var args = method.GetParameters()
            .Select((p, i) => $"{p.Name}: {context.Arguments[i]}")
            .ToArray();
        
        var dynamicInstance = GenerateDynamicTypes.CreateDynamicInstance(args);
        
        var sourceContext = context.TargetMethod.DeclaringType?.FullName ?? "UnknownSource";
        using (LogContext.PushProperty("SourceContext", sourceContext))
        {
            Log.Information("Called: {MethodName}, with {@Arguments}", 
                methodName, dynamicInstance);
        }
        
        // Proceed with the original method execution
        context.Proceed();
    }
}