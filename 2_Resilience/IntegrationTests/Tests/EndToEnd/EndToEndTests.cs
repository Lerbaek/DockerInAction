using FluentAssertions;
using IntegrationTests.Configuration.Fixtures;
using Testcontainers.RabbitMq;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Tests.EndToEnd;

public class EndToEndTests : IAsyncLifetime
{
    private readonly RabbitMqFixture _rabbitMqFixture = new();
    private readonly ServerFixture _serverFixture = new();
    private readonly ClientFixture _clientFixture = new();

    private readonly ITestOutputHelper _output;

    /// <summary>
    /// End-to-end test the entire solution using dynamically created images containers for the <see cref="Client"/>> and <see cref="Server"/>> services.
    /// </summary>
    /// <param name="output"></param>
    public EndToEndTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task ClientGeneratePayment_GetWithoutHeaders_ServerLogsSuccess()
    {
        // Arrange
        var hostname = _clientFixture.Hostname;
        var port = _clientFixture.Port;

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{hostname}:{port}"),
        };

        const string expectedOutput = "Payment succeeded";
        var startTime = DateTime.Now;

        // Act
       await httpClient.GetAsync("/PaymentGenerator");

        // Assert
        var log = string.Empty;

        SpinWait.SpinUntil(() =>
        {
            log = _serverFixture.GetLogsAsync(since: startTime)
                .Result.Stdout;
            return log.Contains(expectedOutput);
        }, TimeSpan.FromSeconds(10));

        _output.WriteLine(log);
        log.Should().Contain(expectedOutput);
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        await _rabbitMqFixture.InitializeAsync();
        await Task.WhenAll(
            _serverFixture.InitializeAsync(),
            _clientFixture.InitializeAsync()
        );


        // Print link to management interface in test output
        _output.WriteLine(
            $"RabbitMQ Management interface: http://localhost:{_rabbitMqFixture.ManagementPort}");
    }
    
    /// <inheritdoc/>
    public async Task DisposeAsync() =>
        await Task.WhenAll(
            _clientFixture.DisposeAsync(),
            _serverFixture.DisposeAsync(),
            _rabbitMqFixture.DisposeAsync());
}