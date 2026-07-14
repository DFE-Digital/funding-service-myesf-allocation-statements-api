using System.Collections.Concurrent;

namespace AllocationStatementsApi.Helpers
{
    /// <summary>
    /// Creates or returns maps of attributes for enum types to help with performance when accessing attribute via reflection.
    /// The idea being that the attribute are accessed once and then held in a static variable.
    /// </summary>
    public static class EnumAttributeMaps
    {
        #region Fields

        private static readonly ConcurrentDictionary<Type, IList<EnumAttributeMap>> EnumValueAttributes = new ConcurrentDictionary<Type, IList<EnumAttributeMap>>();

        #endregion


        #region Public API

        /// <summary>
        /// Gets the <see cref="EnumAttributeMap"/> for the specified Enum value.
        /// </summary>
        /// <param name="maps">The maps.</param>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The <see cref="EnumAttributeMap"/>.</returns>
        public static EnumAttributeMap For(this IEnumerable<EnumAttributeMap> maps, Enum enumValue)
        {
            return maps.First(m => m.EnumValue.Equals(enumValue));
        }

        /// <summary>
        /// Gets the enum attribute maps for the specified Enum type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>All the enum attribute maps for the type specified.</returns>
        public static IEnumerable<EnumAttributeMap> GetEnumAttributeMaps(this Type type)
        {
            if (!EnumValueAttributes.ContainsKey(type))
            {
                var list = new List<EnumAttributeMap>();
                foreach (Enum enumValue in Enum.GetValues(type))
                {
                    var attributes = enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttributes(true).OfType<Attribute>().ToList();
                    list.Add(new EnumAttributeMap(enumValue, attributes));
                }

                EnumValueAttributes[type] = list;
                return list;
            }

            return EnumValueAttributes[type];
        }

        #endregion
    }
}