namespace Gioco.Oggetti.Armi;

/// <summary>
/// Una classe astratta per tutte le armi
/// </summary>
abstract class Arma : Oggetto
{
    protected Arma(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile) { }

    // Il danno dell'arma
    public short Danno { get; set; }
}
