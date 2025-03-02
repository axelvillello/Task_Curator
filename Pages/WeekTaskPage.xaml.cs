/*
 * A class to define functionality to the Weekly Overview page
 * Generates a list of 7 days from the current day and counts each task for those days
 */
using System.Collections.ObjectModel;
using TaskCurator.Classes;
namespace TaskCurator; 

public partial class WeekTaskPage : ContentPage
{
    int targetID;
    int dayIndex;
    DatabaseDisplay dataView;
    string[] weekDays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"]; 
    string today = DateTime.Now.DayOfWeek.ToString();
    //Object to store the newly sorted days of week
    public ObservableCollection<DayDisplay> newWeekDays { get; set; } = new ObservableCollection<DayDisplay>();
    public WeekTaskPage(int i)
	{
        InitializeComponent();

        targetID = i;
        dataView = new DatabaseDisplay();
        //Filters the display list for relevant tasks
        dataView.FilterTasks(targetID);

        //Returns array position matching today's day name
        dayIndex = Array.IndexOf(weekDays, today);

        for (int j = 0; j < weekDays.Length; j++) 
        {
            //Creates a new entry for the current day
            DayDisplay newDay = new DayDisplay();
            newDay.Name = weekDays[dayIndex];

            //Counts the priority levels of all tasks with the current day in the loop
            var countedPriorities = dataView.CountTasks(targetID, weekDays[dayIndex]);
            newDay.NumHigh = countedPriorities.Item1;
            newDay.NumMedium = countedPriorities.Item2;
            newDay.NumLow = countedPriorities.Item3;

            newWeekDays.Add(newDay);

            dayIndex++;

            //Resets the day index if the final day is reached (Saturday)
            if (dayIndex == weekDays.Length) 
            {
                dayIndex = 0;
            }
        }

        BindingContext = this;
    }
}