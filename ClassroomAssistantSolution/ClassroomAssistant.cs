//// Import packages
//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.ChatCompletion;
//using Microsoft.SemanticKernel.Connectors.OpenAI;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.DependencyInjection;
//using Azure.Identity;
//using OpenTelemetry.Logs;
//using Azure.Monitor.OpenTelemetry.Exporter;
//using OpenTelemetry.Resources;

//// Populate values from your OpenAI deployment
//var ModelId = "gpt-4o";
//var endpoint = "https://learn-o-tron.openai.azure.com/"; //Replace with Endpoint of your OpenAI instance

//// Use DefaultAzureCredential for RBAC authentication  
//var credential = new DefaultAzureCredential();

//// Create a kernel with Azure OpenAI chat completion  
//var builder = Kernel
//    .CreateBuilder()
//    .AddAzureOpenAIChatCompletion(
//        deploymentName: ModelId,
//        endpoint: endpoint,
//        credentials: credential
//    );

//// Adding enterprise components - Logging
//var resourceBuilder = ResourceBuilder
//    .CreateDefault()
//    .AddService("TelemetryLogging");

//using var loggerFactory = LoggerFactory.Create(builder =>
//{
//    // Adding OpenTelemetry as a logging provider
//    builder.AddOpenTelemetry(options =>
//    {
//        options.SetResourceBuilder(resourceBuilder);
//        //Adding Console Exporter
//        options.AddConsoleExporter();
//        //Adding Application Insights Exporter
//        options.AddAzureMonitorLogExporter(options => options.ConnectionString = "<APP-INSIGHTS CONNECTION STRING>");
//        // Format log messages. This is default to false.
//        options.IncludeFormattedMessage = true;
//        options.IncludeScopes = true;
//    });
//    builder.SetMinimumLevel(LogLevel.Information);
//});

//builder.Services.AddSingleton(loggerFactory);

//// Build the kernel
//Kernel kernel = builder.Build();
//var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

////Adding Database Plugin
//var databasePlugin = new DatabasePlugin("C:\\learn-o-tron\\semantic-kernel-project\\students.json"); //Replace with correct path
//kernel.Plugins.AddFromObject(databasePlugin);

////Adding File Plugin
//var filePlugin = new FilePlugin("C:\\learn-o-tron\\semantic-kernel-project\\recommendations.txt"); //Replace with correct path
//kernel.Plugins.AddFromObject(filePlugin);

////Adding DateTime Plugin
//var dateTimePlugin = new DateTimePlugin();
//kernel.Plugins.AddFromObject(dateTimePlugin);

//// Enable planning using automatic function calling
//OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
//{
//    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
//};

//// Define the system prompt
//var systemPrompt = @"
//You are a classroom assistant whose job is to provide book recommendations for students.  

//Your task:  
//1. Read all student records from the database.
//2. For each student:  
//   - Calculate their age using their date of birth and today's date. 
//   - Recommend three books based on their age and interests.  
//3. Append the recommendations to a file, including the time of recommendation.
//You should keep updating the user about the progress of the task along with related information.
//You should automatically call the appropriate plugin functions to accomplish this task.";

//Console.WriteLine("Classroom Assistant is running...");

//var history = new ChatHistory();
//history.AddSystemMessage(systemPrompt);

//// Handle user input
//string? userInput;
//do
//{
//    Console.Write("User > ");
//    userInput = Console.ReadLine();

//    if (!string.IsNullOrWhiteSpace(userInput))
//    {
//        // Add user input to history
//        history.AddUserMessage(userInput);

//        // Get the assistant's response
//        var result = await chatCompletionService.GetChatMessageContentAsync(
//            history,
//            executionSettings: openAIPromptExecutionSettings,
//            kernel: kernel);

//        Console.WriteLine("Assistant > " + result);

//        // Add assistant response to history
//        history.AddMessage(result.Role, result.Content ?? string.Empty);
//    }
//} while (userInput is not null);