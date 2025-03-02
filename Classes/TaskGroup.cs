/*
 * A class to define a unique group to organise tasks, and the database table to store them
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace TaskCurator.Classes
{
    [Table("groups")]
    public class TaskGroup()
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; } = "";


    }
}
