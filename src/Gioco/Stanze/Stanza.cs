using Gioco.ConsoleUtils;
using Gioco.Enumeratori;
using Gioco.Interfacce;
using Gioco.Logger;
using Gioco.Oggetti;
using Gioco.Oggetti.Altri;
using Gioco.Personaggi;
using System.IO;

namespace Gioco.Stanze;

/// <summary>
/// Classe che gestisce l'aggiunta di oggetti e personaggi, collegamento ed isolamento delle stanze, salvataggio e carimanento dello stato della stanza
/// </summary>
/// <param name="nome">Nome</param>
/// <param name="descrizione">Descrizione della stanza</param>
/// <param name="porte">Porte presenti nella stanza</param>
/// <param name="oggetti">Lista degli oggetti presenti nella stanza</param>
/// <param name="idoggetti">ID degli oggetti presenti nella stanza</param>
/// <param name="personaggi">Lista dei personaggi presenti nella stanza</param>
/// <param name="id">ID dell'oggetto istanziato</param>
/// <param name="id statico">ID statico che viene incrementato ad ogni istanziamento di un oggetto</param>
/// <param name="visitata">Bool per controllare se la stanza è stata visitata o meno</param>
/// <param name="texture pavimento">Texture del pavimento</param>
public class Stanza : ISalvabile
{
    public static readonly short PORTA_GIA_COLLEGATA = -1;
    public static readonly short COLLEGAMENTO_RIUSCITO = 1;
    public static readonly short NESSUNA_PORTA_DISPONIBILE = 0;

    public string Nome { get; private set; }
    public string Descrizione { get; private set; }
    public Stanza?[] Porte { get; private set; }
    public List<Oggetto> Oggetti { get; set; }
    public List<short> IdOggetti;
    public List<Personaggio> Personaggi { get; set; }
    public short Id { get; private set; }
    public static short ID { get; private set; } = 1;
    public bool Visitata { get; set; }
    public readonly char TexturePavimento;

    private short NumeroPorteCollegate;

    private readonly Random random;

    // Nome del file di salvataggio della stanza specificata
    public string SaveFileName => $"stanza" + Id + ".save";

    internal Stanza(string nome, string descrizione, char texturePavimento)
    {
        Log4Elmo.Log($"Inizio creazione Stanza {nome}");
        Nome = nome;
        Descrizione = descrizione;
        TexturePavimento = texturePavimento;
        random = new Random();
        Porte = new Stanza[4];
        IdOggetti = [];
        Oggetti = [];
        Personaggi = [];
        Visitata = false;
        NumeroPorteCollegate = 0;
        Id = ID++;
        Log4Elmo.Log($"Stanza {nome} creata con ID {Id}");
    }

    /// <summary>
    /// Aggiunge un oggetto alla stanza
    /// </summary>
    /// <param name="oggetto">Oggetto da aggiungere</param>
    public void AggiungiOggetto(Oggetto oggetto)
    {
        Oggetti.Add(oggetto);
        Log4Elmo.Log($"oggetto {oggetto.Nome} di tipo {oggetto.GetType()} aggiunto a {this.Nome} con id {Id}");
    }

    /// <summary>
    /// Aggiunge un personaggio alla stanza
    /// </summary>
    /// <param name="oggetto">Personaggio da aggiungere</param>
    public void AggiungiPersonaggio(Personaggio personaggio)
    {
        Personaggi.Add(personaggio);
        Log4Elmo.Log($"Personaggio {personaggio.Nome} aggiunto a {this.Nome} con id {Id}");
    }

    /// <summary>
    /// Collega le stanze tra di loro
    /// </summary>
    /// <param name="stanza">Stanza da collegare</param>
    /// <param name="direzione">Direzione in cui si collega</param>
    internal int CollegaStanza(Stanza stanza, Direzione direzione)
    {
        if (stanza.NumeroPorteCollegate > 2) return NESSUNA_PORTA_DISPONIBILE;
        if (Porte[(int)direzione] != null) return PORTA_GIA_COLLEGATA;

        Porte[(int)direzione] = stanza;
        stanza.Porte[(((int)direzione) + 2) % 4] = this; // Restituisce la direzione inversa di quella attuale
        stanza.NumeroPorteCollegate++;
        this.NumeroPorteCollegate++;
        Log4Elmo.Log($"Stanza {stanza.Id} e stanza {this.Id} collegate");
        return COLLEGAMENTO_RIUSCITO;
    }

    /// <summary>
    /// Isola la stanza, rimuovendo tutte le porte collegate
    /// </summary>
    public void Isola()
    {
        NumeroPorteCollegate = 0;
        for (int i = 0; i < Porte.Length; i++)
        {
            Porte[i] = null;
        }
        Log4Elmo.Log($"Stanza {this.Id} isolata dalle altre stanze");
    }    

    /// <summary>
    /// Salva lo stato corrente della stanza in un file
    /// </summary>
    /// <param name="SaveFilePath">Percorso del file di salvataggio</param>
    public void Salva(string SaveFilePath)
    {
        Log4Elmo.Log($"Salvataggio stanza in corso nel file '{SaveFilePath + SaveFilePath}'");
        using StreamWriter writer = new(SaveFilePath + SaveFileName);
        SalvaOggetti(writer);
        writer.Close();
    }

    // Salva gli ID degli oggetti nel file di salvataggio
    private void SalvaOggetti(StreamWriter writer)
    {
        Log4Elmo.Log($"Inizio salvataggio oggetti della stanza {this.Id}");
        foreach (var oggetto in Oggetti)
        {
            writer.WriteLine(oggetto.Id);
        }
        Log4Elmo.Log($"Oggetti della stanza {this.Id} salvati");
    }

    /// <summary>
    /// Carica lo stato della stanza da un file di salvataggio
    /// </summary>
    /// <param name="SaveFilePath">Percorso del file di salvataggio</param>
    /// <exception cref="Exception">Se non viene trovato nessun file di salvataggio</exception>
    public void Carica(string SaveFilePath)
    {
        if (!File.Exists(SaveFilePath + SaveFileName))
        {
            Log4Elmo.Log($"Il file '{SaveFilePath + SaveFileName}' non esiste!", Log4Elmo.ERROR);
            return;
        }
        using StreamReader reader = new(SaveFilePath + SaveFileName);

        Isola();
        Personaggi.Clear();
        CaricaIdOggetti(reader);
    }

    // Carica gli ID degli oggetti dal file di salvataggio
    private void CaricaIdOggetti(StreamReader reader)
    {
        Log4Elmo.Log($"Caricamento Id oggetti per la stanza {Id} in corso");
        short oggettoId;
        IdOggetti.Clear();
        while ((oggettoId = Convert.ToByte(reader.ReadLine())) != 0)
        {
            IdOggetti.Add(oggettoId);
        }
        Log4Elmo.Log($"Caricamento Id oggetti per la stanza {Id} completato");
    }

    /// <summary>
    /// Carica gli oggetti nella stanza da un dizionario di oggetti
    /// </summary>
    /// <param name="oggetti">Dizionario di oggetti disponibili</param>
    public void CaricaOggetti(Dictionary<int, Oggetto> oggetti)
    {
        Log4Elmo.Log($"Inizio caricamento oggetti nell'inventario");
        Oggetti.Clear();
        foreach (int id in IdOggetti)
        {
            Oggetto oggetto = oggetti[id];
            if (oggetto != null)
            {
                AggiungiOggetto(oggetto);
            }
        }
        Log4Elmo.Log($"Caricamento oggetti nell'inventario completato");
    }
}
