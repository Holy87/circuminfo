using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;

namespace CircumInfo.Common
{
    public static class Vocabs
    {
        /// <summary>
        /// Ottiene la stringa dalla lingua desiderata
        /// </summary>
        /// <param name="key">la chiave. Ad esempio Title o App/Title</param>
        /// <returns>una stringa dalla chiave a seconda della lingua del sistema</returns>
        public static string getString(string key)
        {
            ResourceLoader loader = new ResourceLoader();
            return loader.GetString(key);
        }
    }
}
