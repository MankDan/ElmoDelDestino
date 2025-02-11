using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Elisir"/>
/// </summary>
internal class ElisirConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Elisir.conf";

    public short RipristinoSalute { get; set; } = 15;
    public string[] Skin { get; set; } = [
        " __ ",
        " || ",
        "(__)"
    ];
}
