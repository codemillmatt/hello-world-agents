using Microsoft.Agents.Workflows;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OllamaSharp;
using OpenAI.Chat;
using Azure.AI.Inference;
using Azure;

IChatClient chatClient =
    new ChatCompletionsClient(
        new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(Environment.GetEnvironmentVariable("GITHUB_TOKEN")!))
        .AsIChatClient("gpt-4o-mini");

AIAgent writer = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "Writer",
        Instructions = "Write stories that are engaging and creative.",
        ChatOptions = new ChatOptions
        {
            Tools = [
                AIFunctionFactory.Create(GetAuthor),
                AIFunctionFactory.Create(FormatStory)
            ],
        }
    });

AIAgent editor = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "Editor",
        Instructions = "Make the story more engaging, fix grammar, and enhance the plot."
    });

// Using AgentWorkflowBuilder for GroupChat
Workflow workflow =
    AgentWorkflowBuilder
        .BuildSequential(writer, editor);

AIAgent workflowAgent = await workflow.AsAgentAsync();

AgentRunResponse workflowResponse =
    await workflowAgent.RunAsync("Write a short story about a haunted house.");

Console.WriteLine(workflowResponse.Text);

[Description("Gets the author of the story.")]
string GetAuthor() => "Jack Torrance";

[Description("Formats the story for display.")]
string FormatStory(string title, string author, string story) =>
    $"Title: {title}\nAuthor: {author}\n\n{story}";