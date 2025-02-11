using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Spada"/>
/// </summary>
internal class SpadaConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Spada.conf";

    public short Danno { get; set; } = 3;
    public string[] Skin { get; set; } = [
        "    ",
        "┤══>",
        "    "
    ];
}
