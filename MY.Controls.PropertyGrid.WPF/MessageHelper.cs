using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MY.Controls.WPF
{
    public class MessageHelper
    {
        public static void ShowMessage(string sMsg)
        {
            MessageBox.Show(sMsg);
        }
    }
}
