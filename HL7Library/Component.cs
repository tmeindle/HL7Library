using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HL7Library
{
    public class Component : MessageObject
    {
        SortedDictionary<int, Subcomponent> m_subcomponents;

        public Component(Delimiters delim = null)
            : base(delim)
        {
            m_subcomponents = new SortedDictionary<int, Subcomponent>();
        }

        private Subcomponent At(int index)
        {
            if (!m_subcomponents.Keys.Contains(index))
            {
                m_subcomponents.Add(index, new Subcomponent("", m_delimiters));
            }
            return m_subcomponents[index];
        }

        public override void Clear()
        {
            foreach (int i in m_subcomponents.Keys)
                m_subcomponents[i].Clear();
            m_subcomponents.Clear();
        }

        public override MessageObject Query(string q)
        {
            //at this level we can only query for individual sub components
            string pattern = @"^([1-9]\d{0,2})$";
            if (Regex.IsMatch(q, pattern))
            {
                return At(int.Parse(q));
            }
            else
            {
                throw (new ArgumentException("Query String Not Formatted Correctly"));
            }
        }

        public override void FromString(string str)
        {
            Clear();

            List<string> subcomponents = SplitIntoSubcomponents(str);
            for (int i = 0; i < subcomponents.Count; i++)
            {
                if (subcomponents[i].Length > 0)
                {
                    Subcomponent s = new Subcomponent("", m_delimiters);
                    s.AsString = subcomponents[i];
                    m_subcomponents[i + 1] = s;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            
            int[] indices = m_subcomponents.Keys.ToArray();
            if (indices.Length > 0)
            {
                int i = 0;
                int last = 1;
                
                for (; i < m_subcomponents.Keys.Count; i++)
                {
                    //for each index present we want to print the include the stored value
                    int index = indices[i];
                    int diff = index - last;
                    string strSubcomponent = m_subcomponents[index].AsString;
                    
                    StringBuilder delimiters = new StringBuilder();
                    if (!String.IsNullOrEmpty(strSubcomponent))
                    {
                        for (int k = 0; k < diff; k++)
                        {
                            delimiters.Append(m_delimiters.SubcomponentDelimiter);
                        }
                        ret.AppendFormat("{0}{1}", delimiters, strSubcomponent);
                        last = index;
                    }
                }
            }
            return ret.ToString();
        }


        private List<string> SplitIntoSubcomponents(string component)
        {
            List<string> subcomponents = new List<string>();
            StringBuilder curr = new StringBuilder();
            char prev = '\0';

            for (int i = 0; i <= component.Length; i++)
            {
                if (i == component.Length || component[i].Equals(m_delimiters.SubcomponentDelimiter) && !prev.Equals('\\'))
                {
                    subcomponents.Add(curr.ToString());
                    curr.Clear();
                    prev = '\0';
                }
                else
                {
                    curr.Append(component[i]);
                    prev = component[i];
                }
            }

            return subcomponents;
        }


    }
}
