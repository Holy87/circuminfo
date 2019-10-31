using System;
using System.Collections.Generic;
using System.Text;

namespace CircumInfo.Common
{
    public class Partenza
    {
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string Ora { get; set; }
        public string ID { get; set; }
        public string Direction { get; set; }

        public int ora, minuto;

        private bool festivo;

        private bool feriale;

        private bool periale;

        private bool noGarantito;

        private string tipo;

        public string Subtitle
        {
            get
            {
                return "Ore: " + Ora;
            }
        }

        public string Tipo
        {
            get {return  tipo;}

        }

        public Partenza (Train treno, int idStazione, bool festivo)
        {
            Title = "Per " +  treno.Direction;
            Direction = treno.Direction;
            this.festivo = festivo;
            setImage(treno.TypeID);
            setOra(treno, idStazione);
            //noGarantito = treno.NoG.Equals("Y");
            ID = treno.ID;
        }

        private void setOra(Train treno, int id)
        {
            foreach (Stop fermata in treno.ArrayOfStop)
            {
                if (id == fermata.StopID)
                {
                    ora = fermata.Ora;
                    minuto = fermata.Minuto;
                    Ora = String.Format("{0}:{1:00}", ora, minuto);
                    feriale = treno.Ferial == "Y" || fermata.Feriale == "Y";
                    periale = treno.Ferial == "P" || fermata.Feriale == "P";
                    if (!festivo)
                        festivo = treno.Festivo || fermata.Festivo=="Y";
                    break;
                }
            }
        }

        private void setImage(string p)
        {
            switch (p)
            {
                case "AC":
                    ImagePath = "Assets/Treni/Accelerato.png";
                    tipo = "Accelerato";
                    break;
                case "D":
                    ImagePath = "Assets/Treni/Diretto.png";
                    tipo = "Diretto";
                    break;
                case "DD":
                    ImagePath = "Assets/Treni/Direttissimo.png";
                    tipo = "Direttissimo";
                    break;
                case "CD":
                    ImagePath = "Assets/Treni/CenDir.png";
                    tipo = "linea Centro Direzionale";
                    break;
                case "FN":
                    ImagePath = "Assets/Treni/Funivia.png";
                    tipo = "Funivia";
                    break;
                case "BS":
                    ImagePath = "Assets/Treni/Bus.png";
                    tipo = "Autobus";
                    break;
                case "CE":
                    ImagePath = "Assets/Treni/CampExpress.png";
                    tipo = "Campania Express";
                    break;
            }
        }

        public string Description
        {
            get
            {
                if (noGarantito)
                {
                    if (feriale || periale)
                    {
                        return "*feriale, NON GARANTITO";
                    } else if (festivo)
                        return "*festivo, NON GARANTITO";
                    else
                        return "*NON GARANTITO";
                }
                if (festivo)
                {
                    return "*solo domenica e giorni festivi";
                }
                else
                {
                    if (feriale)
                        return "*feriale, esclusi domenica e festivi";
                    else
                        return (periale) ? "*feriale, escluso il sabato" : "";
                }
            }
        }

        public DateTime Hour
        {
            get
            {
                DateTime now = DateTime.Now;
                try
                {
                    DateTime orario = new DateTime(now.Year, now.Month, now.Day, ora, minuto, 0);
                    return orario;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    System.Diagnostics.Debug.WriteLine("Treno: " + ID);
                    return new DateTime();
                }
                
                
            }
        }

        public bool Ferial
        {
            get
            {
                return feriale;
            }
        }

        public bool Perial
        {
            get
            {
                return periale;
            }
        }

        public bool Festivo
        {
            get
            {
                return festivo;
            }
        }
    }
}
