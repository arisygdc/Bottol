class TranslatorResponse
{
    public string Text { get; set; }
}

class text2imgRequest
{
    private string Prompt { get; set; } = string.Empty;
    private string Negative_prompt { get; set; } = string.Empty;
    public int Steps { get; set; }
    public int Cfg_scale { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public text2imgRequest(string prompt, string negative_prompt, t2iImagineOpt? opt)
    {
        this.Prompt = prompt;
        this.Negative_prompt = negative_prompt;
        if (opt != null)
        {
            this.Steps = opt.Sampling_steps;
            this.Cfg_scale = opt.Cfg_scale;
            this.Width = opt.Width;
            this.Height = opt.Height;
        }
    }
}

class text2imgResponse
{
    public class Infot2i {
        public string Seed { get; set; } = string.Empty;
    }

    public string[]? Images { get; set; }
    public Infot2i? Info { get; set; }
}

class t2iImagineOpt
{
    public int Sampling_steps { get; set; } = 20;
    public int Cfg_scale { get; set; } = 7;
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
    public string Sampling_method { get; set; } = "DPM++ SDE Karras";

    public t2iImagineOpt(Dictionary<string, object> opt)
    {
        if (opt.ContainsKey("sampling_steps"))
            this.Sampling_steps = int.Parse(opt["sampling_steps"].ToString());
        if (opt.ContainsKey("cfg_scale"))
            this.Cfg_scale = int.Parse(opt["cfg_scale"].ToString());
        if (opt.ContainsKey("width"))
            this.Width = int.Parse(opt["width"].ToString());
        if (opt.ContainsKey("height"))
            this.Height = int.Parse(opt["height"].ToString());
        if (opt.ContainsKey("sampling_method"))
            this.Sampling_method = opt["sampling_method"].ToString();
    }
}