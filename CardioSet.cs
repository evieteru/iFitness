using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace iFitness
{
    //Represents one "set" as a table, with many rows
    public class CardioSet
    {
        public string Name { get; set; } = "New Cardio Set";
        [JsonInclude]
        public ObservableCollection<CardioSetRow> Rows { get; set; } = new ObservableCollection<CardioSetRow>();
        public string SetLabel => Name;

        public int SetReps { get; set; } = 1; //Default to one
    }
}
