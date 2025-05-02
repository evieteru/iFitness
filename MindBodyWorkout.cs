using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    public class MindBodyWorkout : Workout
    {
        public ObservableCollection<MindBodySet> Sets { get; set; } = new ObservableCollection<MindBodySet>();
        public override string Summary => $"{Sets.Count} mind-body set(s)";
    }
}
