using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace OrderPizzaAgent;

public static class OrderPizzaAgent
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


        // Build the kernel
        Kernel kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        //Adding Database Plugin
        var orderPizzaPlugin = new OrderPizzaPlugin();
        kernel.Plugins.AddFromObject(orderPizzaPlugin);

        Console.WriteLine("Defining agent...");
        ChatCompletionAgent agent =
            new()
            {
                Name = "PizzaOrderAgent",
                Instructions =
                    """
                    You are an agent that helps users order pizzas step by step.
                    Follow this process:
                    1. Ask for pizza size (Small, Medium, Large).
                    2. Ask for toppings.
                    3. Confirm the order.
                    4. Place the order when the user confirms.
                    """,
                Kernel = kernel
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