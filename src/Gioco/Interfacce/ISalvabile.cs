namespace Gioco.Interfacce;

/// <summary>
/// Interfaccia per il salvataggio dei dati
/// </summary>
public interface ISalvabile
{
    string SaveFileName { get; }
    abstract void Salva(string SaveFilePath);
    abstract void Carica(string SaveFilePath);
}