using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MY.Controls.Demo
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
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

        private object _VPropertyGrid = null;
        public object VPropertyGrid
        {
            get
            {
                return _VPropertyGrid;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _VPropertyGrid, value);
            }
        }

        private object _VPropertyGrid1 = null;
        public object VPropertyGrid1
        {
            get
            {
                return _VPropertyGrid1;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _VPropertyGrid1, value);
            }
        }

        private object _VPropertyGrid2 = null;
        public object VPropertyGrid2
        {
            get
            {
                return _VPropertyGrid2;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _VPropertyGrid2, value);
            }
        }
    }
}
