using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Teschio"/>
/// </summary>
internal class TeschioConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Teschio.conf";

    public string[] Skin { get; set; } = [
        " .-. ",
        "(o.o)",
        " |=| ",
    ];
}
