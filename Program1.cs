// Import packages
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;

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

// Add enterprise components
builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Information));

// Build the kernel
Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Add Lights plugin
kernel.Plugins.AddFromType<LightsPlugin>();

//Add Time Plugin
kernel.Plugins.AddFromType<TimePlugin>();

// Enable planning using automatic function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Define the system prompt
var systemPrompt = @"
You are a smart home assistant responsible for managing lights.
Your task is to ensure that all the lights are turned on or off according to following conditions:
- Bedroom Lights: Should be turned on between 8AM-10PM, otherwise turned off.
- Porch Light: Ensure it's always on.
- Chandelier: Should be turned on between 10PM-8AM, otherwise turned off.
- Table Lamp: Turn on or off based on user input. You should always ask the user for input.
You should automatically call the appropriate plugin functions to accomplish this task.";

var history = new ChatHistory();
history.AddSystemMessage(systemPrompt);

Console.WriteLine("SmartHome Assistant is running...");

// Handle user input
string? userInput;
do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(userInput))
    {
        // Add user input to history
        history.AddUserMessage(userInput);

        // Get the assistant's response
        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel);

        Console.WriteLine("Assistant > " + result);

        // Add assistant response to history
        history.AddMessage(result.Role, result.Content ?? string.Empty);
    }
} while (userInput is not null);