/*
 * A class to define functionality main task page (task selection menu)
 * Follows similar structure to the main page
 */
using TaskCurator.Classes;

namespace TaskCurator;

public partial class MainTaskPage : ContentPage
{
    int targetID;
    DatabaseDisplay dataView;
    public MainTaskPage(int i)
    {
        InitializeComponent();

        targetID = i;
        dataView = new DatabaseDisplay();
        //Filters the display list for relevant tasks
        dataView.FilterTasks(targetID);
        BindingContext = dataView;

    }

    protected override void OnAppearing()
    {
        //Refreshes when upon loading/reloading the page
        base.OnAppearing();
        dataView.OnPropertyChanged("TaskList");
        //Only loads relevant tasks based on ID
        dataView.FilterTasks(targetID);
    }

    //Pinch gesture on the Tasks page opens the Weekly Overview page
    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e) 
    {
        Shell.Current.Navigation.PushAsync(new WeekTaskPage(targetID));
    }

    //Due to the inability to pinch during Windows execution, double tap was added an alternative
    private void OnDoubleTap(object sender, EventArgs e)
    {
        Shell.Current.Navigation.PushAsync(new WeekTaskPage(targetID));
    }

    //Navigates to the task management page
    private void OnTap(object sender, TappedEventArgs e)
    {
        var element = (View)sender;
        var taskSelected = (ToDoTask)element.BindingContext;
        int ID = taskSelected.Id;

        Shell.Current.Navigation.PushAsync(new ManageTask(ID));
    }

    //Marks as complete upon swiping right on a task
    async void OnRightSwipe(object sender, SwipedEventArgs e)
    {
        var element = (View)sender;
        var nameToComplete = (ToDoTask)element.BindingContext;

        //If task is already complete, displays an error message
        if (nameToComplete.Completion == true)
        {
            await DisplayAlert("Task Already Crossed-Off!", "You can't finish an already finished task!", "OK");
        }
        else
        { 
            ToDoTask tDT = new()
            {
                Name = nameToComplete.Name,
                Description = nameToComplete.Description,
                StartDT = nameToComplete.StartDT,
                EndDT = nameToComplete.EndDT,
                PrevPriority = nameToComplete.Priority,
                PrevRank = nameToComplete.Rank,
                Priority = "Grey",
                Rank = 4,
                Completion = true,
                Reminders = nameToComplete.Reminders,
                FId = nameToComplete.FId,
                ImageSource = nameToComplete.ImageSource,
            };

            dataView.TaskDelete(nameToComplete);
            DatabaseService.SaveTask(tDT);
            dataView.FilterTasks(targetID);
        }
    }

    //Left swiping on a task will delay it. If the task is complete it will instead restart the task
    async void OnLeftSwipe(object sender, SwipedEventArgs e)
    {
        var element = (View)sender;
        var nameToDelay = (ToDoTask)element.BindingContext;

        //If the task is completed, restart the task
        if (nameToDelay.Completion == true) 
        {
            //Checks if the task is allowed be restarted
            if (nameToDelay.EndDT != DateTime.Now && nameToDelay.EndDT > DateTime.Now)
            {
                ToDoTask tDT = new()
                {
                    Name = nameToDelay.Name,
                    Description = nameToDelay.Description,
                    StartDT = nameToDelay.StartDT,
                    EndDT = nameToDelay.EndDT,
                    Priority = nameToDelay.PrevPriority,
                    Rank = nameToDelay.PrevRank,
                    PrevPriority = nameToDelay.PrevPriority,
                    PrevRank = nameToDelay.PrevRank,
                    Completion = false,
                    Reminders = nameToDelay.Reminders,
                    FId = nameToDelay.FId,
                    ImageSource = nameToDelay.ImageSource,
                };

                dataView.TaskDelete(nameToDelay);
                DatabaseService.SaveTask(tDT);
                dataView.FilterTasks(targetID);
            }
            else 
            {
                await DisplayAlert("Task Cannot Restart!", "Edit due date to restart task.", "OK");
            }
        }
        else
        {
            //Checks to see if the time after delay would exceed the due time
            if (nameToDelay.EndDT != nameToDelay.StartDT.AddHours(1) && nameToDelay.EndDT > nameToDelay.StartDT.AddHours(1))
            {
                bool answer = await DisplayAlert("Delay " + nameToDelay.Name + "?", "Need an extra hour before starting?", "Yes", "No");

                if (answer == true)
                {

                    ToDoTask tDT = new()
                    {
                        Name = nameToDelay.Name,
                        Description = nameToDelay.Description,
                        StartDT = nameToDelay.StartDT.AddHours(1),
                        EndDT = nameToDelay.EndDT,
                        Priority = nameToDelay.Priority,
                        Rank = nameToDelay.Rank,
                        PrevPriority = nameToDelay.PrevPriority,
                        PrevRank = nameToDelay.PrevRank,
                        Reminders = nameToDelay.Reminders,
                        FId = nameToDelay.FId,
                        ImageSource = nameToDelay.ImageSource,
                    };

                    dataView.TaskDelete(nameToDelay);
                    DatabaseService.SaveTask(tDT);
                    dataView.FilterTasks(targetID);
                }
            }
            //If the time after a delay could exceed the due time, this message will display instead
            else 
            {
                await DisplayAlert("Cannot Delay Further!", nameToDelay.Name + " is almost due!", "OK");
            }
        }
    }

    //Navigates to the add task screen
    private void OnAddTaskClicked(object sender, EventArgs e)
    {
        Shell.Current.Navigation.PushAsync(new AddTask(targetID));
    }
}