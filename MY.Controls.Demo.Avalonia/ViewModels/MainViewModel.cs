using Avalonia.Controls;
using ReactiveUI;

namespace MY.Controls.Demo.Ava.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting { get; set; } = "fasdfas";

    public MainViewModel()
    {
            InitializeAllControls();
    }

    private void InitializeAllControls()
    {
        // PropertyGrid
        PropertiesContainer propertiesContainer = new PropertiesContainer();
        var ph = new PropertyGridHelper(propertiesContainer, "VmMain", "Props");
        VPropertyGrid = ph.GeneratePropertyGrid();

        var ph1 = new PropertyGridHelper(propertiesContainer, "VmMainSync", "Props");
        VPropertyGrid1 = ph1.GeneratePropertyGrid();

        PropertiesContainer propertiesContainerCopyTo = new PropertiesContainer();
        var ph2 = new PropertyGridHelper(propertiesContainerCopyTo, "VmMainCopyTo", "Props");
        VPropertyGrid2 = ph2.GeneratePropertyGrid();
    }

    private UserControl? vPropertyGrid;
    public UserControl? VPropertyGrid
    {
        get => vPropertyGrid;
        set => this.RaiseAndSetIfChanged(ref vPropertyGrid, value);
    }

    private UserControl? vPropertyGrid1;
    public UserControl? VPropertyGrid1
    {
        get => vPropertyGrid1;
        set => this.RaiseAndSetIfChanged(ref vPropertyGrid1, value);
    }

    private UserControl? vPropertyGrid2;
    public UserControl? VPropertyGrid2
    {
        get => vPropertyGrid2;
        set => this.RaiseAndSetIfChanged(ref vPropertyGrid2, value);
    }
}
