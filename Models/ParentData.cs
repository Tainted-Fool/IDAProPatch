namespace IDAPro.Models;

public class ParentData
{
    public Header header { get; set; }
    public Payload payload { get; set; }
    public string signature { get; set; } = "0x80";
}
