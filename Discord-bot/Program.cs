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
        var token = "YOUR_TOKEN";
        _client = new DiscordSocketClient();
        _commands = new CommandService();

        _client.Ready += Client_Ready;
        _client.SlashCommandExecuted += HandleSlashCommand;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    public async Task Client_Ready()
    {
        var pingCommand = new SlashCommandBuilder()
        .WithName("ping")
        .WithDescription("test ping command");

        var sayCommand = new SlashCommandBuilder()
        .WithName("say")
        .WithDescription("i will say what you want")
        .AddOption("text", ApplicationCommandOptionType.String, "text to say", true);

        var syntsesisTest = new SlashCommandBuilder()
        .WithName("syntsesistest")
        .WithDescription("say \"siapa namamu\" in japanese");

        Console.WriteLine($"{_client.CurrentUser} siap digunakan.");
        

        try {
            await _client.CreateGlobalApplicationCommandAsync(pingCommand.Build());
            await _client.CreateGlobalApplicationCommandAsync(sayCommand.Build());
            await _client.CreateGlobalApplicationCommandAsync(syntsesisTest.Build());
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