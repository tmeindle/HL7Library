using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HL7Library
{
    public class Segment : MessageObject
    {
        SortedDictionary<int, SortedDictionary<int, Field>> m_fields;
                
        public string Type
        {
            get { return m_fields[0][1].AsString; }
            set { m_fields[0][1].AsString = value; }
        }

        public Segment(string type = "", Delimiters delim = null) 
            : base(delim)
        {
           
            m_fields = new SortedDictionary<int, SortedDictionary<int, Field>>();
            m_fields[0] = new SortedDictionary<int, Field>();
            m_fields[0][1] = new Field(m_delimiters);
            m_fields[0][1].AsString = type;
        }

        private Field At(int field, int repeat = 1)
        {
            if (!m_fields.ContainsKey(field))
            {
                m_fields.Add(field, new SortedDictionary<int, Field>());
            }

            if (!m_fields[field].ContainsKey(repeat))
            {
                m_fields[field].Add(repeat, new Field(m_delimiters));
            }

            return m_fields[field][repeat];
        }

        public override void Clear()
        {
            foreach (int i in m_fields.Keys)
            {
                SortedDictionary<int, Field> repeats = m_fields[i];
                foreach (int j in repeats.Keys)
                {
                    repeats[j].Clear();
                }
                repeats.Clear();
            }
            m_fields.Clear();
            m_fields[0] = new SortedDictionary<int, Field>();
            m_fields[0][1] = new Field(m_delimiters);
        }


        public override MessageObject Query(string q)
        {
            MessageObject ret = null;

            string pattern = @"^((?<Field>[1-9]\d{0,2})(\[(?<Repeat>[1-9]\d{0,2})])?)(([-\.])(?<ComponentQuery>(?<Component>[1-9]\d{0,2})(([-\.])(?<Subcomponent>[1-9]\d{0,2}))?))?$";

            MatchCollection matches = Regex.Matches(q, pattern);
            if (matches.Count == 1)
            {
                GroupCollection groups = matches[0].Groups;

                int field = Int32.Parse(groups["Field"].Value);
                int repeat = (String.IsNullOrEmpty(groups["Repeat"].Value)) ? 1 : Int32.Parse(groups["Repeat"].Value);

                if (String.IsNullOrEmpty(groups["ComponentQuery"].Value))
                {
                    ret = At(field, repeat);
                }
                else
                {
                    ret = At(field, repeat).Query(groups["ComponentQuery"].Value);
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

            List<string> fields = SplitIntoFields(str);
            
            if (fields.Count == 0 || !Regex.IsMatch(fields[0], "^[A-Z][A-Z][A-Z1-9]$"))
                throw (new ArgumentException("Input is not a correctly formatted HL7 Segment"));

            m_fields[0][1].AsString = fields[0];
            
            for (int j = 1; j < fields.Count; j++)
            {
                List<string> repeats = SplitIntoRepeats(fields[j]);

                for (int k = 0; k < repeats.Count; k++)
                {
                    if (repeats[k].Length > 0)
                    {
                        if (!m_fields.Keys.Contains(j))
                            m_fields[j] = new SortedDictionary<int, Field>();
                        m_fields[j][k + 1] = new Field(m_delimiters);
                        m_fields[j][k + 1].AsString = repeats[k];
                    }
                }
            }
        }

        //used in the ToString method on each field to 
        //run through all the repeats and make a single string out of them
        private string RepeatsToString(int fieldIndex)
        {
            StringBuilder ret = new StringBuilder();

            int[] indices = m_fields[fieldIndex].Keys.ToArray();
            if (indices.Length > 0)
            {
                int i = 0;
                int last = 1;

                for (; i < indices.Count(); i++)
                {
                    int index = indices[i];
                    int diff = index - last;
                    StringBuilder delimiters = new StringBuilder();
                    string strRepeat = m_fields[fieldIndex][index].AsString;

                    if (!String.IsNullOrEmpty(strRepeat))
                    {
                        for (int k = 0; k < diff; k++)
                        {
                            delimiters.Append(m_delimiters.RepeatDelimiter);
                        }
                        ret.AppendFormat("{0}{1}", delimiters, strRepeat);
                        last = index;
                    }
                }
            }
            return ret.ToString();
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();

            int[] indices = m_fields.Keys.ToArray();
            if (indices.Length > 0)
            {
                int i = 0;
                int last = 0;

                for (; i < m_fields.Keys.Count; i++)
                {
                    int index = indices[i];
                    int diff = index - last;
                    StringBuilder delimiters = new StringBuilder();
                    
                    string strField = RepeatsToString(index);

                    if (!String.IsNullOrEmpty(strField))
                    {
                        for (int k = 0; k < diff; k++)
                        {
                            delimiters.Append(m_delimiters.FieldDelimiter);
                        }
                        ret.AppendFormat("{0}{1}", delimiters, strField);
                        last = index;
                    }
                }
            }
            return ret.ToString();
        }


        private List<string> SplitIntoFields(string segment)
        {
            List<string> fields = new List<string>();
            StringBuilder curr = new StringBuilder();
            char prev = '\0';

            for (int i = 0; i <= segment.Length; i++)
            {
                if (i == segment.Length || (segment[i].Equals(m_delimiters.FieldDelimiter) && !prev.Equals('\\')))
                {
                    fields.Add(curr.ToString());
                    curr.Clear();
                    prev = '\0';
                }
                else
                {
                    curr.Append(segment[i]);
                    prev = segment[i];
                }
            }
            return fields;
        }

        private List<string> SplitIntoRepeats(string field)
        {
            List<string> repeats = new List<string>();
            StringBuilder curr = new StringBuilder();
            char prev = '\0';

            for (int k = 0; k <= field.Length; k++)
            {
                if (k == field.Length || field[k].Equals(m_delimiters.RepeatDelimiter) && !prev.Equals('\\'))
                {
                    repeats.Add(curr.ToString());
                    curr.Clear();
                    prev = '\0';
                }
                else
                {
                    curr.Append(field[k]);
                    prev = field[k];
                }
            }

            return repeats;
        }


    }
}
