using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFitness
{
    //One row, many of these rows will make up a set
    public class CardioSetRow
    {
        public string Distance {  get; set; }
        public string Time {  get; set; }
        public string Note { get; set; }
    }
}
