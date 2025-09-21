using Gioco.Giocatori;
using Gioco.Interfacce;
using Gioco.Logger;
using Gioco.Oggetti.Altri;
using Gioco.Stanze;

namespace Gioco.Personaggi;

/// <summary>
/// Rappresenta un personaggio nel gioco. Questa classe astratta implementa le interfacce ISalvabile e IStampabile
/// </summary>
public abstract class Personaggio(string nome, string descrizione) : ISalvabile, IStampabile
{
    /// <summary>
    /// Ottiene l'ID del personaggio. Questo valore è unico per ogni istanza e viene incrementato automaticamente.
    /// </summary>
    public short Id { get; private set; } = ID++;

    public string Nome { get; set; } = nome;
    public string Descrizione { get; set; } = descrizione;
    public string[] Skin { get; set; } = [
        " O ",
        "/|\\",
        "/ \\"
    ];

    /// <summary>
    /// Ottiene il nome del file di salvataggio basato sul nome del personaggio.
    /// </summary>
    public string SaveFileName => Nome + ".save";

    /// <summary>
    /// Ottiene o imposta l'ID statico per assegnare un ID unico ogni volta che c'è un'istanza del personaggio
    /// </summary>
    public static short ID { get; private set; } = 1;
    protected int StatoInterazione { get; set; } = 0;

    /// <summary>
    /// Salva lo stato del personaggio nel file di salvataggio specificato.
    /// </summary>
    /// <param name="SaveFilePath">Il percorso del file di salvataggio.</param>
    public void Salva(string SaveFilePath)
    {
        Log4Elmo.Log($"Salvataggio dei dati di {Nome} con id {Id} in corso");
        using StreamWriter writer = new(SaveFilePath + SaveFileName);
        writer.WriteLine(StatoInterazione);
        writer.Close();
        Log4Elmo.Log($"Dati di {Nome} con id {Id} salvati");
    }

    /// <summary>
    /// Carica lo stato del personaggio dal file di salvataggio specificato.
    /// </summary>
    /// <param name="SaveFilePath">Il percorso del file di salvataggio.</param>
    public void Carica(string SaveFilePath)
    {
        Log4Elmo.Log($"Caricamento dei dati di {Nome} con id {Id} in corso");
        if (!File.Exists(SaveFilePath + SaveFileName))
        {
            Log4Elmo.Log($"Caricamento dei dati di {Nome} con id {Id} non riuscito. '{SaveFilePath+SaveFileName}' non esiste!", Log4Elmo.WARNING);
            return;
        }
        using StreamReader reader = new(SaveFilePath + SaveFileName);
        StatoInterazione = int.Parse(reader.ReadLine() ?? "0");
        reader.Close();
        Log4Elmo.Log($"Caricamento dei dati di {Nome} con id {Id} Completato");
    }

    public abstract void Interagisci(Giocatore giocatore);
}
