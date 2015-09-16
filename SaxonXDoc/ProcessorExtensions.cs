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
using System.Xml;
using System.Xml.Linq;

namespace ZanyAnts.Saxon.XDoc
{
    public static class ProcessorExtensions
    {
        /// <summary>
        /// Wrap an existing <see cref="XDocument"/> as an <see cref="XdmNode"/>. This is the <see cref="XDocument"/>
        /// equivalent of <see cref="DocumentBuilder.Wrap(XmlDocument)"/>.
        /// </summary>
        /// <remarks>
        /// PoC:
        /// 
        /// Creates wrapper objects for all nodes in the document graph and stores them using 
        /// <see cref="XObject.AddAnnotation(object)"/>. Will throw if any node has already been wrapped.
        /// 
        /// Idealy this would be an extension to <see cref="DocumentBuilder"/>, but DocumentBuilder does not expose
        /// its Configuration object publically.
        /// </remarks>
        public static XdmNode Wrap(this Processor processor, XDocument doc)
        {
            if (doc.Annotation<XObjectWrapper>() != null)
                throw new InvalidOperationException("XDocument is already annotated with a wrapper.");
            var docWrapper = (XDocumentWrapper)XObjectWrapper.MakeWrapper(doc);
            docWrapper.setConfiguration(processor.Implementation);
            doc.AddAnnotation(docWrapper);
            foreach (var node in doc.DescendantNodes())
            {
                if (node.Annotation<XObjectWrapper>() != null)
                    throw new InvalidOperationException(string.Format("{0} is already annotated with a wrapper.", node.GetType().Name));

                node.AddAnnotation(XObjectWrapper.MakeWrapper(node));

                if (node.NodeType == XmlNodeType.Element)
                {
                    foreach (var attr in ((XElement)node).Attributes())
                    {
                        if (attr.Annotation<XObjectWrapper>() != null)
                            throw new InvalidOperationException("Attribute is already annotated with a wrapper.");

                        attr.AddAnnotation(XObjectWrapper.MakeWrapper(attr));
                    }
                }
            }

            return (XdmNode)XdmValue.Wrap(docWrapper);
        }
    }
}
