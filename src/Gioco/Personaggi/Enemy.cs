using Gioco.ConsoleUtils;
using Gioco.Giocatori;
using Gioco.Interfacce;
using Gioco.Logger;
using Gioco.Oggetti;

namespace Gioco.Personaggi;
/// <summary>
/// Classe astratta per i personaggi contro cui il Giocatore può combattere
/// </summary>
abstract class Enemy : Personaggio, ISalute
{
    public abstract bool ProntoPerIlCombattimento { get; }

    public short Salute { get; set; }
    public short SaluteMassima { get; set; }
    public short DannoMin { get; set; }
    public short DannoMax { get; set; }
    private readonly Random random;

    protected Enemy(string nome, string descrizione) : base(nome, descrizione)
    {
        random = new Random();
    }

    /// <summary>
    /// Attacca il giocatore, infliggendo del danno
    /// </summary>
    /// <param name="g">L'entità da attaccare</param>
    public void Attacca(ISalute g)
    {
        short danno = (short)random.Next(DannoMin, DannoMax + 1);
        g.SubisciDanno(danno);
        Log4Elmo.Log($"{this.Nome} ha attaccato infliggendo {danno} danni");
    }
    /// <summary>
    /// Stampa la barra del boss con la vita rimanente usando la classe <see cref="AdvConsole"/>
    /// </summary>
    public bool StampaBossBar()
    {
        if (!ProntoPerIlCombattimento) return false;

        AdvConsole.StampaCentrato($"Boss: {Nome} ({Salute} / {SaluteMassima} HP)", AdvConsole.SEZIONE_TITOLO, '!', '=', '=');
        AdvConsole.StampaCentrato(((ISalute)this).OttieniBarraSalute(Console.WindowWidth/2), AdvConsole.SEZIONE_DESCRIZIONE, '#', '=', '=', 0);
        return true;
    }
}
