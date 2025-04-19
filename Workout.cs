using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    public abstract class Workout
    {
        public string Description { get; set; } //User given name of workout
        public WorkoutType Type { get; set; } //Cardio, Strength
        public DateTime Date { get; set; } //Date of workoout
        public abstract string Summary {  get; } //User review of their performance, completion

    }
}
