using Gioco.Logger;
using Gioco.Oggetti;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gioco.Interfacce
{
    /// <summary>
    /// Interfaccia per gestire la salute salute del personaggio, verificare se è ancora vivo e generare una barra visiva della salute
    /// </summary>
    /// <param name="salute">Salute del personaggio</param>
    /// <param name="massima salute">Massima salute che il personaggio può avere</param>
    internal interface ISalute
    {
        private const int lunghezzaBarraMinima = 10;
        private const float RatioLunghezzaBarraMassima = 0.3f;
        public short Salute { get; set; }
        public short SaluteMassima { get; set; }
        
        /// <summary>
        /// Riduce la salute del personaggio
        /// </summary>
        /// <param name="danno">Il danno applicato al personaggio</param>
        public void SubisciDanno(short danno)
        {
            Salute -= danno;
            Log4Elmo.Log($"Il personaggio {this.GetType()} ha subito {danno} danni");
        }

        /// <summary>
        /// Controlla se il personaggio è vivo
        /// </summary>
        /// <returns>True se ancora vivo, altrimenti False</returns>
        public bool IsVivo()
        {
            return Salute > 0;
        }

        /// <summary>
        /// Genera una stringa contenente la barra della saluta, di lunghezza compresa tra <see cref="lunghezzaBarraMinima"/> e larghezza finestra * <see cref="RatioLunghezzaBarraMassima"/>
        /// </summary>
        /// <param name="lunghezzaBarraDellaSalute">La lunghezza della barra della salute</param>
        /// <returns></returns>
        public string OttieniBarraSalute(int lunghezzaBarraDellaSalute)
        {
            lunghezzaBarraDellaSalute = Math.Max(lunghezzaBarraDellaSalute, lunghezzaBarraMinima);
            lunghezzaBarraDellaSalute = Math.Min(lunghezzaBarraDellaSalute, (int)(Console.WindowWidth * RatioLunghezzaBarraMassima));

            float rapportoSalute = Math.Max((float)Salute / SaluteMassima, 0f);
            return (
                '|' +
                new string('█', (int)Math.Ceiling(lunghezzaBarraDellaSalute * rapportoSalute)) +
                new string(' ', (int)Math.Floor(lunghezzaBarraDellaSalute * (1f - rapportoSalute))) +
                "|"
            );
        }
    }
}
