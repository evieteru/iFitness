using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    internal class CardioWorkout : Workout
    {
        public ObservableCollection<CardioSet> Sets { get; set; } = new ObservableCollection<CardioSet>();
    }
}
