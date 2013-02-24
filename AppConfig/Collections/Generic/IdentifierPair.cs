using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Collections.Generic
{
    public class IdentifierPair<T>
    {
        public IdentifierPair(T ID1, T ID2)
        {
            this.ID1 = ID1;
            this.ID2 = ID2;
        }

        public T ID1 { get; set; }
        public T ID2 { get; set; }
    }
}
