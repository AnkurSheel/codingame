using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CombineFilesCsharp
{
    internal class Program
    {
        private const string InputDirectory = @".\";

        private const string Output = @".\FinalOutput\Output.cs";

        private static async Task Main(string[] args)
        {
            Console.WriteLine(Path.GetFullPath(InputDirectory));
            var lastWriteTime = DateTime.MinValue;

            while (true)
            {
                await Task.Delay(500);
                var files = GetFiles();
                var currentLastWriteTime = files.Max(File.GetLastWriteTime);
                if (currentLastWriteTime == lastWriteTime)
                {
                    continue;
                }

                lastWriteTime = currentLastWriteTime;
                await File.WriteAllTextAsync(Output, CreateFile(files, currentLastWriteTime));
                Console.WriteLine("Updated " + lastWriteTime.ToString("dd/MM/yyyy hh:mm"));
            }
        }

        private static string CreateFile(string[] files, DateTime writeTime)
        {
            var content = files.SelectMany(File.ReadAllLines).ToList();
            var usings = content.Where(line => line.StartsWith("using")).Distinct().ToList();
            content.RemoveAll(lines => lines.StartsWith("using"));

            return string.Join($"{Environment.NewLine}", usings)
                   + $"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine} // "
                   + writeTime.ToString("dd/MM/yyyy hh:mm")
                   + $"{Environment.NewLine}{Environment.NewLine}"
                   + string.Join($"{Environment.NewLine}", content);
        }

        private static string[] GetFiles()
        {
            return Directory.GetFiles(InputDirectory, "*.cs", SearchOption.AllDirectories)
                            .Where(f => !f.Contains("\\obj\\") && !f.Contains("\\bin\\") && !f.Contains("\\FinalOutput\\"))
                            .ToArray();
        }
    }
}
