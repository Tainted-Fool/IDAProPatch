namespace IDAPro;

public class Program
{
    static void Main()
    {
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
        var newBytes = new byte[] { 0x90, 0x90 };
        const string baseSubKey = @"SOFTWARE\Hex-Rays\IDA";
        var registryValues = new Dictionary<string, object>
        {
            { "AutoCheckUpdates", 0 },
            { "AutoRequestUpdates", 0 },
            { "InformedAboutUpdates3", 0 },
            { "AutoUseLumina", 0 },
            { "DisplayWelcome", 0 },
        };
        var myFunctions = new Functions();
        var jsonCreator = new JsonFileCreator();
        
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
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var outputFilePath = Path.Combine(appDataPath, @"Hex-Rays\IDA Pro\ida.hexlic");

                jsonCreator.CreateJson(outputFilePath);
                Console.WriteLine($"License file wrote: {outputFilePath}\n");
                
                myFunctions.CreateRegistry(baseSubKey, registryValues);
                
                for (var i = 0; i < filePaths.Length; i++)
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
