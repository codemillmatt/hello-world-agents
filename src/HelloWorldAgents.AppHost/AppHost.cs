using Aspire.Hosting.GitHub;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var chatModel = builder.AddGitHubModel("chat", GitHubModel.OpenAI.OpenAIGPT4oMini);

var api =
    builder.AddProject<HelloWorldAgents_API>("api")
        .WithIconName("brainSparkle")
        .WithEnvironment("MODEL_NAME", GitHubModel.OpenAI.OpenAIGPT4oMini.Id)
        .WithReference(chatModel);

if (builder.Environment.IsDevelopment())
    api.WithUrl("/index.html", "ChatUI");

builder.Build().Run();
