using Gioco.Interfacce;

namespace Gioco.Config;

/// <summary>
/// Contiene i valori di default configurabili della classe <see cref="Giocatore"/>
/// </summary>
internal class GiocatoreConfig : IConfigurabile
{
    // nome del file contente il file di configurazione
    public static string ConfigFileName { get; set; } = "Giocatore.conf";

    public string Nome { get; set; } = "Giocatore";
    public short IdStanza { get; set; } = 1;
    public short CapacitaMassima { get; set; } = 20;
    public short Forza { get; set; } = 2;
    public short SaluteMax { get; set; } = 20;
    public short MinMoltiplicatoreDanno { get; set; } = 3;
    public short MaxMoltiplicatoreDanno { get; set; } = 7;
}