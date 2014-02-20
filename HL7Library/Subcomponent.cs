using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Library
{
    //a subcomponent is where the data is actually stored in string form
    public class Subcomponent : MessageObject
    {
        string _value;

        public Subcomponent(string value = "", Delimiters delim = null)
            : base(delim)
        {
            _value = value;
        }

        public override void Clear()
        {
            _value = String.Empty;
        }
        
        public override string ToString()
        {
            return _value;
        }

        public override void FromString(string str)
        {
             _value = str;
        }

        //at this level we can only query for this exact sub components
        public override MessageObject Query(string q)
        {
            return null;
        }
    }
}
