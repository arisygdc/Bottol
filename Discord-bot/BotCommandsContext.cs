using Discord.WebSocket;
using Newtonsoft.Json;

public class BotCommandsContext
{
    private SocketSlashCommand cmd;
    public BotCommandsContext(SocketSlashCommand command)
        => cmd = command;

    public async Task CmdAction()
    {
        Console.WriteLine($"New Command Request: {this.cmd.Data.Name}");
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
            case "imagine":
                var text2imgText = this.cmd.Data.Options.First().Value.ToString();
                Console.WriteLine(text2imgText);
                await this.cmd.RespondAsync(text2imgText);
                var opt = this.cmd.Data.Options.ToDictionary(x => x.Name, x => x.Value);
                var t2iOpt = new t2iImagineOpt(opt);

                if (text2imgText == null)
                    await this.cmd.FollowupAsync("text tidak boleh kosong");

                await this.cmd.FollowupAsync($"promt: {text2imgText}\n" +
                    $"negative_prompt: {opt["negative_prompt"]}\n" +
                    $"sampling_steps: {opt["sampling_steps"]}\n" +
                    $"cfg_scale: {opt["cfg_scale"]}\n" +
                    $"sampling_method: {opt["sampling_method"]}");

                var (images, seed) = await text2img(
                    prompt: text2imgText ?? string.Empty, 
                    negative_prompt: opt["negative_prompt"].ToString() ?? string.Empty, 
                    opt: t2iOpt);
                foreach (var image in images) {
                    await this.cmd.FollowupWithFileAsync(image, seed + ".png");
                }
                break;
            case "tts-test":
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

    // text to image stable diffusion api
    private async Task<(List<MemoryStream>, string?)> text2img(string prompt, string negative_prompt, t2iImagineOpt opt) {
        // stable diffusion text to image api
        var url = "http://localhost:7861/sdapi/v1/txt2img";
        var client = new HttpClient();
        var obj = new text2imgRequest(
            prompt: prompt, 
            negative_prompt: negative_prompt,
            opt: opt
        );

        string json = JsonConvert.SerializeObject(obj);
        Console.WriteLine(json);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        if (!response.IsSuccessStatusCode) 
            throw new Exception("service text2img bejat");
        
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
        var jsonResult = JsonConvert.DeserializeObject<text2imgResponse>(result);
        if (jsonResult == null) 
            throw new Exception("service text2img bejat");
        
        if (jsonResult.Images == null)
            throw new Exception("empty images");
            
        var images = new List<MemoryStream>();
        foreach (var image in jsonResult.Images) {
            byte[] base64Bytes = Convert.FromBase64String(image);
            images.Add(new MemoryStream(base64Bytes));
        }

        return (images, jsonResult.Info.Seed);
    }
}