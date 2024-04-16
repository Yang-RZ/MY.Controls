using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;


#if WPF
using System.Windows;
#endif

namespace MY.Controls
{

    public class LogViewerViewModel : ReactiveUI.ReactiveObject
    {
        private int MaxRowCount_;

#if AVALONIA
        private IClipboard clipboard_;
#endif


        public LogViewerViewModel()
        {
#if AVALONIA
            if (!Design.IsDesignMode)
            {
                throw new InvalidOperationException("Can't instantiate in DesignMode!");
            }
            LogEntries = new BufferedObservableCollection<string>(Dispatcher.UIThread) { "asdf", "asdfasd" };
#endif
#if WPF
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                throw new InvalidOperationException("Can't instantiate in DesignMode!");
            }
            LogEntries = new BufferedObservableCollection<string>(Dispatcher.CurrentDispatcher) { "asdf", "asdfasd" };
#endif
        }

#if AVALONIA
        public LogViewerViewModel(int iBufferSize, Dispatcher dispatcher, IClipboard clipboard)
        {
            clipboard_ = System.Windows.Clipboard;
#endif

#if WPF
        public LogViewerViewModel(int iBufferSize, Dispatcher dispatcher)
        {
#endif
            MaxRowCount = iBufferSize;
            LogEntries = new BufferedObservableCollection<string>(dispatcher);
            SelectedMsgs = new List<string>();
            //LogEntries.ItemAdded += LogEntries_ItemAdded;
        }

        private void LogEntries_ItemAdded(object? sender, EventArgs e)
        {
            //ItemAdded(this, (NLogEvent)((LogEventViewModel)sender).EventInfo);
        }

        public BufferedObservableCollection<string> LogEntries { get; private set; }

        public int MaxRowCount { get => MaxRowCount_; set => MaxRowCount_ = value; }

        public void AppendMsg(string? msg)
        {
            lock (this)
            {
                if (MaxRowCount > 0)
                    LogEntries.SetMaxLimit(MaxRowCount);
                LogEntries.AddToBuffer(msg ?? "null msg");
            }
        }

        private List<string> SelectedMsgs_;
        public List<string> SelectedMsgs
        {
            get { return SelectedMsgs_; }
            set
            {
                this.RaiseAndSetIfChanged(ref SelectedMsgs_, value);
            }
        }

#if WPF
        private ICommand _CopyToClipboardCommand;
        public ICommand CopyToClipboardCommand
        {
            get
            {
                if (_CopyToClipboardCommand == null)
                {
                    _CopyToClipboardCommand = ReactiveCommand.Create((object o) =>
                    {
                        CopyToClipboard();
                    }
                    );
                }
                return _CopyToClipboardCommand;
            }
        }
#endif

        public void CopyToClipboard()
        {
#if AVALONIA
        clipboard_.SetTextAsync(string.Join("\n", SelectedMsgs));
#endif
        }

#if WPF
        private ICommand _ClearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_ClearCommand == null)
                {
                    _ClearCommand = ReactiveCommand.Create((object o) =>
                    {
                        Clear();
                    }
                    );
                }
                return _ClearCommand;
            }
        }
#endif

        public void Clear()
        {
            lock (this)
            {
                LogEntries.Clear();
            }
        }
    }
}
