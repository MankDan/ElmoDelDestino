using Gioco.Logger;
using Gioco.Managers;
using System.Text.Json;

namespace Gioco.Config;

/// <summary>
/// Classe generica per caricare e creare le configurazioni degli oggetti del gioco
/// </summary>
/// <typeparam name="IConfigurabile">Classe contenente i dati di default per il file di configurazione</typeparam>
public class GenericConfig<IConfigurabile> where IConfigurabile : new()
{
    /// <summary>
    /// Carica i dati dal file di configurazione, altrimenti lo crea usando i dati di default
    /// </summary>
    /// <param name="ConfigFileName">nome del file di configurazione</param>
    /// <returns>l'oggetto della classe T contenente i dati di configurazione</returns>
    /// <exception cref="Exception"></exception>
    public static IConfigurabile CaricaConfigurazione(string ConfigFileName)
    {
        Log4Elmo.Log($"Caricamente configurazione da '{ConfigFileName}'");
        string path = ConfigurationManager.PathCompleto(ConfigFileName);
        IConfigurabile? configurazione;
        try
        {
            string jsonString = File.ReadAllText(path);
            configurazione = JsonSerializer.Deserialize<IConfigurabile>(jsonString);
        }
        catch (Exception e)
        {
            Log4Elmo.Log($"Problema nel file di configurazione: {e.Message}. Provo a ripristinarlo.", Log4Elmo.WARNING);
            configurazione = CreaConfigurazione(path);
            Log4Elmo.Log($"Ripristino file {path} completato");
        }

        return configurazione ?? throw new Exception("la configurazione è null!");
    }

    // Crea il file di configurazione usando i dati di default contenuti in IConfigurabile
    private static IConfigurabile CreaConfigurazione(string path)
    {
        Log4Elmo.Log($"Creazione file di configurazione in '{path}'");
        IConfigurabile configurazione = new();
        string json = JsonSerializer.Serialize(configurazione);
        File.WriteAllText(path, json);
        return configurazione;
    }
}
