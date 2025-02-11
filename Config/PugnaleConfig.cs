using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Pugnale"/>
/// </summary>
internal class PugnaleConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Pugnale.conf";

    public short Danno { get; set; } = 2;
    public string[] Skin { get; set; } = [
        "   ",
        "┤═>",
        "   "
    ];
}

