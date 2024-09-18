using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;

namespace IDAPro;

public class Functions
{
    public bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public void RelaunchAsAdmin()
    {
        var exePath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetEntryAssembly().GetName().Name}.exe");

        var procInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = exePath,
            Verb = "runas",
            Arguments = Environment.CommandLine
        };
        try
        {
            Process.Start(procInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: fail to start process {ex.Message}");
        }
    }

    public void PatchFile(string filePath, long offset, byte[] newBytes)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
        fileStream.Seek(offset, SeekOrigin.Begin);
        Console.WriteLine($"Moved to offset: {offset:X} in {filePath}");

        var originalBytes = new byte[newBytes.Length];
        fileStream.Read(originalBytes, 0, originalBytes.Length);
        Console.WriteLine($"Original bytes: {BitConverter.ToString(originalBytes)}");

        fileStream.Seek(offset, SeekOrigin.Begin);

        fileStream.Write(newBytes, 0, newBytes.Length);
        Console.WriteLine($"Wrote bytes: {BitConverter.ToString(newBytes)}\n");
    }

    public void CreateRegistry(string baseSubKey, Dictionary<string, object> registryValues)
    {
        try
        {
            var writeKey = Registry.CurrentUser.CreateSubKey(baseSubKey);
            if (writeKey != null)
            {
                foreach (var (valueName, valueData) in registryValues)
                {
                    if (valueData is int)
                    {
                        writeKey.SetValue(valueName, valueData, RegistryValueKind.DWord);
                    }
                }
                Console.WriteLine($"Registry keys changed: {baseSubKey}\n");
                writeKey.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}