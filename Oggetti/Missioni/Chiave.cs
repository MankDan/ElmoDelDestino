using Gioco.Logger;

namespace Gioco.Oggetti.Missioni;
/// <summary>
/// Una Chiave consegnata dal <see cref="Saggio"/> dopo aver consegnato tutti gli oggetti <see cref="Pergamena"/>. Serve per sbloccare il combattimento finale.
/// </summary>
internal class Chiave : Oggetto
{
    public Chiave(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile)
    {
        Skin = [
            "    ",
            "O═╦╗",
            "     "
        ];
        Log4Elmo.Log($"Chiave creata");
    }
}
