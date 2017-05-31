using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace WPFHexaEditor.Core.Localization
{
    public class LocManager
    {
        // Cultures disponibles
        private static ReadOnlyCollection<CultureInfo> _availableCultures = null;
        public static ReadOnlyCollection<CultureInfo> AvailableCultures
        {
            get
            {
                if (_availableCultures == null)
                {
                    _availableCultures = GetAvailableCultures();
                }
                return _availableCultures;
            }
        }

        // Enumère les cultures disponibles
        private static ReadOnlyCollection<CultureInfo> GetAvailableCultures()
        {
            List<CultureInfo> list = new List<CultureInfo>();

            string startupDir = Environment.CurrentDirectory;
            Assembly asm = GetAssembly();

            CultureInfo neutralCulture = CultureInfo.InvariantCulture;
            if (asm != null)
            {
                NeutralResourcesLanguageAttribute attr = Attribute.GetCustomAttribute(asm, typeof(NeutralResourcesLanguageAttribute)) as NeutralResourcesLanguageAttribute;
                if (attr != null)
                    neutralCulture = CultureInfo.GetCultureInfo(attr.CultureName);
            }
            list.Add(neutralCulture);

            if (asm != null)
            {
                string baseName = asm.GetName().Name;

                foreach (string dir in Directory.GetDirectories(startupDir))
                {
                    // On vérifie que le nom du répertoire correspond à une culture valide
                    DirectoryInfo dirinfo = new DirectoryInfo(dir);
                    CultureInfo tCulture = null;
                    try
                    {
                        tCulture = CultureInfo.GetCultureInfo(dirinfo.Name);
                    }
                    // Ce n'est pas une culture valide : on ignore l'exception et on passe
                    // au répertoire suivant 
                    catch (ArgumentException)
                    {
                        continue;
                    }

                    // On vérifie que le répertoire contient des assemblies satellites
                    if (dirinfo.GetFiles(baseName + ".resources.dll").Length > 0)
                    {
                        list.Add(tCulture);
                    }

                }
            }
            return list.AsReadOnly();
        }

        // ResourceManager à utiliser pour récupérer les ressources
        private static ResourceManager _resourceManager = null;
        public static ResourceManager ResourceManager
        {
            get
            {
                if (_resourceManager == null)
                {
                    _resourceManager = GetResourceManager();
                }
                return _resourceManager;
            }
            set { _resourceManager = value; }
        }

        // Tente de récupérer automatiquement le ResourceManager
        private static ResourceManager GetResourceManager()
        {
            // On récupère l'assembly courant
            Assembly asm = GetAssembly();

            if (asm != null)
            {
                // On utilise le jeu de ressources nommé <nom de l'assembly>.Properties.Resources
                //string baseName = asm.GetName().Name + ".Properties.Resources";
                string baseName = asm.GetName().Name + ".Properties.Resources";
                ResourceManager rm = new ResourceManager(baseName, asm);
                return rm;
            }
            else
            {
                return null;
            }
        }

        private static Assembly GetAssembly()
        {
            Assembly asm = Assembly.GetEntryAssembly();

            // Mode design
            if (asm == null)
            {
                // En mode design, Assembly.GetEntryAssembly ne fonctionne pas
                // On cherche donc un assembly avec un point d'entrée qui contient
                // une classe héritée de System.Windows.Application
                asm = (
                       from a in AppDomain.CurrentDomain.GetAssemblies()
                       where a.EntryPoint != null
                       && a.GetTypes().Any(t => t.IsSubclassOf(typeof(System.Windows.Application)))
                       select a
                      ).FirstOrDefault();
            }

            return asm;
        }

        // Culture courante
        public static CultureInfo UICulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                Thread.CurrentThread.CurrentUICulture = value;
                OnUICultureChanged();
            }
        }

        // Handlers de l'évènement UICultureChanged
        // On utilise un HashSet pour éviter les doublons
        private static HashSet<EventHandler> uiCultureChangedHandlers = new HashSet<EventHandler>();

        public static event EventHandler UICultureChanged
        {
            add
            {
                uiCultureChangedHandlers.Add(value);
            }
            remove
            {
                uiCultureChangedHandlers.Remove(value);
            }
        }


        private static void OnUICultureChanged()
        {
            try
            {
                foreach (EventHandler handler in uiCultureChangedHandlers)
                {
                    handler(typeof(LocManager), EventArgs.Empty);
                }
            }
            catch (System.InvalidOperationException ex)
            {
                //
            }
        }
    }
}
