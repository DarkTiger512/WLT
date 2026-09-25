using WarlordAwajiTwitch.Adoption;
using WarlordAwajiTwitch.Commands;
using WarlordAwajiTwitch.Game;
using WarlordAwajiTwitch.Twitch;

namespace WarlordAwajiTwitch.Tests;

public sealed class CommandRouterTests
{
    [Fact]
    public async Task RouteAsync_ForAdoptCommand_PersistsRecordAndCallsIntegration()
    {
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "adoptions.json");
        var store = new AdoptionStore(filePath);
        var integration = new RecordingGameIntegration();
        var service = new AdoptionService(store, integration);
        var command = new AdoptCommand(service);
        var router = new CommandRouter(new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
        {
            [command.Name] = command,
        });

        var result = await router.RouteAsync(new TwitchMessage("viewer_one", "!adopt")).ConfigureAwait(false);
        var records = await store.LoadAsync().ConfigureAwait(false);

        Assert.True(result.Handled);
        Assert.True(result.Succeeded);
        Assert.Single(records);
        Assert.Equal("viewer_one", records[0].ViewerName);
        Assert.Single(integration.ReceivedViewerNames);
        Assert.Equal("viewer_one", integration.ReceivedViewerNames[0]);
    }

    [Fact]
    public async Task RouteAsync_ForRejectedAdoptCommand_DoesNotPersistRecord()
    {
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "adoptions.json");
        var store = new AdoptionStore(filePath);
        var integration = new RecordingGameIntegration(accepted: false);
        var service = new AdoptionService(store, integration);
        var command = new AdoptCommand(service);
        var router = new CommandRouter(new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
        {
            [command.Name] = command,
        });

        var result = await router.RouteAsync(new TwitchMessage("viewer_one", "!adopt")).ConfigureAwait(false);
        var records = await store.LoadAsync().ConfigureAwait(false);

        Assert.True(result.Handled);
        Assert.False(result.Succeeded);
        Assert.Empty(records);
        Assert.Single(integration.ReceivedViewerNames);
        Assert.Equal("viewer_one", integration.ReceivedViewerNames[0]);
    }

    [Theory]
    [InlineData("!adopt soon")]
    [InlineData("!adopt\tsoon")]
    public async Task RouteAsync_ForCommandWithWhitespaceSeparatedArguments_ParsesCommandName(string messageText)
    {
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "adoptions.json");
        var store = new AdoptionStore(filePath);
        var integration = new RecordingGameIntegration();
        var service = new AdoptionService(store, integration);
        var command = new AdoptCommand(service);
        var router = new CommandRouter(new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
        {
            [command.Name] = command,
        });

        var result = await router.RouteAsync(new TwitchMessage("viewer_one", messageText)).ConfigureAwait(false);

        Assert.True(result.Handled);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task RouteAsync_ForUnknownCommand_DoesNotHandleMessage()
    {
        var router = new CommandRouter(new Dictionary<string, ICommand>());

        var result = await router.RouteAsync(new TwitchMessage("viewer_one", "!unknown")).ConfigureAwait(false);

        Assert.False(result.Handled);
        Assert.False(result.Succeeded);
        Assert.Equal("Unknown command: unknown", result.Message);
    }

    private sealed class RecordingGameIntegration : IGameIntegration
    {
        private readonly bool _accepted;

        public RecordingGameIntegration(bool accepted = true)
        {
            _accepted = accepted;
        }

        public List<string> ReceivedViewerNames { get; } = new();

        public Task<GameIntegrationResult> RegisterAdoptionAsync(AdoptionRecord record, CancellationToken cancellationToken = default)
        {
            ReceivedViewerNames.Add(record.ViewerName);
            return Task.FromResult(new GameIntegrationResult(_accepted, $"{record.ViewerName} recorded"));
        }
    }
}
