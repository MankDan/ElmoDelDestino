
using Gioco.ConsoleUtils;
using Gioco.Enumeratori;
using Gioco.Giocatori;
using Gioco.Interfacce;
using Gioco.Oggetti;
using Gioco.Oggetti.Altri;
using Gioco.Oggetti.Armi;
using Gioco.Oggetti.Consumabili;
using Gioco.Oggetti.Missioni;
using Gioco.Personaggi;
using Gioco.Stanze;
using Gioco.Managers;
using Gioco.Config;
using Gioco.Logger;

namespace Gioco;

/// <summary>
/// La classe da istanziare per iniziare il gioco
/// </summary>
public class Game : ISalvabile
{
    private readonly SaveLoadManager saveLoadManager;

    public static int NumeroPergamene { get; private set; }

    public Giocatore giocatore;
    public readonly Dictionary<int, Stanza> stanze;
    public readonly Dictionary<int, Oggetto> oggetti;
    public readonly Dictionary<int, Personaggio> personaggi;
    private readonly Random random;
    private int Seed;
    private int IdChiave;
    private int IdElmo;

    /// <summary>
    /// Nome del file di salvataggio per il seed della partita
    /// </summary>
    public string SaveFileName => "seed.save";

    /// <summary>
    /// Prepara il gioco per essere inizializzato
    /// </summary>
    public Game()
    {
        saveLoadManager = new(this);
        SaveLoadManager.CreaCartellaDiSalvataggio();
        ConfigurationManager.CreaCartellaDiConfigurazione();

        // equivale a:
        // stanza = new Dictionary<int, Stanza>();
        stanze = [];
        oggetti = [];
        personaggi = [];
        giocatore = new();
        random = new();

        GameConfig config = GenericConfig<GameConfig>.CaricaConfigurazione(GameConfig.ConfigFileName);
        NumeroPergamene = config.NumeroPergamene;
        Seed = config.Seed != 0 ? config.Seed : random.Next();

        CreaStanze();
        CreaOggetti();
        CreaPersonaggi();

        if (ChiediCaricaSalvataggio())
        {
            saveLoadManager.Carica();
        }
        else
        {
            PreparaNuovaPartita();
        }

        giocatore.Muovi(stanze[giocatore.IdStanza]);

        AdvConsole.SalvaStato();

        AdvConsole.AbilitaInputVeloce();
    }

    private void PreparaNuovaPartita()
    {
        AggiungiOggettiAlleStanze();
        AdvConsole.Clear();
        giocatore.InserisciNome();
        MostraMessaggioIntroduttivo();
        CollegaStanze();
        AggiungiPersonaggiAlleStanze();
    }

    private void CreaStanze()
    {
        // Creazione delle stanze
        Log4Elmo.Log("Inizio creazione stanze");

        stanze.Add(Stanza.ID, new Stanza("Ingresso", "Sei all'ingresso della rocca. Le porte di legno aprono un vasto atrio.", '^'));
        stanze.Add(Stanza.ID, new Stanza("Corridoio", "Corridoio lungo e scarsamente illuminato da qualche torcia tremolante.", '\''));
        stanze.Add(Stanza.ID, new Stanza("Sala del Guerriero Caduto", "Le statue dei guerrieri caduti sembrano osservarti con occhi spenti.", ':'));
        stanze.Add(Stanza.ID, new Stanza("Corridoio", "Un corridoio stretto.", '.'));
        stanze.Add(Stanza.ID, new Stanza("Biblioteca del Sapiente", "Stanza piena di scaffali traboccanti di libri arcaici e polverosi.", '+'));
        stanze.Add(Stanza.ID, new Stanza("Corridoio", "Corridoio con arazzi sbiaditi di battaglie passate.", '.'));
        stanze.Add(Stanza.ID, new Stanza("Ripostiglio", "Una stanza disordinata dove sono accatastati oggetti superflui.", '*'));
        stanze.Add(Stanza.ID, new Stanza("Corridoio", "Un corridoio con un soffitto parecchio basso.", '.'));
        stanze.Add(Stanza.ID, new Stanza("Sala delle Pozioni", "Alambicchi, bottiglie, vasi ed ebre riempiono questa stanza.", ';'));
        stanze.Add(Stanza.ID, new Stanza("Corridoio", "Corridoio ricco di antichi supporti per torce ormai vuoti.", '.'));
        stanze.Add(Stanza.ID, new Stanza("Sala del Trono", "Una sala maestosa con un trono d'oro ornato al centro.", ','));
        stanze.Add(Stanza.ID, new Stanza("Corridoio", "Un ennesimo corridoio stretto.", '.'));
        stanze.Add(Stanza.ID, new Stanza("Sala", "Sala relativamente spoglia, senza particolari elementi di rilievo.", ','));
        stanze.Add(Stanza.ID, new Stanza("Corridoio", "Corridoio con mosaici che ritraggono scene di vita quotidiana del tempo.", '*'));
        stanze.Add(Stanza.ID, new Stanza("Cucina", "Cucina piena di utensili di ogni genere e ricca di molte spezie.", ','));
        stanze.Add(Stanza.ID, new Stanza("Corridoio", "Corridoio avvolto in un silenzio surreale, rotto solo dal suono dei passi.", '.'));
        stanze.Add(Stanza.ID, new Stanza("Armeria", "Luogo dove spade e altri oggetti contundenti risiedono inutilizzati.", '.'));
        stanze.Add(Stanza.ID, new Stanza("Anticamera dell'elmo", "Anticamera con mura spoglie e un'atmosfera tetra.", '.'));
        stanze.Add(Stanza.ID, new Stanza("Sala da gaming", "Un luogo per giocare, forse a Pong.", '.'));
        stanze.Add(Stanza.ID, new Stanza("Camera dell'Elmo", "Camera circolare dove l'Elmo del Destino è protetto dal Guardiano.", '\''));

        Log4Elmo.Log("Fine creazione stanze, stanze aggiunte alla lista di stanze");
    }

    private void CreaOggetti()
    {
        Log4Elmo.Log("Inizio creazione oggetti");
        // Creazione degli oggetti       
        for (int i = 0; i < NumeroPergamene; i++)
            oggetti.Add(Oggetto.ID, new Pergamena("Pergamena Antica", $"Una delle {NumeroPergamene} da portare al vecchio saggio.", 5, true, false));
        
        oggetti.Add(Oggetto.ID, new Spada("Spada Antica", "Una spada antica con rune magiche. La lama emana uno strano bagliore.", 30, true, true));
        oggetti.Add(Oggetto.ID, new Pozione("Pozione di Forza", "Una pozione dal color rosso fuoco che aumenta la tua forza.", 10, true, true));
        oggetti.Add(Oggetto.ID, new Zaino("Zaino", "Uno zaino che aumenta la capacità dell'inventario quando usato.", 0, true, true));
        
        IdChiave = Oggetto.ID;
        oggetti.Add(Oggetto.ID, new Chiave("Chiave Dorata", "Una chiave dorata. È molto più pesante rispetto alle chiavi normali.", 15, true, false));
        
        oggetti.Add(Oggetto.ID, new Statua("Statua di Marco", "Una Statua enorme raffigurante Marco Montemagno.", 255, false, false));
        
        IdElmo = Oggetto.ID;
        oggetti.Add(Oggetto.ID, new Elmo("Elmo del Destino", "L'oggetto finale da ottenere. L'Elmo è decorato con intricati dettagli.", 10, false, false));
        oggetti.Add(Oggetto.ID, new Pugnale("Pugnale antico", "Un piccolo pugnale seghettato. La lama mostra segni di usura.", 15, true, true));
        oggetti.Add(Oggetto.ID, new Teschio("Teschio", "Un teschio dell'avventuriero che ti ha preceduto in questa avventura.", 255, false, false));
        oggetti.Add(Oggetto.ID, new Elisir("Elisir di cura", "Un elisir in grado di ripristinare ? HP", 25, true, true));
        Log4Elmo.Log("Fine creazione oggetti");
    }

    private void AggiungiOggettiAlleStanze()
    {
        Random random = new(Seed);
        Log4Elmo.Log($"Inizio Agginta oggetti alle Stanze");

        int numeroStanze = stanze.Count;
        Log4Elmo.Log($"Inizio aggiunta pergamene alle stanze con seed: {Seed}");
        for (int i = 1; i < NumeroPergamene+1; i++)
        {
            stanze[random.Next( 2+((i-1) * (int)(numeroStanze / NumeroPergamene)), i * (int)(numeroStanze / NumeroPergamene))].AggiungiOggetto(oggetti[i]);
        }
        Log4Elmo.Log($"Pergamene aggiunte alle stanze correttamente");
        stanze[17].AggiungiOggetto(oggetti[NumeroPergamene + 1]);
        stanze[9].AggiungiOggetto(oggetti[NumeroPergamene + 2]);
        int idStanzaZaino = (random.Next(3, 8) * 2) + 1;
        stanze[idStanzaZaino].AggiungiOggetto(oggetti[NumeroPergamene + 3]);
        stanze[18].AggiungiOggetto(oggetti[NumeroPergamene + 5]);
        stanze[20].AggiungiOggetto(oggetti[NumeroPergamene + 6]);
        stanze[14].AggiungiOggetto(oggetti[NumeroPergamene + 7]);
        stanze[random.Next(2, 9)].AggiungiOggetto(oggetti[NumeroPergamene + 8]);
        stanze[19].AggiungiOggetto(oggetti[NumeroPergamene + 9]);
        Log4Elmo.Log($"Oggetti aggiunti correttamente alle stanze");
    }

    private void CreaPersonaggi()
    {
        Log4Elmo.Log($"Creazione personaggi in corso");
        personaggi.Add(Personaggio.ID, new Saggio("Saggio", "Un saggio che custodisce antichi segreti con la sua immensa conoscenza.", oggetti[IdChiave]));
        personaggi.Add(Personaggio.ID, new Enderman("Enderman", "Un enderman in grado di teletrasportarti in giro", Teletrasporta));
        personaggi.Add(Personaggio.ID, new Guardiano("Guardiano", "Un valoroso guardiano che protegge l'Elmo con tutta la sua volontà.", oggetti[IdElmo]));
        Log4Elmo.Log($"Creazione personaggi completata");
    }

    /// <summary>
    /// Aggiungi al massimo un NPC per stanza in un set di stanze preimpostate
    /// </summary>
    public void AggiungiPersonaggiAlleStanze()
    {
        Log4Elmo.Log($"Inizio aggiunta personaggi alle stanze con seed: {Seed}");
        Random random = new(Seed);
        stanze[random.Next(2, 5)].AggiungiPersonaggio(personaggi[1]);
        stanze[random.Next(6, 10)].AggiungiPersonaggio(personaggi[2]);
        stanze[Stanza.ID - 1].AggiungiPersonaggio(personaggi[3]);
        Log4Elmo.Log($"Personaggi aggiunti alle stanze");
    }

    /// <summary>
    /// Crea dei collegamenti casuali tra le stanze
    /// </summary>
    public void CollegaStanze()
    {
        Log4Elmo.Log($"Inizio collegamento stanze con seed {Seed}");
        Stanza? stanzaPrecedente = null;
        Random rnd = new(Seed);
        Direzione direzioneCasuale;
        int skip = 0;
        foreach (Stanza stanza in stanze.Values)
        {
            if (stanzaPrecedente == null)
            {
                stanzaPrecedente = stanza;
                continue;
            }

            int risultato;
            do
            {
                direzioneCasuale = (Direzione)rnd.Next(0, 4);
                
                risultato = stanzaPrecedente.CollegaStanza(stanza, direzioneCasuale);
            } while (risultato == Stanza.PORTA_GIA_COLLEGATA);

            if (skip == 0)
                stanzaPrecedente = stanza;
            skip = (skip + 1) % 2;
        }
        Log4Elmo.Log($"Collegamento stanze completato");

    }

    // Permette di scegliere quale oggetto raccogliere
    private void Raccogli()
    {
        string nomeOggetto = AdvConsole.OttieniInputUtente("Raccogli oggetto:");
        AdvConsole.AbilitaInputVeloce();
        AdvConsole.PulisciSchermo();
       // AdvConsole.ResettaPosizione();
        Oggetto? oggetto = giocatore.StanzaAttuale.Oggetti.Find(o => o.Nome.ToLower().Split()[0] == nomeOggetto.ToLower().Split()[0]);
        
        if (oggetto == null)
        {
            AdvConsole.StampaMessaggio("Oggetto non trovato.");
            Log4Elmo.Log($"Impossibile raccogliere l'oggetto {nomeOggetto}. Oggetto non trovato.", Log4Elmo.WARNING);
            return;
        }
         
        int result = giocatore.RaccogliOggetto(oggetto);

        if (result == Giocatore.SPAZIO_INVENTARIO_INSUFFICIENTE)
        {
            AdvConsole.StampaMessaggio("Non puoi raccogliere questo oggetto. Spazio nell'inventario insufficiente.");
            Log4Elmo.Log($"Impossibile raccogliere l'oggetto {oggetto.Nome} con id {oggetto.Id}. Spazio nell'inventario insufficiente", Log4Elmo.WARNING);
        }

        if (result == Giocatore.OGGETTO_NON_RACCOGLIBILE)
        {
            AdvConsole.StampaMessaggio("Questo oggetto non è raccoglibile.");
            Log4Elmo.Log($"Impossibile raccogliere l'oggetto {oggetto.Nome} con id {oggetto.Id}. Oggetto non raccoglibile", Log4Elmo.WARNING);
        }

        if (result == Giocatore.SUCCESSO)
        {
            AdvConsole.StampaMessaggio($"Hai raccolto {oggetto.Nome}.");
            giocatore.StanzaAttuale.Oggetti.Remove(oggetto);
            Log4Elmo.Log($"Oggetto {oggetto.Nome} raccolto.");
        }
    }

    private void MostraMessaggioIntroduttivo()
    {
        
        AdvConsole.StampaCentrato("L'ELMO DEL DESTINO", AdvConsole.SEZIONE_TITOLO);
        AdvConsole.StampaMessaggio($"In un tempo remoto, nascosta tra le ombre delle montagne più alte, giaceva una rocca antica e dimenticata, conosciuta solo come la Rocca del Destino. Questo luogo, avvolto nel mistero, era un tempo la dimora di un potente sovrano che ne possedeva il suo Elmo, un artefatto leggendario, forgiato presumibilmente dagli Dei stessi, capace di conferire immensi poteri a chiunque lo indossasse. Con il passare dei secoli, la rocca cadde in rovina e l'Elmo fu protetto da un Guardiano temibile, capace di respingere chiunque osasse avvicinarsi.\nTu, {giocatore.Nome}, intrepido avventuriero, hai deciso di accettare la sfida. La tua missione è chiara: trovare l'Elmo del Destino e sconfiggere il Guardiano. Solo allora potrai reclamare il tuo posto tra le leggende. La tua avventura inizia ai piedi di questa rocca, pronto ad affrontare i pericoli e i labirinti che ti attendono.");
    }

    /// <summary>
    /// Stampa un messaggio che chiede al giocatore se vuole caricare un salvataggio
    /// </summary>
    public bool ChiediCaricaSalvataggio()
    {
        if (!SaveLoadManager.Esiste(SaveFileName))
        {
            Log4Elmo.Log($"Nessun file di salvataggio già esistente");
            return false;
        }

        string input = AdvConsole.OttieniInputUtente("Vuoi caricare una partita già salvata? (s/n)").ToLower();
        if (input.Length > 0 && input[0] == 's')
        {
            Log4Elmo.Log($"Invio richiesta caricamento partita");
            return true;
        }

        Log4Elmo.Log($"Inizio nuova partita");
        return false;
    }


    /// <summary>
    /// Contiene il Main Loop del gioco e gestisce l'input dell'utente
    /// </summary>
    public void Inizia() 
    {
        while (true)
        {
            AdvConsole.PulisciSchermo();
            StampaStanza.Stampa(giocatore.StanzaAttuale);
            StampaStanza.StampaDescrizione(giocatore.StanzaAttuale);
            string stringaGiocatore = $"{giocatore.Nome} ({giocatore.Salute} / {giocatore.SaluteMassima} HP) ";
            AdvConsole.Stampa(stringaGiocatore, AdvConsole.SEZIONE_INFO_GIOCATORE);
            AdvConsole.Stampa(((ISalute)giocatore).OttieniBarraSalute(stringaGiocatore.Length - 1), AdvConsole.SEZIONE_BARRA_SALUTE);
            AdvConsole.StampaComandi();
            StampaBossBar();

            ConsoleKeyInfo input = AdvConsole.OttieniInput();
            if (input.Modifiers == ConsoleModifiers.Control)
            {
                if (input.Key == ConsoleKey.S)
                    saveLoadManager.Salva();
                else if (input.Key == ConsoleKey.L)
                {
                    if (SaveLoadManager.Esiste(SaveFileName))
                        saveLoadManager.Carica();
                    else
                        AdvConsole.StampaMessaggio("Nessun salvataggio trovato!");
                }
                else if (input.Key == ConsoleKey.C)
                    break;
            }
            else
            {
                switch (input.Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        giocatore.Muovi(Direzione.Nord);
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        giocatore.Muovi(Direzione.Sud);
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        giocatore.Muovi(Direzione.Est);
                        break;
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        giocatore.Muovi(Direzione.Ovest);
                        break;
                    case ConsoleKey.R:
                        Raccogli();
                        break;
                    case ConsoleKey.U:
                        giocatore.UsaOggetto();
                        break;
                    case ConsoleKey.L:
                        giocatore.LasciaOggetto();
                        break;
                    case ConsoleKey.P:
                        giocatore.Parla();
                        break;
                    case ConsoleKey.I:
                        giocatore.StampaInventario();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void StampaBossBar()
    {
        var personaggi = giocatore.StanzaAttuale.Personaggi;
        if (personaggi.Count > 0 && personaggi[0] is Enemy)
            if (((Enemy)personaggi[0]).StampaBossBar())
                giocatore.StanzaAttuale.Isola();
    }

    // Teletrasporta il giocatore in una stanza casuale
    private void Teletrasporta()
    {
        giocatore.Muovi(stanze[random.Next(stanze.Count) + 1]);
        AdvConsole.StampaMessaggio("Sei stato teletrasportato in una stanza casuale.");
        Log4Elmo.Log($"Giocatore teletrasportato in {giocatore.StanzaAttuale.Nome} con id: {giocatore.StanzaAttuale.Id}");
    }

    /// <summary>
    /// Salva il seed della partita
    /// </summary>
    /// <param name="savefilepath">Percorso di salvataggio</param>
    public void Salva(string SaveFilePath)
    {
        Log4Elmo.Log($"Salvataggio del seed in {SaveFilePath + SaveFileName}");
        using StreamWriter writer = new(SaveFilePath + SaveFileName);
        writer.WriteLine(Seed);      
        writer.Close();
        Log4Elmo.Log($"Seed salvato in {SaveFilePath + SaveFileName}");
    }

    /// <summary>
    /// Carica il seed della partita
    /// </summary>
    /// <param name="savefilepath">Percorso di salvataggio</param>
    public void Carica(string SaveFilePath)
    {
        if (!SaveLoadManager.Esiste(SaveFileName))
        {
            AdvConsole.StampaMessaggio("Nessun salvataggio trovato.");
            Log4Elmo.Log($"File di salvataggio '{SaveFilePath}' non trovato");
            return;
        }

        using StreamReader reader = new(SaveFilePath + SaveFileName);
        Seed = int.Parse(reader.ReadLine() ?? "" + random.Next());
        Log4Elmo.Log($"Seed caricato con valore: {Seed}");
        reader.Close();
    }
}
