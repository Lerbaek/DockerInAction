using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Images;
using Testcontainers.RabbitMq;
using Xunit;

namespace IntegrationTests.Tests
{
    public class OtherTests
    {
        [Fact]
        public async Task StartRabbitMq()
        {
            TestcontainersSettings.ResourceReaperImage = new DockerImage("remote-docker-hub.artifactory.danskenet.net/testcontainers/ryuk:0.5.1");
            RabbitMqContainer container = new RabbitMqBuilder()
                .WithImage("remote-docker-hub.artifactory.danskenet.net/rabbitmq:3.11.20-management")
                .WithPortBinding(15672, assignRandomHostPort: true)
                .WithName($"rabbitmq-{Guid.NewGuid()}")
                .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Integration")
                .Build();

            await container.StartAsync();

        }
    }
}
