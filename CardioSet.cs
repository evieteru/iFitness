using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    //Represents one "set" as a table, with many rows
    public class CardioSet
    {
        public ObservableCollection<CardioSetRow> Rows { get; set; } = new ObservableCollection<CardioSetRow>();
        public string SetLabel => $"{Rows.Count} rounds(s)";
    }
}
