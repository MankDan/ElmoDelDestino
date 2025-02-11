using Gioco.Config;

namespace Gioco.Oggetti.Consumabili;
/// <summary>
/// Un elisir in grado di ripristinare la salute del giocatore
/// </summary>
internal class Elisir : Oggetto
{
    // Di quanto l'elisir aumenterà la salute del giocatore
    public short RipristinoSalute;

    public Elisir(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile)
    {
        // Carica i dati di configurazione
        ElisirConfig config = GenericConfig<ElisirConfig>.CaricaConfigurazione(ElisirConfig.ConfigFileName);
        RipristinoSalute = config.RipristinoSalute;
        Descrizione = descrizione.Replace("?", ""+RipristinoSalute);
        Skin = config.Skin;
    }
        
}
