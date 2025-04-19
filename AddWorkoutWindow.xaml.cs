using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AddWorkout.xaml
    /// Add workout and button to add sets, dynamically change display based on workout type
    /// Select sets to edit and delete sets
    /// </summary>
    public partial class AddWorkoutWindow : Window
    {
        //Final workout that gets passed to MainWindow
        public Workout WorkoutResult { get; private set; }

        //Instances of each workout type (make both so you can switch)
        private CardioWorkout _cardioWorkout = new CardioWorkout { Type = WorkoutType.Cardio };
        private StrengthWorkout _strengthWorkout = new StrengthWorkout { Type = WorkoutType.Strength };

        //returns the currently selected workout based on dropdown
        private Workout SelectedWorkout => WorkoutTypeComboBox.SelectedIndex == 0 ? _cardioWorkout : _strengthWorkout;

        public AddWorkoutWindow()
        {
            InitializeComponent();
            WorkoutTypeComboBox.SelectedIndex = 0; //default to cardio
            SetList.ItemsSource = _cardioWorkout.Sets; //Show any sets cardio has
        }

        //When user changes workout type in dropdown
        private void WorkoutTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedWorkout();
        }

        //Update which set list is shown depending on which workout type is selected
        private void UpdateSelectedWorkout()
        {
            var selected = (WorkoutTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (selected == "Cardio")
            {
                SetList.ItemsSource = _cardioWorkout.Sets;
            }
            else if (selected == "Strength")
            {
                SetList.ItemsSource = _strengthWorkout.Sets;
            }
        }


        //Adds a new set (either cardio or strength) to the selected workout
        private void AddSet_Click(object sender, RoutedEventArgs e)
        {
            var selected = (WorkoutTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (selected == "Cardio")
            {
                //Add one CardioSet with a default row
                _cardioWorkout.Sets.Add(new CardioSet
                {
                    Rows = new ObservableCollection<CardioSetRow>
                    {
                        new CardioSetRow { Distance = "0 miles", Time = "0 mins", Note="" }
                    }
                }); 
            }
            else if (selected == "Strength")
            {
                //Add one StrengthSet with a default row
                _strengthWorkout.Sets.Add(new StrengthSet
                {
                    Rows = new ObservableCollection<StrengthSetRow>
                    {
                        new StrengthSetRow { Exercise = "Push-ups", Reps = 10, Weight = "Bodyweight" }
                    }
                });
            }

            SetList.Items.Refresh(); //update display
        }


        //Called when Edit button for a set is clicked
        private void EditSet_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button; //Cast to button
            var set = button?.Tag; //if button is not null, assign it's value to set
                                   //wont crash if button is null
                                   //tag is used to attach any data to the WPF control

            if (set is StrengthSet strengthSet)
            {
                //Later
                MessageBox.Show("Strength editing coming soon.");
            }
            else if (set is CardioSet cardioSet)
            {
                //Later
                MessageBox.Show("Cardio editing coming soon.");
            }

            SetList.Items.Refresh();
        }

        //Called when Delete button is clicked
        private void DeleteSet_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var set = button?.Tag;

            //Ask for deletion confirmation
            var result = MessageBox.Show(
                "Are you sure you want to delete this set?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            //If user cancelled, do nothing
            if (result == MessageBoxResult.No)
            {
                return;
            }

            //If user confirmed, remove the set from the workout
            if (set is StrengthSet strengthSet)
            {
                _strengthWorkout.Sets.Remove(strengthSet);
            }
            else if (set is CardioSet cardioSet)
            {
                _cardioWorkout.Sets.Remove(cardioSet);
            }

            SetList.Items.Refresh();
        }

        //Called when Save button clicked 
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string description = DescriptionBox.Text.Trim(); //assign description 

            if (string.IsNullOrWhiteSpace(description)) //if there is no description
            {
                MessageBox.Show("Please enter a description for the workout.");
                return;
            }

            var selected = (WorkoutTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString(); //get workout type from the dropdown

            if (selected == "Cardio")
            {
                _cardioWorkout.Description = description; //assign description to workout
                WorkoutResult = _cardioWorkout; //set the workout to be sent to main window as this workout
            }
            else if (selected == "Strength")
            {
                _strengthWorkout.Description = description;
                WorkoutResult = _strengthWorkout;
            }

            DialogResult = true; //signals that user successfully saved (rather than cancel or close)
            Close(); //Close the AddWorkoutWindow
        }



    }
}
