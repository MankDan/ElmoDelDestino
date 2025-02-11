using Gioco.Config;

namespace Gioco.Oggetti.Altri;
/// <summary>
/// L'oggetto teschio, un oggetto decorativo
/// </summary>
internal class Teschio : Oggetto
{
    public Teschio(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile)
    {
        // Carica i dati di configurazione
        TeschioConfig config = GenericConfig<TeschioConfig>.CaricaConfigurazione(TeschioConfig.ConfigFileName);
        Skin = config.Skin;
    }
}
