using Gioco.ConsoleUtils;
using Gioco.Giocatori;
using Gioco.Interfacce;
using Gioco.Personaggi;
using Gioco.Stanze;
using Gioco.Logger;
using Gioco.Oggetti.Altri;

namespace Gioco.Managers;

/// <summary>
/// Gestisce il salvataggio della partita
/// </summary>
/// <param name="gioco">La partita da salvare</param>
class SaveLoadManager(Game gioco)
{
    // Percorso file della cartella dei file di salvataggio
    private static readonly string saveFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ".elmodeldestino/save/");
#pragma warning disable
    private readonly Game Gioco = gioco;
#pragma warning enable
    /// <summary>
    /// Crea la cartella di salvataggio se non già esistente
    /// </summary>
    public static void CreaCartellaDiSalvataggio()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath) ?? "");
        Log4Elmo.Log($"Cartella di salvataggio creata");
    }

    
    /// <summary>
    /// Controlla se il file di salvataggio specificato esiste
    /// </summary>
    /// <param name="fileName">File da cercare</param>
    /// <returns>True se il file esiste, altrimenti False</returns>
    public static bool Esiste(string fileName)
    {
        return File.Exists(saveFilePath + fileName);
    }


    /// <summary>
    /// Salva tutti i dati di salvataggio
    /// </summary>
    public void Salva()
    {
        Log4Elmo.Log("Salvataggio dei dati del gioco");
        Salva(Gioco.giocatore);
        SalvaStanze();
        SalvaPersonaggi();
        Salva(Gioco);

        AdvConsole.StampaMessaggio("Partita salvata.");
    }

    // Salva i dati del personaggio
    private void SalvaPersonaggi()
    {
        Log4Elmo.Log($"Inizio salvataggio personaggi");
        foreach (Personaggio personaggio in Gioco.personaggi.Values)
        {
            Salva(personaggio);
        }
        Log4Elmo.Log($"Personaggi salvati");
    }

    // Salva i dati delle stanze
    private void SalvaStanze()
    {
        foreach (Stanza s in Gioco.stanze.Values)
        {
            Salva(s);
        }
    }

    /// <summary>
    /// Carica tutti i dati dai salvataggi
    /// </summary>
    public void Carica()
    {
        if (!Esiste(gioco.SaveFileName))
        {
            AdvConsole.StampaMessaggio("Nessun salvataggio trovato!");
            Log4Elmo.Log($"Nessun file di salvataggio tovato", Log4Elmo.WARNING);
            return;
        }
        Log4Elmo.Log("Caricamento dei dati");
        CaricaGiocatore();
        CaricaStanze();
        CaricaPersonaggi();
        CaricaGioco();
        AdvConsole.Clear();
        AdvConsole.StampaMessaggio("Partita caricata.");
        Log4Elmo.Log($"Partita caricata");
    }

    // Carica i dati degli NPC
    private void CaricaPersonaggi()
    {
        Log4Elmo.Log($"Inizio caricamento dati dei personaggi");
        foreach (Personaggio personaggio in Gioco.personaggi.Values)
        {
            Carica(personaggio);
        }
        Log4Elmo.Log($"Dati dei personaggi caricati");
    }

    // Carica i dati del gioco
    private void CaricaGioco()
    {
        Carica(Gioco);
        Gioco.CollegaStanze();
        Gioco.AggiungiPersonaggiAlleStanze();
    }

    // Carica i dati delle stanze
    private void CaricaStanze()
    {
        Log4Elmo.Log($"Inizio caricamento dati nelle stanze");
        foreach (Stanza s in Gioco.stanze.Values)
        {
            Carica(s);
            s.CaricaOggetti(Gioco.oggetti);
        }
        Log4Elmo.Log($"Dati nelle stanze caricate");
    }

    // Carica i dati del giocatore, l'inventario e muove il giocatore nella stanza corretta
    private void CaricaGiocatore()
    {
        Log4Elmo.Log($"Inizio caricamento dati giocatore");
        Giocatore giocatore = Gioco.giocatore;
        Carica(giocatore);
        giocatore.Muovi(Gioco.stanze[Gioco.giocatore.IdStanza]);
        giocatore.CaricaOggetti(Gioco.oggetti);
        Log4Elmo.Log($"Dati giocatore caricati");
    }

    // Salva i dati usando il metodo Salva dell'interfaccia ISalvabile
    /// <see cref="ISalvabile.Salva(string)"/>
    private static void Salva(ISalvabile s)
    {
        try
        {
            Log4Elmo.Log($"Salvataggio in corso");
            s.Salva(saveFilePath);
        } 
        catch (Exception e)
        {
            AdvConsole.StampaMessaggio($"Errore nel salvataggio dei dati: {e.Message}");
            Log4Elmo.Log($"Errore nel salvataggio dei dati: {e.Message}", Log4Elmo.ERROR);
        }
    }

    // Carica i dati usando il metodo Carica dell'interfaccia ISalvabile
    /// <see cref="ISalvabile.Carica(string)"/>
    private static void Carica(ISalvabile s)
    {
        try
        {
            s.Carica(saveFilePath);
        } 
        catch (Exception e)
        {
            AdvConsole.StampaMessaggio($"Errore nel caricamento dei dati: {e.Message}");
            Log4Elmo.Log($"Errore nel caricamento dei dati: {e.Message}", Log4Elmo.ERROR);
        }
    }
}
