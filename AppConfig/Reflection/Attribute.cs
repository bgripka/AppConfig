using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace AppConfig.Reflection
{
    public class AttributeUtil
    {
        public static T GetTypeAttribute<T>(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), false);
            return (T)((attributes.Length == 1) ? attributes[0] : null);
        }
        public static T GetPropertyAttribute<T>(PropertyInfo propertyInfo)
        {
            object[] attributes = propertyInfo.GetCustomAttributes(typeof(T), false);
            return (T)((attributes.Length == 1) ? attributes[0] : null);
        }
        public static T GetAssemblyAttribute<T>(Assembly assembly)
        {
            try
            {
                object[] attributes = assembly.GetCustomAttributes(typeof(T), false);
                return (T)((attributes.Length == 1) ? attributes[0] : null);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw new Exception("Error getting assembly attribute of type '" + typeof(T).FullName + "' from assembly '" + assembly.FullName + "'.", ex);
            }
        }
    }
}
