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
        .WithName("tts-test")
        .WithDescription("say \"siapa namamu\" in japanese");

        var text2img = new SlashCommandBuilder()
        .WithName("imagine")
        .WithDescription("text to image")
        .AddOptions(
            new SlashCommandOptionBuilder()
            .WithName("prompt")
            .WithDescription("text to imagine")
            .WithRequired(true)
            .WithType(ApplicationCommandOptionType.String),
            new SlashCommandOptionBuilder()
            .WithName("negative_prompt")
            .WithDescription("tweak")
            .WithRequired(false)
            .WithType(ApplicationCommandOptionType.String),
            new SlashCommandOptionBuilder()
            .WithName("sampling_steps")
            .WithDescription("may produce a slightly different picture, but not necessarily better quality")
            .WithRequired(false)
            .WithType(ApplicationCommandOptionType.Integer),
            new SlashCommandOptionBuilder()
            .WithName("cfg_scale")
            .WithDescription("how closely Stable Diffusion should follow your text prompt")
            .WithRequired(false)
            .WithType(ApplicationCommandOptionType.Integer),
            new SlashCommandOptionBuilder()
            .WithName("sampling_method")
            .WithDescription("sampling method")
            .WithRequired(false)
            .WithType(ApplicationCommandOptionType.String)
            .AddChoice("DPM++ 2M Karras", "DPM++ 2M Karras")
            .AddChoice("UniPC", "UniPC")
            .AddChoice("DPM++ SDE Karras", "DPM++ SDE Karras")
        );
        Console.WriteLine($"{_client.CurrentUser} siap digunakan.");
        Console.WriteLine("-----------------------");
        
        try {
            
            await _client.CreateGlobalApplicationCommandAsync(pingCommand.Build());
            await _client.CreateGlobalApplicationCommandAsync(sayCommand.Build());
            await _client.CreateGlobalApplicationCommandAsync(syntsesisTest.Build());
            await _client.CreateGlobalApplicationCommandAsync(text2img.Build());
        
        } catch (HttpException e) {
            Console.WriteLine(e.ToString());
        }

        var cmds = _client.GetGlobalApplicationCommandsAsync();
        foreach (var cmd in cmds.Result)
        {
            Console.WriteLine(cmd.Name);
        }
    }

    private async Task HandleSlashCommand(SocketSlashCommand command)
    {
        var context = new BotCommandsContext(command);
        await context.CmdAction();
    }
}