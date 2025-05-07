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
        public WorkoutType Type { get; set; } //Cardio, Strength, Mind-Body
        public DateTime Date { get; set; } //Date of workoout
        public abstract string Summary {  get; } //User review of their performance, completion

        public string Status { get; set; } // "Completed", "Did not finish", "Did not start"
        public string Notes { get; set; } // User's notes
        public int? Difficulty { get; set; } // Difficulty rating (nullable int, 1-10)
    }
}
