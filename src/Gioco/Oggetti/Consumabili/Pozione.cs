using Gioco.Config;

namespace Gioco.Oggetti.Consumabili;
/// <summary>
/// Una pozione in grado di aumentare la forza del giocatore
/// </summary>
internal class Pozione : Oggetto
{
    // Di quanto la pozione aumenterà la forza del giocatore
    public short AumentoForza;

    public Pozione(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile)
    {
        // Carica i dati di configurazione
        PozioneConfig config = GenericConfig<PozioneConfig>.CaricaConfigurazione(PozioneConfig.ConfigFileName);
        AumentoForza = config.AumentoForza;
        Skin = config.Skin;
    }
        
}

