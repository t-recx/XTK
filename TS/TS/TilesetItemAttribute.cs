using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TS
{
    public class TilesetItemAttribute
    {
        public string Name;
        public object Value;

        public TilesetItemAttribute(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public TilesetItemAttribute()
        {

        }
    }
}
