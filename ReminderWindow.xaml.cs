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

using System.Text.RegularExpressions;


namespace iFitness
{
    public partial class ReminderWindow : Window
    {
        public string Email { get; private set; }
        public TimeSpan ReminderTime { get; private set; }

        public ReminderWindow()
        {
            InitializeComponent();
        }
        private void Schedule_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();

            // Validate email
            if (!Regex.IsMatch(email, @"^\S+@\S+\.\S+$"))
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            // Get the time input from the user
            string timeString = TimeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(timeString))
            {
                MessageBox.Show("Please enter a time for the reminder.");
                return;
            }

            // Try parsing the time with a specific format (e.g., "hh:mm tt" for 12-hour format)
            if (DateTime.TryParseExact(timeString, "h:mm tt",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedTime))
            {
                // Save email and reminder time
                Email = email;
                ReminderTime = parsedTime.TimeOfDay;
                DialogResult = true;  // Indicate that the dialog was successful
            }
            else
            {
                MessageBox.Show("Please enter a valid time in the format 'hh:mm AM/PM'.");
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
