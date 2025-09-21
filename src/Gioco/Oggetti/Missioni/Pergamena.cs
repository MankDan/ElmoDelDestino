using Gioco.Logger;

namespace Gioco.Oggetti.Missioni;

/// <summary>
/// Una pergamena consegnabile al <see cref="Saggio"/> 
/// </summary>
internal class Pergamena : Oggetto
{    
    public Pergamena(string nome, string descrizione, short peso, bool raccoglibile, bool usabile) : base(nome, descrizione, peso, raccoglibile, usabile)
    {
        Skin = [
            "+══+",
            "|░░|",
            "+══+"
        ];
        Log4Elmo.Log($"Pergamena creata");
    }     
}

