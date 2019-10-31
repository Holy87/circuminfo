using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircumInfo.Common
{
    class InvalidTrainStopException : Exception
    {
        public InvalidTrainStopException(string treno)
        {
            IDTreno = treno;
        }
        public new string Message
        {
            get
            {
                return "C'è un problema nella fermata del treno " + IDTreno;
            }
        }

        public string IDTreno { get; set; }
    }
}
