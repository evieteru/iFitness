using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iFitness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<DateTime, Workout> workoutByDate = new();
        public MainWindow()
        {
            InitializeComponent();
            WorkoutCalendar.SelectedDate = DateTime.Today;
            UpdateTodayPanel();
            UpdateWeeklyView();
        }




        private void WorkoutCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTodayPanel();
            UpdateWeeklyView();
        }

        private void UpdateTodayPanel()
        {
            DateTime selectedDate = WorkoutCalendar.SelectedDate ?? DateTime.Today; //selectedDate is date selected on calendar, if none selected, then it's today

            //Set the today label to show selected date
            if (selectedDate.Date == DateTime.Today) //if selected date is current date
            {
                TodayLabel.Text = "Today";
            }
            else
            {
                TodayLabel.Text = $"{selectedDate:dddd, MMMM d, yyyy}"; //Format: Thursday, April 18th, 2025
            }

            //Update the workout title and summary
            if (workoutByDate.TryGetValue(selectedDate.Date, out var workout))
            {
                WorkoutTitleText.Text = workout.Description;
                WorkoutCompletedText.Text = workout.Summary;
            }
            else
            {
                WorkoutTitleText.Text = "No workout scheduled";
                WorkoutCompletedText.Text = "";
            }
        }

        private void UpdateWeeklyView()
        {
            //Start of the week (Monday)
            DateTime baseDate = WorkoutCalendar.SelectedDate ?? DateTime.Today;
            int diff = DayOfWeek.Monday - baseDate.DayOfWeek;
            if (diff > 0) diff -= 7; //Handle Sunday correctly
            DateTime monday = baseDate.AddDays(diff);

            //Update label to display date ranges for new week
            WeekLabel.Text = $"This Week: {monday:MMM d} – {monday.AddDays(6):MMM d}";


            // Loop over the 7 days
            for (int i = 0; i < 7; i++)
            {
                DateTime day = monday.AddDays(i); //Get to proper day
                
                string summary = workoutByDate.TryGetValue(day.Date, out var workout)
                    ? $"{workout.Type}: {workout.Description}"
                    : "No workout";

                //Update workout label text
                var workoutText = FindName($"Day{i}WorkoutText") as TextBlock;
                if (workoutText != null)
                {
                    workoutText.Text = summary;
                }

                //Update the day label (Mon, Tue, etc.)
                var labelText = FindName($"Day{i}Label") as TextBlock;
                if (labelText != null)
                {
                    labelText.Text = day.ToString("ddd");
                }

            }

        }

        private void AddWorkout_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddWorkoutWindow();
            if (addWindow.ShowDialog() == true) // if saved
                                                // ShowDialog() returns true if user clicked save/confirm, returns false otherwise indicating cancel
            {
                var workout = addWindow.WorkoutResult; //After user hits save, gets the resulting workout object back, stores it
                if (workout != null)
                {
                    workout.Date = WorkoutCalendar.SelectedDate ?? DateTime.Today; //set date
                    workoutByDate[workout.Date] = workout; //add to dictionary
                    UpdateTodayPanel();
                    UpdateWeeklyView();
                }

            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = WorkoutCalendar.SelectedDate ?? DateTime.Today;

            if(workoutByDate.TryGetValue(selectedDate.Date, out var workout))
            {
                MessageBox.Show($"Workout Details:\n\n{workout.Description}\n{workout.Summary}", "Workout Info");
            }
            else
            {
                MessageBox.Show("No workout for today");
            }
        }

        private void LogWorkout_Click(object sender, RoutedEventArgs e)
        {
            var logWindow = new LogWorkoutWindow();
            logWindow.ShowDialog(); // I think this just makes it pop up which idk if thats what we want yet????
        }

        private void PreviousWeek_Click(object sender, RoutedEventArgs e)
        {
            WorkoutCalendar.SelectedDate = WorkoutCalendar.SelectedDate?.AddDays(-7); //Currently selected date and subtract 7 days
            UpdateWeeklyView();
        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        { 
            WorkoutCalendar.SelectedDate = WorkoutCalendar.SelectedDate?.AddDays(7);
            UpdateWeeklyView();
        }

    }
}