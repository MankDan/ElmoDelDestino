using Gioco.Config;
using Gioco.ConsoleUtils;
using Gioco.Giocatori;
using Gioco.Logger;
using Gioco.Oggetti;
using Gioco.Oggetti.Armi;
using Gioco.Oggetti.Missioni;

namespace Gioco.Personaggi;
/// <summary>
/// Il Guardiano finale contro cui combattere per ottenere l'elmo del destino
/// </summary>
internal class Guardiano : Enemy
{
    public Oggetto OggettoPosseduto { get; set; }

    public Guardiano(string nome, string descrizione, Oggetto oggetto) : base(nome, descrizione)
    {
        GuardianoConfig config = GenericConfig<GuardianoConfig>.CaricaConfigurazione(GuardianoConfig.ConfigFileName);
        Skin = config.Skin;
        SaluteMassima = config.SaluteMassima;
        Salute = SaluteMassima;
        DannoMin = config.DannoMin;
        DannoMax = config.DannoMax;
        OggettoPosseduto = oggetto;
    }

    /// <summary>
    /// Interagisce con il Guardiano finale qualora vengono rispettate le sue condizioni
    /// </summary>
    /// <param name="giocatore">Giocatore con cui interagire</param>
    public override void Interagisci(Giocatore giocatore)
    {
        Log4Elmo.Log($"Interazione {StatoInterazione} tra {giocatore.Nome} e {Nome}");

        AdvConsole.StampaCentrato(Nome, AdvConsole.SEZIONE_TITOLO);
        if (StatoInterazione == 0)
        {
            AdvConsole.StampaMessaggio("Vuoi l'Elmo del destino?");
            if (!giocatore.Possiede<Chiave>())
            {
                AdvConsole.StampaMessaggio($"Non hai nemmeno la chiave dorata.\n{giocatore.Nome}, Vattene.");
                return;
            }
            if (!giocatore.Possiede<Arma>())
            {
                AdvConsole.StampaMessaggio($"Ritorna quando avrai un'arma per combattere, novellino!\n");
                return;
            }
            AdvConsole.StampaMessaggio("Non hai più possibilità di ritirarti. Combattiamo fino alla fine.");
            giocatore.StanzaAttuale.Isola();
            StatoInterazione++;
        }
        else if (StatoInterazione == 1)
        {
                
            AdvConsole.StampaMessaggio("Ti distruggerò.");
            StatoInterazione++;
        }
        else if (StatoInterazione == 2)
        {
            AdvConsole.StampaMessaggio("Cosa stai aspettando? Combatti.");
            StatoInterazione++;
        }
        else if (StatoInterazione == 3)
        {
            AdvConsole.StampaMessaggio("Non hai il coraggio di attaccarmi?");
            StatoInterazione = 1;
        }
    }

    /// <summary>
    /// Indica se il personaggio è pronto per il combattimento
    /// </summary>
    public override bool ProntoPerIlCombattimento => (StatoInterazione >= 1);
}
