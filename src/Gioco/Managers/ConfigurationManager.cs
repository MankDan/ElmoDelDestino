using Gioco.Logger;
using Gioco.Oggetti;
using System;

namespace Gioco.Managers;

/// <summary>
/// Si occupa di gestire i file di configurazione
/// </summary>
internal class ConfigurationManager
{
    // Persorso della cartella di configurazione
    private static readonly string configFilePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    ".elmodeldestino/config/");

    /// <summary>
    /// Crea la cartella per i file di configurazione
    /// </summary>
    public static void CreaCartellaDiConfigurazione()
    {
         Directory.CreateDirectory(Path.GetDirectoryName(configFilePath) ?? "");
        Log4Elmo.Log($"Cartella di configurazione {Path.GetDirectoryName(configFilePath)} creata");
    }

    /// <summary>
    /// Restituisce il percorso file completo per accedere ad un determinato file di configurazione
    /// </summary>
    /// <param name="fileName">Nome del file di configurazione</param>
    /// <returns>Il percorso file completo del file di configurazione</returns>
    public static string PathCompleto(string fileName) => configFilePath + fileName;

}

