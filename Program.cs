using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;

namespace IDAPro;

public class Program
{
    static void Main()
    {
        string[] platforms =
        [
            "w",
            "l",
            "m",
        ];
        string[] products =
        [
            "IDA",
            "IDAHOME",
            "TEAMSSRV",
            "LUMINASRV",
            "LICENSESRV"
        ];
        string[] addons =
        [
            "hexx86",
            "hexx64",
            "hexarm",
            "hexarm64",
            "hexppc",
            "hexppc64",
            "hexmips64",
            "hexcx86",
            "hexcx64",
            "hexcarm",
            "hexcarm64",
            "hexcppc",
            "hexcppc64",
            "hexcmisp64"
        ];
        int counter = 0;
        string[] filePaths =
        [
            @"C:\Program Files\IDA Professional 9.0\ida.dll",
            @"C:\Program Files\IDA Professional 9.0\ida64.dll"
        ];
        long[] offsets =
        [
            0x32b273,
            0x342e53
        ];
        byte[] newBytes = new byte[] { 0x90, 0x90 };
        const string baseSubKey = @"SOFTWARE\Hex-Rays\IDA";
        var registryValues = new Dictionary<string, object>
        {
            { "AutoCheckUpdates", 0 },
            { "AutoRequestUpdates", 0 },
            { "InformedAboutUpdates3", 0 },
            { "AutoUseLumina", 0 },
            { "DisplayWelcome", 0 },
        };
        Functions myFunctions = new Functions();
        
        var payload = new Payload
        {
            licenses = new List<LicenseFormat>()
        };
        
        foreach (var product in products)
        {
            counter++;
            var license = new LicenseFormat
            {
                id = $"{counter:X2}-0000-0000-00",
                product = product,
                features = [],
                add_ons = new List<AddonFormat>()
            };
            
            foreach (var addon in addons)
            {
                var ownerLicense = $"{counter:X2}-0000-0000-00";
                counter++;
                var addOn = new AddonFormat
                {
                    id = $"{counter:X2}-0000-0000-00",
                    code = addon.ToUpper(),
                    owner = ownerLicense,
                };
                license.add_ons.Add(addOn);

                foreach (var platform in platforms)
                {
                    var addOn2 = new AddonFormat
                    {
                        id = $"{counter:X2}-0000-0000-00",
                        code = $"com.hexrays.{addon}{platform}",
                        owner = ownerLicense,
                    };
                    license.add_ons.Add(addOn2);
                }
            }
            payload.licenses.Add(license);
        }
        
        var parentData = new ParentData
        {
            header = new Header { version = 1 },
            payload = payload,
        };

        string jsonString = JsonSerializer.Serialize(parentData, MyJsonContext.Default.ParentData);
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string outputFilePath = Path.Combine(appDataPath, @"Hex-Rays\IDA Pro\ida.hexlic");

        if (!myFunctions.IsAdministrator())
        {
            Console.WriteLine("Need administrator privileges to patch file");
            Console.WriteLine("Relaunch as admin? y or n?");
            if (Console.ReadLine() == "y")
            {
                myFunctions.RelaunchAsAdmin();
            }
        }
        else
        {
            try
            {
                File.WriteAllText(outputFilePath, jsonString);
                Console.WriteLine($"License file wrote: {outputFilePath}\n");
                myFunctions.CreateRegistry(baseSubKey, registryValues);
                
                for (int i = 0; i < filePaths.Length; i++)
                {
                    myFunctions.PatchFile(filePaths[i], offsets[i], newBytes);
                }
                Console.WriteLine("File patch successfully");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: File not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}

public class Header
{
    public int version { get; set; }
}

public class ParentData
{
    public Header header { get; set; }
    public Payload payload { get; set; }
    public string signature { get; set; } = "0x80";
}

public class Payload
{
    public string name { get; set; } = "what";

    public string email { get; set; } = "glop";
    public List<LicenseFormat> licenses { get; set; }
}

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

public class AddonFormat
{
    public string id { get; set; }
    public string code { get; set; }
    public string owner { get; set; }
    public string start_date { get; set; } = "2024-04-21";
    public string end_date { get; set; } = "2030-04-21";
}

[JsonSerializable(typeof(Header))]
[JsonSerializable(typeof(ParentData))]
[JsonSerializable(typeof(Payload))]
[JsonSerializable(typeof(LicenseFormat))]
[JsonSerializable(typeof(AddonFormat))]
public partial class MyJsonContext : JsonSerializerContext
{
}