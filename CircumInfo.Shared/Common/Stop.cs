using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CircumInfo.Common
{
    public class Stop
    {
        public int StopID { get; set; }
        public string Hour { get; set; }
        public string Festivo { get; set; }

        public string Feriale { get; set; }
        [XmlIgnore]
        public DateTime Orario
        {
            get
            {
                DateTime ora = new DateTime();
                ora.AddHours(Ora);
                ora.AddMinutes(Minuto);
                return ora;
            }
        }
        [XmlIgnore]
        public int Ora
        {
            get
            {
                if (Hour == null || Hour == "")
                    return 23;
                return Convert.ToInt32(Hour.Split('.')[0]);
            }
        }
        [XmlIgnore]
        public int Minuto
        {
            get
            {
                if (Hour == null || Hour == "")
                    return 59;
                return Convert.ToInt32(Hour.Split('.')[1]);
            }
        }

        public string Title
        {
            get
            {
                try
                {
                    return DBSource.getStop(StopID).Result.Name;
                } catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return "ERRORE FERMATA " + StopID;
                }
            }
        }

        public string Subtitle
        {
            get
            {
                if (Hour == null)
                    return "ATTENZIONE!!!!";
                return String.Format("Ore: {0}:{1:00}", Ora, Minuto);
            }
        }

        public string Description
        {
            get
            {
                if (Festivo == "Y")
                {
                    return "*solo domenica e giorni festivi";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
