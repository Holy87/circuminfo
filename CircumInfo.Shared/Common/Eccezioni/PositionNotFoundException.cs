using System;
using System.Collections.Generic;
using System.Text;

namespace CircumInfo.Common
{
    class PositionNotFoundException : Exception
    {
        
        public new string Message
        {
            get
            {
                return "Impossibile determinare la tua posizione.";
            }
        }
    }
}
