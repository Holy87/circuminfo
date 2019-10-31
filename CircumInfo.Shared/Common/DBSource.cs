using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Web;
using System.Net.Http;
using Windows.UI.Popups;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.ObjectModel;

namespace CircumInfo.Common
{
    public static class DBSource
    {
        private static ObservableCollection<TrainStop> stations;    //collezione delle stazioni
        private static ObservableCollection<Linea> tratte;          //collezioni delle linee
        private static ObservableCollection<Train> treni;           //collezione dei treni con fermate
        
        /*
         * Ottiene le stazioni
         * **/
        public static Task<ObservableCollection<TrainStop>> Stations
        {
            get
            {
                return getStops();
            }
        }
        public static Task<ObservableCollection<Linea>> Tratte
        {
            get
            {
                return getLinea();
            }
        }

        public static Task<ObservableCollection<Train>> Treni
        {
            get
            {
                return getTrains();
            }
        }

        public static async Task<ObservableCollection<TrainStop>> getStops()
        {
            if (stations == null)
            {
                var obj = await getSource<TrainStop>("Stops");
                stations = (ObservableCollection<TrainStop>)obj;
            }
            return stations;
        }

        public static async Task<ObservableCollection<Train>>getTrains()
        {
            if (treni == null)
            {
                var obj = await getSource<Train>("Trains");
                treni = (ObservableCollection<Train>)obj;
            }
            return treni;
        }

        public static async Task<ObservableCollection<Linee>> getLinee()
        {
            var obj = await getSource<Linee>("Tratte");
            return(ObservableCollection<Linee>)obj;
        }

        public static async Task<Object> getSource<T>(string name)
        {
            return await getSource<T>(name, false);
        }

        public static async Task<Object> getSource<T>(string name, bool preload)
        {
            System.Diagnostics.Debug.WriteLine("GET SOURCE DI " + name);
            if (preload)
                System.Diagnostics.Debug.WriteLine("IL FILE IN PRELOAD");
            else
                System.Diagnostics.Debug.WriteLine("IL FILE IN DOWNLOAD");
            string file = name + ".xml";
            string text;
            bool updated;
            ObservableCollection<T> collezione;
            if (!preload)
            {
                updated = await TextHandler.isUpToDate(file);
                System.Diagnostics.Debug.WriteLine("Aggiornamento " + name + ": " + updated);
            }
            else
                updated = true;
            if (updated)
            {
                try
                {
                    text = await TextHandler.getFileString(file);
                    collezione = ObjectSerializer<ObservableCollection<T>>.FromXml(text);
                    return collezione;
                }
                catch (InvalidOperationException ec)
                {
                    System.Diagnostics.Debug.WriteLine(name);
                    System.Diagnostics.Debug.WriteLine(ec.Message);
                    return new ObservableCollection<T>();
                }
                
            }
            else
            {
                try
                {
                    text = await TextHandler.download_text("getData.php?file=" + file);
                    collezione = ObjectSerializer<ObservableCollection<T>>.FromXml(text);
                    TextHandler.saveText(file, text);
                    return collezione;
                }
                catch (System.InvalidOperationException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString() + "RISCARICO IL FILE");
                    return new ObservableCollection<T>();
                }
            }
        }

        public static async Task<ObservableCollection<Linee>>getLineePreLoad()
        {
            var obj = await getSource<Linee>("Tratte", true);
            return (ObservableCollection<Linee>)obj;
        }

        public static async Task<ObservableCollection<Linea>> getTrattePreLoad()
        {
            return await getLinea(true);
        }

        public static async Task<ObservableCollection<TrainStop>>getStopsPreload()
        {
            var obj = await getSource<TrainStop>("Stops", true);
            return (ObservableCollection<TrainStop>)obj;
        }

        public static async Task<ObservableCollection<Train>>getTrainsPreload()
        {
            var obj = await getSource<Train>("Trains");
            return (ObservableCollection<Train>)obj;
        }

        public static async Task<ObservableCollection<Linea>> getLinea()
        {
            return await getLinea(false);
        }

        public static async Task<ObservableCollection<Linea>> getLinea(bool preload)
        {
            if (tratte == null)
            {
                ObservableCollection<Linea> tempTratte = new ObservableCollection<Linea>();
                ObservableCollection<Linee> tempLinee;
                if (preload)
                {
                   tempLinee = await getLineePreLoad();
                }
                else
                {
                    tempLinee = await getLinee();
                }
                foreach (Linee linea in tempLinee)
                {
                    try
                    {
                        tempTratte.Add(new Linea(linea.Name, linea.Stops, linea.Sub));
                    }
                    catch (System.NullReferenceException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
                if (!preload)
                    tratte = tempTratte;
                return tempTratte;
            }
            return tratte;
        }

        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static async Task<TrainStop> getStop(int stopId)
        {
            foreach (TrainStop stop in await Stations)
            {
                if (stop.Id == stopId)
                {
                    return stop;
                }
            }
            return null;
        }

        public static async Task<Train> getTrain(string trainId)
        {
            foreach (Train treno in await Treni)
            {
                if (treno.ID == trainId)
                    return treno;
            }
            return null;
        }

        public static async Task<Linea> getLinea(string nome)
        {
            foreach (Linea linea in await Tratte)
            {
                if (nome == linea.Name)
                {
                    return linea;
                }
            }
            return new Linea("Errore!", "", "");
        }

        public static async Task<ObservableCollection<TrainStop>> getFermate(string fermate)
        {
            string[] id = fermate.Split(',');
            ObservableCollection<TrainStop> elencoFermate = new ObservableCollection<TrainStop>();
            foreach (string idFermata in id)
            {
                elencoFermate.Add(await getStop(Convert.ToInt16(idFermata)));
            }
            return elencoFermate;
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata)
        {
            return await getPartenze(idFermata, "");
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata, DateTime orario, int maxx, string direzione)
        {
            ObservableCollection<Partenza> partenze = new ObservableCollection<Partenza>();
            int max = 0;
            foreach (Partenza partenza in await getPartenze(idFermata))
            {
                if (partenza.Hour >= orario && ferialRight(orario, partenza) && (direzione == partenza.Direction || direzione == ""))
                {
                    partenze.Add(partenza);
                    max++;
                    if (max >= maxx)
                        break;
                }
            }
            return partenze;
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata, DateTime orario, int maxx)
        {
            return await getPartenze(idFermata, orario, maxx, "");
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata, bool preload, string direzione)
        {
            if (!preload)
                return await getPartenze(idFermata, direzione);
            ObservableCollection<Partenza> partenze = new ObservableCollection<Partenza>();
            System.Diagnostics.Debug.WriteLine(direzione + " è la direzione");
            foreach (Train treno in await getTrainsPreload())
            {
                if (treno.stopsAt(idFermata) && (treno.Direction == direzione || direzione == ""))
                {
                    partenze.Add(new Partenza(treno, idFermata, treno.festive(idFermata)));
                }
            }
            return partenze.OrderBy(x => x.Hour);
            
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata, bool preload)
        {
            return await getPartenze(idFermata, preload, "");
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata, DateTime orario, bool preload, int maxx, string direzione)
        {
            if (!preload)
                return await getPartenze(idFermata, orario, maxx, direzione);
            ObservableCollection<Partenza> partenze = new ObservableCollection<Partenza>();
            int max = 0;
            foreach (Partenza partenza in await getPartenze(idFermata, true))
            {
                if (partenza.Hour > orario && ferialRight(orario, partenza) && (partenza.Direction == direzione || direzione == "") && max < maxx)
                {
                    partenze.Add(partenza);
                    max++;
                }
            }
            return partenze;
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata, DateTime orario, bool preload, int maxx)
        {
            return await getPartenze(idFermata, orario, preload, maxx, "");
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata, DateTime orario, string direzione)
        {
            return await getPartenze(idFermata, orario, false, getMaxx(), direzione);
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata, string direzione)
        {
            ObservableCollection<Partenza> partenze = new ObservableCollection<Partenza>();
            foreach (Train treno in await Treni)
            {
                if (treno.stopsAt(idFermata) && (treno.Direction == direzione || direzione == ""))
                {
                    partenze.Add(new Partenza(treno, idFermata, treno.festive(idFermata)));
                }
            }
            return partenze.OrderBy(x => x.Hour);
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int p, DateTime dateTime)
        {
            return await getPartenze(p, dateTime, getMaxx());
        }

        public static async Task<IEnumerable<Partenza>> getPartenze(int idFermata, DateTime orario, bool preload)
        {
            
            System.Diagnostics.Debug.WriteLine("MAXX " + getMaxx());
            return await getPartenze(idFermata, orario, preload, getMaxx());
        }

        private static int getMaxx()
        {
            int maxx = 0;
            switch (Settings.MaxTrains)
            {
                case 0:
                    maxx = 5;
                    break;
                case 1:
                    maxx = 10;
                    break;
                case 2:
                    maxx = 15;
                    break;
                case 3:
                    maxx = 99;
                    break;
            }
            return maxx;
        }

        private static bool ferialRight(DateTime orario, Partenza partenza)
        {
            if (ferialDay(orario) && partenza.Festivo)
                return false;
            if (!ferialDay(orario) && partenza.Ferial)
                return false;
            if (orario.DayOfWeek == DayOfWeek.Saturday && partenza.Perial)
                return false;
            return true;
        }

        public static bool ferialDay(DateTime data)
        {
            if (data.DayOfWeek == DayOfWeek.Sunday)
                return false;
            if (data.Month == 1)
            {
                if (data.Day == 1 || data.Day == 6)
                    return false;
            }
            if(data.Month == 4 && data.Day == 25)
                return false;
            if (data.Month == 5 && data.Day == 1)
                return false;
            if (data.Month == 6 && data.Day == 2)
                return false;
            if (data.Month == 8 && data.Day == 15)
                return false;
            if (data.Month == 12)
            {
                if (data.Day == 8 || data.Day == 25 || data.Day == 26)
                    return false;
            }
            DateTime pasqua = giornoPasqua(data.Year);
            if (pasqua.Day == data.Day && pasqua.Month == data.Month)
                return true;
            if (pasqua.AddDays(1).Day == data.Day && pasqua.AddDays(1).Month == data.Month)
                return true;
            return true;
        }

        public static DateTime giornoPasqua(int anno)
        {
            int giorno, mese;
            int a, b, c, d, e, f, n, h, i, k, l, m;
            a = anno % 19;
            b = anno / 100;
            c = anno % 100;
            d = b / 4;
            e = b % 4;
            f = (b + 8) / 25;
            n = (b - f + 1) / 3;
            h = (19 * a + b - d - n + 15) % 30;
            i = c / 4;
            k = c % 4;
            l = (32 + 2 * e + 2 * i - h - k) % 7;
            m = (a + 11 * h + 22 * l) / 451;
            giorno = ((h + l - 7 * m + 114) % 31) + 1;
            mese = (h + l - 7 * m + 114) / 31;
            return new DateTime(anno, mese, giorno);
        }

        internal static bool trainsLoaded()
        {
            if (treni == null)
                return false;
            if (treni.Count == 0)
                return false;
            return true;
        }



        public static ObservableCollection<string> TempDirezioni { get; set; }

        public static bool TempMostraTutto { get; set; }

        public static bool DialogOk { get; set; }

        public static string tempDirezione { get; set; }

        
    }
}
