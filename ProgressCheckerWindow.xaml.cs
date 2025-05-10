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
    /// Interaction logic for ProgressCheckerWindow.xaml
    /// </summary>
    public partial class ProgressCheckerWindow : Window
    {
        private readonly List<Workout> _allWorkouts; // Original list of all workouts
        private ObservableCollection<Workout> _displayedWorkouts; // Workouts currently shown in ListView

        public ProgressCheckerWindow(IEnumerable<Workout> workouts)
        {
            InitializeComponent();
            _allWorkouts = workouts?.ToList() ?? new List<Workout>();
            _displayedWorkouts = new ObservableCollection<Workout>();
            WorkoutsListView.ItemsSource = _displayedWorkouts;

            // Initialize controls
            SortByComboBox.SelectedIndex = 0; // Default sort by Date
            SortDirectionComboBox.SelectedIndex = 0; // Default sort Ascending

            PopulateYearComboBox();
            ApplyFiltersAndSort(); // Initial load
        }

        // Populate the YearComboBox with distinct years from the workouts
        private void PopulateYearComboBox()
        {
            var years = _allWorkouts
                .Select(w => w.Date.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            YearComboBox.ItemsSource = years;
            if (years.Any())
            {
                YearComboBox.SelectedItem = years.Contains(DateTime.Now.Year) ? DateTime.Now.Year : years.First();
            }
            else
            {
                // Add current year if no workouts exist, or handle as needed
                YearComboBox.ItemsSource = new List<int> { DateTime.Now.Year };
                YearComboBox.SelectedIndex = 0;
            }
        }

        private void ApplyFiltersAndSort()
        {
            if (_allWorkouts == null) return;

            IEnumerable<Workout> filteredWorkouts = _allWorkouts;

            //Apply Search Filter
            string searchTerm = SearchTextBox.Text.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredWorkouts = filteredWorkouts.Where(w =>
                    (w.Description?.ToLower().Contains(searchTerm) ?? false) ||
                    (w.Notes?.ToLower().Contains(searchTerm) ?? false) ||
                    (w.Type.ToString().ToLower().Contains(searchTerm))
                );
            }

            //Apply Sorting
            string sortBy = (SortByComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Date";
            bool ascending = (SortDirectionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() == "Ascending";

            switch (sortBy)
            {
                case "Name":
                    filteredWorkouts = ascending ? filteredWorkouts.OrderBy(w => w.Description) : filteredWorkouts.OrderByDescending(w => w.Description);
                    break;
                case "Status":
                    // Handle null or empty status by putting them at the end or beginning consistently
                    filteredWorkouts = ascending ?
                        filteredWorkouts.OrderBy(w => string.IsNullOrEmpty(w.Status) ? "ZZZ" : w.Status) :
                        filteredWorkouts.OrderByDescending(w => string.IsNullOrEmpty(w.Status) ? "" : w.Status);
                    break;
                case "Type":
                    filteredWorkouts = ascending ? filteredWorkouts.OrderBy(w => w.Type.ToString()) : filteredWorkouts.OrderByDescending(w => w.Type.ToString());
                    break;
                case "Date":
                default:
                    filteredWorkouts = ascending ? filteredWorkouts.OrderBy(w => w.Date) : filteredWorkouts.OrderByDescending(w => w.Date);
                    break;
            }

            _displayedWorkouts.Clear();
            foreach (var workout in filteredWorkouts)
            {
                _displayedWorkouts.Add(workout);
            }

            UpdateGraph(); //Update graph whenever filters/sorts change
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void SortDirectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGraph();
        }

        private void UpdateGraph()
        {
            GraphCanvas.Children.Clear(); //Clear previous graph elements

            if (YearComboBox.SelectedItem == null || !_allWorkouts.Any())
            {
                // Display a message if no year selected or no data
                TextBlock noDataText = new TextBlock
                {
                    Text = "Select a year to view graph or no workout data available.",
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Canvas.SetLeft(noDataText, GraphCanvas.ActualWidth / 2 - noDataText.ActualWidth / 2); // Requires ActualWidth to be available
                Canvas.SetTop(noDataText, GraphCanvas.ActualHeight / 2 - noDataText.ActualHeight / 2);
                // Just return if canvas size isn't ready or no data
                if (GraphCanvas.ActualWidth == 0 || GraphCanvas.ActualHeight == 0) return;

                GraphCanvas.Children.Add(noDataText);
                return;
            }

            int selectedYear = (int)YearComboBox.SelectedItem;

            // Filter workouts by selected year and status
            var completedWorkoutsByMonth = _allWorkouts
                .Where(w => w.Date.Year == selectedYear && "Completed".Equals(w.Status, StringComparison.OrdinalIgnoreCase))
                .GroupBy(w => w.Date.Month)
                .ToDictionary(g => g.Key, g => g.Count());

            DrawBarGraph(completedWorkoutsByMonth);
        }

        // Draw the bar graph based on the monthly data
        private void DrawBarGraph(Dictionary<int, int> monthlyData)
        {
            GraphCanvas.Children.Clear(); // Ensure canvas is clear

            double canvasHeight = GraphCanvas.ActualHeight;
            double canvasWidth = GraphCanvas.ActualWidth;

            if (canvasHeight <= 20 || canvasWidth <= 50 || !monthlyData.Any()) // Basic check for drawable area and data
            {
                TextBlock noDataMsg = new TextBlock { Text = !monthlyData.Any() && YearComboBox.SelectedItem != null ? "No completed workouts for this year." : "Graph area too small or no data. Try sorting data!", Foreground = Brushes.DarkGray };
                GraphCanvas.Children.Add(noDataMsg);
                Canvas.SetLeft(noDataMsg, 10);
                Canvas.SetTop(noDataMsg, 10);
                return;
            }

            int maxCount = 0;
            if (monthlyData.Values.Any()) // Check if there are any values before calling Max
            {
                maxCount = monthlyData.Values.Max();
            }
            if (maxCount == 0) maxCount = 5; // Default max Y-axis if no completed workouts, to draw an empty graph

            double barWidth = (canvasWidth - 60) / 12; // 60 for padding/labels
            double XaxisYPosition = canvasHeight - 30; // Position of X-axis line and labels
            double YaxisXPosition = 35; // Position of Y-axis line and labels

            // Draw X-axis line
            Line xAxisLine = new Line { X1 = YaxisXPosition, Y1 = XaxisYPosition, X2 = canvasWidth - 10, Y2 = XaxisYPosition, Stroke = Brushes.Black, StrokeThickness = 1 };
            GraphCanvas.Children.Add(xAxisLine);

            // Draw Y-axis line
            Line yAxisLine = new Line { X1 = YaxisXPosition, Y1 = XaxisYPosition, X2 = YaxisXPosition, Y2 = 10, Stroke = Brushes.Black, StrokeThickness = 1 };
            GraphCanvas.Children.Add(yAxisLine);

            // Y-axis labels (e.g., 0, maxCount/2, maxCount)
            for (int i = 0; i <= 2; i++)
            {
                double val = (maxCount / 2.0) * i;
                TextBlock yLabel = new TextBlock { Text = val.ToString("F0"), FontSize = 9, TextAlignment = TextAlignment.Right, Width = YaxisXPosition - 5 };
                double yPos = XaxisYPosition - (val / maxCount * (XaxisYPosition - 20)); // 20 for top margin
                if (val > maxCount) yPos = XaxisYPosition - (XaxisYPosition - 20); // Cap at maxCount drawing position
                if (yPos < 10) yPos = 10;

                // Set position for Y-axis label
                Canvas.SetTop(yLabel, yPos - (yLabel.FontSize / 2)); 
                Canvas.SetLeft(yLabel, 0);
                GraphCanvas.Children.Add(yLabel);
            }


            string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            for (int month = 1; month <= 12; month++)
            {
                int count = monthlyData.ContainsKey(month) ? monthlyData[month] : 0;
                double barHeight = 0;
                if (maxCount > 0) // Avoid division by zero if no completed workouts
                {
                    barHeight = (double)count / maxCount * (XaxisYPosition - 20); // 20 for top margin
                }


                Rectangle bar = new Rectangle
                {
                    Width = Math.Max(5, barWidth * 0.7), //Ensure bar has some width, 70% of available slot
                    Height = barHeight,
                    Fill = Brushes.SteelBlue,
                    ToolTip = $"{monthNames[month - 1]}: {count} completed"
                };

                double xPos = YaxisXPosition + (month - 1) * barWidth + (barWidth * 0.15); // Center the bar in it's slot
                Canvas.SetLeft(bar, xPos);
                Canvas.SetTop(bar, XaxisYPosition - barHeight);
                GraphCanvas.Children.Add(bar);

                // Month Label
                TextBlock monthLabel = new TextBlock
                {
                    Text = monthNames[month - 1],
                    FontSize = 10,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth
                };
                Canvas.SetLeft(monthLabel, xPos - (barWidth * 0.15)); // Align with start of bar slot
                Canvas.SetTop(monthLabel, XaxisYPosition + 5);
                GraphCanvas.Children.Add(monthLabel);
            }
        }

        // Call UpdateGraph when the window size changes to redraw the graph correctly
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
            UpdateGraph();
        }
    }
}
