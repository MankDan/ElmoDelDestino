using Gioco.Logger;

namespace Gioco.Oggetti.Missioni;
/// <summary>
/// L'oggetto finale del gioco. Si ottiene Uccidendo il <see cref="Guardiano"/>
/// </summary>
internal class Elmo : Oggetto
{
    public Elmo(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile)
    {
        Skin = [
            ",,   ,,",
            "===O===",
            "|  ,  |",
            "|└(¯)┘|",
            "|-----|"
        ];
        Log4Elmo.Log($"Elmo creato");
    }
}
