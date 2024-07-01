using System.Text.RegularExpressions;
using Clouded.Core.DataSource.Shared;
using Clouded.Platform.App.Web.Options;
using OpenAI_API;
using OpenAI_API.Models;

namespace Clouded.Platform.App.Web.Services.AI;

public partial class SqlAIService(ApplicationOptions options) : ISqlAIService
{
    private readonly OpenAIAPI _aiClient = new(new APIAuthentication(options.OpenAi.ApiKey));

    public async Task<string> Generate(
        DatabaseType type,
        string schema,
        string input,
        CancellationToken cancellationToken = default
    )
    {
        schema = EveryStartOfLineRegex().Replace(schema, "# ");

        // TODO: Hardcoded for SELECT
        // TODO: Create enum SELECT/INSERT/UPDATE/DELETE and let user to decide
        const string action = "SELECT";

        var prompt = $"""
            ### {Enum.GetName(type)} SQL schema:
            #
            {schema}
            #
            ### {input}
            {action}
            """;

        var output = await _aiClient.Completions.CreateCompletionAsync(
            prompt,
            Model.DavinciCode,
            500,
            0.5d,
            1d,
            1,
            0d,
            0d,
            0,
            false,
            "#",
            ";"
        );

        return $"{action} {string.Join("", output.Completions?.Select(x => x.Text) ?? Array.Empty<string>())}";
    }

    [GeneratedRegex("^", RegexOptions.Multiline)]
    private static partial Regex EveryStartOfLineRegex();
}
