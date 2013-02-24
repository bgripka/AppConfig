using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Database
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class ManyToManyRelationshipAttribute : Attribute
    {
        public ManyToManyRelationshipAttribute(Type ForeignEntity)
        {

        }
        public ManyToManyRelationshipAttribute(Type ForeignEntity, Type ConnectingEntity)
        {

        }
    }
}
