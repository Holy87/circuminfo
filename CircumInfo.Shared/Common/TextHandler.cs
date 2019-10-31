using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CircumInfo.Common
{
    /// <summary>
    /// Si occupa di gestire i file di testo
    /// </summary>
    public static class TextHandler
    {
        /// <summary>
        /// Il dominio del web service dove prendere le informazioni sul treno
        /// </summary>
        public static readonly string DOMINIO = "http://holyres.altervista.org/circuminfo/";
        /// <summary>
        /// Carica il file dalla memoria del telefono e ne legge il contenuto
        /// </summary>
        /// <param name="filename">è il nome del file del telefono</param>
        /// <returns>la stringa del file se è stato letto, una stringa vuota altrimenti.</returns>
        public async static Task<string> getFileString(string filename)
        {
            try
            {
                IStorageItem storageItem = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                IRandomAccessStream randomAccessStream = await file.OpenAsync(FileAccessMode.Read);
                using (DataReader reader = new DataReader(randomAccessStream.GetInputStreamAt(0)))
                {
                    uint bytesloaded = await reader.LoadAsync((uint)randomAccessStream.Size);
                    string readString = reader.ReadString(bytesloaded);
                    return readString;
                }
            }
        
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("IL FILE " + filename + "NON È STATO TROVATO. " + ex.ToString());
                return "";
            }

        }
        /// <summary>
        /// Scarica il file di testo dal dominio.
        /// </summary>
        /// <param name="text">È il file di testo da scaricare.</param>
        /// <returns>La stringa di testo del file online. Può anche essere un XML</returns>
        public static async Task<string> download_text(string text)
        {
            //HttpClient client = new HttpClient();
            //string result = await client.GetStringAsync(new Uri(DOMINIO + "getData.php?file=" + text, UriKind.Absolute));
            //System.Diagnostics.Debug.WriteLine(text);
            //return result;
            HttpClientHandler aHandler = new HttpClientHandler();
            aHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            HttpClient aClient = new HttpClient(aHandler);
            aClient.DefaultRequestHeaders.ExpectContinue = false;
            System.Diagnostics.Debug.WriteLine("DOWNLOAD DA " + DOMINIO + text);
            HttpResponseMessage response = await aClient.GetAsync(DOMINIO + text, HttpCompletionOption.ResponseHeadersRead);
            System.Diagnostics.Debug.WriteLine("DOWNLOAD COMPLETATO");
            return await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// Ottiene la data di ultima modifica del file IN LOCALE
        /// </summary>
        /// <param name="filename">nome del file</param>
        /// <returns>La data di modifica</returns>
        public static async Task<DateTime> getLastUpdatedDate(string filename)
        {
            try
            {
                Windows.Storage.StorageFile tsaved = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                Windows.Storage.FileProperties.BasicProperties prop = await tsaved.GetBasicPropertiesAsync();
                return prop.DateModified.DateTime;
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }
        /// <summary>
        /// Ottiene la data di ultima modifica di un file ONLINE
        /// </summary>
        /// <param name="filename">nome del file</param>
        /// <returns>La data di ultima modifica</returns>
        public static async Task<DateTime> getOnlineFileDate(string filename)
        {
            try
            {
                string timeText = await download_text("getFileTime.php?file=" + filename);
                string[] t = timeText.Split(',');
                int year = Convert.ToInt32(t[0]);
                int month = Convert.ToInt32(t[1]);
                int day = Convert.ToInt32(t[2]);
                int hour = Convert.ToInt32(t[3]);
                int min = Convert.ToInt32(t[4]);
                int sec = Convert.ToInt32(t[5]);
                return new DateTime(year, month, day, hour, min, sec);
            }
            catch (Exception)
            {
                return new DateTime();
            }

        }
        /// <summary>
        /// Controlla l'aggiornamento di un file online rispetto a quello locale
        /// </summary>
        /// <param name="filename">nome del file da confrontare</param>
        /// <returns>True se il file in locale è aggiornato, false se è più aggiornato quello in remoto</returns>
        public static async Task<bool> isUpToDate(string filename)
        {
            DateTime onlineFileDate = await getOnlineFileDate(filename);
            DateTime offineFileDate = await getLastUpdatedDate(filename);
            System.Diagnostics.Debug.WriteLine(filename + "ONLINE " + onlineFileDate + ", OFFLINE " + offineFileDate);
            return onlineFileDate <= offineFileDate;
        }
        /// <summary>
        /// Salva una stringa su un file nella memoria locale
        /// </summary>
        /// <param name="fileName">nome del file</param>
        /// <param name="text">stringa da scrivere nel file</param>
        internal static async void saveText(string fileName, string text)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            IRandomAccessStream randomAccessStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            using (DataWriter writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0)))
            {
                writer.WriteString(text);
                await writer.StoreAsync();
            }
        }
    }
}
