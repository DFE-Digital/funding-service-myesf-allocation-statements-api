using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllocationStatementsApi.Helpers
{
    /// <summary>
    /// Copied from Sfa.Core's EnumAttributeMap class.
    /// </summary>
    public class EnumAttributeMap
    {
        public object EnumValue { get; private set; }

        public IList<Attribute> Attributes { get; private set; }

        public EnumAttributeMap(Enum enumValue, IEnumerable<Attribute> attributes)
        {
            if (enumValue == null)
            {
                throw new ArgumentNullException("enumValue");
            }

            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }

            EnumValue = enumValue;
            Attributes = attributes.ToList();
        }

        public TAttribute GetFirstAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return Attributes.OfType<TAttribute>().FirstOrDefault();
        }

        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            return Attributes.OfType<TAttribute>();
        }
    }
}