using Gioco.Config;

namespace Gioco.Oggetti.Armi;

/// <summary>
/// Un oggetto di tipo arma, in grado di infliggere danni
/// </summary>
internal class Pugnale : Arma
{
    public Pugnale(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile)
    {
        // Carica i dati di configurazione
        PugnaleConfig config = GenericConfig<PugnaleConfig>.CaricaConfigurazione(PugnaleConfig.ConfigFileName);
        Skin = config.Skin;
        Danno = config.Danno;
    }
}
