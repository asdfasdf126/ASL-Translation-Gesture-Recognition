using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SeniorProject
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
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
            for(int x = 0; x < dataGrid.Items.Count; x++)
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
                gd.Percent = percent;
                dataGrid.Items.Refresh();
            }              
        }
    }
}
