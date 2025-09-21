using Gioco.Config;

namespace Gioco.Oggetti.Altri;
/// <summary>
/// L'oggetto zaino, in grado di aumentare lo spazio nell'inventario del giocatore
/// </summary>
internal class Zaino : Oggetto
{
    public short AumentoCapacita { get; set; }

    public Zaino(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile)
    {
        // Carica i dati di configurazione
        ZainoConfig config = GenericConfig<ZainoConfig>.CaricaConfigurazione(ZainoConfig.ConfigFileName);
        AumentoCapacita = config.aumentoCapacita;
        Skin = config.Skin;
    }
}
