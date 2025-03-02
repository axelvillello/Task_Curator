/*
 * A class to define functionality main page (group selection menu)
 */

using TaskCurator.Classes;
using System.Diagnostics;

namespace TaskCurator
{
    public partial class MainPage : ContentPage
    {
        DatabaseDisplay dataView;
        public MainPage()
        {
            InitializeComponent();
            BindingContext = dataView = new DatabaseDisplay();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            dataView.OnPropertyChanged("GroupList");

            //Get permission for notification usage
            PermissionStatus status = await GetNotificationPermission();
        }

        //Check for notifications permissions, and request them if not granted
        public async Task<PermissionStatus> GetNotificationPermission()
        {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            if (status == PermissionStatus.Granted)
                return status;
            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // On iOS once a permission has been denied it may not be requested again from the application
                // In this case, ask the user to manually go and enable access
                await DisplayAlert("Warning", "You must manually enable notifications for this app in settings.",
                "OK");
                return status;
            }
            if (Permissions.ShouldShowRationale<Permissions.PostNotifications>())
            {
                //This check is true if they have denied it in the past, and you are requesting it again
                // Prompt the user with additional information as to why the permission is needed
                await DisplayAlert("Notifications", "Notifications will help remind you of tasks!", "OK");
            }
            //Display popup requesting permission
            status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            return status;
        }

        //Navigates to the task selection menu
        void OnTap(object sender, TappedEventArgs e)
        {
            var element = (View)sender;
            var targetName = (TaskGroup)element.BindingContext;
            //Parses Id for later comparisons amd filtering
            Shell.Current.Navigation.PushAsync(new MainTaskPage(targetName.Id));
        }

        //Deletes the group element, and database entry swiped left on
        async void OnLeftSwipe(object sender, SwipedEventArgs e)
        {
            var element = (View)sender;
            var nameToDelete = (TaskGroup)element.BindingContext;
            bool answer = await DisplayAlert("Delete", "Would you like to delete " + nameToDelete.Name + "?", "Yes", "No");

            if (answer == true)
            {
                dataView.DeleteGroupTasks(nameToDelete.Id);
                dataView.GroupDelete(nameToDelete);
            }
        }

        //Navigates to the add task group screen
        private void OnAddTaskGroupClicked(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PushAsync(new AddTaskGroup());
        }
    }

}
