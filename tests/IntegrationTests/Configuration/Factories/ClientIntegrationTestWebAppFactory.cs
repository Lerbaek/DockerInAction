namespace IntegrationTests.Configuration.Factories;

/// <summary>
/// Integration test web application factory for the Client application.
/// <para>
/// This factory creates an in-memory test server hosting the Client application,
/// allowing tests to interact directly with the Client's HTTP endpoints and messaging components.
/// </para>
/// </summary>
/// <remarks>
/// This factory is used primarily in hybrid testing approaches where the Client application
/// runs in-process while the Server and RabbitMQ run in containers. This approach provides:
/// <list type="bullet">
/// <item>Fast test execution with direct access to the Client's internal components</item>
/// <item>The ability to mock or substitute Client dependencies</item>
/// <item>Integration with real containerized dependencies for comprehensive testing</item>
/// </list>
/// </remarks>
public class ClientIntegrationTestWebAppFactory : IntegrationTestWebAppFactory<Client.Program>;