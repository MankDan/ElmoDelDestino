namespace Gioco.Interfacce;

/// <summary>
/// Interfaccia per tutti gli elementi stampabili a schermo
/// </summary>
interface IStampabile
{
    string[] Skin { get; }
    string Nome { get; }
}
