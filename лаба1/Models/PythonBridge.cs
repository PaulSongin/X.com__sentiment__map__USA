using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace лаба1.Services
{
    public static class PythonBridge
    {
        public static void RunMapingExe(object pointsData, string exePath)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(pointsData);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    RedirectStandardInput = true,  
                    RedirectStandardOutput = true, 
                    RedirectStandardError = true,  
                    UseShellExecute = false,
                    CreateNoWindow = true      
                };

                startInfo.WorkingDirectory = Path.GetDirectoryName(exePath);

                using (Process process = Process.Start(startInfo))
                {
                    using (StreamWriter writer = process.StandardInput)
                    {
                        writer.Write(jsonString);
                    } 

                    string errors = process.StandardError.ReadToEnd();
                    string output = process.StandardOutput.ReadToEnd();

                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(errors))
                    {
                        Console.WriteLine("Ошибки Python: " + errors);
                    }
                    if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine("Вывод Python: " + output);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка запуска Python-модуля: {ex.Message}");
            }
        }
    }
}
