using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHexEditor.Sample.MVVM.Models
{
    
    //<DockPanel MinWidth = "120" >
    //                            < TextBlock Text="{Binding KeyName}" DockPanel.Dock="Left"/>
    //                            <TextBlock Text = "{Binding Value}" DockPanel.Dock="Right"/>
    //                            <Border/>
    //                        </DockPanel>
    public class ToolTipItemDataModel:BindableBase
    {
        private string _keyName;
        public string KeyName {
            get => _keyName;
            set => SetProperty(ref _keyName, value);
        }


        private string _value ;
        public string Value {
            get => _value;
            set => SetProperty(ref _value, value);
        }

    }
}
