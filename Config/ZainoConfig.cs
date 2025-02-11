using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Zaino"/>
/// </summary>
internal class ZainoConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Zaino.conf";

    public short aumentoCapacita { get; set; } = 30;
    public string[] Skin { get; set; } = [
        " ,═, ",
        "(| |)",
        " └─┘ "
    ];
}

