using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Pozione"/>
/// </summary>
internal class PozioneConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Pozione.conf";

    public short AumentoForza { get; set; } = 2;
    public string[] Skin { get; set; } = [
        " __ ",
        " || ",
        "(__)"
    ];
}
