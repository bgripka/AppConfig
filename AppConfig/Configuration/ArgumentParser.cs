using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Configuration
{
    public abstract class ArgumentParser : Dictionary<string, object>
    {
        protected ArgumentParser(string[] args, Dictionary<string, Type> arguementTypes)
        {
            foreach (var arg in args)
            {
                var parts = arg.Split('=');
                if (parts.Length != 2)
                    continue;
                var name = parts[0].ToLowerInvariant();
                var value = parts[1];
                if (!arguementTypes.ContainsKey(name))
                    continue;
                try
                {
                    this.Add(name, Convert.ChangeType(value, arguementTypes[name]));
                }
                catch
                {
                    throw new Exception("Arguement '" + name + "' cannot be casted to the required type '" + arguementTypes[name].FullName + "'.");
                }
            }
        }

        protected T GetValue<T>(string key, T defaultValue)
        {
            object rtn;
            if (!base.TryGetValue(key, out rtn))
                return defaultValue;
            else
                return (T)rtn;
        }
    }
}
