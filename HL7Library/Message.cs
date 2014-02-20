using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HL7Library
{

    public class Message : MessageObject
    {
        //a message is a list of segments
        List<Segment> m_segments;

        public Message(Delimiters delim = null)
            : base(delim)
        {
            //if we don't pass in class definining the delimiters, 
            //The base creates its own using default values

            //create an empty list used to store the segments
            m_segments = new List<Segment>();
        }

        //return the segment in the list at the given index
        private Segment At(int index)
        {
            //warning: if the segment doesn't exist in the structure,
            //for example we want index 4 and we only have index 0,1,and 2:
            //this function will create empty semgments without types defined
            //for segments 3 and 4
            while (m_segments.Count <= index)
            {
                //we need the count to be greater than the index
                //so add segments using the messages delimiters
                m_segments.Add(new Segment("", m_delimiters));
            }
            //and return it
            return m_segments[index];
        }

        //return the given repeat of a type segment from the list
        private Segment At(string segmentType, int segmentRepeat = 1)
        {
            int i = 0;
            int r = 0;
            int index = -1;
            Segment ret = null;

            //warning: if the segment doesn't exist in the structure,
            //for example we want repeat 4 and we only have repeats 1 and 2:
            //this function will create 2 semgments of the given type and place
            //them at the end of the list
            
            //first calculate how repeats there are for segments of segmentType
            //note there could be none (r=0)
            for (; i < m_segments.Count() && r != segmentRepeat; i++)
            {
                if (m_segments[i].Type.Equals(segmentType))
                {
                    index = i;
                    r++;
                }
            }

            if (r == segmentRepeat)
            {
                ret = At(index);
            }
            else
            {
                index = m_segments.Count();
                int diff = (segmentRepeat - r);
                for(int k = 0; k < diff; k++)
                {
                    ret = At(index);
                    ret.Type = segmentType;
                    index++;
                }
                    
            }
            return ret;
        }

        public override void Clear()
        {
            foreach (Segment s in m_segments)
                s.Clear();
            m_segments.Clear();

        }

        public override MessageObject Query(string q)
        {
            MessageObject ret = null;
            string pattern = @"^(?<Segment>(?<SegmentIndex>[0]|[1-9][0-9]{0,2})|((?<SegmentType>[A-Z][A-Z][A-Z1-9])(\[(?<SegmentRepeat>[1-9]\d{0,2})])?))(([-\.])(?<FieldQuery>((?<Field>[1-9]\d{0,2})(\[(?<FieldRepeat>[1-9]\d{0,2})])?)(([-\.])((?<Component>[1-9]\d{0,2})(([-\.])(?<Subcomponent>[1-9]\d{0,2}))?))?))?$";
            
            MatchCollection matches = Regex.Matches(q, pattern);
            if (matches.Count == 1)
            {
                GroupCollection groups;
                groups = matches[0].Groups;

                string segmentType = groups["SegmentType"].Value;
                int segmentRepeat = (String.IsNullOrEmpty(groups["SegmentRepeat"].Value)) ? 1 : Int32.Parse(groups["SegmentRepeat"].Value);
                int segmentIndex = (String.IsNullOrEmpty(groups["SegmentIndex"].Value)) ? -1 : Int32.Parse(groups["SegmentIndex"].Value);
                
                if (String.IsNullOrEmpty(matches[0].Groups["FieldQuery"].Value))
                {
                    if (segmentIndex > -1)
                    {
                        ret = At(segmentIndex);
                    }
                    else
                    {
                        ret = At(segmentType, segmentRepeat);
                    }
                }
                else
                {
                    if (segmentIndex > -1)
                    {
                        ret = At(segmentIndex).Query(groups["FieldQuery"].Value); 
                    }
                    else
                    {
                        ret = At(segmentType, segmentRepeat).Query(groups["FieldQuery"].Value);
                    }
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

            List<string> segments = SplitIntoSegments(str);
            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i].Length > 0)
                {
                    Segment s = new Segment("", m_delimiters);
                    s.AsString = segments[i];
                    m_segments.Add(s);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            if (m_segments.Count > 0)
            {
                ret.Append(m_segments[0].AsString);

                for (int i = 1; i < m_segments.Count(); i++)
                {
                    ret.AppendFormat("\n{0}",m_segments[i].AsString);
                }
            }
            return ret.ToString();

        }

        private List<string> SplitIntoSegments(string msg)
        {
            List<string> segments = new List<string>();

            StringBuilder currSegment = new StringBuilder();

            char prev = '\0';

            for (int i = 0; i <= msg.Length; i++)
            {
                if (i == msg.Length || (msg[i].Equals(m_delimiters.SegmentDelimiter) && !prev.Equals('\\')))
                {
                    segments.Add(currSegment.ToString());
                    currSegment.Clear();
                    prev = '\0';
                }
                else
                {
                    currSegment.Append(msg[i]);
                    prev = msg[i];
                }
            }
            return segments;
        }


    }
}
