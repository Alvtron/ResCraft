using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ResourceCraft.Utilities
{
    public static class Logger
    {
        private static List<string> FailedEntries { get; } = new List<string>();

        public static readonly string FilePath;

        public static string Entries
        {
            get
            {
                if (!File.Exists(FilePath))
                {
                    return null;
                }

                try
                {
                    return File.ReadAllText(FilePath);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private static string Time
        {
            get
            {
                var time = DateTime.Now;

                return $"{time.Year}{time.Month}{time.Day}{time.Hour}_{time.Minute}{time.Second}{time.Millisecond}";
            }
        }

        static Logger()
        {
            FilePath = Path.Combine(Path.GetTempPath(), $"codeshare_log_{Time}.txt");
            WriteLine($"Log file is saved to '{FilePath}'.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void WriteLine(string message, [CallerMemberName] string callerName = "NO_NAME", [CallerFilePath] string callerFilePath = "NO_PATH", [CallerLineNumber] int callerLineNumberAttribute = 0)
        {
            var callerAssembly = Assembly.GetCallingAssembly()?.GetName()?.Name;
            var callerFile = Path.GetFileNameWithoutExtension(callerFilePath);
            var text = CreateCallerString(message, callerAssembly, callerName, callerFile, callerLineNumberAttribute);

            AppendTextToFile(text);
            System.Diagnostics.Debug.WriteLine(text);
        }

        private static string CreateCallerString(string message, string callerAssembly, string callerName, string callerFile, int callerLineNumber)
        {
            var text = new StringBuilder();
            var hasCallerInfo = false;

            if (!string.IsNullOrEmpty(callerAssembly))
            {
                text.Append($"{callerAssembly}:");
                hasCallerInfo = true;
            }

            if (!string.IsNullOrEmpty(callerFile))
            {
                text.Append($"{callerFile}.");
                hasCallerInfo = true;
            }

            if (!string.IsNullOrEmpty(callerName))
            {
                text.Append($"{callerName}");
                if (callerLineNumber > 0)
                {
                    text.Append($"({callerLineNumber})");
                }
                hasCallerInfo = true;
            }

            if (hasCallerInfo)
            {
                text.Append($" --> ");
            }

            if (!string.IsNullOrEmpty(message))
            {
                text.Append($"{message}");
            }

            return text.ToString();
        }

        private static void AppendTextToFile(string text)
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    text = $"{Time} {text}";

                    using (var streamWriter = File.CreateText(FilePath))
                    {
                        streamWriter.WriteLine(text);
                    }
                }
                else
                {
                    text = $"\n{Time} {text}";
                    File.AppendAllText(FilePath, text);
                }
            }
            catch (Exception)
            {
                FailedEntries.Add(text);
                return;
            }
        }
    }
}
