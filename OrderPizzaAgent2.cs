using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.IO;
using System.Threading.Tasks;

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

        // Create a kernel with Azure OpenAI chat completion  
        var builder = Kernel
            .CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: ModelId,
                endpoint: endpoint,
                credentials: credential
            );

        // Register IPromptTemplateFactory properly
        builder.Services.AddSingleton<IPromptTemplateFactory>(_ => new KernelPromptTemplateFactory());

        // Build the kernel
        Kernel kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Adding Database Plugin
        var orderPizzaPlugin = new OrderPizzaPlugin();
        kernel.Plugins.AddFromObject(orderPizzaPlugin);

        Console.WriteLine("Defining agent...");

        // Read YAML resource
        string orderPizzaYaml = File.ReadAllText("C:\\learn-o-tron\\semantic-kernel-project\\OrderPizza.yaml");
        // Convert to a prompt template config correctly
        PromptTemplateConfig templateConfig = new PromptTemplateConfig(orderPizzaYaml);
        // Create agent with Instructions, Name, and Description provided by the template config
        ChatCompletionAgent agent =
            new(templateConfig, kernel.GetRequiredService<IPromptTemplateFactory>())
            {
                Kernel = kernel,
                // Provide default values for template parameters
                Arguments = new KernelArguments()
                {
                    { "step", "Start" } // Default step
                }
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
                    { "now", now.ToShortDateString() + " " + now.ToShortTimeString() }
                };
            await foreach (ChatMessageContent response in agent.InvokeAsync(message, agentThread, options: new() { KernelArguments = arguments }))
            {
                // Display response.
                Console.WriteLine(response.Content);
            }

        } while (!isComplete);
    }
}
