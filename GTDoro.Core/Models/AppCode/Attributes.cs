using System;
namespace GTDoro.Core.Attributes
{
    public class IconCssClassAttribute : Attribute
    {
        internal IconCssClassAttribute(String cssClassName)
        {
            this.cssClassName = cssClassName;
        }
        public String cssClassName { get; private set; }
    }
}