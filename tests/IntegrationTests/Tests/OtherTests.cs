using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using FluentAssertions;
using Testcontainers.RabbitMq;
using Xunit;

namespace IntegrationTests.Tests
{
    public class OtherTests
    {
        [Fact]
        public async Task StartRabbitMq()
        {
            TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.11.0");
            RabbitMqContainer container = new RabbitMqBuilder()
                .WithImage("rabbitmq:3.11.20-management")
                .WithPortBinding(15672, assignRandomHostPort: true)
                .WithName($"rabbitmq-{Guid.NewGuid()}")
                .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Integration")
                .Build();

            await container.StartAsync();

            container.State.Should().Be(TestcontainersStates.Running);
        }
    }
}
