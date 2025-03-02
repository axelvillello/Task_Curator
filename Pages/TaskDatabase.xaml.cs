/*
 * A class to define functionality for the Task Database page
 * Task database displays all tasks created that are not deleted
 */
using TaskCurator.Classes;
namespace TaskCurator;

public partial class TaskDatabase : ContentPage
{
	DatabaseDisplay dataView;
	public TaskDatabase()
	{
		InitializeComponent();

        BindingContext = dataView = new DatabaseDisplay();
    }

    protected override void OnAppearing()
    {
        //Refreshes when upon loading/reloading the page
        base.OnAppearing();
        dataView.OnPropertyChanged("TaskList");
    }

    private void OnTap(object sender, EventArgs e) 
    {
        var element = (View)sender;
        var taskSelected = (ToDoTask)element.BindingContext;
        int ID = taskSelected.Id;

        Shell.Current.Navigation.PushAsync(new ManageDBTask(ID));
    }
}