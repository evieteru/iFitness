using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Diagnostics;

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
            LoadWorkoutsFromJson();
            //Debug print for developers/graders
            foreach (var kvp in workoutByDate)
            {
                Debug.WriteLine($"{kvp.Key:d}: {kvp.Value.Description} ({kvp.Value.Type})");
            }


            WorkoutCalendar.SelectedDate = DateTime.Today;
            UpdateTodayPanel();
            UpdateWeeklyView();
        }




        private void WorkoutCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTodayPanel();
            UpdateWeeklyView();
            UpdateWeeklyReport();
        }




        //Update UI ===============================================================================================================================================================
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
                LogWorkoutButton.IsEnabled = true; //Enable the log button if there is a workout
            }
            else
            {
                WorkoutTitleText.Text = "No workout scheduled";
                WorkoutCompletedText.Text = "";
                LogWorkoutButton.IsEnabled = false;
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

        //Update the weekly report
        private void UpdateWeeklyReport()
        {
            DateTime baseDate = WorkoutCalendar.SelectedDate ?? DateTime.Today;
            int diff = (int)DayOfWeek.Monday - (int)baseDate.DayOfWeek;
            if (diff > 0) diff -= 7; // Ensure Monday is the start, even if baseDate is Sunday
            DateTime monday = baseDate.AddDays(diff).Date;
            DateTime sunday = monday.AddDays(6).Date;

            // Date Range
            WeeklyReportDateRangeText.Text = $"Week: {monday:MMM d} - {sunday:MMM d, yyyy}";

            List<Workout> workoutsThisWeek = new List<Workout>();
            for (int i = 0; i < 7; i++)
            {
                if (workoutByDate.TryGetValue(monday.AddDays(i), out var workout))
                {
                    workoutsThisWeek.Add(workout);
                }
            }

            // Days with Workouts
            int daysWithWorkoutsCount = workoutsThisWeek.Select(w => w.Date.Date).Distinct().Count();
            WeeklyReportDaysWithWorkoutsText.Text = $"Active Days: {daysWithWorkoutsCount}/7";

            // Completion Rate
            int scheduledWorkoutsCount = workoutsThisWeek.Count;
            int completedWorkoutsCount = workoutsThisWeek.Count(w => "Completed".Equals(w.Status, StringComparison.OrdinalIgnoreCase));
            double completionRate = 0;
            if (scheduledWorkoutsCount > 0)
            {
                completionRate = ((double)completedWorkoutsCount / scheduledWorkoutsCount) * 100;
            }
            WeeklyReportCompletionRateText.Text = $"Completion: {completionRate:F0}% ({completedWorkoutsCount}/{scheduledWorkoutsCount})";

            // Weekly Grade
            int score = 0;
            if (scheduledWorkoutsCount > 0) // Only calculate grade if workouts were scheduled
            {
                bool hasCardio = workoutsThisWeek.Any(w => w.Type == WorkoutType.Cardio);
                bool hasStrength = workoutsThisWeek.Any(w => w.Type == WorkoutType.Strength);

                // Variety Bonus
                if (hasCardio && hasStrength)
                {
                    score += 1;
                }

                // Completion Bonus (Volume)
                score += Math.Min(completedWorkoutsCount, 6);

                // Perfection Bonus
                if (completionRate == 100.0 && scheduledWorkoutsCount > 0) // Ensure it's truly 100% of scheduled
                {
                    score += 1;
                }
                score = Math.Min(score, 8); // Cap score at 8
            }

            string letterGrade = GetLetterGrade(score, scheduledWorkoutsCount > 0);
            WeeklyReportGradeText.Text = $"Grade: {letterGrade}";
        }


        // Get corresponding grade for calculated weekly score
        private string GetLetterGrade(int score, bool workoutsScheduled)
        {
            if (!workoutsScheduled) return "N/A (No Workouts)";

            return score switch
            {
                8 => "A",
                7 => "B+",
                6 => "B",
                5 => "B-",
                4 => "C+",
                3 => "C",
                2 => "C-",
                1 => "D",
                _ => "F", // Covers 0 and any unexpected lower values
            };
        }



        //Buttons==========================================================================================================================================
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
                    SaveWorkoutsToJson();
                }

            }
        }

        private void DeleteWorkout_Click(object sender, RoutedEventArgs e) // This is a new example method
        {
            DateTime selectedDate = WorkoutCalendar.SelectedDate ?? DateTime.Today;
            if (workoutByDate.ContainsKey(selectedDate.Date))
            {
                var workoutDescription = workoutByDate[selectedDate.Date].Description;
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the workout \"{workoutDescription}\" for {selectedDate:d}?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    workoutByDate.Remove(selectedDate.Date);
                    UpdateTodayPanel();
                    UpdateWeeklyView();
                    SaveWorkoutsToJson();
                    MessageBox.Show("Workout deleted.", "Success");
                }
            }
            else
            {
                MessageBox.Show("No workout scheduled on this date to delete.", "Deletion Failed");
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = WorkoutCalendar.SelectedDate ?? DateTime.Today;

            if (workoutByDate.TryGetValue(selectedDate.Date, out var workout))
            {
                var sb = new StringBuilder();

                sb.AppendLine($"Workout: {workout.Description}");
                sb.AppendLine($"Type: {workout.Type}");
                sb.AppendLine($"Summary: {workout.Summary}");
                sb.AppendLine();

                // Show sets and rows depending on type
                if (workout is CardioWorkout cardio)
                {
                    int setIndex = 1;
                    foreach (var set in cardio.Sets)
                    {
                        sb.AppendLine($"Set {setIndex++}: {set.Name} (Reps: {set.SetReps})");
                        foreach (var row in set.Rows)
                        {
                            sb.AppendLine($"  - {row.Distance}, {row.Time}, {row.Note}");
                        }
                    }
                }
                else if (workout is StrengthWorkout strength)
                {
                    int setIndex = 1;
                    foreach (var set in strength.Sets)
                    {
                        sb.AppendLine($"Set {setIndex++}: {set.Name} (Reps: {set.SetReps})");
                        foreach (var row in set.Rows)
                        {
                            sb.AppendLine($"  - {row.Exercise}, {row.Reps} reps, {row.Weight}");
                        }
                    }
                }
  
                MessageBox.Show(sb.ToString(), "Workout Info");
            }
            else
            {
                MessageBox.Show("No workout for today");
            }
        }

        private void LogWorkout_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = WorkoutCalendar.SelectedDate ?? DateTime.Today; // if no date selected default to today

            //Check if there is a workout for the selected date
            if (workoutByDate.TryGetValue(selectedDate.Date, out var workout))
            {
                var logWindow = new LogWorkoutWindow(workout); //pass the workout to log workout
                if (logWindow.ShowDialog() == true)
                {
                    // After saving, refresh
                    UpdateTodayPanel();
                    UpdateWeeklyView();
                    SaveWorkoutsToJson();
                }
            }
            else
            {
                MessageBox.Show("No workout to log for this day.");
            }
        }

        private void ViewProgressButton_Click(object sender, RoutedEventArgs e)
        {
            //This is a display window, so pass a copy
            //We don't want the workouts to be modified fron ProgressChccker
            var allWorkoutsCopy = workoutByDate.Values.ToList();

            ProgressCheckerWindow progressWindow = new ProgressCheckerWindow(allWorkoutsCopy);
            progressWindow.Owner = this; // Optional: Sets the owner of the new window
            progressWindow.Show(); // Use Show() for a non-modal window, or ShowDialog() for a modal one
        }












        //Calendar Navigation ==========================================================================================================================
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



        //JSON File ==========================================================================================================================================

        //Load the workouts from a JSON file when the application starts
        private void LoadWorkoutsFromJson()
        {
            string path = "sample_workouts.json"; // Path to the JSON file
            if (!File.Exists(path)) // Check if the file exists
            {
                // If the file doesn't exist, show a MessageBox
                MessageBox.Show($"Workout data file not found: {path}.",
                                "File Not Found",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return; //Exit the method since there's nothing to load
            }

            try
            {
                string json = File.ReadAllText(path);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                var rawWorkouts = JsonSerializer.Deserialize<List<JsonElement>>(json, options);

                // Deserialize each workout based on its type
                foreach (var element in rawWorkouts)
                {
                    var type = element.GetProperty("Type").GetString();

                    Workout workout = type switch
                    {
                        "Cardio" => element.Deserialize<CardioWorkout>(options),
                        "Strength" => element.Deserialize<StrengthWorkout>(options),
                        // ...
                        _ => null
                    };

                    // Set the date and add to the dictionary
                    if (workout != null)
                    {
                        workoutByDate[workout.Date.Date] = workout;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load workouts: {ex.Message}", "Load Error");
            }
        }

        // Save the workouts to a JSON file when the application closes or when a workout is added/modified
        private void SaveWorkoutsToJson()
        {
            string path = "sample_workouts.json"; //
            try
            {
                // Convert the dictionary values to a list of Workout objects
                List<Workout> workoutsToSave = workoutByDate.Values.ToList();

                /*Handles WorkoutType enum serialization                */
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true, // Makes the JSON file human-readable
                    Converters = { new JsonStringEnumConverter() },
                };

                string json = JsonSerializer.Serialize<List<Workout>>(workoutsToSave, options);
                File.WriteAllText(path, json); //
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save workouts: {ex.Message}", "Save Error");
            }
        }



    }
}