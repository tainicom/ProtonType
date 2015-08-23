using System;
using System.Windows;
using tainicom.ProtonType.Framework.Modules;

namespace tainicom.ProtonType.Framework.Modules
{
    abstract public class ModuleBase : DependencyObject, IModule
    {
        #region IModule Members

        protected ISiteViewModel Site { get; private set; }

        public virtual void Initialize(ISiteViewModel site)
        {
            this.Site = site;
        }
        #endregion
    }
}
