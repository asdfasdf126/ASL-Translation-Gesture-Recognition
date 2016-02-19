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
    /// Interaction logic for DebugUC.xaml
    /// </summary>
    public partial class DebugUC : UserControl
    {
        public DebugUC()
        {
            InitializeComponent();

            dataGrid.AutoGenerateColumns = false;

            DataGridTextColumn nameCol = new DataGridTextColumn();
            Binding nameBind = new Binding("Name");
            nameCol.Binding = nameBind;
            nameCol.Header = "Gesture Name";

            DataGridTextColumn percentCol = new DataGridTextColumn();
            Binding percentBind = new Binding("Percent");
            percentCol.Binding = percentBind;
            percentCol.Header = "Confidence level";

            dataGrid.Columns.Add(nameCol);
            dataGrid.Columns.Add(percentCol);

            dataGrid.FontSize = 32;
            dataGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            this.SizeChanged += DebugUC_SizeChanged;
        }

        private void DebugUC_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            dataGrid.Height = this.Height;
            dataGrid.Width = this.Width;
        }

        public void addGesture(string gestureName)
        {
            if (contains(gestureName) != -1)
                return;

            dataGrid.Items.Add(new GestureData() { Name = gestureName, Percent = 0 });
        }

        private int contains(string name)
        {
            for (int x = 0; x < dataGrid.Items.Count; x++)
            {
                GestureData gd = (GestureData)dataGrid.Items[x];

                if (gd.Name.Equals(name))
                    return x;
            }

            return -1;
        }

        public void reset()
        {
            for (int x = 0; x < dataGrid.Items.Count; x++)
                ((GestureData)dataGrid.Items[x]).Percent = 0;
        }

        public void setPercent(string name, float percent)
        {
            int pos = contains(name);

            if (pos == -1)
                return;

            GestureData gd = (GestureData)dataGrid.Items[contains(name)];
            
            //if (gd.Percent < percent)
            {
                gd.Percent = (float)Math.Round(percent,2);
                dataGrid.Items.Refresh();
            }
        }
    }
}
