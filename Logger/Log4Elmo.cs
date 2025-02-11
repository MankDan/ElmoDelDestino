using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Gioco.Logger;

/// <summary>
/// Classe che gestisce la registrazione degli eventi e dei messaggi di log del gioco attraverso diversi livelli di gravità
/// </summary>
/// <param name="info">Livello di tipo INFORMAZIONE</param>
/// <param name="warning">Livello di tipo WARNING</param>
/// <param name="error">Livello di tipo ERROR</param>
internal static class Log4Elmo
{
    public const byte INFO = 0;
    public const byte WARNING = 1;
    public const byte ERROR = 2;

    // Persorso della cartella di logging
    private static readonly string logFilePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    ".elmodeldestino/logs/");

    private static readonly string logFileName;

    public static void CreaCartellaDiLogging()
        => Directory.CreateDirectory(Path.GetDirectoryName(logFilePath) ?? "");

    static Log4Elmo()
    {
        CreaCartellaDiLogging();
        string data = $"{DateTime.Now}";
        logFileName = $"{data.Split()[0].Replace('/', '_')}-{data.Split()[1].Replace(':', '_')}.log";
    }

    /// <summary>
    /// Registra un messaggio nel file di log con un timestamp e informazioni sul chiamante
    /// </summary>
    /// <param name="messaggio">Il messaggio da registrare</param>
    /// <param name="mode">Il livello di gravità del messaggio (INFO, WARNING, ERROR)</param>
    public static void Log(string messaggio, byte mode = INFO)
    {
        using StreamWriter writer = new(logFilePath + logFileName, true);
        string timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        switch (mode)
        {
            case INFO:
                writer.WriteLine($"[INFO | {timestamp} | {GetCallerInfo()}]\t" + messaggio);
                break;
            case WARNING:
                writer.WriteLine($"[WARNING | {timestamp} | {GetCallerInfo()}]\t" + messaggio);
                break;
            case ERROR:
                writer.WriteLine($"[ERROR | {timestamp} | {GetCallerInfo()}]\t" + messaggio);
                break;
        }
    }

    // Ottiene le informazioni sul metodo chiamante che ha invocato il log.
    private static string GetCallerInfo()
    {
        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();

        // Verifica il chiamante diretto (escludendo Logger.Log o Logger.LogError)
        foreach (StackFrame frame in stackFrames)
        {
            MethodBase? method = frame.GetMethod();
            if (method?.DeclaringType != typeof(Log4Elmo) && method?.DeclaringType != null)
            {
                return $"{method.DeclaringType.Name}.{method.Name}";
            }
        }

        return "Chiamante non disponibile";
    }
}
