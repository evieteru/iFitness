using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    internal class StrengthSet
    {
        public ObservableCollection<StrengthSetRow> Rows { get; set; } = new ObservableCollection<StrengthSetRow>();
    }
}
