using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace MY.Controls.Ava;

public partial class PropertyGrid : UserControl
{
    public PropertyGrid()
    {
        InitializeComponent();
    }
    private void ListBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Tab)
        {
            ListBox lb = sender as ListBox;

            if (lb == null) return;
            int iNext = 0;
            if ((e.KeyModifiers & KeyModifiers.Shift) != KeyModifiers.Shift)
            {
                iNext = lb.SelectedIndex + 1;
                if (iNext >= lb.Items.Count)
                {
                    iNext = 0;
                }
            }
            else
            {
                iNext = lb.SelectedIndex - 1;
                if (iNext < 0)
                {
                    iNext = lb.Items.Count - 1;
                }
            }
            if (GiveItemFocus(lb, iNext, typeof(TextBox)))
            {
                e.Handled = true;
            }
        }
    }

    private bool GiveItemFocus(ListBox lb, int index, Type descentdantType)
    {
        if (index >= lb.Items.Count || index < 0)
        {
            return false;
        }

        ListBoxItem lbi = (ListBoxItem)lb.ItemContainerGenerator.ContainerFromIndex(index);
        PropertyHelper cont = (PropertyHelper)lbi.Content;
        (cont.View as TemplatedControl).Focus();
        return true;
    }
}