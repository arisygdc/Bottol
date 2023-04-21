using Discord.WebSocket;
// using System;
// using System.Net.Http;
// using System.Threading.Tasks;
using Newtonsoft.Json;

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
            case "say":
                await Translate(cmd);
                break;
        }
    }

    private async Task Ping(SocketSlashCommand command)
    {
        await command.RespondAsync("pong!");
    }

    private async Task Translate(SocketSlashCommand command)
    {
        var text = command.Data.Options.First().Value.ToString();
        var url = $"http://localhost:3000/translate?text={text}";
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode) {
            await command.RespondAsync("service translator bejat");
            return;
        }
        
        var result = await response.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<TranslatorResponse>(result);
        if (json == null) {
            await command.RespondAsync("service translator bejat");
            return;
        }

        Console.WriteLine(json.Text);
        await command.RespondAsync(json.Text);
    }
}