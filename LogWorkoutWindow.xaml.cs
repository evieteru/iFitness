using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iFitness
{
    /// <summary>
    /// Interaction logic for LogWorkoutWindow.xaml
    /// </summary>
    public partial class LogWorkoutWindow : Window
    {
        private Workout _workout;
        public string WorkoutStatus { get; private set; } = "Not specified";
        public string Notes { get; private set; } = "";
        public int Difficulty { get; private set; } = 1;

        public LogWorkoutWindow(Workout workout)
        {
            InitializeComponent();
            _workout = workout;

            // Populate the UI with the log details if they exist
            NotesTextBox.Text = _workout.Notes ?? "";
            DifficultyComboBox.SelectedIndex = (_workout.Difficulty ?? 1) - 1; // ComboBox is 0-indexed, so subtract 1

            // Set the radio buttons based on the workout status
            if (_workout.Status == "Completed")
                CompletedRadioButton.IsChecked = true;
            else if (_workout.Status == "Did not finish")
                DidNotFinishRadioButton.IsChecked = true;
            else if (_workout.Status == "Did not start")
                DidNotStartRadioButton.IsChecked = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save status
            if (CompletedRadioButton.IsChecked == true)
                _workout.Status = "Completed";
            else if (DidNotFinishRadioButton.IsChecked == true)
                _workout.Status = "Did not finish";
            else if (DidNotStartRadioButton.IsChecked == true)
                _workout.Status = "Did not start";

            // Save difficulty
            var selectedDifficulty = DifficultyComboBox.SelectedItem as ComboBoxItem;
            if (selectedDifficulty != null)
            {
                _workout.Difficulty = int.Parse(selectedDifficulty.Content.ToString());
            }

            // Save notes
            _workout.Notes = NotesTextBox.Text.Trim();

            // Close the window
            DialogResult = true; //success
            Close();
        }

    }
}
