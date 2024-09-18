using System.Text.Json;
using IDAPro.Models;

namespace IDAPro;

public class JsonFileCreator
{
    public void CreateJson(string outputFilePath)
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
        var counter = 0;
        
        var payload = new Payload
        {
            licenses = []
        };

        foreach (var product in products)
        {
            counter++;
            var license = new LicenseFormat
            {
                id = $"{counter:X2}-0000-0000-00",
                product = product,
                features = [],
                add_ons = []
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
            payload = payload
        };

        var jsonString = JsonSerializer.Serialize(parentData, MyJsonContext.Default.ParentData);
        File.WriteAllText(outputFilePath, jsonString);
    }
}