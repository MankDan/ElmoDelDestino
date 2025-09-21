using Gioco.Config;

namespace Gioco.Oggetti.Altri;
/// <summary>
/// L'oggetto statua, un oggetto decorativo
/// </summary>
internal class Statua : Oggetto
{
    private string[] espressioni;
    private int counter = 0;
    private string[] skin;

    public Statua(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile)
    {
        // Carica i dati di configurazione
        StatuaConfig config = GenericConfig<StatuaConfig>.CaricaConfigurazione(StatuaConfig.ConfigFileName);
        espressioni = config.Espressioni;
        skin = config.Skin;
    }

    /// <summary>
    /// Override del metodo Skin per ciclare l'espressione della statua
    /// </summary>
    public override string[] Skin
    {
        get
        {
            string[] updatedSkin = new string[skin.Length];
            updatedSkin[0] = espressioni[counter++ % espressioni.Length];
            for (int i = 1; i < skin.Length; i++)
            {
                updatedSkin[i] = skin[i];
            }
            return updatedSkin;
        }
    }
}
