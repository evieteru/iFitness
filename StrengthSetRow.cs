using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    //One row for a set
    public class StrengthSetRow
    {
        public string Exercise { get; set; }
        public int Reps { get; set; }
        public string Weight { get; set; }
    }
}
