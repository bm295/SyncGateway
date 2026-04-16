using System.ComponentModel.DataAnnotations;

namespace SyncGateway.Api.Security;

public sealed class GatewayOptions
{
    public const string SectionName = "Gateway";

    [Required]
    public string ApiKey { get; init; } = "local-dev-api-key";
}
