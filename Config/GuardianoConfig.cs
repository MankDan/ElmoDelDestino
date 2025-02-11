using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Guardiano"/>
/// </summary>
internal class GuardianoConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Guardiano.conf";

    public short SaluteMassima { get; set; } = 300;
    public short DannoMin { get; set; } = 3;
    public short DannoMax { get; set; } = 9;
    public string[] Skin { get; set; } = [
        "<O>",
        "/|\\",
        "/ \\"
    ];
}