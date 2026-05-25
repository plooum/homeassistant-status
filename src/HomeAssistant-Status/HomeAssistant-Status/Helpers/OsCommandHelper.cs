using System.Diagnostics;

namespace HomeAssistant_Status.Helpers;

public static class OsCommandHelper
{
    public static void ExecuteCommand(string fileName, string args, out string stdOut, out string errOut)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false, // Requis pour rediriger les flux (Output/Error)
            CreateNoWindow = false
        };

        using var process = Process.Start(startInfo);
        
        if(process is null)
            throw new NullReferenceException("Process returned null");
        
        stdOut = process.StandardOutput.ReadToEnd();
        errOut = process.StandardError.ReadToEnd();

        process.WaitForExit();
    }
}