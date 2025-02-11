namespace Gioco.Interfacce;

/// <summary>
/// Interfaccia per configurare i dati degli oggetti
/// </summary>
internal interface IConfigurabile
{
    public static abstract string ConfigFileName { get; set; }
}

