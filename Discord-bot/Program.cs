using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;

public class Program
{
    private DiscordSocketClient _client  = new DiscordSocketClient();
    private CommandService _commands = new CommandService();

    public static void Main(string[] args) 
    {
        new Program().MainAsync().GetAwaiter().GetResult();
    }

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        _commands = new CommandService();

        _client.Ready += Client_Ready;
        _client.SlashCommandExecuted += HandleSlashCommand;

        var token = "YOUR_TOKEN";
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    public async Task Client_Ready()
    {
        var globalCommand = new SlashCommandBuilder();
        globalCommand.WithName("ping");
        globalCommand.WithDescription("test ping command");

        Console.WriteLine($"{_client.CurrentUser} siap digunakan.");

        try {
            await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
        } catch (HttpException e) {
            Console.WriteLine(e.ToString());
        }
    }

    private async Task HandleSlashCommand(SocketSlashCommand command)
    {
        var context = new BotCommandsContext(command);
        await context.CmdAction();
    }
}