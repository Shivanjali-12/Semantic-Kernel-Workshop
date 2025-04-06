using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace OrderPizzaAgent;

public static class OrderPizzaAgent2
{
    public static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Populate values from your OpenAI deployment
        var ModelId = "gpt-4o";
        var endpoint = "https://learn-o-tron.openai.azure.com/"; //Replace with Endpoint of your OpenAI instance

        // Use DefaultAzureCredential for RBAC authentication  
        var credential = new DefaultAzureCredential();

        // Create a kernel builder with Azure OpenAI chat completion  
        var builder = Kernel
            .CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: ModelId,
                endpoint: endpoint,
                credentials: credential
            );

        // Adding enterprise components - Metrics and Logging
        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService("TelemetryLogging");

        var meterProvider = Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddMeter("Microsoft.SemanticKernel*")
            .AddAzureMonitorMetricExporter(options => options.ConnectionString = "<APP-INSIGHTS CONNECTION STRING>")
            .Build();

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            // Adding OpenTelemetry as a logging provider
            builder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(resourceBuilder);
                //Adding Console Exporter
                options.AddConsoleExporter();
                //Adding Application Insights Exporter
                options.AddAzureMonitorLogExporter(options => options.ConnectionString = "<APP-INSIGHTS CONNECTION STRING>");
                // Format log messages. This is default to false.
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
            });
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Add loggerFactory to the kernel builder
        builder.Services.AddSingleton(loggerFactory);

        // Register IPromptTemplateFactory properly
        builder.Services.AddSingleton<IPromptTemplateFactory>(_ => new KernelPromptTemplateFactory());

        // Build the kernel
        Kernel kernel = builder.Build();

        //Adding Order Pizza Plugin
        var orderPizzaPlugin = new OrderPizzaPlugin();
        kernel.Plugins.AddFromObject(orderPizzaPlugin);

        Console.WriteLine("Defining agent...");

        //Read YAML resource
        string orderPizzaYaml = File.ReadAllText("C:\\learn-o-tron\\semantic-kernel-project\\OrderPizza.yaml");

        // Convert to a prompt template config correctly
        PromptTemplateConfig templateConfig = new PromptTemplateConfig(orderPizzaYaml);

        // Create agent with Instructions, Name, and Description provided by the template config
        ChatCompletionAgent agent =
            new(templateConfig, kernel.GetRequiredService<IPromptTemplateFactory>())
            {
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
            };

        Console.WriteLine("Welcome to Shiv's Pizza! What would you like to have today?");

        ChatHistoryAgentThread agentThread = new();
        bool isComplete = false;
        do
        {
            Console.WriteLine();
            Console.Write("> ");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            if (input.Trim().Equals("EXIT", StringComparison.OrdinalIgnoreCase))
            {
                isComplete = true;
                break;
            }

            var message = new ChatMessageContent(AuthorRole.User, input);

            Console.WriteLine();

            DateTime now = DateTime.Now;
            KernelArguments arguments =
                new()
                {
                    { "now", $"{now.ToShortDateString()} {now.ToShortTimeString()}" }
                };
            await foreach (ChatMessageContent response in agent.InvokeAsync(message, agentThread, options: new() { KernelArguments = arguments }))
            {
                // Display response.
                Console.WriteLine($"{response.Content}");
            }

        } while (!isComplete);
    }
}