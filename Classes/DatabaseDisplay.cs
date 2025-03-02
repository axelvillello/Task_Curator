/*
 *  Class based on the QuestViewModel class from INFT6009 W7
 */

using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaskCurator.Classes
{
    internal class DatabaseDisplay : ObservableObject
    {
        public ObservableCollection<ToDoTask> FilteredTasks { get; set; } = new ObservableCollection<ToDoTask>();   //Lists only tasks that are to be displayed
        SQLiteConnection _connection;

        public DatabaseDisplay()
        {
            _connection = DatabaseService.Connection;
        }

        public List<TaskGroup> GroupList
        {
            get
            {
                //Return all group objects in the table, and order them alphabetically
                var avlTaskGroups = _connection.Table<TaskGroup>()
                .OrderBy(x => x.Name)
                .ToList();
                return avlTaskGroups;
            }
        }

        public List<ToDoTask> TaskList
        {
            get
            {
                //Return all task objects in the table, and orders them based on priority rank
                var avlTasks = _connection.Table<ToDoTask>()
                .OrderBy(x => x.Rank)
                .ToList();
                return avlTasks;
            }
        }

        public void GroupDelete(TaskGroup i)
        {
            _connection.Delete(i);

            //Refreshes page on delete
            base.OnPropertyChanged("GroupList");
        }

        public void TaskDelete(ToDoTask i)
        {
            _connection.Delete(i);

            //Refreshes page on delete
            base.OnPropertyChanged("TaskList");
        }


        //Finds and lists tasks belonging to a certain group
        public void FilterTasks(int i)
        {
            //resets the FilterTask list
            FilteredTasks.Clear();
            
            //Only lists tasks with the same FId as the selected group's Id
            var filteredTasks = TaskList.Where(task => task.FId == i);

            foreach (var task in filteredTasks)
            {
                FilteredTasks.Add(task);
            }

            base.OnPropertyChanged("FilteredTasks");
        }

        //Deletes all task belonging to a group
        public void DeleteGroupTasks(int i)
        {

            //Only lists tasks with the same FId as the selected group's Id
            var deleteTasks = TaskList.Where(task => task.FId == i);

            foreach (var task in deleteTasks)
            {
                _connection.Delete(task);
            }

            base.OnPropertyChanged("TaskList");
        }

        //Counts tasks based on if they are of the correct day of the week
        public (int, int, int) CountTasks(int id, string day)
        {

            //Only lists tasks with the same FId as the selected group's Id and with tthe correct day of the week
            var filteredTasks = TaskList.Where(task => task.FId == id && task.StartDT.DayOfWeek.ToString() == day);

            int high = 0 , medium = 0, low = 0;

            //Checks the priority level of each filtered task, and increments a counter for each priority level
            foreach (var task in filteredTasks)
            {
                switch (task.Priority) 
                {
                    case "Red":
                        high++;
                        break;
                    case "Purple":
                        medium++; 
                        break;
                    case "Blue":
                        low++; 
                        break;
                    default:
                        break;
                }
            }

            return (high, medium, low);

        }

        //Search method for a specific task, attempts to reuse FilterTask list for operations on a single object
        public void SpecifiedTasks(int i)
        {
            
            FilteredTasks.Clear();
            var specifiedTasks = TaskList.Where(task => task.Id == i);

            foreach (var task in specifiedTasks)
            {
                FilteredTasks.Add(task);
            }
        }

    }
}
