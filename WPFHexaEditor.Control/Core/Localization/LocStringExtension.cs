using System;
using System.Windows.Markup;

namespace WPFHexaEditor.Core.Localization
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocStringExtension : LocResourceExtension
    {
        public LocStringExtension() { }

        public LocStringExtension(string resourceKey) : base(resourceKey) { }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            if (LocManager.ResourceManager != null)
                try
                {
                    return LocManager.ResourceManager.GetString(ResourceKey);
                }
                catch
                {
                    return "[" + ResourceKey + "]";
                }
            else
                // ResourceManager introuvable : on renvoie un texte par défaut
                return "[" + ResourceKey + "]";
        }
    }
}