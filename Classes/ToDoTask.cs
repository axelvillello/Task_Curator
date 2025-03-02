/*
 * A class to define a task, and a database table to store them in
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace TaskCurator.Classes
{
    [Table("tasks")]
    public class ToDoTask ()
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int FId { get; set; }

        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; } = "";

        public DateTime StartDT { get; set; }
        public DateTime EndDT { get; set; }
        //String containing the color assigned to the priority level
        //Red = High, Purple = Medium, Blue = Low, Grey = COMPLETE
        public string Priority { get; set; }    
        public int Rank {  get; set; }
        public bool Completion { get; set; }
        public bool Reminders { get; set; }
        //The previous priority and rank of tasks are recorded as completion of tasks alters Priority and Rank
        public string PrevPriority { get; set; }
        public int PrevRank { get; set; }
        public string ImageSource { get; set; }
    }

}
