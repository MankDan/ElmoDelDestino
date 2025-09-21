using Gioco.Interfacce;
using Gioco.Logger;

namespace Gioco.Oggetti;

/// <summary>
/// Classe astratta contentente le informazioni di base per tutti gli oggetti
/// </summary>
/// <param name="nome">Nome</param>
/// <param name="descrizione">Descrizione dell'oggetto</param>
/// <param name="peso">Spazio occupato dall'oggetto nell'inventario</param>
/// <param name="raccoglibile">Indica se l'oggetto è raccoglibile dal giocatore</param>
/// <param name="usabile">Indica se l'oggetto può essere usato dal giocatore</param>
public abstract class Oggetto : IStampabile
{
    // Assegna un Id univoco all'oggetto istanziato
    public readonly short Id;

    public short Peso { get; set; }
    public string Nome { get; set; }
    public string Descrizione { get; set; }
    public bool Raccoglibile { get; set; }
    public bool Usabile { get; set; }

    public Oggetto(string nome, string descrizione, short peso, bool raccoglibile, bool usabile)
    {
        Log4Elmo.Log($"Inizio creazione oggetto {nome}");
        Peso = peso;
        Nome = nome;
        Descrizione = descrizione;
        Raccoglibile = raccoglibile;
        Usabile = usabile;
        Id = ID++;
        Log4Elmo.Log($"Oggetto {nome} creato con ID {Id}");
    }

    // Skin di base
    virtual public string[] Skin { get; set; } = [
        "╔═╗",
        "║?║",
        "╚═╝"
    ];

    /// <summary>
    /// L'ID del prossimo oggetto di tipo <see cref="Oggetto"/> da istanziare
    /// </summary>
    public static short ID { get; private set; } = 1;
}
