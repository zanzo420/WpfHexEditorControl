using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace WPFHexaEditor.Core.Localization
{
    [MarkupExtensionReturnType(typeof(object))]
    public abstract class LocResourceExtension : MarkupExtension
    {
        protected LocResourceExtension() { }

        protected LocResourceExtension(string resourceKey)
        {
            this.ResourceKey = resourceKey;
        }

        [ConstructorArgument("resourceKey")]
        public string ResourceKey { get; set; }

        // Objet à mettre à jour lors d'un changement de culture
        // On ne garde qu'une référence faible pour ne pas empêcher que l'objet soit ramassé par le GC
        protected WeakReference targetObjectRef;
        // Propriété à mettre à jour lors d'un changement de culture
        protected object targetProperty;

        #region Implémentation de MarkupExtension

        // On ne veut pas que les classes héritées redéfinissent cette méthode, donc on la déclare sealed
        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(ResourceKey))
                throw new InvalidOperationException("The ResourceKey property cannot be null or empty");

            // On récupère l'objet et la propriété cible pour pouvoir la mettre à
            // jour lors d'un changement de culture
            IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (target != null)
            {
                // En mode design, la cible n'est pas définie : elle ne sera donc pas mise à jour
                if (target.TargetObject != null && target.TargetProperty != null)
                {
                    this.targetObjectRef = new WeakReference(target.TargetObject);
                    this.targetProperty = target.TargetProperty;
                    // On s'abonne à l'évènement UICultureChanged pour mettre à jour la propriété
                    LocManager.UICultureChanged += LocManager_UICultureChanged;
                }
            }

            return ProvideValueInternal(serviceProvider);
        }

        #endregion

        // Cette méthode devra être redéfinie par les classes héritées
        protected abstract object ProvideValueInternal(IServiceProvider serviceProvider);

        private void LocManager_UICultureChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        // Cette méthode met à jour la cible pour prendre en compte le changement de culture
        private void Refresh()
        {
            // On vérifie que l'objet à mettre à jour existe encore
            if (!targetObjectRef.IsAlive)
            {
                // L'objet a été ramassé par le GC, on se désabonne de l'évènement UICultureChanged
                LocManager.UICultureChanged -= LocManager_UICultureChanged;
                return;
            }

            object value = ProvideValueInternal(null);

            // Si targetProperty est une DependencyProperty, targetObject est un DependencyObject
            // Sinon, targetProperty est un PropertyInfo, et targetObject un object quelconque
            if (targetProperty is DependencyProperty)
            {
                DependencyObject obj = targetObjectRef.Target as DependencyObject;
                DependencyProperty prop = targetProperty as DependencyProperty;
                obj.SetValue(prop, value);
            }
            else
            {
                object obj = targetObjectRef.Target;
                PropertyInfo prop = targetProperty as PropertyInfo;
                prop.SetValue(obj, value, null);
            }
        }
    }
}
