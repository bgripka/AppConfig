using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Linq;

namespace AppConfig.Reflection
{
    public static class AssemblyExtensions
    {
        public static bool IsCompiledAsRelease(this Assembly context)
        {
            if (context == null)
                return false;

            object[] attributes = context.GetCustomAttributes(typeof(DebuggableAttribute), true);
            if (attributes == null || attributes.Length == 0)
                return true;

            var d = (DebuggableAttribute)attributes[0];
            if ((d.DebuggingFlags & DebuggableAttribute.DebuggingModes.Default) == DebuggableAttribute.DebuggingModes.None)
                return true;

            return false;
        }

        public static List<Type> GetAllSubclassesOf(this Assembly context, Type BaseType)
        {
            return context.GetTypes()
                .Where(a => a.IsSubclassOf(BaseType))
                .ToList();
        }
    }
}
