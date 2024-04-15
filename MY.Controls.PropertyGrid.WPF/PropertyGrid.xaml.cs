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

namespace MY.Controls.WPF
{
    /// <summary>
    /// Interaction logic for PropertyGrid.xaml
    /// </summary>
    public partial class PropertyGrid : UserControl
    {
        public PropertyGrid()
        {
            InitializeComponent();
        }

        private void lstProperties_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                ListView lb = sender as ListView;

                if (lb == null) return;

                int iNext = lb.SelectedIndex + 1;
                if (iNext >= lb.Items.Count)
                {
                    iNext = 0;
                }
                if (GiveItemFocus(lb, iNext, typeof(TextBox)))
                {
                    e.Handled = true;
                }
            }
        }

        private bool GiveItemFocus(ListView lb, int index, Type descentdantType)
        {
            if (index >= lb.Items.Count || index < 0)
            {
                return false;
            }

            ListViewItem lbi = (ListViewItem)lb.ItemContainerGenerator.ContainerFromIndex(index);
            PropertyHelper cont = (PropertyHelper)lbi.Content;
            (cont.View as UserControl).Focus();
            return true;
        }

    }
}
