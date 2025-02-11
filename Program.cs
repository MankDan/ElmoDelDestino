using Gioco.Logger;

namespace Gioco;

/// <summary>
/// Inizio del programma
/// </summary>
public class Program
{
    private static Game? gioco;

    public static void Main()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        Log4Elmo.Log("Avvio del gioco in corso");
        gioco = new();
        gioco.Inizia();
        Log4Elmo.Log($"Chiusura del gioco");
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception? ex = e.ExceptionObject as Exception;

        // Ottieni il nome del metodo e la classe tramite StackTrace
        string? stackTrace = ex?.StackTrace;
        string[] stackFrames = (stackTrace ?? "").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        string firstStackFrame = stackFrames.Length > 0 ? stackFrames[0] : "Informazione sul punto non disponibile";

        // Costruisci il messaggio da registrare nel log
        string errorMessage = $"Eccezione non gestita: {ex?.Message}{Environment.NewLine}Punto di origine: {firstStackFrame}";

        Log4Elmo.Log(errorMessage, Log4Elmo.ERROR);
    }
}
