using System;
using System.Collections.Generic;
using System.Text;

namespace CircumInfo.Common
{
    class DisabledPositionException : Exception
    {
        public int code = 0;
        public DisabledPositionException( int codice)
        {
            code = codice;
        }
        public new string Message
        {
            get
            {
                return "Posizione disattivata";
            }
        }

        public string Solution
        {
            get
            {
                if (code == 0)
                {
                    return "Attiva la posizione dalle impostazioni dell'app.";
                }
                else
                {
                    return "Attiva la posizione dalle impostazioni del telefono.";
                }
            }
        }
    }
}
