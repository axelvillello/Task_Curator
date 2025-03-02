using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskCurator.Classes
{
    //Class for usage by the WeekTaskPage to access data relevant to one day of the week
    public class DayDisplay ()
    {
        public string Name { get; set; }
        public int NumHigh { get; set; }
        public int NumMedium { get; set; }
        public int NumLow { get; set; }
    }
}
