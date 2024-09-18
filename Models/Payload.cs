namespace IDAPro.Models;

public class Payload
{
    public string name { get; set; } = "what";

    public string email { get; set; } = "glop";
    public List<LicenseFormat> licenses { get; set; }
}
