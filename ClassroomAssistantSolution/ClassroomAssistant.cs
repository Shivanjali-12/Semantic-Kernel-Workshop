//// Import packages
//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.ChatCompletion;
//using Microsoft.SemanticKernel.Connectors.OpenAI;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.DependencyInjection;
//using Azure.Identity;

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

//// Add enterprise components
//builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Information));

//// Build the kernel
//Kernel kernel = builder.Build();
//var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

////Add Database Plugin
//var databasePlugin = new DatabasePlugin.DatabasePlugin("C:\\learn-o-tron\\semantic-kernel-project\\students.json"); //Replace with correct path
//kernel.Plugins.AddFromObject(databasePlugin);

////Add File Plugin
//var filePlugin = new FilePlugin("C:\\learn-o-tron\\semantic-kernel-project\\recommendations.txt"); //Replace with correct path
//kernel.Plugins.AddFromObject(filePlugin);

////Add Time Plugin
//kernel.Plugins.AddFromType<TimePlugin>();

//// Enable planning using automatic function calling
//OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
//{
//    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
//};

//// Define the system prompt
//var systemPrompt = @"
//You are a classroom assistant who provides 3 book recommendations for students.  

//Your task:  
//1. Read all student records from the database.
//2. For each student:  
//   - Calculate their age using their date of birth and today's date. 
//   - Recommend three books based on their age and interests.  
//3. Append the recommendations to a file, including the time of recommendation.
//You should keep updating the user about the progress of the task along with related information.
//You should automatically call the appropriate plugin functions to accomplish this task.";

//var history = new ChatHistory();
//history.AddSystemMessage(systemPrompt);

//Console.WriteLine("Classroom Assistant is running...");

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
