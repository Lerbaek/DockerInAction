using IntegrationTests.Tests.Testcontainers.EndToEnd;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <inheritdoc cref="ClientTestHarnessCollection"/>
[CollectionDefinition(nameof(EndToEndTestcontainersCollection))]
public sealed class EndToEndTestcontainersCollection : ICollectionFixture<EndToEndTestFixtures>;