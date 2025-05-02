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
        public string WorkoutStatus { get; private set; } = "Not specified";
        public string Notes { get; private set; } = "";
        public int Difficulty { get; private set; } = 1;

        public LogWorkoutWindow()
        {
            InitializeComponent();
        }

    }
}
