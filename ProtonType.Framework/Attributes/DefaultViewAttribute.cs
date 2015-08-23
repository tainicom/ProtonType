using System;

namespace tainicom.ProtonType.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultViewAttribute : Attribute
    {
        public Type DefaultViewType { get; private set; }
        public string DefaultLayoutAnchorablePaneName { get; private set; }

        public DefaultViewAttribute(Type defaultViewType)
        {
            this.DefaultViewType = defaultViewType;
            DefaultLayoutAnchorablePaneName = null;
        }

        public DefaultViewAttribute(Type defaultViewType, string defaultLayoutAnchorablePaneName)
        {
            this.DefaultViewType = defaultViewType;
            this.DefaultLayoutAnchorablePaneName = defaultLayoutAnchorablePaneName;
        }
    }
}
