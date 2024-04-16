using DynamicData.Binding;
using MY.Controls.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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

            // LogViewer
            VLogViewer = new LogViewer();
            VmLogViewer = new LogViewerViewModel(10000, Dispatcher.CurrentDispatcher);
            (VLogViewer as UserControl).DataContext = VmLogViewer;
            VmLogViewer.LogEntries.ItemAdded += (s, e) =>
            {
                var v = (VLogViewer as LogViewer);
                v.LbLog.SelectedIndex = v.LbLog.Items.Count - 1;
                v.LbLog.ScrollIntoView(v.LbLog.SelectedItem);
            };
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

        private object _VLogViewer = null;
        public object VLogViewer
        {
            get
            {
                return _VLogViewer;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _VLogViewer, value);
            }
        }

        private LogViewerViewModel _VmLogViewer = null;
        public LogViewerViewModel VmLogViewer
        {
            get
            {
                return _VmLogViewer;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _VmLogViewer, value);
            }
        }

        private int _LogRate = 20000;
        public int LogRate
        {
            get
            {
                return _LogRate;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _LogRate, value);
            }
        }

        private bool _InLogging = false;
        public bool InLogging
        {
            get
            {
                return _InLogging;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _InLogging, value);
            }
        }

        private ICommand _StartLogCommand = null;
        public ICommand StartLogCommand
        {
            get
            {
                if (_StartLogCommand == null)
                {
                    _StartLogCommand = ReactiveCommand.Create(async () =>
                    {
                        try
                        {
                            InLogging = true;
                            await Task.Run(() =>
                            {
                                Random rd = new Random();
                                int iLogCount = 0;
                                while (InLogging)
                                {
                                    for (int i = 0; i < LogRate; i++)
                                    {
                                        VmLogViewer.AppendMsg($"Msg {iLogCount}: {rd.Next(100)}");
                                        iLogCount++;
                                    }
                                    System.Threading.Thread.Sleep(1000);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                        finally { InLogging = false; }
                    }, this.WhenAnyValue(x => x.InLogging, b => !b));
                }
                return _StartLogCommand;
            }
        }

        private ICommand _StopLogCommand = null;
        public ICommand StopLogCommand
        {
            get
            {
                if (_StopLogCommand == null)
                {
                    _StopLogCommand = ReactiveCommand.Create(() =>
                    {
                        InLogging = false;
                    }, this.WhenAnyValue(x => x.InLogging));
                }
                return _StopLogCommand;
            }
        }
    }
}
