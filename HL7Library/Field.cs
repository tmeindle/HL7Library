using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HL7Library
{
    //An hl7 field is made up of N components where all N components aren't necessarily present
    //by default HL7 components are separated by a caret (^) in HL7 messages
    public class Field : MessageObject
    {
        SortedDictionary<int, Component> m_components;

        public Field(Delimiters delim = null)
            : base(delim)
        {
            m_components = new SortedDictionary<int, Component>();
        }

        private Component At(int index)
        {
            if (!m_components.Keys.Contains(index))
            {
                m_components.Add(index, new Component(m_delimiters));
            }
            return m_components[index];
        }

        public override void Clear()
        {
            foreach (int i in m_components.Keys)
                m_components[i].Clear();
            m_components.Clear();
        }
        
        public override MessageObject Query(string q)
        {
            MessageObject ret = null;
            string pattern = @"^(?<Component>[1-9]\d{0,2})(([-\.])(?<Subcomponent>[1-9]\d{0,2}))?$";
            MatchCollection matches = Regex.Matches(q, pattern);
            if (matches.Count == 1)
            {
                GroupCollection groups = matches[0].Groups;
                int component = Int32.Parse(groups["Component"].Value);

                if (String.IsNullOrEmpty(groups["Subcomponent"].Value))
                {
                    ret = At(component);
                }
                else
                {
                    ret = At(component).Query(groups["Subcomponent"].Value);
                }
                return ret;
            }
            else
            {
                throw (new ArgumentException("Query String Not Formatted Correctly"));
            }
        }

        public override void FromString(string str)
        {
            Clear();

            List<string> components = SplitIntoComponents(str);
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].Length > 0)
                {
                    Component c = new Component(m_delimiters);
                    c.AsString = components[i];
                    m_components[i + 1] = c;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();

            int[] indices = m_components.Keys.ToArray();
            if (indices.Length > 0)
            {
                int i = 0;
                int last = 1;

                for (; i < m_components.Keys.Count; i++)
                {
                    int index = indices[i];
                    int diff = index - last;
                    StringBuilder delimiters = new StringBuilder();
                    string strComponent = m_components[index].AsString;

                    if (!String.IsNullOrEmpty(strComponent))
                    {
                        for (int k = 0; k < diff; k++)
                        {
                            delimiters.Append(m_delimiters.ComponentDelimiter);
                        }
                        ret.AppendFormat("{0}{1}", delimiters, strComponent);
                        last = index;
                    }
                }
            }
            return ret.ToString();
        }

        private List<string> SplitIntoComponents(string field)
        {
            List<string> components = new List<string>();
            StringBuilder curr = new StringBuilder();
            char prev = '\0';

            for (int i = 0; i <= field.Length; i++)
            {
                if (i == field.Length || field[i].Equals(m_delimiters.ComponentDelimiter) && !prev.Equals('\\'))
                {
                    components.Add(curr.ToString());
                    curr.Clear();
                    prev = '\0';
                }
                else
                {
                    curr.Append(field[i]);
                    prev = field[i];
                }
            }

            return components;
        }

    }
}
