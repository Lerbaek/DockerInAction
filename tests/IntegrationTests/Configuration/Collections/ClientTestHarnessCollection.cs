using IntegrationTests.Configuration.Factories;
using Xunit;

namespace IntegrationTests.Configuration.Collections;

/// <summary>
/// xUnit collection definition for Client application tests using the in-memory test harness approach.
/// <para>
/// This class defines a test collection that shares the <see cref="ClientIntegrationTestWebAppFactory"/>
/// fixture across all tests in the collection, ensuring the web application is only started once.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// This class has no code and is never created. Its purpose is simply to be the place to apply 
/// <see cref="CollectionDefinitionAttribute"/> and the <see cref="ICollectionFixture{TFixture}"/> interface.
/// </para>
/// <para>
/// For more details, see: <see href="https://xunit.net/docs/shared-context"/>
/// </para>
/// </remarks>
[CollectionDefinition(nameof(ClientTestHarnessCollection))]
public sealed class ClientTestHarnessCollection : ICollectionFixture<ClientIntegrationTestWebAppFactory>;