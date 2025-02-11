using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Statua"/>
/// </summary>
internal class StatuaConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Statua.conf";

    public string[] Espressioni { get; set; } = ["(¬_¬)", "(^v^)", "(O.O)", "(@.@)", "(°-°)"];
    public string[] Skin { get; set; } = [
        "(¬_¬)",
        " /|\\",
        " / \\",
        "▄███▄"
    ];
}
