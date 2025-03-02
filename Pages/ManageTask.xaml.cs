/*
    A class to define functionality for the Manage Task page
    Displays options to perform on the selected task
 */

using TaskCurator.Classes;

namespace TaskCurator;

public partial class ManageTask : ContentPage
{
    int idTemp;
    DatabaseDisplay dataView;

    public ManageTask(int taskSelected)
	{
        InitializeComponent();

        idTemp = taskSelected;
        dataView = new DatabaseDisplay();
        dataView.SpecifiedTasks(idTemp);
        BindingContext = dataView.FilteredTasks[0];

    }

    async void OnDeleteTaskClicked(object sender, EventArgs e)
    {
        var taskDelete = dataView.FilteredTasks[0];

        bool answer = await DisplayAlert("Delete " + taskDelete.Name, "Are you sure you want to delete this task?", "Yes", "No");

        if (answer == true)
        {
            dataView.TaskDelete(taskDelete);
        }

        await Navigation.PopAsync();
    }

    async void OnDelayTaskClicked(object sender, EventArgs e)
    {
        var element = (View)sender;
        var nameToDelay = (ToDoTask)element.BindingContext;

        if (nameToDelay.EndDT != nameToDelay.StartDT.AddHours(1) && nameToDelay.EndDT > nameToDelay.StartDT.AddHours(1))
        {
            bool answer = await DisplayAlert("Delay " + nameToDelay.Name + "?", "Need an extra hour before starting? ", "Yes", "No");

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
                    Completion = false,
                    Reminders = nameToDelay.Reminders,
                    FId = nameToDelay.FId,
                    ImageSource = nameToDelay.ImageSource
                };

                dataView.TaskDelete(nameToDelay);
                DatabaseService.SaveTask(tDT);
                await Navigation.PopAsync();
            }
        }
        else
        {
            await DisplayAlert("Cannot Delay Further!", nameToDelay.Name + " is almost due!", "OK");
        }

    }

    //Opens task edit screen for the task
    private void OnEditTaskClicked(object sender, EventArgs e)
    {
        var element = (View)sender;
        var taskSelected = (ToDoTask)element.BindingContext;
        int ID = taskSelected.Id;

        Shell.Current.Navigation.PushAsync(new EditTask(ID));
    }

    //Sets the task as complete as if they had swiped right
    async void OnCompletedClicked(object sender, EventArgs e)
    {
        var element = (View)sender;
        var taskSelected = (ToDoTask)element.BindingContext;

        //When Completion is true, this button's functionality changes to Restarting a task
        if (taskSelected.Completion == true)
        {
            ToDoTask tDT = new()
            {
                Name = taskSelected.Name,
                Description = taskSelected.Description,
                StartDT = taskSelected.StartDT,
                EndDT = taskSelected.EndDT,
                Priority = taskSelected.PrevPriority,
                Rank = taskSelected.PrevRank,
                PrevPriority = taskSelected.PrevPriority,
                PrevRank = taskSelected.PrevRank,
                Completion = false,
                Reminders = taskSelected.Reminders,
                FId = taskSelected.FId,
                ImageSource = taskSelected.ImageSource
            };

            DatabaseService.SaveTask(tDT);
            
            NotificationManager.ShowToast("The task of " + taskSelected.Name + " has restarted!");
        }
        else 
        {
            ToDoTask tDT = new()
            {
                Name = taskSelected.Name,
                Description = taskSelected.Description,
                StartDT = taskSelected.StartDT,
                EndDT = taskSelected.EndDT,
                PrevPriority = taskSelected.Priority,
                PrevRank = taskSelected.Rank,
                Priority = "Grey",
                Rank = 4,
                Completion = true,
                Reminders = taskSelected.Reminders,
                FId = taskSelected.FId,
                ImageSource = taskSelected.ImageSource
            };

            DatabaseService.SaveTask(tDT);
        }

        dataView.TaskDelete(taskSelected);
        await Navigation.PopAsync();
    }
}