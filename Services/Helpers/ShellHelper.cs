using System.Diagnostics;
using System.Runtime.InteropServices;

public static class ShellHelper
{
    public static async Task<int> RunCommandAsync(string command, string args, string workingDirectory)
    {
        string shell, shellArgsFormat;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            shell = "cmd.exe";
            shellArgsFormat = "/c \"{0} {1}\"";
        }
        else
        {
            shell = "/bin/bash";
            shellArgsFormat = "-c \"{0} {1}\"";
        }

        var psi = new ProcessStartInfo
        {
            FileName            = shell,
            Arguments           = string.Format(shellArgsFormat, command, args),
            WorkingDirectory    = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute     = false,
            CreateNoWindow      = true
        };

        var tcs = new TaskCompletionSource<int>();
        var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
        process.Exited += (s, e) => tcs.SetResult(process.ExitCode);

        process.OutputDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
        process.ErrorDataReceived  += (s, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return await tcs.Task;
    }
}