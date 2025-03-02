/*
 * A class to define functionality for the Add Task Group page
 * Acts as an entry form for new groups. Groups are to organise certain tasks together
 */

using TaskCurator.Classes;

namespace TaskCurator;

public partial class AddTaskGroup : ContentPage
{
    public AddTaskGroup() 
    { 
        InitializeComponent(); 
    }

    private void OnCreateGroupClicked(object sender, EventArgs e)
    {
        //Exception handling for invalid groups
        if (GroupNameEntry.Text == null) 
        {
            DisplayAlert("Names cannot be blank!", "Please create a name for this group of tasks.", "OK");
            return;
        }
        else 
        {
            TaskGroup tGroup = new() { Name = GroupNameEntry.Text, Description = GroupDescEntry.Text };

            DatabaseService.SaveGroup(tGroup);

            NotificationManager.ShowToast("The " + GroupNameEntry.Text + " group was created!");

            Shell.Current.Navigation.PopAsync();
        }

    }

}