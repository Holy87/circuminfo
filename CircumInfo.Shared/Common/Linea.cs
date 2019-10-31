using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CircumInfo.Common
{
    public class Linea
    {
        private string nome;
        private string sub;
        private string tempFermate;
        private ObservableCollection<TrainStop> fermate;
        public Linea (string nome, string fermate, string sub)
        {
            this.nome = nome;
            this.sub = sub;
            tempFermate = fermate;
        }

        private async Task<TrainStop> getStop(int initialId)
        {
            foreach (TrainStop stop in await DBSource.Stations)
            {
                if (stop.Id == initialId)
                    return stop;
            }
            return null;
        }

        public Task<ObservableCollection<TrainStop>> Fermate
        {
            get
            {
                if (fermate == null)
                    this.fermate = new ObservableCollection<TrainStop>();
                    
                    return DBSource.getFermate(tempFermate);
                    
            }
        }

        public string Name
        {
            get
            {
                return nome;
            }
        }

        public string Sub
        {
            get
            {
                return sub;
            }
        }
    }
}
