using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    internal abstract class Workout
    {
        public string Description { get; set; } //User given name of workout
        public WorkoutType Type { get; set; } //Cardio, Strength

    }
}
