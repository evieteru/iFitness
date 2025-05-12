using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace iFitness
{
    //One single set (table), can have many rows
    public class StrengthSet
    {
        public string Name { get; set; } = "New Strength Set";    //Default name
        [JsonInclude] //JsonInclude attribute to include this property in serialization
        public ObservableCollection<StrengthSetRow> Rows { get; set; } = new ObservableCollection<StrengthSetRow>();
        public string SetLabel => Name;
        public int SetReps { get; set; } = 1; //Default to one

    }
}

