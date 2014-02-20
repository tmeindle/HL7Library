using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Library
{
    //An hl7 message is made up of Segments, Fields, Components, and SubComponents.
    //MessageObject is a common base for all these parts including the message itself
    //which includes the most common functionality such as querying and setting values
    abstract public class MessageObject : Object
    {
        protected Delimiters m_delimiters = null;
        public Delimiters Delimiters
        {
            get { return m_delimiters; }
        }

        public MessageObject(Delimiters delim = null)
            : base()
        {
            m_delimiters = (delim == null) ? new Delimiters() : delim;
        }

        //string indexer allows us to perform queries
        public virtual MessageObject this[string query]
        {
            get
            {
                return Query(query);
            }
        }
        
        //do nothing it is overridden in the derived classes so the override will get called
        public abstract void Clear();
        
        //a virtual query function for getting sub parts
        public abstract MessageObject Query(string q);

        //AsString function uses the virtual ToString and FromString functions
        public string AsString
        {
            get { return ToString(); }
            set { FromString(value); }
        }
        
        //function to return object as a string representation 
        public override string ToString() { return base.ToString(); } 

        //function to create the object from a string representation
        public abstract void FromString(string str);

    }
}
