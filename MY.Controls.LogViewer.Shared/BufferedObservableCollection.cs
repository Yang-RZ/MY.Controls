using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Linq;

#if AVALONIA
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using MY.Controls.Ava;
#endif

#if WPF
using System.Windows.Controls;
using MY.Controls.WPF;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
#endif

namespace MY.Controls
{
    public class BufferedObservableCollection<T> : ObservableCollection<T>
    {
        private IDisposable dispBuffer_;
        private Dispatcher dispatcher_;
        private Subject<T> obs_;
        private int iMaxCount_ = 100;
        private object objLockUpdate_ = new object();

        public object LockUpdate { get => objLockUpdate_; }

        public event EventHandler ItemAdded = delegate { };

        public BufferedObservableCollection(Dispatcher dispatcher)
        {
            dispatcher_ = dispatcher;
            obs_ = new Subject<T>();

            dispBuffer_ = obs_
                .SubscribeOn(TaskPoolScheduler.Default)
                .Buffer(new TimeSpan(0, 0, 0, 0, 100), 10000)
                .ObserveOn(new EventLoopScheduler())
                .Subscribe(items =>
                {
                    if (items.Count > 0)
                    {
                        int iOldCount = this.Items.Count;
                        lock (objLockUpdate_)
                        {
                            foreach (var item in items)
                            {
                                this.Items.Add(item);
                                //if (this.Items.Count > iMaxCount_)
                                //{
                                //    this.Items.RemoveAt(0);
                                //}
                            }
                            while (this.Items.Count > iMaxCount_)
                            {
                                this.Items.RemoveAt(0);
                            }
                        }

                        dispatcher_.InvokeAsync(new Action(() =>
                        {
                            lock (objLockUpdate_)
                            {
                                //if (this.Items.Count != iOldCount)
                                //{
                                //    this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                                //}

                                this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                                this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                ItemAdded(items.Last(), new EventArgs());

                            }
                        }));
                    }
                });
        }

        public void Shutdown()
        {
            dispBuffer_.Dispose();
        }

        public void AddToBuffer(T tNewItem)
        {
            obs_.OnNext(tNewItem);
        }

        public void SetMaxLimit(int iLimit)
        {
            iMaxCount_ = iLimit;
        }
    }
}
