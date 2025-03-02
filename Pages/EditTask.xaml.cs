/*
 * A class to define functionality for the Edit Task page
 * Similar to the Add Task page, however only used when editing a pre-exisiting class
 */
using TaskCurator.Classes;

namespace TaskCurator;
public partial class EditTask : ContentPage
{
    int idTemp;
    string newFile;
    DatabaseDisplay dataView;

    public EditTask(int taskSelected)
	{
        InitializeComponent();

        idTemp = taskSelected;
        dataView = new DatabaseDisplay();
        dataView.SpecifiedTasks(idTemp);
        BindingContext = dataView.FilteredTasks[0];
    }

    //Check for camera permissions, and request them if not granted
    public async Task<PermissionStatus> GetCameraPermission()
    {
        PermissionStatus status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status == PermissionStatus.Granted)
            return status;
        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // On iOS once a permission has been denied it may not be requested again from the application
            // In this case, ask the user to manually go and enable access
            await DisplayAlert("Warning", "You must manually enable camera access for this app in settings.",
            "OK");
            return status;
        }
        if (Permissions.ShouldShowRationale<Permissions.Camera>())
        {
            //This check is true if they have denied it in the past, and you are requesting it again
            // Prompt the user with additional information as to why the permission is needed
            await DisplayAlert("Optional Images", "Grant permission if you wish to add images!", "OK");
        }
        //Display popup requesting permission
        status = await Permissions.RequestAsync<Permissions.Camera>();
        return status;
    }
    private async void AddImageFrame_Tapped(object sender, TappedEventArgs e)
    {
        FileResult photo = null;
        //Check whether we have permission using the function we added in the previous exercise
        PermissionStatus status = await GetCameraPermission();
        if (status != PermissionStatus.Granted)
            return;
        //If we can use the camera, take a photo. Otherwise allow the user to select an image from
        if (MediaPicker.Default.IsCaptureSupported && DeviceInfo.Platform != DevicePlatform.WinUI)
            photo = await MediaPicker.CapturePhotoAsync();
        else
            photo = await MediaPicker.PickPhotoAsync();
        //If the user successfully took/selected an image, save it and update the Image element to show it.
        if (photo != null)
        {
            //Check if images folder exists
            string imagesDir = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "images");
            System.IO.Directory.CreateDirectory(imagesDir);
            //Must save the image to a file before it can be displayed
            newFile = Path.Combine(imagesDir, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
            {
                await stream.CopyToAsync(newStream);
            }
            TaskImage.Source = newFile;
        }
    }
    private void OnUpdateTaskClicked(object sender, EventArgs e)
    {
        //Exception handling for invalid tasks
        if (TaskNameEntry.Text == null)
        {
            DisplayAlert("Names cannot be blank!", "Please create a name for this task.", "OK");
            return;
        }
        else
        {

            DateTime dueDateTime = new DateTime(DueDatePicker.Date.Year, DueDatePicker.Date.Month, DueDatePicker.Date.Day, DueTimePicker.Time.Hours, DueTimePicker.Time.Minutes, DueTimePicker.Time.Seconds);
            DateTime startDateTime = new DateTime(StartDatePicker.Date.Year, StartDatePicker.Date.Month, StartDatePicker.Date.Day, StartTimePicker.Time.Hours, StartTimePicker.Time.Minutes, StartTimePicker.Time.Seconds);

            if ((dueDateTime != startDateTime && dueDateTime > startDateTime) && (dueDateTime != DateTime.Now && dueDateTime > DateTime.Now))
            {
                string SelectedPriority;
                int SelectedRank;

                //Radio button handling, int value is for ordering purposes
                if (HighPriorityRadio.IsChecked)
                {
                    SelectedPriority = "Red";
                    SelectedRank = 1;
                }
                else if (MediumPriorityRadio.IsChecked)
                {
                    SelectedPriority = "Purple";
                    SelectedRank = 2;
                }
                else if (LowPriorityRadio.IsChecked)
                {
                    SelectedPriority = "Blue";
                    SelectedRank = 3;
                }
                else
                {
                    SelectedPriority = "White";
                    SelectedRank = 1;
                }

                if (newFile == null)
                {
                    newFile = dataView.FilteredTasks[0].ImageSource;
                }

                //Create new task object
                ToDoTask tDT = new()
                {
                    Name = TaskNameEntry.Text,
                    Description = TaskDescEntry.Text,
                    StartDT = startDateTime,
                    EndDT = dueDateTime,
                    Priority = SelectedPriority,
                    Rank = SelectedRank,
                    PrevPriority = SelectedPriority,
                    PrevRank = SelectedRank,
                    Reminders = RemindersChB.IsChecked,
                    FId = dataView.FilteredTasks[0].FId,
                    ImageSource = newFile
                };

                DatabaseService.SaveTask(tDT);
                dataView.TaskDelete(dataView.FilteredTasks[0]);

                //Notification for when the task starts
                DateTime notificationTime = tDT.StartDT.Add(tDT.StartDT.TimeOfDay);
                NotificationManager.SendNotification("Are You Ready?", "The task of " + tDT.Name + " begins now.", notificationTime);

                if (tDT.Reminders == true)
                {
                    //Notification to remind that a task is starting later
                    notificationTime = notificationTime.AddHours(-3);
                    NotificationManager.SendNotification("Task Starting Later!", tDT.Name + " should be started later.", notificationTime);
                }

                //Notification for when a task is due
                notificationTime = tDT.EndDT.Add(tDT.EndDT.TimeOfDay);
                NotificationManager.SendNotification("Task Due!", "The task of " + tDT.Name + " is due right now.", notificationTime);

                if (tDT.Reminders == true)
                {
                    //Notification to remind that a task is due later
                    notificationTime = notificationTime.AddHours(-3);
                    NotificationManager.SendNotification("Task Due Later!", tDT.Name + " should be finished for then.", notificationTime);
                }

                NotificationManager.ShowToast("The task of " + TaskNameEntry.Text + " was edited!");

                Shell.Current.Navigation.PopAsync();
                Shell.Current.Navigation.PopAsync();
            }
            else if (dueDateTime == DateTime.Now || dueDateTime < DateTime.Now)
            {
                DisplayAlert("Date/Time Error", "Due dates must be in the future!", "OK");
                return;
            }
            else
            {
                DisplayAlert("Date/Time Error", "Ensure tasks start before they're due!", "OK");
                return;
            }
        }
    }
}