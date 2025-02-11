using Gioco.Logger;
using System.Runtime.InteropServices;

namespace Gioco.ConsoleUtils;

/// <summary>
/// Classe per la gesione dell'interfaccia grafica testuale su console
/// </summary>
static partial class AdvConsole
{
    /*
     *        VISUALIZZAZIONE GRAFICA DELLE AREE DELLO SCHERMO
     *        
     *    |=======================================================|
     *    | prompt dei comandi :)                            -[]X |
     *    |=======================================================|
     *    |>....................SEZIONE TITOLO....................|
     *    |>,,,,,,,,,,,,,,,,,SEZIONE DESCRIZIONE,,,,,,,,,,,,,,,,,,|-----
     *    |>.................SEZIONE INFORMAZIONI.................|  |
     *    |.......................................................|  | AREA DI AZIONE
     *    |                                                       |  | PulisciSchermo()
     *    |                                                       |  |
     *    |                                                       |  |  
     *    |                                                       |  |  
     *    |                     SPAZIO LIBERO                     |  |
     *    |                                                       |  |
     *    |                                                       |  |
     *    |                                                       |  |
     *    |                                                       |  |
     *    |> MESSAGGIO INFO INPUT              ..INFO GIOCATORE...|  |
     *    |> INPUT UTENTE                      ,,,BARRA SALUTE,,,,|-----
     *    |>......................................................|
     *    |.................SEZIONE INFO COMANDI..................|
     *    |.......................................................|
     *    |=======================================================|
     *            
     */

    public static int ALLINEAMENTO_A_DESTRA = -1;

    public static (int x, int y) SEZIONE_INFO_COMANDI => (0, Console.WindowHeight - 3);
    public static (int x, int y) SEZIONE_INPUT_UTENTE => (0, Console.WindowHeight - 4);
    public static (int x, int y) SEZIONE_INFO_INPUT => (0, Console.WindowHeight - 5);
    public static (int x, int y) SEZIONE_TITOLO { get; private set; } = (0, 0);
    public static (int x, int y) SEZIONE_DESCRIZIONE { get; private set; } = (0, 1);
    public static (int x, int y) SEZIONE_INFORMAZIONI { get; private set; } = (0, 3);
    public static (int x, int y) SEZIONE_INFO_GIOCATORE => (ALLINEAMENTO_A_DESTRA, Console.WindowHeight - 5);
    public static (int x, int y) SEZIONE_BARRA_SALUTE => (ALLINEAMENTO_A_DESTRA, Console.WindowHeight - 4);

    private const int TimeoutCaratteriDialogo = 50;
    
    private const int STD_INPUT_HANDLE = -10;
    private const int STD_OUTPUT_HANDLE = -11;
    const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    // importa le funzioni da kernel32.dll
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GetStdHandle(int nStdHandle);


    // Flags di input per la console
    [Flags]
    private enum ConsoleInputModes : uint
    {
        ENABLE_PROCESSED_INPUT = 0x0001,
        ENABLE_LINE_INPUT = 0x0002,
        ENABLE_ECHO_INPUT = 0x0004,
        ENABLE_WINDOW_INPUT = 0x0008,
        ENABLE_MOUSE_INPUT = 0x0010,
        ENABLE_INSERT_MODE = 0x0020,
        ENABLE_QUICK_EDIT_MODE = 0x0040,
        ENABLE_EXTENDED_FLAGS = 0x0080,
        ENABLE_AUTO_POSITION = 0x0100,
        ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200
    }

    // Flags di output per la console
    [Flags]
    private enum ConsoleOutputModes : uint
    {
        ENABLE_PROCESSED_OUTPUT = 0x0001,
        ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
        ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
        DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
        ENABLE_LVB_GRID_WORLDWIDE = 0x0010
    }

    private static uint originalConsoleMode = 0;
    private static IntPtr hStdin;

    /// <summary> 
    /// Salva lo stato attuale della console
    /// </summary>
    public static void SalvaStato()
    {
        hStdin = GetStdHandle(STD_INPUT_HANDLE);
        if (hStdin == IntPtr.Zero)
            throw new InvalidOperationException("Impossibile ottenere console handle.");

        if (!GetConsoleMode(hStdin, out originalConsoleMode))
            throw new InvalidOperationException("Impossibile ottenere console mode");
    }

    /// <summary>
    /// Permette di gestire direttamente l'input dell'utente in maniera rapida
    /// </summary>
    public static void AbilitaInputVeloce()
    {
        AbilitaSupportoANSI();

        SetConsoleMode(GetStdHandle(STD_INPUT_HANDLE), (uint)(
            ConsoleInputModes.ENABLE_WINDOW_INPUT |
            ConsoleInputModes.ENABLE_MOUSE_INPUT |
            ConsoleInputModes.ENABLE_EXTENDED_FLAGS
        ));

        Console.CursorVisible = false;
    }

    // Abilita il supporto per i caratteri di escape ANSI
    private static void AbilitaSupportoANSI()
    {
        IntPtr hOutput = GetStdHandle(STD_OUTPUT_HANDLE);

        if (GetConsoleMode(hOutput, out uint dwOriginalOutputMode))
        {
            uint dwNewOutputMode = dwOriginalOutputMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            if (!SetConsoleMode(hOutput, dwNewOutputMode))
            {
                Log4Elmo.Log("Impossibile cambiare output mode della console", Log4Elmo.ERROR);
            }
        }
    }

    /// <summary>
    /// Pulisce la console
    /// </summary>
    public static void Clear() => Console.Clear();


    /// <summary>
    /// Muove il cursore della posizione di default (Sezione informazioni)
    /// </summary>
    [Obsolete]
    private static void ResettaPosizione() => MuoviCursore(SEZIONE_INFORMAZIONI);

    /*
     * carattereBordo = '#', carattereRiempimento = '=', 
     * carattereLimiteTesto = '$', margine = 4
     * 
     * #================$    TESTO    $================#
     *                  
     *                  |----|   |----|
     *                 margine   margine
     */
    /// <summary>
    /// Stampa il testo centrato in una specifica riga della console
    /// </summary>
    /// <param name="testo">Il testo da centrare e stampare</param>
    /// <param name="sezione">La sezione in cui stampare il titolo</param>
    /// <param name="carattereBordo">Il carattere di delimitazione dei bordi del testo (default '#')</param>
    /// <param name="carattereRiempimento">Il carattere per riempire lo spazio attorno al testo (default = '=')</param>
    /// <param name="carattereLimiteTesto">Il carattere che delimita il testo (default '$')</param>
    /// <param name="margine">Lo spazio tra il carattere che delimita il testo e il testo (default 4)</param>
    /// <example>
    /// <code>
    /// AdvConsole.StampaCentrato("TestoDiProva", 0);
    /// // Output:
    /// // #=================$    TestoDiProva    $=================#
    /// </code>
    /// </example>
    public static void StampaCentrato(string testo, (int, int) sezione, char carattereBordo = '#', char carattereRiempimento = '=', char carattereLimiteTesto = '$', int margine = 4)
    {
        MuoviCursore(sezione);
        int width = Console.WindowWidth;
        string buffer = "" + carattereBordo;

        int padding = Math.Max((width - testo.Length - 4) / 2, 0);
        int numeroCaratteriRiempimento = Math.Max(padding - margine, 0);
        buffer += new string(carattereRiempimento, numeroCaratteriRiempimento) + carattereLimiteTesto + new string(' ', margine) + testo +
                new string(' ', margine) + carattereLimiteTesto + new string(carattereRiempimento, numeroCaratteriRiempimento);

        if (buffer.Length < width - 1)
        {
            buffer += carattereRiempimento;
        }

        buffer += carattereBordo;
        Console.Write(buffer);
    }

    /// <summary>
    /// Pulisce la parte di schermo compresa tra la sezione del titolo (esclusa) e la sezione comandi (esclusa)
    /// </summary>
    public static void PulisciSchermo()
    {
        MuoviCursore(SEZIONE_DESCRIZIONE);
        string buffer = new(' ', Console.WindowWidth);
        for (int y = SEZIONE_DESCRIZIONE.y; y < SEZIONE_INFO_COMANDI.y; y++)
        {
            Console.WriteLine(buffer);
        }
    }

    /// <summary>
    /// Stampa il testo nella sezione scelta
    /// </summary>
    /// <param name="testo">Il testo da stampare</param>
    /// <param name="sezione">La sezione dove stampare il testo</param>
    /// <param name="dialogo">Se true, stamperà il testo un carattere alla volta</param>
    public static void Stampa(string testo, (int x, int y) sezione, bool dialogo = false)
    {
        (int x, int y) nuovaPosizione = CalcolaPosizioneCursore(testo, sezione);

        MuoviCursore(nuovaPosizione);

        string testoDaStampare = RitornoACapoAutomatico(testo);

        if (dialogo)
            StampaLentamente(testoDaStampare, sezione);
        else
            Console.Write(testoDaStampare);
    }

    // Aggiunge i ritorni a capo automatici all'interno del testo
    private static string RitornoACapoAutomatico(string testo)
    {
        string[] frase = testo.Split(' ');
        int counter = 0;
        for (int i = 0; i < frase.Length; i++)
        {
            if (frase[i].Contains('\n'))
            {
                counter = frase[i].Length;
                continue;
            }

            if (counter + frase[i].Length >= Console.WindowWidth)
            {
                frase[i] = "\n" + frase[i];
                counter = frase[i].Length;
            }
            else
            {
                counter += frase[i].Length + 1;
            }
        }
        return String.Join(' ', frase);
    }

    private static (int x, int y) CalcolaPosizioneCursore(string testo, (int x, int y) sezione)
    {
        (int x, int y) nuovaPosizione = sezione;
        if (sezione.x == ALLINEAMENTO_A_DESTRA)
        {
            nuovaPosizione.x = Math.Max(Console.WindowWidth - (testo.Length + 1), 0);
        }

        return nuovaPosizione;
    }

    /// <summary>
    /// Stampa il testo aspettando <see cref="TimeoutCaratteriDialogo"/> millisecondi tra un carattere e l'altro
    /// </summary>
    /// <param name="testo">Il testo da stampare</param>
    /// <param name="sezione">La sezione dove stampare il testo</param>
    private static void StampaLentamente(string testo, (int x, int y) sezione)
    {
        foreach (char c in testo)
        {
            Console.Write(c);

            Thread.Sleep(TimeoutCaratteriDialogo);
            if (Console.KeyAvailable)
            {
                OttieniInput();
                break;
            }
        }
        Stampa(testo, sezione);
    }

    /// <summary> 
    /// Stampa un messaggio e aspetta l'input dell'utente per continuare
    /// </summary>
    /// <param name="testo"></param>
    public static void StampaMessaggio(string testo)
    {
        PulisciSchermo();
        Stampa(testo, SEZIONE_INFORMAZIONI, true);
        Console.ReadKey();
    }

    /// <summary>
    /// Stampa i comandi nella parte inferiore della console
    /// <see cref="AdvConsole"/>
    /// </summary>
    public static void StampaComandi()
    {
        MuoviCursore(SEZIONE_INFO_COMANDI);
        string legendaComandi = "Comandi:                                                                     \n" +
            "\u001b[30;47mW\u001b[0m Nord       \u001b[30;47mA\u001b[0m Sud       \u001b[30;47mD\u001b[0m Est       \u001b[30;47mA\u001b[0m Ovest     \u001b[30;47mR\u001b[0m Raccogli  \u001b[30;47mU\u001b[0m Usa\n" +
            "\u001b[30;47mP\u001b[0m Parla      \u001b[30;47mL\u001b[0m Lascia    \u001b[30;47mI\u001b[0m Oggetti   \u001b[30;47m^S\u001b[0m Salva    \u001b[30;47m^L\u001b[0m Carica   \u001b[30;47m^C\u001b[0m Esci";
        Console.Write(legendaComandi);
    }

    /// <summary>
    /// Ottieni l'input dall'utente
    /// </summary>
    /// <param name="messaggio"></param>
    /// <returns>string </returns>
    public static string OttieniInputUtente(string messaggio)
    {
        Stampa(messaggio, SEZIONE_INFO_INPUT);
        StampaSegnoComando();
        CaricaConsoleModeSalvata();
        return Console.ReadLine() ?? "";
    }

    /// <summary>
    /// Ottiene il carattere premuto dall'utente 
    /// </summary>
    /// <returns>Il carattere premuto</returns>
    public static ConsoleKeyInfo OttieniInput()
        => Console.ReadKey(true);

    /// <summary>
    /// Stampa il carattere '>' e riposizione il cursore 
    /// </summary>
    private static void StampaSegnoComando()
    {
        MuoviCursore(SEZIONE_INPUT_UTENTE);
        Console.Write("> ");
    }

    private static void MuoviCursore((int x, int y) sezione)
        => Console.SetCursorPosition(sezione.x, sezione.y);


    /// <summary>
    /// Aspetta che l'utente prema un tasto per continuare
    /// </summary>
    private static void AspettaInput()
        => Console.ReadKey();

    /// <summary>
    /// Carica l'ultima modalità della console salvata
    /// </summary>
    private static void CaricaConsoleModeSalvata()
    {
        Console.CursorVisible = true;
        SetConsoleMode(hStdin, originalConsoleMode);
    }
}