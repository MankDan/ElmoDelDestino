using Gioco.Config;
using Gioco.ConsoleUtils;
using Gioco.Enumeratori;
using Gioco.Interfacce;
using Gioco.Logger;
using Gioco.Oggetti;
using Gioco.Oggetti.Altri;
using Gioco.Oggetti.Armi;
using Gioco.Oggetti.Consumabili;
using Gioco.Oggetti.Missioni;
using Gioco.Personaggi;
using Gioco.Stanze;
using System.IO;

namespace Gioco.Giocatori;

/// <summary>
/// Rappresenta il giocatore del gioco
/// </summary>
public class Giocatore : ISalvabile, ISalute
{
    // Return codes per il metodo RaccogliOggetto
    /// <see cref="RaccogliOggetto(Oggetto)"/>
    public static readonly short OGGETTO_NON_RACCOGLIBILE = -2;
    public static readonly short SPAZIO_INVENTARIO_INSUFFICIENTE = -1;
    public static readonly short SUCCESSO = 1;

    private readonly GiocatoreConfig config;

    private readonly short MinMoltiplicatoreDanno;
    private readonly short MaxMoltiplicatoreDanno;
    public string Nome { get; private set; }
    public Stack<Oggetto> Inventario { get; set; }
    public Stanza StanzaAttuale { get; set; }
    private readonly List<short> IdOggetti;
    private readonly Random random;
    public short IdStanza;

    // File di salvataggio delle informazioni di Giocatore
    public string SaveFileName => "Giocatore.save";

    public short SaluteMassima { get; set; }
    public short Salute { get; set; }

    private short forza;
    private short capacitaMassima;
    private short pesoAttuale;

#pragma warning disable
    public Giocatore()
    {
        // Importa i dati dal file di configurazione
        config = GenericConfig<GiocatoreConfig>.CaricaConfigurazione(GiocatoreConfig.ConfigFileName);
        Log4Elmo.Log($"Caricamento dati di configurazione da '{GiocatoreConfig.ConfigFileName}' in corso");
        IdStanza = config.IdStanza;
        Nome = config.Nome;
        capacitaMassima = config.CapacitaMassima;
        forza = config.Forza;
        SaluteMassima = config.SaluteMax;
        Salute = SaluteMassima;
        MaxMoltiplicatoreDanno = config.MaxMoltiplicatoreDanno;
        MinMoltiplicatoreDanno = config.MinMoltiplicatoreDanno;
        Log4Elmo.Log($"Caricamento dati di configurazione da '{GiocatoreConfig.ConfigFileName}' completato");

        pesoAttuale = 0;
        Inventario = [];
        IdOggetti = [];
        random = new();
    }
#pragma warning restore

    /// <summary>
    /// Raccoglie un oggetto e lo mette nell'inventario
    /// </summary>
    /// <param name="oggetto">L'oggetto da raccogliere</param>
    /// <returns><see cref="OGGETTO_NON_RACCOGLIBILE"/> se l'oggetto non è raccoglibile, <see cref="SPAZIO_INVENTARIO_INSUFFICIENTE"/> se non c'è abbastanza spazio nell'inventario, altrimenti <see cref="SUCCESSO"/></returns>
    public short RaccogliOggetto(Oggetto oggetto)
    {
        if (!oggetto.Raccoglibile)
            return OGGETTO_NON_RACCOGLIBILE;

        if (pesoAttuale + oggetto.Peso > capacitaMassima)
            return SPAZIO_INVENTARIO_INSUFFICIENTE;


        Inventario.Push(oggetto);
        pesoAttuale += oggetto.Peso;
        Log4Elmo.Log($"Oggetto {oggetto.Nome} raccolto e aggiunto all'inventario");
        return SUCCESSO;
    }

    /// <summary>
    /// Utilizza l'ultimo oggetto raccolto nell'inventario, usando la logica LIFO (Last In First Out)
    /// </summary>
    public void UsaOggetto()
    {
        if (Inventario.Count < 1)
        {
            AdvConsole.StampaMessaggio("Non hai oggetti nel tuo inventario! Assicurati di raccogliere prima qualcosa.");
            Log4Elmo.Log($"Impossibile usare l'oggetto, inventario vuoto!", Log4Elmo.WARNING);
            return;
        }

        Oggetto oggetto = RimuoviOggettoInventario();
        GestisciOggetto(oggetto);
    }

    /// <summary>
    /// Stampa gli oggetti contenuti nell'inventario
    /// </summary>
    public void StampaInventario()
    {
        string titolo = $"INVENTARIO {pesoAttuale} / {capacitaMassima}";
        AdvConsole.StampaCentrato(titolo, AdvConsole.SEZIONE_TITOLO);
        string buffer = $"";
        Inventario.ToList().ForEach(element => buffer += ($"- {element.Nome} ({element.Peso}) \n  {element.Descrizione}\n\n"));
        AdvConsole.StampaMessaggio(buffer);
    }

    /// <summary>
    /// Rimuove l'ultimo oggetto raccolto dall'inventario usando la logica LIFO (Last In First Out)
    /// </summary>
    public void LasciaOggetto()
    {
        if (Inventario.Count < 1)
        {
            AdvConsole.StampaMessaggio("Non hai oggetti da lasciare.");
            Log4Elmo.Log($"Non puoi lasciare oggetti. L'inventario è vuoto", Log4Elmo.WARNING);
            return;
        }
        Oggetto oggetto = RimuoviOggettoInventario();
        StanzaAttuale.AggiungiOggetto(oggetto);
        AdvConsole.StampaMessaggio($"Hai lasciato {oggetto.Nome} seguendo la logica LIFO (Last In First Out). Se non sai cos'è informati.");
        Log4Elmo.Log($"L'oggetto {oggetto.Nome} con ID: {oggetto.Id} è stato lasciato in {StanzaAttuale.Nome} con ID: {StanzaAttuale.Id}");

    }

    /// <summary>
    /// Conta quanti oggetti del tipo T sono presenti nell'inventario
    /// </summary>
    /// <typeparam name="T">Il tipo di oggetto da cercare</typeparam>
    /// <returns>Numero di oggetti di tipo T presenti nell'inventario</returns>
    public short Conta<T>() => (short)Inventario.Count(oggetto => oggetto is T);

    /// <summary>
    /// Cerca un oggetto di un tipo specifico nell'inventario del giocatore
    /// </summary>
    /// <typeparam name="T">Tipo dell'oggetto da cercare</typeparam>
    /// <returns>True se trova un oggetti del tipo T, altrimenti False</returns>
    public bool Possiede<T>() => Inventario.Any(o => o is T);

    /// <summary>
    /// Rimuove tutti gli oggetti di un tipo T generico dall'inventario
    /// </summary>
    /// <typeparam name="T">Tipo di oggetto da rimuovere nell'inventario</typeparam>
    public void RimuoviAll<T>()
    {
        List<Oggetto> tempList = new(Inventario);
        tempList.RemoveAll(oggetto => oggetto is T);
        tempList.Reverse();
        Inventario = new Stack<Oggetto>(tempList);
        RicalcolaSpazioInventario();
        Log4Elmo.Log($"Tutti gli oggetti di tipo {typeof(T).Name} sono stati rimossi");
    }

#pragma warning disable
    /// <summary>
    /// Muove il giocatore in una specifica direzione, se disponibile.
    /// </summary>
    /// <param name="direzione">La direzione scelta per il movimento del giocatore</param>
    public void Muovi(Direzione direzione)
    {
        Log4Elmo.Log($"Giocatore si muove: direzione = {direzione}");
        if (StanzaAttuale.Porte[(short)direzione] == null)
        {
            AdvConsole.StampaMessaggio("Non puoi andare in quella direzione.");
            Log4Elmo.Log($"Impossibile muoversi in quella direzione", Log4Elmo.WARNING);
            return;
        }

        Muovi(StanzaAttuale.Porte[(short)direzione]);
    }
#pragma warning restore

    /// <summary>
    /// Muove il giocatore in una specifica <see cref="Stanza"/>
    /// </summary>
    /// <param name="stanza">La stanza in cui muovere il giocatore</param>
    public void Muovi(Stanza stanza)
    {
        StanzaAttuale = stanza;
        IdStanza = StanzaAttuale.Id;
        Log4Elmo.Log($"Il giocatore si è mosso nella stanza con ID {IdStanza}");
    }


    /// <summary>
    /// Interagisci con il personaggio presente nella stanza, se presente
    /// </summary>
    public void Parla()
    {
        if (StanzaAttuale.Personaggi.Count < 1)
        {
            AdvConsole.StampaMessaggio("Nessun personaggio presente nella stanza con cui interagire");
            Log4Elmo.Log($"Nessun personaggio presente nella stanza con cui interagire", Log4Elmo.WARNING);
            return;
        }

        StanzaAttuale.Personaggi[0].Interagisci(this);
    }

    /// <summary>
    /// Riduce la salute del giocatore e stampa un messaggio con i dettagli
    /// </summary>
    /// <param name="attacco">Il quantitativo di danni ricevuti dal giocatore</param>
    public void SubisciDanno(short attacco)
    {
        Salute -= attacco;
        AdvConsole.StampaMessaggio($"Hai subito danni:\n\n{this.Nome}\n{((ISalute)this).OttieniBarraSalute(Math.Max(this.Nome.Length, 6))} {Math.Max(Salute, (short)0)} / {SaluteMassima} HP   ( -{attacco} )");
        Log4Elmo.Log($"{Nome} ha subito {attacco} danni, {Salute} / {SaluteMassima} HP" );
    }

    /// <summary>
    /// Chiede in input il nome di personaggio utilizzando <see cref="AdvConsole.OttieniInputUtente(string)"/>e pulisce lo schermo 
    /// <seealso cref="AdvConsole"/>
    /// </summary>
    public void InserisciNome()
    {
        AdvConsole.Clear();
        Nome = AdvConsole.OttieniInputUtente("Inserisci il nome del personaggio: ");
        Log4Elmo.Log($"L'utente ha inserito il nome {Nome}");
        AdvConsole.Clear();
    }

    /// <summary>
    /// Controlla se il giocatore è ancora vivo.
    /// </summary>
    /// <returns>True se il giocatore è vivo, altrimenti False.</returns>
    public bool IsVivo() => Salute > 0;

    private void GestisciOggetto(Oggetto oggetto)
    {
        Log4Elmo.Log($"Provo ad usare l'oggetto di tipo {oggetto.GetType()}, chiamato {oggetto.Nome}");
        switch (oggetto)
        {
            case Zaino:
                AumentaCapacitaMassima(((Zaino)oggetto).AumentoCapacita);
                Log4Elmo.Log($"Oggetto {oggetto.Nome} usato");
                break;
            case Pozione:
                AumentaForza(((Pozione)oggetto).AumentoForza);
                Log4Elmo.Log($"Oggetto {oggetto.Nome} usato");
                break;
            case Elisir:
                RipristinaSalute(((Elisir)oggetto).RipristinoSalute);
                Log4Elmo.Log($"Oggetto {oggetto.Nome} usato");
                break;
            case Arma:
                GestisciArma((Arma)oggetto);
                return;
        }

        if (oggetto.Usabile)
        {
            AdvConsole.StampaMessaggio($"Hai usato {oggetto.Nome}. {oggetto.Descrizione}");
            Log4Elmo.Log($"Oggetto {oggetto.Nome} usato");
        }
        else
        {
            AdvConsole.StampaMessaggio($"Non puoi usare l'oggetto {oggetto.Nome}");
            RaccogliOggetto(oggetto);
            Log4Elmo.Log($"Oggetto {oggetto.Nome} non è usabile");
        }
    }

    // Gestisce le interazioni con le armi e il combattimento
    private void GestisciArma(Arma arma)
    {
        RaccogliOggetto(arma);

        if (StanzaAttuale.Personaggi.Count < 1 || StanzaAttuale.Personaggi[0] is Enemy == false)
        {
            AdvConsole.StampaMessaggio("Non c'è nessuno da attaccare qui!");
            Log4Elmo.Log($"Oggetto {arma.Nome} non usato. Non c'è nessuno da attaccare qui", Log4Elmo.WARNING);
            return;
        }

        Enemy nemico = (Enemy)StanzaAttuale.Personaggi[0];
        if (!nemico.ProntoPerIlCombattimento)
        {
            Log4Elmo.Log($"Non puoi ancora attaccare il nemico {nemico.Nome}. Non sei ancora pronto!", Log4Elmo.WARNING);
            AdvConsole.StampaMessaggio("Non puoi ancora attaccare questo nemico! Non sei ancora pronto al combattimento!");
            return;
        }
        Combatti(arma, nemico);
    }

    // Turno di combattimento tra il Giocatore e il nemico finale
    private void Combatti(Arma arma, Enemy nemico)
    {
        Log4Elmo.Log($"{nemico.Nome} e {Nome} stanno combattendo");
        short danno = CalcolaDanno(arma);
        ((ISalute)nemico).SubisciDanno(danno);
        AdvConsole.StampaMessaggio($"Hai attaccato: \n\n{nemico.Nome}\n{((ISalute)nemico).OttieniBarraSalute(nemico.Nome.Length)} {Math.Max(nemico.Salute, (short)0)} / {nemico.SaluteMassima} hp   (-{danno})");

        if (((ISalute)nemico).IsVivo())
        {
            nemico.Attacca(this);
            VerificaMorteGiocatore(nemico);
        }
        else
        {
            Log4Elmo.Log($"{nemico.Nome} è morto. Partita terminata! Il giocatore ha vinto");
            AdvConsole.StampaCentrato($"VITTORIA", AdvConsole.SEZIONE_TITOLO);
            AdvConsole.Stampa($"Hai sconfitto {nemico.Nome} e raccolto l'elmo del destino utilizzando la chiave.   ", AdvConsole.SEZIONE_INFORMAZIONI, true);
            AdvConsole.StampaMessaggio("Hai vinto!");
            Environment.Exit(0);
        }
    }

    // Calcola il danno applicato dal giocatore
    private short CalcolaDanno(Arma arma) => (short)(forza * arma.Danno * random.Next(MinMoltiplicatoreDanno, MaxMoltiplicatoreDanno) + random.Next(-(forza * arma.Danno), forza * arma.Danno));

    // Restituisce un messaggio di "Game Over" non appena la Salute del Giocatore raggiunge 0 
    private void VerificaMorteGiocatore(Enemy nemico)
    {
        if (!IsVivo())
        {
            AdvConsole.StampaMessaggio($"Sei stato ucciso da {nemico.Nome}.\nHai perso!\n\nInizia una nuova avventura e riprova!");
            Log4Elmo.Log($"{Nome} è morto! Partita terminata");
            Environment.Exit(0);
        }
    }

    // Ricalcola lo spazio dell'inventario
    private void RicalcolaSpazioInventario() => pesoAttuale = (short)Inventario.Sum(o => o.Peso);

    // Rimuove l'ultimo oggetto dall'inventario, aggiornando il peso
    private Oggetto RimuoviOggettoInventario()
    {
        Oggetto oggetto = Inventario.Pop();
        pesoAttuale -= oggetto.Peso;
        return oggetto;
    }

    // Aumenta lo spazio dell'inventario
    private void AumentaCapacitaMassima(short capacitaMassima) => this.capacitaMassima += capacitaMassima;

    private void AumentaForza(short forza) => this.forza += forza;

    private void RipristinaSalute(short cura) => Salute = (short)Math.Min(SaluteMassima, Salute + cura);

    /// <summary>
    /// Salva le informazioni di <see cref="Giocatore"/> all'interno di un file di salvataggio utilizzando <see cref="StreamWriter"/>
    /// </summary>
    /// <param name="SaveFilePath">Percorso file della cartella contenente il file di salvataggio</param>
    public void Salva(string SaveFilePath)
    {
        using StreamWriter writer = new(SaveFilePath + SaveFileName);
        writer.WriteLine(Nome);
        writer.WriteLine(StanzaAttuale.Id);
        writer.WriteLine(capacitaMassima);
        writer.WriteLine(forza);
        SalvaOggetti(writer);
        writer.Close();
    }

    /// <summary>
    /// Scrive gli id degli oggetti contenuti nell'inventario utilizzando <see cref="StreamWriter"/>
    /// </summary>
    /// <param name="writer">lo <see cref="StreamWriter"/>Utilizzato per scrivere gli id degli oggetti</param>
    private void SalvaOggetti(StreamWriter writer)
    {
        foreach (var oggetto in Inventario)
        {
            writer.WriteLine(oggetto.Id);
        }
    }

    /// <summary>
    /// Carica i dati contenuti nel salvataggio
    /// </summary>
    /// <remarks>
    /// Questo metodo non carica gli oggetti nell'inventario, gli oggetti vengono aggiunti successivamente da <see cref="CaricaOggetti(Dictionary{int, Oggetto})"/>
    /// </remarks>
    /// <param name="SaveFilePath">Nome del file contenente il salvataggio</param>
    /// <exception cref="FileNotFoundException"></exception>
    public void Carica(string SaveFilePath)
    {
        if (!File.Exists(SaveFilePath + SaveFileName))
            return;
            // throw new FileNotFoundException(SaveFilePath + SaveFileName);

        using StreamReader reader = new(SaveFilePath + SaveFileName);

        Salute = SaluteMassima;
        Nome = reader.ReadLine() ?? "" + config.Nome;
        IdStanza = short.Parse(reader.ReadLine() ?? "" + config.IdStanza);
        capacitaMassima = short.Parse(reader.ReadLine() ?? "" + config.CapacitaMassima);
        forza = short.Parse(reader.ReadLine() ?? "" + config.Forza);
        CaricaIdOggetti(reader);
    }

    /// <summary>
    /// Carica gli Id degli oggetti da inserire nell'inventario.
    /// </summary>
    /// <param name="reader"> il <see cref="StreamReader"> utilizzato per leggere gli oggetti</see>/></param>
    /// <remarks>
    /// Questo metodo non carica gli oggetti nell'inventario, gli oggetti vengono aggiunti successivamente da <see cref="CaricaOggetti(Dictionary{int, Oggetto})"/>
    /// </remarks>
    private void CaricaIdOggetti(StreamReader reader)
    {
        IdOggetti.Clear();

        short oggettoId;
        while ((oggettoId = Convert.ToByte(reader.ReadLine())) != 0)
        {
            IdOggetti.Add(oggettoId);
        }
        IdOggetti.Reverse();
    }

    /// <summary>
    /// Svuota l'inventario e carica i nuovi oggetti usando gli id in <see cref="IdOggetti"/>
    /// </summary>
    /// <param name="oggetti">Oggetto del giocatore</param>
    public void CaricaOggetti(Dictionary<int, Oggetto> oggetti)
    {
        Inventario.Clear();

        foreach (int id in IdOggetti)
        {
            Oggetto oggetto = oggetti[id];
            if (oggetto != null)
            {
                Inventario.Push(oggetto);
            }
        }

        RicalcolaSpazioInventario();
    }
}