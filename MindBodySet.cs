using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    //One single set (table), can have many rows
    public class MindBodySet
    {
        public ObservableCollection<MindBodySetRow> Rows { get; set; } = new ObservableCollection<MindBodySetRow>();
        public string SetLabel => $"{Rows.Count} exercise(s)";
    }
}
