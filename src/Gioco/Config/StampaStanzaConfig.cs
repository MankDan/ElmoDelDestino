using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="StampaStanza"/>
/// </summary>
internal class StampaStanzaConfig : IConfigurabile
{
    public static string ConfigFileName { get; set; } = "StampaStanza.conf";

    public int OffsetYPersonaggi { get; set; } = -2;
    public int OffsetYOggetti { get; set; } = 3;
    public int InizioX { get; set; } = 10;
    public int InizioY { get; set; } = 8;
    public float RapportoPorta { get; set; } = 0.4f;
}
