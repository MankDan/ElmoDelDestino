using Gioco.ConsoleUtils;
using Gioco.Giocatori;
using Gioco.Logger;

namespace Gioco.Personaggi;

/// <summary>
/// Delegate del metodo <see cref="Game.Teletrasporta()"/>
/// </summary>
public delegate void Teletrasporta();

/// <summary>
/// Enderman in grado di teletrasportato il giocatore in una stanza casuale quando quando interagisce
/// </summary>
internal class Enderman : Personaggio
{

    private readonly Teletrasporta teletrasporta;
    public Enderman(string nome, string descrizione, Teletrasporta teletrasporta) : base(nome, descrizione)
    {
        this.teletrasporta = teletrasporta;

        Skin = [
            "[ ]",
            "/|\\",
            " ║ "
        ];
        StatoInterazione = 0;
    }

    /// <summary>
    /// Interagisce con l'enderman, e dalla seconda interazione teletrasportando il giocatore in una stanza casuale
    /// </summary>
    /// <param name="giocatore">Il giocatore che interagisce con l'enderman</param>
    public override void Interagisci(Giocatore giocatore)
    {
        Log4Elmo.Log($"Interazione {StatoInterazione} tra {giocatore.Nome} e {Nome}");

        AdvConsole.StampaCentrato(Nome, AdvConsole.SEZIONE_TITOLO);
        AdvConsole.StampaMessaggio("Vwoops");
        if(StatoInterazione == 0) {
            AdvConsole.StampaMessaggio($"*{Nome} vuole parlare di nuovo*");
            StatoInterazione++;
        }
        else if(StatoInterazione == 1)
        {
            AdvConsole.StampaMessaggio($"*{Nome} ti fissa*");
            teletrasporta();
        } 
    }
}
