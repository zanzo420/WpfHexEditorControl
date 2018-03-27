using Microsoft.Practices.ServiceLocation;
using Prism.Mef;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfHexEditor.Sample.MVVM.Common;
using WpfHexEditor.Sample.MVVM.Contracts;

namespace WpfHexEditor.Sample.MVVM {
   
    public class BootStrapper : MefBootstrapper {
        protected override void ConfigureAggregateCatalog() {
            base.ConfigureAggregateCatalog();

            //Framework assembly;
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Dummy).Assembly));
            
        }


        protected override IModuleCatalog CreateModuleCatalog() {
            return new ConfigurationModuleCatalog();
        }

        protected override DependencyObject CreateShell() {
            ServiceProvider.SetServiceProvider(new PracticeServiceProvider(ServiceLocator.Current));
            return this.Container.GetExportedValue<IShell>() as DependencyObject;
        }


        protected override void InitializeShell() {
            base.InitializeShell();
            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }


    }
}
