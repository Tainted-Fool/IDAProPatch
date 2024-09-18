using System.Text.Json.Serialization;
using IDAPro.Models;

namespace IDAPro;

[JsonSerializable(typeof(Header))]
[JsonSerializable(typeof(ParentData))]
[JsonSerializable(typeof(Payload))]
[JsonSerializable(typeof(LicenseFormat))]
[JsonSerializable(typeof(AddonFormat))]
public partial class MyJsonContext : JsonSerializerContext
{
}