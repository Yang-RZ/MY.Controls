using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#if AVALONIA
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Layout;
using System.Reactive.Linq;
using MY.Controls.Ava;
#endif

#if WPF
using System.Windows.Controls;
using MY.Controls.WPF;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Reactive.Linq;
#endif

namespace MY.Controls
{
    public partial class PropertyHelper : ReactiveUI.ReactiveObject
    {
        private PropertyGridHelper parent_;
        private IReactiveObject _source;
        private string _sParentTitle;
        private string _sConfigName;
        private bool _bReadOnly;

        public PropertyHelper(PropertyGridHelper parent, IReactiveObject source, string sPropName, string sDisplayName, string sParentTitle, string sConfigName, bool bReadOnly)
        {
            parent_ = parent;
            _source = source;
            _sParentTitle = sParentTitle;
            _sConfigName = sConfigName;
            _bReadOnly = bReadOnly;
            PropName = sPropName;
            DisplayName = sDisplayName;

            GenerateView();
        }

        public void GenerateView()
        {
            //string pvname = "PropValue";
            PropertyInfo propertyInfo = _source.GetType().GetProperty(PropName);
            InitValue = propertyInfo.GetValue(_source);
            Type type = propertyInfo.PropertyType;
            IsReadOnly = false;
            if (!propertyInfo.CanWrite)
            {
                IsReadOnly = true;
                var textbox = new TextBox();
                textbox.MinWidth = 100;
                textbox.IsReadOnly = true;
                textbox.HorizontalContentAlignment = HorizontalAlignment.Center;
                textbox.HorizontalAlignment = HorizontalAlignment.Stretch;
                textbox.Text = propertyInfo.GetValue(_source)?.ToString();
                _source.WhenAnyPropertyChanged(PropName)

#if AVALONIA
                        .ObserveOn(System.Reactive.Concurrency.Scheduler.Immediate)
#endif

#if WPF
                        .ObserveOn(Application.Current.Dispatcher)
#endif
                        .Select(x => propertyInfo.GetValue(_source)?.ToString())
                        .BindTo(textbox, t => t.Text);
                textbox.Background = new SolidColorBrush(Colors.LightGray);
                textbox.GotFocus += Textbox_GotFocus;
                View = textbox;
            }
            else if (type.IsEnum)
            {
                var cbo = new ComboBox();
                cbo.IsEnabled = !_bReadOnly;
                cbo.HorizontalAlignment = HorizontalAlignment.Stretch;
                cbo.ItemsSource = Enum.GetValues(type);
                cbo.SelectedItem = (Enum)propertyInfo.GetValue(_source);
                _source.WhenAnyPropertyChanged(PropName)
#if AVALONIA
                        .ObserveOn(System.Reactive.Concurrency.Scheduler.Immediate)
#endif

#if WPF
                        .ObserveOn(Application.Current.Dispatcher)
#endif
                        .Select(x => propertyInfo.GetValue(_source))
                        .BindTo(cbo, c => c.SelectedItem);
                cbo.WhenAnyValue(c => c.SelectedItem)
                    .Subscribe(o =>
                    {
                        PropValue = o;
                        //propertyInfo.SetValue(_source, o);
                    });
                //cbo.SetBinding(ComboBox.SelectedItemProperty, pvname);
                View = cbo;
            }
            else if (type == typeof(Boolean))
            {
                var chk = new CheckBox();
                chk.IsEnabled = !_bReadOnly;
                chk.IsChecked = (bool)propertyInfo.GetValue(_source);
                _source.WhenAnyPropertyChanged(PropName)
#if AVALONIA
                        .ObserveOn(System.Reactive.Concurrency.Scheduler.Immediate)
#endif

#if WPF
                        .ObserveOn(Application.Current.Dispatcher)
#endif
                        .Select(x => propertyInfo.GetValue(_source))
                        .BindTo(chk, c => c.IsChecked);
                chk.WhenAnyValue(c => c.IsChecked)
                    .Subscribe(o =>
                    {
                        PropValue = o;
                    });
                //chk.SetBinding(CheckBox.IsCheckedProperty, pvname);
                View = chk;
            }
            else if (type.IsPrimitive || type == typeof(String))
            {
                var textbox = new TextBox();
                textbox.IsReadOnly = _bReadOnly;
                textbox.MinWidth = 100;
                ErrorInfo = propertyInfo.GetValue(_source)?.ToString();
                textbox.Text = ErrorInfo;
                //binding.UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
                _source.WhenAnyPropertyChanged(PropName)
#if AVALONIA
                        .ObserveOn(System.Reactive.Concurrency.Scheduler.Immediate)
#endif

#if WPF
                        .ObserveOn(Application.Current.Dispatcher)
#endif
                        .Select(x => propertyInfo.GetValue(_source).ToString())
                        .BindTo(textbox, t => t.Text);
                textbox.WhenAnyValue(t => t.Text)
                        .Subscribe(s =>
                        {
                            ErrorOccured = false;
                            var conv = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                            try
                            {
                                var vNew = conv.ConvertFrom(s);
                                PropValue = vNew;
                                //propertyInfo.SetValue(_source, vNew);
                                ErrorInfo = s;
                            }
                            catch (Exception ex)
                            {
                                ErrorOccured = true;
                            }
                        });
                //textbox.SetBinding(TextBox.TextProperty, binding);
                textbox.GotFocus += Textbox_GotFocus;
                textbox.HorizontalAlignment = HorizontalAlignment.Stretch;
                textbox.TextWrapping = TextWrapping.Wrap;
                textbox.AcceptsReturn = true;

                View = textbox;
            }
            else
            {
                var btn = new Button();
                btn.MinWidth = 40;
                btn.Content = "...";
                btn.Click += Btn_Click;
                View = btn;
            }
        }

#if AVALONIA
    private void Textbox_GotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
#endif

#if WPF
        private void Textbox_GotFocus(object sender, RoutedEventArgs e)
#endif
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

#if AVALONIA
    private void Btn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
#endif

#if WPF
        private void Btn_Click(object sender, System.Windows.RoutedEventArgs e)
#endif
        {
            MessageHelper.ShowMessage(PropName);

            //try
            //{
            //    if (PropValue is null)
            //    {
            //        MessageBox.Show("Can't set this property!".Translate());
            //        return;
            //    }
            //    PropertyGridHelper propertyGridHelper = new PropertyGridHelper(PropValue, $"{_sParentTitle}::{PropName}", _sConfigName);
            //    Window window = propertyGridHelper.GeneratePropertyGridWindow();
            //    window.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private string _PropName;
        public string PropName
        {
            get
            {
                return _PropName;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _PropName, value);
            }
        }

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _IsReadOnly, value);
            }
        }

        private string _DisplayName;
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _DisplayName, value);
            }
        }

        private SolidColorBrush _HighlightColor = new SolidColorBrush(Colors.Transparent);
        public SolidColorBrush HighlightColor
        {
            get
            {
                return _HighlightColor;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _HighlightColor, value);
            }
        }

        public object InitValue { get; set; }

        public object PropValue
        {
            get
            {
                return _source.GetType().GetProperty(PropName).GetValue(_source);
            }
            set
            {
                _source.GetType().GetProperty(PropName).SetValue(_source, value);
                this.RaisePropertyChanged();
                PropChanged = true;
                if (PropChanged)
                {
                    parent_.RaisePropertyChangedEvent();
                }
            }
        }

        private Object _View;
        public Object View
        {
            get
            {
                return _View;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _View, value);
            }
        }

        private bool _ErrorOccured;
        public bool ErrorOccured
        {
            get
            {
                return _ErrorOccured;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _ErrorOccured, value);
            }
        }


        private string _ErrorInfo;
        public string ErrorInfo
        {
            get
            {
                return _ErrorInfo;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _ErrorInfo, value);
            }
        }

        private bool _PropChanged;
        public bool PropChanged
        {
            get
            {
                var o = InitValue; // 刷新InitValue如果InitValue为null
                if (o == null)
                {
                    return _source.GetType().GetProperty(PropName).GetValue(_source) != null;
                }
                if (_source.GetType().GetProperty(PropName).GetValue(_source) == null)
                {
                    return o == null;
                }
                else
                {
                    return !o.Equals(_source.GetType().GetProperty(PropName).GetValue(_source));
                }
            }
            set
            {
                if (!object.Equals(_PropChanged, value))
                    _PropChanged = value;

                this.RaisePropertyChanged();
            }
        }
    }
}
