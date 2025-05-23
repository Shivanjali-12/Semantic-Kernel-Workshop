﻿using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

/// <summary>
/// Example of function invocation filter to perform logging before and after function invocation.
/// </summary>
public class LoggingFilter(ILogger logger) : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        logger.LogInformation("FunctionInvoking - {PluginName}.{FunctionName}", context.Function.PluginName, context.Function.Name);

        await next(context);

        logger.LogInformation("FunctionInvoked - {PluginName}.{FunctionName}", context.Function.PluginName, context.Function.Name);
    }
}

/// <summary>
/// Example of function invocation filter to perform input validation before function invocation.
/// </summary>
public class InputValidationFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        var input = context.Arguments.ToString();

        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be empty.");
        }

        await next(context);
    }
}

/// <summary>
/// Example of prompt render filter which replaces "something bad" with "something good" in the rendered prompt before sending it to AI.
/// </summary>
public class SafePromptFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        var originalPrompt = context.RenderedPrompt;

        var modifiedPrompt = originalPrompt.Replace("something bad", "something safe");

        context.RenderedPrompt = modifiedPrompt;

        await next(context);
    }
}