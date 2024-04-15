using System.Runtime.CompilerServices;
using System;

namespace MY.Controls.Attributes
{
    /// <summary>
    /// Specifies property index.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyIndexAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyIndexAttribute"/> class.
        /// </summary>
        /// <param name="iIndex"><c>0</c> Indicating the order of the property.</param>
        public PropertyIndexAttribute(int iIndex)
        {
            this.PropertyIndex = iIndex;
        }

        /// <summary>
        /// Gets a value indicating the order of the property.
        /// </summary>
        public int PropertyIndex { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoredPropertyInPropertyGridAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyIndexAttribute"/> class.
        /// </summary>
        /// <param name="iIndex"><c>0</c> Indicating if the PropertyGrid will initialize it.</param>
        public IgnoredPropertyInPropertyGridAttribute(bool bIgnored = true)
        {
            this.Ignored = bIgnored;
        }

        /// <summary>
        /// Gets a value indicating if the PropertyGrid will initialize it.
        /// </summary>
        public bool Ignored { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyTranslationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyIndexAttribute"/> class.
        /// </summary>
        /// <param name="iIndex"><c>0</c> Indicating if the PropertyGrid will initialize it.</param>
        public PropertyTranslationAttribute(string sTranslation)
        {
            this.Translation = sTranslation;
        }

        /// <summary>
        /// Gets a value indicating if the PropertyGrid will initialize it.
        /// </summary>
        public string Translation { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int order_;
        public OrderAttribute([CallerLineNumber] int order = 0)
        {
            order_ = order;
        }

        public int Order { get { return order_; } }
    }
}
