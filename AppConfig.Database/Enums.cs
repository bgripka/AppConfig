using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Database
{
    public enum LengthType
    {
        Variable,
        VariableMax,
        Fixed,
        NoMax
    }
    public enum EnumStorageMethod
    {
        Text,
        Numeric
    }
    public enum TimestampBehavior
    {
        None,
        CreatedTime,
        ModifiedTime
    }
}
