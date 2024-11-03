using UnityEngine;
using System.IO;

public static class CustomLogger
{
    // Define a path for log files (optional)
    private static readonly string logFilePath = Path.Combine(Application.persistentDataPath, "Logs", "game_log.txt");

    // Ensure directory exists
    static CustomLogger()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
    }

    /// <summary>
    /// Logs a standard message with the first 2 lines of stack trace.
    /// </summary>
    public static void Log(string message, bool showStackTrace = true)
    {
        string finalMessage = message;
        if (showStackTrace)
        {
            string stackTraceSnippet = GetStackTraceSnippet();
            finalMessage = $"{message}\n{stackTraceSnippet}";
        }

        Debug.Log(finalMessage);
        WriteToFile("[LOG] " + finalMessage);
    }

    /// <summary>
    /// Logs a warning message with the first 2 lines of stack trace.
    /// </summary>
    public static void LogWarning(string message)
    {
        string stackTraceSnippet = GetStackTraceSnippet();
        string finalMessage = $"{message}\n{stackTraceSnippet}";

        Debug.LogWarning(finalMessage);
        WriteToFile("[WARNING] " + finalMessage);
    }

    /// <summary>
    /// Logs an error message with the first 2 lines of stack trace.
    /// </summary>
    public static void LogError(string message)
    {
        string stackTraceSnippet = GetStackTraceSnippet();
        string finalMessage = $"{message}\n{stackTraceSnippet}";

        Debug.LogError(finalMessage);
        WriteToFile("[ERROR] " + finalMessage);
    }

    /// <summary>
    /// Helper method to write messages to a log file (optional).
    /// </summary>
    private static void WriteToFile(string message)
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"[{System.DateTime.Now}] {message}");
        }
    }

    /// <summary>
    /// Helper method to retrieve the first 2 lines of the current stack trace.
    /// </summary>
    private static string GetStackTraceSnippet()
    {
        var stackTrace = new System.Diagnostics.StackTrace();
        string[] lines = stackTrace.ToString().Split('\n');
        return lines.Length >= 3 ? $"{lines[1].Trim()}\n{lines[3].Trim()}" : lines[1].Trim();
    }
}
