/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Zany Ants Limited
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 */

using Saxon.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ZanyAnts.Saxon.XDoc.Demo
{
    /*
    Note: The XDoc library and this demo program have been developed purely as a rapid
    proof of concept (PoC). At the time of writing, the XDoc library has some unimplemented
    methods (these will throw), and is untested.
    */
    class Program
    {
        static int numIters = 1000;

        static void Main(string[] args)
        {
            if ( args.Length < 2)
            {
                Console.Error.WriteLine("Syntax: demo <path-to-xml-file> <xpath-expression> [<num-iterations(default={0})>]", numIters);
                return;
            }

            var file = args[0];
            var xpath = args[1];

            if ( args.Length > 2)
            {
                numIters = int.Parse(args[2]);
            }

            Console.WriteLine("Loading {0}", file);

            var proc = new Processor();

            var ms_xp = System.Xml.XPath.XPathExpression.Compile(xpath);

            var xpc = proc.NewXPathCompiler();
            var xpe = xpc.Compile(xpath);
            var sel = xpe.Load();

            var doc = XDocument.Load(file);
            var ctx = proc.Wrap(doc);
            sel.ContextItem = ctx;

            var nt = new NameTable();
            XmlDocument xd = new XmlDocument(nt);
            xd.Load(file);

            var ctxXmlDoc = proc.NewDocumentBuilder().Wrap(xd);

            Console.WriteLine("Evaluating {0}", xpath);

            Time(() => Saxon(sel), "XDoc (Saxon)");
            Time(() => Native(ms_xp, doc), "XDoc (Native)");

            sel.ContextItem = ctxXmlDoc;

            Time(() => Saxon(sel), "XmlDoc (Saxon)");
            Time(() => Native(ms_xp, xd), "XmlDoc (Native)");
        }

        static void Time( Func<int> a, string title )
        {
            var sw = Stopwatch.StartNew();
            var count = a();
            sw.Stop();
            Console.WriteLine("{0} took {1:0.000}ms (Count = {2})", title, sw.ElapsedMilliseconds,count);
        }
        
        private static int Native(XPathExpression ms_xp, XDocument doc)
        {
            var l = new List<int>();
            for (int i = 0; i < numIters; ++i)
            {
                l.Add(((XPathNodeIterator)doc.CreateNavigator().Evaluate(ms_xp)).Count);
            }
            return l.FirstOrDefault();
        }
        private static int Native(XPathExpression ms_xp, XmlDocument doc)
        {
            var l = new List<int>();
            for (int i = 0; i < numIters; ++i)
            {
                l.Add(((XPathNodeIterator)doc.CreateNavigator().Evaluate(ms_xp)).OfType<XPathNavigator>().Count());
            }
            return l.FirstOrDefault();
        }

        private static int Saxon(XPathSelector sel)
        {
            var l = new List<int>();
            for (int i = 0; i < numIters; ++i)
            {
                var result = sel.Evaluate();
                l.Add(result.OfType<XdmValue>().Count());
            }
            return l.FirstOrDefault();
        }
    }
}
