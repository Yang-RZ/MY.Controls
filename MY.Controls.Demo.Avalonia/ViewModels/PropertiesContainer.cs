using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MY.Controls.Attributes;

namespace MY.Controls.Demo
{
    public enum EnumTest
    {
        Item1,
        Item2,
        Item3,
        Item4,
    }

    public class PropertiesContainer : ReactiveObject
    {
        private string PropName_ = "Test";
        [Order]
        public string PropName
        {
            get => PropName_;
            set
            {
                this.RaiseAndSetIfChanged(ref PropName_, value);
            }
        }

        private bool PropChecked_ = true;
        [Order]
        public bool PropChecked
        {
            get => PropChecked_;
            set
            {
                this.RaiseAndSetIfChanged(ref PropChecked_, value);
            }
        }

        private EnumTest PropEnums_ = EnumTest.Item2;
        [Order]
        public EnumTest PropEnums
        {
            get => PropEnums_;
            set
            {
                this.RaiseAndSetIfChanged(ref PropEnums_, value);
            }
        }

        private double PropDouble_ = 22.1;
        [Order]
        public double PropDouble
        {
            get => PropDouble_;
            set
            {
                this.RaiseAndSetIfChanged(ref PropDouble_, value);
            }
        }
    }
}
