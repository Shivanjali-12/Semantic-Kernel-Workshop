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

//// Add enterprise components here


//// Build the kernel
//Kernel kernel = builder.Build();
//var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

////Add Database Plugin
//var databasePlugin = new DatabasePlugin.DatabasePlugin("C:\\learn-o-tron\\semantic-kernel-project\\students.json"); //Replace with correct path
//kernel.Plugins.AddFromObject(databasePlugin);

////Add Time Plugin here


////Add File Plugin here


//// Enable planning using automatic function calling
//OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
//{
//    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
//};

////Modify the system prompt according to following use case:
////  - A Classroom assistant that provides 3 book recommendations for students based on their age and interests.
////  - Appends the Recommendations and time of recommendations to a text file.

////Modify the below section with with implementation of above use case
////Implement Chat history also

//// Define the system prompt
//var systemPrompt = @"
//You are a classroom assistant who provides 3 book recommendations for students.
//Your task is to read all student records from the database and for each student, recommend three books based on their interests.";

//Console.WriteLine("Classroom Assistant is running...");

//Console.Write("User > ");

//Console.ReadLine();

//var result = await chatCompletionService.GetChatMessageContentAsync(
//            systemPrompt,
//            executionSettings: openAIPromptExecutionSettings,
//            kernel: kernel);

//Console.WriteLine("Assistant > " + result);