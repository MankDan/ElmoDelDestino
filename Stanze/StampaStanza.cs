using Gioco.Config;
using Gioco.ConsoleUtils;
using Gioco.Enumeratori;
using Gioco.Interfacce;
using Gioco.Logger;
using Gioco.Oggetti.Altri;
using Gioco.Personaggi;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace Gioco.Stanze;

/// <summary>
/// Si occupa della stampa delle stanze su schermo attraverso <see cref="AdvConsole"/>
/// </summary>
internal static class StampaStanza
{
    private static int OffsetYPersonaggi;
    private static int OffsetYOggetti;
    private static int inizioX;
    private static int inizioY;
    private static float rapportoPorta;
    private static int lunghezzaX;
    private static int lunghezzaY;
    private static int lunghezzaBucoY;
    private static int lunghezzaBucoX;
    private static int inizioBucoY;
    private static int fineBucoY;
    private static int inizioBucoX;
    private static int fineBucoX;
    private static int maxLunghezzaNome;
    private static char[,] buffer = new char[lunghezzaY, lunghezzaX];
    static bool datiCaricati = false;

    static bool[] direzioniConPorte = [false, false, false, false];

    /// <summary>
    /// Stampa una rappresentazione grafica della stanza della console
    /// </summary>
    public static void Stampa(Stanza stanza)
    {
        Log4Elmo.Log($"Inizio stampa della stanza {stanza.Nome} con id: {stanza.Id}");
        AggiornaParametri(stanza);

        InizializzaBuffer(stanza);

        if (stanza.Personaggi.Count > 0 && stanza.Oggetti.Count > 0)
        {
            InserisciStampabileNelBuffer(stanza.Personaggi.Cast<IStampabile>().ToList(), OffsetYPersonaggi);
            InserisciStampabileNelBuffer(stanza.Oggetti.Cast<IStampabile>().ToList(), OffsetYOggetti);
        }
        else
        {
            InserisciStampabileNelBuffer(stanza.Personaggi.Cast<IStampabile>().ToList());
            InserisciStampabileNelBuffer(stanza.Oggetti.Cast<IStampabile>().ToList());
        }

        ScriviBufferInConsole();
        Log4Elmo.Log($"Stanza {stanza.Nome} stampata");
    }

    private static void AggiornaParametri(Stanza stanza)
    {

        if (datiCaricati == false)
        {
            CaricaDatiConfigurazione();
        }

        lunghezzaX = Math.Max(Console.WindowWidth - (inizioX * 2), 0);
        lunghezzaY = Math.Max(Console.WindowHeight - (inizioY * 2), 0);
        lunghezzaBucoY = (int)(lunghezzaY * rapportoPorta);
        lunghezzaBucoX = lunghezzaBucoY * 2;

        inizioBucoY = (lunghezzaY - lunghezzaBucoY) / 2;
        fineBucoY = inizioBucoY + lunghezzaBucoY;
        inizioBucoX = (lunghezzaX - lunghezzaBucoX) / 2;
        fineBucoX = inizioBucoX + lunghezzaBucoX;

        direzioniConPorte = [
            stanza.Porte[(int)Direzione.Nord] != null,
            stanza.Porte[(int)Direzione.Est] != null,
            stanza.Porte[(int)Direzione.Sud] != null,
            stanza.Porte[(int)Direzione.Ovest] != null
        ];

        buffer = new char[lunghezzaY, lunghezzaX];

        maxLunghezzaNome = CalcolaMaxLunghezzaNome(stanza);
    }

    private static void CaricaDatiConfigurazione()
    {
        StampaStanzaConfig config = GenericConfig<StampaStanzaConfig>.CaricaConfigurazione(StampaStanzaConfig.ConfigFileName);
        OffsetYPersonaggi = config.OffsetYPersonaggi;
        OffsetYOggetti = config.OffsetYOggetti;
        inizioX = config.InizioX;
        inizioY = config.InizioY;
        rapportoPorta = config.RapportoPorta;
        datiCaricati = true;
    }

    // Permette di stampare gli elementi a schermo, specificando un offset e una spaziatura
    private static void InserisciStampabileNelBuffer(ICollection<IStampabile> elementiStampabili, int offsetY = 0, int spaziatura = 2)
    {
        if (elementiStampabili.Count == 0) return;

        spaziatura += maxLunghezzaNome;

        int count = 0;
        int larghezzaStampabile = elementiStampabili.First().Skin[0].Length;
        foreach (IStampabile stampabile in elementiStampabili)
        {
            int altezzaStampabile = stampabile.Skin.Length;
            int numeroStampabili = elementiStampabili.Count;
            int larghezzaTotaleRichiesta = numeroStampabili * larghezzaStampabile + (numeroStampabili - 1) * spaziatura;
            int inizioPersonaX = (lunghezzaX - larghezzaTotaleRichiesta) / 2;
            int inizioPersonaY = (lunghezzaY - (altezzaStampabile + 2)) / 2 + offsetY;
            int inizioCorrentePersonaX = inizioPersonaX + count++ * (larghezzaStampabile + spaziatura);
            int inizioCorrentePersonaY = inizioPersonaY;

            InserisciSkinNelBuffer(stampabile.Skin, inizioCorrentePersonaX, inizioCorrentePersonaY);
            InserisciNomeNelBuffer(stampabile.Nome, inizioCorrentePersonaX, inizioCorrentePersonaY + altezzaStampabile, larghezzaStampabile);
        }
    }

    // Calcola la lunghezza massima del nome tra i personaggi e gli oggetti presenti nella stanza
    private static int CalcolaMaxLunghezzaNome(Stanza stanza)
    {
        int maxLunghezzaNome = 0;
        if (stanza.Personaggi.Count > 0)
            maxLunghezzaNome = stanza.Personaggi.Max(p => p.Nome.Length);
        if (stanza.Oggetti.Count > 0)
            maxLunghezzaNome = Math.Max(maxLunghezzaNome, stanza.Oggetti.Max(o => o.Nome.Length));
        return maxLunghezzaNome;
    }

    private static void InizializzaBuffer(Stanza stanza)
    {
        // random per generare la stessa pavimentazione della stanza ogni volta
        Random random = new(stanza.Id);
        for (int y = 0; y < lunghezzaY; y++)
        {
            for (int x = 0; x < lunghezzaX; x++)
            {
                if (ControllaMargineStanza(x, y))
                {
                    if (ControllaBucoPorta(x, y))
                    {
                        buffer[y, x] = ' ';
                    }
                    else
                    {
                        buffer[y, x] = '█';
                    }
                }
                else
                {
                    GeneraPavimento(x, y, stanza.TexturePavimento, random);
                }
            }
        }
    }

    // Genera il pavimento della stanza
    private static void GeneraPavimento(int x, int y, char texturePavimento, Random random) => buffer[y, x] = random.Next(0, 50) == 0 ? texturePavimento : ' ';

    // Controlla se in determinate coordinate x, y dovrebbe essere presente il margine della stanza
    private static bool ControllaMargineStanza(int x, int y)
    {
        return y == 0 || y == lunghezzaY - 1 || x == 0 || x == lunghezzaX - 1;
    }

    // Controlla se in determinate coordinate x, y dovrebbe essere presente una porta
    private static bool ControllaBucoPorta(int x, int y)
    {
        bool intervalloBucoX = (x >= inizioBucoX && x < fineBucoX);
        bool intervalloBucoY = (y >= inizioBucoY && y < fineBucoY);

        return (y == 0 && direzioniConPorte[(int)Direzione.Nord] && intervalloBucoX) ||
                                (y == lunghezzaY - 1 && direzioniConPorte[(int)Direzione.Sud] && intervalloBucoX) ||
                                (x == 0 && direzioniConPorte[(int)Direzione.Ovest] && intervalloBucoY) ||
                                (x == lunghezzaX - 1 && direzioniConPorte[(int)Direzione.Est] && intervalloBucoY);
    }

    /// <summary>
    /// Stampa la skin del personaggio
    /// </summary> 
    private static void InserisciSkinNelBuffer(string[] asciiArt, int startX, int startY)
    {
        for (int i = 0; i < asciiArt.Length; i++)
        {
            for (int j = 0; j < asciiArt[i].Length; j++)
            {
                int bufferX = startX + j;
                int bufferY = startY + i;

                if (bufferX >= 0 && bufferX < lunghezzaX && bufferY >= 0 && bufferY < lunghezzaY)
                {
                    buffer[bufferY, bufferX] = asciiArt[i][j];
                }
            }
        }
    }

    // Inserisci il nome della stanza nel buffer
    private static void InserisciNomeNelBuffer(string nome, int startX, int startY, int larghezzaElemento)
    {
        int inizioNomeX = startX + (larghezzaElemento - nome.Length) / 2;
        for (int i = 0; i < nome.Length; i++)
        {
            int bufferX = inizioNomeX + i;
            int bufferY = startY;

            if (bufferX >= 0 && bufferX < lunghezzaX && bufferY >= 0 && bufferY < lunghezzaY)
            {
                buffer[bufferY, bufferX] = nome[i];
            }
        }
    }

    // Scrive il contenuto del buffer sulla console
    private static void ScriviBufferInConsole()
    {
        for (int y = 0; y < lunghezzaY; y++)
        {
            Console.SetCursorPosition(inizioX, inizioY + y);
            char[] riga = new char[lunghezzaX];
            for (int x = 0; x < lunghezzaX; x++)
            {
                riga[x] = buffer[y, x];
            }
            Console.WriteLine(new string(riga));
        }
    }

    /// <summary>
    /// Stampa la descrizione della stanza, inclusi gli oggetti e i personaggi presenti
    /// </summary>
    public static void StampaDescrizione(Stanza stanza)
    {

        AdvConsole.StampaCentrato(stanza.Nome, AdvConsole.SEZIONE_TITOLO);
        if (!stanza.Visitata)
        {
            AdvConsole.StampaCentrato(stanza.Descrizione, AdvConsole.SEZIONE_DESCRIZIONE, '-', '-', '|', 10);
            stanza.Visitata = true;
        }

        string buffer = "";

        foreach (var oggetto in stanza.Oggetti)
        {
            buffer += ($"Oggetto presente: {oggetto.Nome} - {oggetto.Descrizione}\n");
        }

        foreach (var personaggio in stanza.Personaggi)
        {
            buffer += ($"Personaggio presente: {personaggio.Nome} - {personaggio.Descrizione}\n");
        }

        direzioniConPorte = [
            stanza.Porte[(int)Direzione.Nord] != null,
            stanza.Porte[(int)Direzione.Est] != null,
            stanza.Porte[(int)Direzione.Sud] != null,
            stanza.Porte[(int)Direzione.Ovest] != null
        ];

        buffer += (
            $"Direzioni:" +
                (direzioniConPorte[0] ? "\n- nord" : "") +
                (direzioniConPorte[1] ? "\n- est" : "") +
                (direzioniConPorte[2] ? "\n- sud" : "") +
                (direzioniConPorte[3] ? "\n- ovest" : "") + "\n"
        );

        AdvConsole.Stampa(buffer, AdvConsole.SEZIONE_INFORMAZIONI);
    }
}
