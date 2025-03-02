/* 
 *  Class is based on the DatabaseService class from INFT6009 W7
 */

using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskCurator.Classes
{
    internal static class DatabaseService
    {
        private static string _databaseFile;
        private static string DatabaseFile
        {
            get
            {
                if (_databaseFile == null)
                {
                    string databaseDir = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "Data");
                    System.IO.Directory.CreateDirectory(databaseDir);
                    _databaseFile = Path.Combine(databaseDir, "TaskCurator_Data.sqlite");
                }
                return _databaseFile;
            }
        }

        private static SQLiteConnection _connection;
        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(DatabaseFile);
                    //Initialise tables for groups and tasks
                    _connection.CreateTable<TaskGroup>();
                    _connection.CreateTable<ToDoTask>();
                }
                return _connection;
            }
        }
        //Static method to add new groups to the database
        public static void SaveGroup(TaskGroup model)
        {
            if (model.Id > 0)
                DatabaseService.Connection.Update(model);
            else
                DatabaseService.Connection.Insert(model);
        }

        //Static method to add new tasks to the database
        public static void SaveTask(ToDoTask model)
        {
            if (model.Id > 0)
                DatabaseService.Connection.Update(model);
            else
                DatabaseService.Connection.Insert(model);
        }
    }
}
