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
    /// Interaction logic for EditSetWindow.xaml
    /// </summary>
    public partial class EditSetWindow : Window
    {
        //private StrengthSet _strengthSet; // testing on strength first
        //private CardioSet _cardioSet;
        //private MindBodySet = _mindbodySet;



        public EditSetWindow(Object set)
        {
            InitializeComponent();

            if(set is StrengthSet strengthSet)
            {
                DataContext = strengthSet;
                RowGrid.ItemsSource = strengthSet.Rows;
                GenerateColumnsForStrength();
            }
            else if(set is CardioSet cardioSet)
            {
                DataContext = cardioSet;
                RowGrid.ItemsSource = cardioSet.Rows;
                GenerateColumnsForCardio();

            }
            else if(set is MindBodySet mindbodySet)
            {
                DataContext = mindbodySet;
                RowGrid.ItemsSource = mindbodySet.Rows;
                GenerateColumnsForMindBody();

            }
  
        }

        private void Save_Click(object sender, RoutedEventArgs e) //
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) //
        {
            DialogResult = false;
            Close();
        }


        // below is just the names of the headers for each to generate because they have different headers in each
        // Don't know if there was an easier approach to this or not since I couldn't do it in the xmal file 
        // I think i fucked this part up because I wasn't sure if I could call something else that is holding these values
        private void GenerateColumnsForStrength() // Keep 
        {
            RowGrid.Columns.Clear();
            RowGrid.Columns.Add(new DataGridTextColumn { Header = "Exercise", Binding = new Binding("Exercise") });
            RowGrid.Columns.Add(new DataGridTextColumn { Header = "Reps", Binding = new Binding("Reps") });
            RowGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
        }

        private void GenerateColumnsForCardio()
        {
            RowGrid.Columns.Clear();
            RowGrid.Columns.Add(new DataGridTextColumn { Header = "Distance", Binding = new Binding("Distance") });
            RowGrid.Columns.Add(new DataGridTextColumn { Header = "Time", Binding = new Binding("Time") });
            RowGrid.Columns.Add(new DataGridTextColumn { Header = "Note", Binding = new Binding("Note") });
        }

        private void GenerateColumnsForMindBody()
        {
            RowGrid.Columns.Clear();
            RowGrid.Columns.Add(new DataGridTextColumn { Header = "Exercise", Binding = new Binding("Exercise") });
            RowGrid.Columns.Add(new DataGridTextColumn { Header = "Time", Binding = new Binding("Time") });
            RowGrid.Columns.Add(new DataGridTextColumn { Header = "Note", Binding = new Binding("Note") });
        }

        private void RowGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        /*
        public EditSetWindow()
        {
            InitializeComponent();
        }
        */

    }
}
