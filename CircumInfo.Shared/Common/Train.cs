using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CircumInfo.Common
{
    public class Train
    {
        public const int NORMAL = 0; //Stato del treno normale
        public const int RETARD = 1; //Stato del treno in ritardo
        public const int SUPPRESSED = 2; //Stato del treno soppresso
        public string ID { get; set; }
        public string TypeID { get; set; }
        public string Ferial { get; set; }
        public string Direction { get; set; }
        public string NoG { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public ObservableCollection<Stop> ArrayOfStop { get; set; }

        public bool stopsAt(int stopID)
        {
            foreach (Stop fermata in ArrayOfStop)
            {
                if (fermata.StopID == stopID && fermata != ArrayOfStop[ArrayOfStop.Count-1])
                {
                    return true;
                }
            }
            return false;
        }

        internal bool festive(int idFermata)
        {
            foreach (Stop fermata in ArrayOfStop)
            {
                if (fermata.StopID == idFermata)
                {
                    return (fermata.Festivo == "Y");
                }
            }
            return false;
        }

        public bool Festivo
        {
            get
            {
                return Ferial == "F";
            }
        }

        public int getRitardo()
        {
            return 0;
        }
    }
}
