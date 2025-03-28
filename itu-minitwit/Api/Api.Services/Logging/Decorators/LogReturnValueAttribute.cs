using ArxOne.MrAdvice.Advice;
using Serilog;
using Serilog.Context;

namespace Api.Services.LogDecorator;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
public class LogReturnValueAttribute : Attribute, IMethodAdvice
{
    public void Advise(MethodAdviceContext context)
    {
        var sourceContext = context.TargetMethod.DeclaringType?.FullName ?? "UnknownSource";
        using (LogContext.PushProperty("SourceContext", sourceContext))
        {
            context.Proceed();

            var returnValue = context.ReturnValue;
            var methodName = $"{context.TargetType.FullName}.{context.TargetMethod.Name}";
            Log.Information("Returning: \"{MethodName}\" returned: {@ReturnValue}",
                methodName, returnValue);
        }
    }
}