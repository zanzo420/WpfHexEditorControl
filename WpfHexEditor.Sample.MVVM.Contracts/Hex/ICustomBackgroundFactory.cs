using System;
using System.Collections.Generic;
using System.Text;
using WpfHexaEditor.Core.Interfaces;
using WpfHexEditor.Sample.MVVM.Contracts.Common;

namespace WpfHexEditor.Sample.MVVM.Contracts.Hex
{
    public interface ICustomBackgroundFactory
    {
        ICustomBackgroundBlock CreateNew();
    }

    public class CustomBackgroundFactory : GenericServiceStaticInstance<ICustomBackgroundFactory> {
        public static ICustomBackgroundBlock CreateNew() => Current.CreateNew();
    }
}
