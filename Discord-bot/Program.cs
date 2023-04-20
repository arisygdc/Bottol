using Discord;
using Discord.Commands;
using Discord.WebSocket;
public class Program
{
    private DiscordSocketClient _client;
    private CommandService _commands;

    public static void Main(string[] args) =>
        new Program().MainAsync().GetAwaiter().GetResult();

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        _commands = new CommandService();

        var token = "token";
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }
}