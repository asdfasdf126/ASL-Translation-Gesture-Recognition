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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeniorProject
{
    /// <summary>
    /// Interaction logic for GestureWatch.xaml
    /// </summary>
    public partial class GestureWatch : UserControl
    {
        public string Name { get; set; }
        public float Percent { get; set; }

        public GestureWatch(string name)
        {
            InitializeComponent();

            Name = name;
            Percent = 0;
        }

        public void setPercent(string percent)
        {
            lblPercent.Content = percent;
            Percent = float.Parse(percent);
        }
    }
}
