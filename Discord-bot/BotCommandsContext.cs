using Discord.WebSocket;
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
                await Ping();
                break;
            case "say":
                var text = this.cmd.Data.Options.First().Value.ToString();
                if (text == null)
                    await this.cmd.RespondAsync("text tidak boleh kosong");
                    
                await this.cmd.RespondAsync(text);
                try {
                    var translatedText = await Translate(text);
                    var synthesisStream = await Synthesis(translatedText);
                    await this.cmd.FollowupWithFileAsync(synthesisStream, "audio.mp3");
                } catch (Exception e) {
                    await this.cmd.FollowupAsync(e.ToString());
                    Console.WriteLine(e);
                }
                break;
            case "syntsesistest":
                var textTest = "あなたの名前は何ですか";
                await this.cmd.RespondAsync(textTest);
                try {
                    var synthesisStream = await Synthesis(textTest);
                    await this.cmd.FollowupWithFileAsync(synthesisStream, "siapa namamu.mp3");
                } catch (Exception e) {
                    await this.cmd.FollowupAsync(e.ToString());
                    Console.WriteLine(e);
                }
            break;
        }
    }

    private async Task Ping()
    {
        await this.cmd.RespondAsync("pong!");
    }

    private async Task<string> Translate(string text)
    {
        var url = $"http://localhost:3000/translate?text={text}";
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode) 
            throw new Exception("service translator bejat");
        
        
        var result = await response.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<TranslatorResponse>(result);
        if (json == null) 
            throw new Exception("service translator bejat");

        Console.WriteLine(json.Text);
        return json.Text;
    }

    private async Task<Stream> Synthesis(string japaneseText) {
        var speaker = 8;

        var url = $"http://localhost:50021/audio_query?text={japaneseText}&speaker={speaker}";
        var client = new HttpClient();
        var response = await client.PostAsync(url, null);
        if (!response.IsSuccessStatusCode) 
            throw new Exception("service synthesis bejat");
        Console.WriteLine(response.StatusCode);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var content = new StringContent(responseString, System.Text.Encoding.UTF8, "application/json");

        url = $"http://localhost:50021/synthesis?speaker={speaker}&enable_interrogative_upspeak=true";
        var synthesisResponse = await client.PostAsync(url, content);
        if (!synthesisResponse.IsSuccessStatusCode)
            throw new Exception("service synthesis bejat");
        Console.WriteLine(synthesisResponse.StatusCode);
        return await synthesisResponse.Content.ReadAsStreamAsync();
    }
}