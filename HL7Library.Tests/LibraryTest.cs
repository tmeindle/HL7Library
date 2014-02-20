using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HL7Library;

namespace HL7Library.Tests
{
    [TestFixture]
    public class LibraryTest
    {
        [Test]
        public void SubcomponentTests()
        {
            Subcomponent s = new Subcomponent();

            //test getting and setting the value
            s.FromString("Value1");
            Assert.AreEqual(s.AsString, "Value1");
            
            s.AsString = "Value2";
            Assert.AreEqual(s.ToString(), "Value2");
            
            //test that clearing works
            s.Clear();
            Assert.AreEqual(s.ToString(), String.Empty);

            //subcomponents queries always return nothing
            Assert.IsNull(s.Query(""));
        }

        [Test]
        public void ComponentTests()
        {
            Component c = new Component();

            //test getting and setting the value
            c.FromString("One");
            Assert.AreEqual(c.AsString, "One");
            Assert.AreEqual(c.Query("1").AsString == "One", true);

            c.AsString = "One&Two";
            Assert.AreEqual(c.ToString(), "One&Two");

            Assert.AreEqual(c.Query("1").AsString == "One", true);
            Assert.AreEqual(c.Query("2").AsString, "Two");
            
            //test that clearing works
            c.Clear();
            Assert.AreEqual(c.ToString(), String.Empty);
        }

        [Test]
        public void FieldTests()
        {
            Field f = new Field();
            
            //test getting and setting the value
            f.AsString = "c1^c2^c3.1&c3.2^^^&c6.2";
            Assert.AreEqual(f.AsString, "c1^c2^c3.1&c3.2^^^&c6.2");
            
            //test various queries for data
            Assert.AreEqual(f.Query("1").AsString, "c1");
            Assert.AreEqual(f["1.1"].AsString, "c1");
            Assert.AreEqual(f.Query("3.1").AsString, "c3.1");
            Assert.AreEqual(f.Query("5").AsString, "");
            Assert.AreEqual(f.Query("6.1").AsString, "");
            Assert.AreEqual(f.Query("6.2").AsString, "c6.2");

            Assert.AreEqual(f.Query("6") is Component, true);
            Assert.AreEqual(f.Query("6.2") is Subcomponent, true);
            
            //test that clearing works
            f.Clear();
            Assert.AreEqual(f.ToString(), String.Empty);
        }

        [Test]
        public void SegmentTests()
        {
            Segment s = new Segment();

            //test getting and setting the value
            s.AsString = "PID|^Smith&&Joe||04/07/1991|1021 22nd St.&Beverly Hills&CA&90210~999 33rd St.&Somewhere Else&NY&10010||";
            Assert.AreEqual(s.AsString, "PID|^Smith&&Joe||04/07/1991|1021 22nd St.&Beverly Hills&CA&90210~999 33rd St.&Somewhere Else&NY&10010");

            //test various queries for data
            Assert.AreEqual(s["1.2.1"].AsString, "Smith");
            Assert.AreEqual(s["1.2.2"].AsString, "");
            Assert.AreEqual(s["1.2.3"].AsString, "Joe");
            Assert.AreEqual(s["4[1].1.3"].AsString, "CA");
            Assert.AreEqual(s["4[1].1.4"].AsString, "90210");
            Assert.AreEqual(s["4[2].1.3"].AsString, "NY");
            
            //test that clearing works
            s.Clear();
            Assert.AreEqual(s.ToString(), String.Empty);
        }

        [Test]
        public void MessageTests()
        {
            Message m = new Message();

            //test setting the value
            m.AsString = "MSH|1|2|3|4.1.1~4[2].1.1^&4[2].2.2|||7.1^7.2.1&7.2.2||\rPID|^Smith&&Joe||04/07/1991|1021 22nd St.&Beverly Hills&CA&90210~999 33rd St.&Somewhere Else&NY&10010||";
            

            //test various queries for data
            Assert.AreEqual(m["MSH-1.1"].AsString, "1");
            Assert.AreEqual(m["MSH-1.1"].AsString, "1");
            Assert.AreEqual(m["MSH-4[2].1.1"].AsString, "4[2].1.1");
            Assert.AreEqual(m["MSH-4[2].2.2"].AsString, "4[2].2.2");
            
            //test that clearing works
            m.Clear();
            Assert.AreEqual(m.ToString(), String.Empty);
        }


    
    }
}
