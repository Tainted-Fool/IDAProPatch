namespace IDAPro.Models;

public class LicenseFormat
{
    public string id { get; set; }
    public string license_type { get; set; } = "named";
    public string product { get; set; }
    public int seats { get; set; } = 6969420;
    public string start_date { get; set; } = "2024-04-21";
    public string end_date { get; set; } = "2030-04-21";
    public string issued_on { get; set; } = "2024-04-21 06:24:20";
    public string owner { get; set; } = "glopmorp";
    public List<string> features { get; set; }
    public List<AddonFormat> add_ons { get; set; }
}

