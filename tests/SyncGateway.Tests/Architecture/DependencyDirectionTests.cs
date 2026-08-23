using SyncGateway.Application.Abstractions;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Tests.Architecture;

public sealed class DependencyDirectionTests
{
    [Fact]
    public void Domain_does_not_reference_outer_sync_gateway_layers()
    {
        var references = typeof(SyncChange).Assembly.GetReferencedAssemblies();

        Assert.DoesNotContain(references, IsOuterSyncGatewayLayer);
    }

    [Fact]
    public void Application_only_references_the_domain_sync_gateway_layer()
    {
        var syncGatewayReferences = typeof(ISyncEngine).Assembly
            .GetReferencedAssemblies()
            .Where(reference => reference.Name?.StartsWith("SyncGateway.", StringComparison.Ordinal) == true)
            .Select(reference => reference.Name)
            .ToArray();

        Assert.Equal(new[] { "SyncGateway.Domain" }, syncGatewayReferences);
    }

    private static bool IsOuterSyncGatewayLayer(System.Reflection.AssemblyName reference) =>
        reference.Name is "SyncGateway.Api" or "SyncGateway.Application" or "SyncGateway.Infrastructure";
}
