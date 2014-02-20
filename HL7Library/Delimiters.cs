using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Library
{
    public class Delimiters 
    {
        protected char m_segmentDelimiter;
        protected char m_fieldDelimiter;
        protected char m_repeatDelimiter;
        protected char m_componentDelimiter;
        protected char m_subcomponentDelimiter;
        
        public Delimiters(char segmentDelimiter = '\r',
                         char fieldDelimiter = '|',
                         char repeatDelimiter = '~',
                         char componentDelimiter = '^',
                         char subcomponentDelimiter = '&')
        {
            m_segmentDelimiter = segmentDelimiter;
            m_fieldDelimiter = fieldDelimiter;
            m_repeatDelimiter = repeatDelimiter;
            m_componentDelimiter = componentDelimiter;
            m_subcomponentDelimiter = subcomponentDelimiter;
        }
          
        public char SegmentDelimiter
        {
            get { return m_segmentDelimiter; }
            set { m_segmentDelimiter = value; }
        }

        public char FieldDelimiter
        {
            get { return m_fieldDelimiter; }
            set { m_fieldDelimiter = value; }
        }


        public char RepeatDelimiter
        {
            get { return m_repeatDelimiter; }
            set { m_repeatDelimiter = value; }
        }

        public char ComponentDelimiter
        {
            get { return m_componentDelimiter; }
            set { m_componentDelimiter = value; }
        }

        public char SubcomponentDelimiter
        {
            get { return m_subcomponentDelimiter; }
            set { m_subcomponentDelimiter = value; }
        }
    }
}
