using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    public class StrengthWorkout : Workout
    {
        public ObservableCollection<StrengthSet> Sets { get; set; } = new ObservableCollection<StrengthSet>();
        public override string Summary => $"{Sets.Count} strength set(s)";
    }
}
