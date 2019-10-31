using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Popups;

namespace CircumInfo.Common
{
    public static class Settings
    {
        public static string comando;
        public static string idStazione;
        public static ObservableCollection<TrainStop> preferiti;
        public static bool positionActivated = false;
        public static Geoposition position;
        /// <summary>
        /// Ottiene o imposta l'opzione di localizzazione dell'app
        /// </summary>
        public static bool Location {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("POS"))
                {
                    bool enabled = (bool)ApplicationData.Current.LocalSettings.Values["POS"];
                    return enabled;
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values.Add("POS", false);
                    return false;
                }
            }
            set
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("POS"))
                {
                    ApplicationData.Current.LocalSettings.Values["POS"] = (bool)value;
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values.Add("POS", (bool)value);
                }
            }
        }
        /// <summary>
        /// Ottiene o imposta l'attivazione della pubblicità
        /// </summary>
        public static bool Spot
        {
            get
            {
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("Spot"))
                    return (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["Spot"];
                else
                    return true;
            }
            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["Spot"] = value;
            }
        }
        /// <summary>
        /// Ottiene o imposta l'impostazione di mostrare tutti i treni che partono nelle stazioni
        /// </summary>
        public static bool? ShowAll
        {
            get
            {
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("SAL"))
                {
                    bool enabled = (bool)ApplicationData.Current.RoamingSettings.Values["SAL"];
                    return enabled;
                }
                else
                {
                    ApplicationData.Current.RoamingSettings.Values.Add("SAL", true);
                    return true;
                }
            }
            set
            {
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("SAL"))
                {
                    ApplicationData.Current.RoamingSettings.Values["SAL"] = (bool)value;
                }
                else
                {
                    ApplicationData.Current.RoamingSettings.Values.Add("SAL", (bool)value);
                }
            }
        }
        /// <summary>
        /// Ottiene o imposta l'ID dell'ultimo messaggio visualizzato.
        /// </summary>
        public static int LastMessage
        {
            get
            {
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("MSGID"))
                {
                    return (int)ApplicationData.Current.RoamingSettings.Values["MSGID"];
                }
                else
                {
                    ApplicationData.Current.RoamingSettings.Values.Add("MSGID", 0);
                    return 0;
                }
            }
            set
            {
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("MSGID"))
                {
                    ApplicationData.Current.RoamingSettings.Values["MSGID"] = value;
                }
                else
                {
                    ApplicationData.Current.RoamingSettings.Values.Add("MSGID", value);
                }
            }
        }
        /// <summary>
        /// Ottiene o imposta il numero massimo di treni visibili quando c'è l'impostazione di non visualizzare tutti i treni.
        /// </summary>
        public static int MaxTrains
        {
            get
            {
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("MAX"))
                {
                    int trains = (int)ApplicationData.Current.RoamingSettings.Values["MAX"];
                    return trains;
                }
                else
                {
                    ApplicationData.Current.RoamingSettings.Values.Add("MAX", 0);
                    return 0;
                }
            }
            set
            {
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("MAX"))
                {
                    ApplicationData.Current.RoamingSettings.Values["MAX"] = value;
                }
                else
                {
                    ApplicationData.Current.RoamingSettings.Values.Add("MAX", value);
                }
            }
        }
        /// <summary>
        /// Ottiene o modifica il numero di volte che è stata aperta l'app
        /// </summary>
        public static int AppOpened
        {
            get
            {
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("OPEN"))
                {
                    int trains = (int)ApplicationData.Current.RoamingSettings.Values["OPEN"];
                    return trains;
                }
                else
                {
                    ApplicationData.Current.RoamingSettings.Values.Add("OPEN", 0);
                    return 0;
                }
            }
            set
            {
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("OPEN"))
                {
                    ApplicationData.Current.RoamingSettings.Values["OPEN"] = value;
                }
                else
                {
                    ApplicationData.Current.RoamingSettings.Values.Add("OPEN", value);
                }
            }
        }
        /// <summary>
        /// Mostra se è il primo lancio dell'applicazione.
        /// </summary>
        public static bool firstLaunch
        {
            get
            {
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings.Values["1launch"];
                if (settings == null || (bool)settings == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["1launch"] = (bool)value;
            }
        }
        /// <summary>
        /// Ottiene la posizione dell'utente
        /// </summary>
        public static Task<Geoposition> Position
        {
            get
            {
                try
                {
                    return getPosition();
                } catch(System.UnauthorizedAccessException)
                {
                    throw new DisabledPositionException(0);
                }
            }
        }

        public async static Task<Geoposition> getPosition()
        {
            if (position != null)
            {
                return position;
            }
            if (!Location)
            {
                throw new DisabledPositionException(0);
            }
            Geolocator geolocator = new Geolocator();
            if (geolocator.LocationStatus == PositionStatus.Disabled)
            {
                throw new DisabledPositionException(1);
            }
            else
            {
                geolocator.DesiredAccuracyInMeters = 50;
                Geoposition geoposition = await geolocator.GetGeopositionAsync();
                        //maximumAge: TimeSpan.FromMinutes(5),
                        //timeout: TimeSpan.FromSeconds(10)
                        //);
                if (geoposition == null)
                    throw new PositionNotFoundException();
                position = geoposition;
                return geoposition;
            }
        }

        //public static async Task<Geoposition> getPosition(bool refresh)
        //{
        //    if (refresh)
        //        position = null;
        //    return await getPosition();
        //}

        public static TrainStop nearestStop(Geopoint posizione, ObservableCollection<TrainStop> stops)
        {
            double distance = 9999.0;
            TrainStop selectedStop = stops[0];
            foreach (TrainStop fermata in stops)
            {
                double d2 = calcDistance(posizione, fermata.Latitude, fermata.Longitude);
                //System.Diagnostics.Debug.WriteLine("Distanza " + fermata.Name + ": " + d2);
                if (d2 < distance)
                {
                    distance = d2;
                    selectedStop = fermata;
                }
            }
            return selectedStop;
        }
        /// <summary>
        /// Calcola la distanza tra la posizione attuale e un punto sulla mappa
        /// </summary>
        /// <param name="posizione">posizione dell'utente</param>
        /// <param name="p1">Latitudine del secondo punto</param>
        /// <param name="p2">Longitudine del secondo punto</param>
        /// <returns></returns>
        private static double calcDistance(Geopoint posizione, double p1, double p2)
        {
            //return (posizione.Position.Latitude - p1) * (posizione.Position.Latitude - p1) + (posizione.Position.Longitude - p2) * (posizione.Position.Longitude - p2);
            return Haversine.calculate(posizione.Position.Latitude, posizione.Position.Longitude, p1, p2);
        }
        /// <summary>
        /// Ottiene i preferiti salvati dell'utente
        /// </summary>
        public static void caricaPreferiti()
        {
            if (preferiti == null)
                try
                {
                    preferiti = ObjectSerializer<ObservableCollection<TrainStop>>.FromXml((string)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["favs"]);
                }
                catch(InvalidOperationException ex)
                {
                    preferiti = new ObservableCollection<TrainStop>();
                    System.Diagnostics.Debug.WriteLine("OPERAZIONE NON VALIDA CARICAMENTO PREFERITI " + ex.Message);
                } catch (System.ArgumentNullException ex)
                {
                    preferiti = new ObservableCollection<TrainStop>();
                    System.Diagnostics.Debug.WriteLine("NESSUN ARGOMENTO CARICAMENTO PREFERITI " + ex.Message);
                }
        }
        /// <summary>
        /// Salva i preferiti dell'utente nella roamingstorage
        /// </summary>
        public static void salvaPreferiti()
        {
            caricaPreferiti();
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["favs"] = ObjectSerializer<ObservableCollection<TrainStop>>.ToXml(preferiti);
        }

        public static ObservableCollection<TrainStop> Favorites
        {
            get
            {
                caricaPreferiti();
                return preferiti;
            }
            set
            {
                preferiti = value;
                salvaPreferiti();
            }
        }
        /// <summary>
        /// Controlla che una certa stazione sia tra i preferiti
        /// </summary>
        /// <param name="stationId">ID della stazione</param>
        /// <returns>true se è tra i preferiti, false altrimenti</returns>
        public static bool inFavs(int stationId)
        {
            caricaPreferiti();
            foreach (TrainStop stop in preferiti)
            {
                if (stop.Id == stationId)
                return true;
            }
            return false;
        }
        /// <summary>
        /// Aggiunge una stazione ai preferiti
        /// </summary>
        /// <param name="stop">ID della stazione</param>
        public static void addFavs(TrainStop stop)
        {
            caricaPreferiti();
            if (!inFavs(stop.Id))
            {
                preferiti.Add(stop);
                salvaPreferiti();
            }
        }
        /// <summary>
        /// Rimuove una stazione dai preferiti
        /// </summary>
        /// <param name="stationId">ID della stazione da rimuovere</param>
        public static void removeFavs(int stationId)
        {
            TrainStop stazioneCancellare = null;
            caricaPreferiti();
            foreach (TrainStop stop in preferiti)
            {
                if (stationId == stop.Id)
                    stazioneCancellare = stop;
            }
            if (stazioneCancellare != null)
            {
                preferiti.Remove(stazioneCancellare);
                salvaPreferiti();
            }
        }

        
    }

    
}
