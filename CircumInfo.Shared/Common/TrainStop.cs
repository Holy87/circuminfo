using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Devices.Geolocation;
/*
 * Questa classe viene usata per mostrare le fermate dei treni
 * */
namespace CircumInfo.Common
{
    public class TrainStop
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string City { get; set; }

        public double Latitude
        {
            get {
                try {
                    return Convert.ToDouble(Lat);
                }
                catch (System.FormatException ex)
                {
                    Debug.WriteLine(Name + " Latitudine: " + Lat);
                    Debug.WriteLine(ex.Message);
                    return 0.0;
                }
                
            }
            
        }

        public double Longitude
        {
            get
            {
                try
                {
                    return Convert.ToDouble(Lon);
                } catch (System.FormatException ex) {
                    Debug.WriteLine(ex.Message);
                    return 0.0;// Convert.ToDouble(Lon);
                }
            }
        }

        public string Title
        {
            get
            {
                return Name;
            }
        }

        public string Subtitle
        {
            get
            {
                if (City != null)
                    return City;
                return "Stazione";
            }
        }

        public Geopoint Location
        {
            get
            {
                return new Geopoint(new BasicGeoposition()
                {
                    Longitude = Longitude,
                    Latitude = Latitude
                });
            }
        }
    }
}
