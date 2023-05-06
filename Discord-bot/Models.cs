using Discord.WebSocket;

class TranslatorResponse
{
    public string Text { get; set; }
}

class text2imgResponse
{
    public string[]? Images { get; set; }
    public string? Info { get; set; }
}

class t2iInfo
{
    public string? Sampler_index { get; set; }
    public string? Seed { get; set; }
}

class t2iImagineRequest
{
    public string prompt { get; set; } = string.Empty;
    public string negative_prompt { get; set; } = string.Empty;
    public int steps { get; set; } = 20;
    public int cfg_scale { get; set; } = 7;
    public int width { get; set; } = 512;
    public int height { get; set; } = 512;
    public string sampler_index { get; set; } = "DPM++ SDE Karras";

    public t2iImagineRequest(List<SocketSlashCommandDataOption>? opt)
    {
        if (opt == null)
            return;

        foreach (var item in opt) {
            string val = item.Value.ToString() ?? string.Empty;
            if (val == string.Empty)
                continue;
            val.Trim();

            switch (item.Name) {
                case "prompt":
                    this.prompt = val;
                    break;
                case "negative_prompt":
                    this.negative_prompt = val;
                    break;
                case "sampling_steps":
                    this.steps = int.Parse(val);
                    break;
                case "cfg_scale":
                    this.cfg_scale = int.Parse(val);
                    break;
                case "width":
                    this.width = int.Parse(val);
                    break;
                case "height":
                    this.height = int.Parse(val);
                    break;
                case "sampling_method":
                    this.sampler_index = val;
                    break;
            }
        }
    }
}