using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Game"/>
/// </summary>
internal class GameConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Game.conf";

    // se 0 genera il seed casualmente
    public int Seed { get; set; } = 0;
    public int NumeroPergamene { get; set; } = 5;
}

