using System;
using System.Collections.Generic;
using System.Text;

namespace CircumInfo.Common
{
    /// <summary>
    /// Oggetto che mostra il messaggio dell'amministratore. Non ha costruttore perché dev'essere serializzato.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Ottiene e imposta l'elemento titolo del messaggio
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Ottiene e imposta l'elemento del testo del messaggio
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Ottiene o imposta l'ID del messaggio.
        /// </summary>
        public int ID { get; set; }
    }
}
