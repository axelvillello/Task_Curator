/*
 * A class to define functionality for the Manage Database Task page
 * Displays options available when selecting a task from the Task Database
 */
using TaskCurator.Classes;
namespace TaskCurator;

public partial class ManageDBTask : ContentPage
{
    int idTemp;
    DatabaseDisplay dataView;
    public ManageDBTask(int taskSelected)
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
    private void OnEditTaskClicked(object sender, EventArgs e)
    {
        var element = (View)sender;
        var taskSelected = (ToDoTask)element.BindingContext;
        int ID = taskSelected.Id;

        Shell.Current.Navigation.PushAsync(new EditTask(ID));
    }
}