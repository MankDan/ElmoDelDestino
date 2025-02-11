
using Gioco.ConsoleUtils;
using Gioco.Giocatori;
using Gioco.Logger;
using Gioco.Oggetti;
using Gioco.Oggetti.Missioni;

namespace Gioco.Personaggi;

/// <summary>
/// Il Saggio a cui consegnare gli oggetti <see cref="Pergamena"/> in cambio di una <see cref="Chiave"/>
/// </summary>
/// <param name="nome">Nome del personaggio</param>
/// <param name="descrizione">Descrizione del personaggio</param>
/// <param name="oggetto">Oggetto da consegnare</param>
class Saggio(string nome, string descrizione, Oggetto oggetto) : Personaggio(nome, descrizione)
{
    public Oggetto OggettoPosseduto { get; set; } = oggetto;

    /// <summary>
    /// Interagisce con il saggio
    /// </summary>
    public override void Interagisci(Giocatore giocatore)
    {
        Log4Elmo.Log($"Interazione {StatoInterazione} tra {giocatore.Nome} e {Nome}");
        AdvConsole.StampaCentrato(Nome, AdvConsole.SEZIONE_TITOLO);

        if (StatoInterazione == 0)
        {
            AdvConsole.StampaMessaggio($"Benvenuto, {giocatore.Nome}, ti stavo aspettando. "+
                    "Sono il Saggio Custode della Conoscenza, " +
                    "e il tuo arrivo non è stato frutto del caso. Ho bisogno del tuo aiuto in una " +
                    $"missione di grande importanza. \n{Game.NumeroPergamene} pergamene antiche, contenenti segreti " +
                    "dimenticati dal tempo, sono state disperse per queste terre.");
            AdvConsole.StampaMessaggio(
                    $"Solo colui che possiede coraggio e saggezza potrà raccoglierle e portarmele. " +
                    "Viaggia per il mondo, cerca con diligenza e riportami queste pergamene. " +
                    "Solo allora, le porte del sapere ti saranno spalancate.\n" +
                    $"Torna quando avrai raccolto {Game.NumeroPergamene} pergamene."
                    );
            StatoInterazione++;
        }  

        else if (StatoInterazione == 1)
        {
            AdvConsole.StampaMessaggio($"Hey {giocatore.Nome},\nhai portato le pergamene che ti ho chiesto?");
            int pergamenePossedute = giocatore.Conta<Pergamena>();
            if (pergamenePossedute < Game.NumeroPergamene)
            {
                AdvConsole.StampaMessaggio($"Non hai le pergamene richieste ({pergamenePossedute} su {Game.NumeroPergamene})");
            }
            else {
                AdvConsole.StampaMessaggio($"Grazie per le pergamene! Tieni questa chiave.");
                giocatore.RimuoviAll<Pergamena>();

                if(OggettoPosseduto != null)
                    giocatore.RaccogliOggetto(OggettoPosseduto);

                StatoInterazione++;
            }
        } 
        
        else if (StatoInterazione == 2)
        {
            AdvConsole.StampaMessaggio($"Ancora grazie per le pergamene! Usa la chiave con saggezza.");
            StatoInterazione++;
        } 
        
        else if (StatoInterazione == 3) {
            AdvConsole.StampaMessaggio($"Puoi andare adesso. Non voglio più nulla.");
            StatoInterazione++;
        }
         
        else if (StatoInterazione == 4)
        {
            AdvConsole.StampaMessaggio($"Vattene prima che mi riprenda la chiave. Sei fastidioso.");
            StatoInterazione++;
        }
        
        else if (StatoInterazione == 5)
        {
            AdvConsole.StampaMessaggio($"*Il saggio si rifiuta di interagire con te*");
        }
    }
}
