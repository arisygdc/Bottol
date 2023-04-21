using Discord.WebSocket;
public class BotCommandsContext
{
    private SocketSlashCommand cmd;
    public BotCommandsContext(SocketSlashCommand command)
        => cmd = command;

    public async Task CmdAction()
    {
        switch (this.cmd.Data.Name)
        {
            case "ping":
                await Ping(cmd);
                break;
        }
    }

    private async Task Ping(SocketSlashCommand command)
    {
        await command.RespondAsync("pong!");
    }
}