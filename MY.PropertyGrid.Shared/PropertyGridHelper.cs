using MY.Extensions;
using MY.Controls.Attributes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

#if AVALONIA
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MY.Controls.Ava;
#endif

#if WPF
using System.Windows.Controls;
using MY.Controls.WPF;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
#endif

namespace MY.Controls
{
    public partial class PropertyGridHelper : ReactiveObject
    {
        private IReactiveObject _source;
        private string _sTitle;
        private string _sConfigName;

        //private HashSet<string> sChangedPropertySet_ = new HashSet<string>();

        private static List<PropertyHelper> SHARED_PROPERTIES;
        private static string SHARED_CONFIG_NAME;

        public PropertyGridHelper(IReactiveObject source, string sTitle, string sConfigName)
        {
            _source = source;
            _sTitle = sTitle;
            _sConfigName = sConfigName;
            string sDisplay = "";

            List<PropertyInfo> props = new List<PropertyInfo>();
            List<PropertyInfo> propsUnordered = new List<PropertyInfo>();
            List<string> lstPRUI = new List<string>();
            foreach (var prop in typeof(ReactiveObject).GetProperties(
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public))
            {
                lstPRUI.Add(prop.Name);
            }
            foreach (var prop in _source.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (lstPRUI.Contains(prop.Name))
                {
                    continue;
                }
                var attributes = prop.GetCustomAttributes(typeof(OrderAttribute), true);
                if (attributes is not null && attributes.Count() > 0)
                {
                    props.Add(prop);
                }
                else
                {
                    propsUnordered.Add(prop);
                }
            }

            var propsOrdered = props.OrderBy(p => ((OrderAttribute)p.GetCustomAttributes(typeof(OrderAttribute), true)[0]).Order).ToArray();

            List<PropertyInfo> propsFinal = new List<PropertyInfo>();
            propsFinal.AddRange(propsOrdered);
            propsFinal.AddRange(propsUnordered);

            foreach (var prop in propsFinal)
            {
                var attributes = prop.GetCustomAttributes(typeof(IgnoredPropertyInPropertyGridAttribute), true);
                if (attributes is not null && attributes.Count() > 0 && ((IgnoredPropertyInPropertyGridAttribute)attributes[0]).Ignored)
                {
                    continue;
                }
                bool bReadOnly = false;
                attributes = prop.GetCustomAttributes(typeof(PropertyTranslationAttribute), true);
                sDisplay = prop.Name;
                if (attributes is not null && attributes.Count() > 0)
                {
                    sDisplay = ((PropertyTranslationAttribute)attributes[0]).Translation;
                }
                var ph = new PropertyHelper(this, _source, prop.Name, sDisplay, _sTitle, _sConfigName, bReadOnly);
                Properties.Add(ph);
            }
        }

        public delegate void DelPropertyChangedEvent();

        public event DelPropertyChangedEvent PropertyChangedEvent;

        public void RaisePropertyChangedEvent()
        {
            //if (!sChangedPropertySet_.Contains(sPropName))
            //{
            //    sChangedPropertySet_.Add(sPropName);
            //}
            PropertyEverChanged = true;
            PropertyChangedEvent?.Invoke();
        }

        public void ClearAllAttachedPropertyChangedEvent()
        {
            PropertyChangedEvent = null;
        }

        public PropertyGrid GeneratePropertyGrid(int iMaxColumn = 1)
        {
            //PropertyGrid propertyGrid = new PropertyGrid(iMaxColumn);
            PropertyGrid propertyGrid = new PropertyGrid();
            propertyGrid.DataContext = this;
            return propertyGrid;
        }

        private SolidColorBrush _NormalColor = new SolidColorBrush(Colors.Transparent);
        private SolidColorBrush _HighlightColor = new SolidColorBrush(Colors.Yellow);

        public void ResetSearch()
        {
            Properties.ForEach(p => p.HighlightColor = _NormalColor);
        }

        public void HighlightProperties(string sSearch)
        {
            Properties.Where(p => p.DisplayName.ToUpper().Contains(sSearch.ToUpper()))
                .ForEach(p => p.HighlightColor = _HighlightColor);
        }

        private bool _PropertyEverChanged = false;
        public bool PropertyEverChanged
        {
            get
            {
                return _PropertyEverChanged;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _PropertyEverChanged, value);
            }
        }

        private string _StringToSearch = "";
        public string StringToSearch
        {
            get
            {
                return _StringToSearch;
            }
            set
            {
                ResetSearch();
                if (!String.IsNullOrEmpty(value))
                {
                    HighlightProperties(value);
                }
                this.RaiseAndSetIfChanged(ref _StringToSearch, value);
            }
        }

#if WPF
        private ICommand _CopyProperties;
        public ICommand CopyProperties
        {
            get
            {
                if (_CopyProperties == null)
                {
                    _CopyProperties = ReactiveCommand.Create((object o) =>
                    {
                        CopyPropertiesCommand();
                    }
                    );
                }
                return _CopyProperties;
            }
        }
#endif

        public void CopyPropertiesCommand()
        {
            try
            {
                if (SHARED_PROPERTIES == null)
                {
                    SHARED_PROPERTIES = new List<PropertyHelper>();
                }
                SHARED_CONFIG_NAME = _sConfigName;
                SHARED_PROPERTIES.Clear();
                foreach (var item in Properties)
                {
                    SHARED_PROPERTIES.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowMessage(ex.ToString());
            }
        }

#if WPF
        private ICommand _PasteProperties;
        public ICommand PasteProperties
        {
            get
            {
                if (_PasteProperties == null)
                {
                    _PasteProperties = ReactiveCommand.Create((object o) =>
                    {
                        PastePropertiesCommand();
                    }
                    );
                }
                return _PasteProperties;
            }
        }
#endif

        public void PastePropertiesCommand()
        {
            try
            {
                if (SHARED_PROPERTIES == null)
                {
                    throw new Exception("No properties copied!");
                }
                if (_sConfigName != SHARED_CONFIG_NAME)
                {
                    throw new Exception("Unmatched config type!" + $"{SHARED_CONFIG_NAME} => {_sConfigName}");
                }
                for (int i = 0; i < SHARED_PROPERTIES.Count; i++)
                {
                    if (!Properties[i].IsReadOnly)
                    {
                        Properties[i].PropValue = SHARED_PROPERTIES[i].PropValue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowMessage(ex.ToString());
            }
        }

        private ObservableCollection<PropertyHelper> _Properties = new ObservableCollection<PropertyHelper>();
        public ObservableCollection<PropertyHelper> Properties
        {
            get
            {
                return _Properties;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _Properties, value);
            }
        }
    }
}
