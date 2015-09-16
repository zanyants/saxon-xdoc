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

using java.util;
using javax.xml.transform;
using net.sf.saxon;
using net.sf.saxon.evpull;
using net.sf.saxon.om;
using net.sf.saxon.type;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SaxonType = net.sf.saxon.type.Type;

namespace ZanyAnts.Saxon.XDoc
{
    public sealed class XDocumentWrapper : XContainerWrapper<XDocument>, DocumentInfo, NodeInfo, Source, Item, Sequence, PullEvent
    {
        static readonly XName XmlID = XName.Get("ID", "http://www.w3.org/XML/1998/namespace");
        Configuration _config;
        //string _baseUri;
        long _documentNumber;
        Dictionary<string, object> _userData;

        internal XDocumentWrapper(XDocument obj)
        {
            _node = obj;
            _userData = new Dictionary<string, object>();
        }
        public override int getNodeKind()
        {
            return SaxonType.DOCUMENT;
        }
        public override NamePool getNamePool()
        {
            return _config.getNamePool();
        }
        public void setConfiguration(Configuration config)
        {
            _config = config;
            _documentNumber = config.getDocumentNumberAllocator().allocateDocumentNumber();
        }
        public override Configuration getConfiguration()
        {
            return _config;
        }
        public override long getDocumentNumber()
        {
            return _documentNumber;
        }
        public override string getSystemId()
        {
            return _node.BaseUri;
        }
        /**
         * Get the unparsed entity with a given name
         *
         * @param name the name of the entity
         * @return null: JDOM does not provide access to unparsed entities
         */
        public string[] getUnparsedEntity(string str)
        {
            // Emulate DotNetDocumentWrapper:
            return null;
        }
        /**
         * Get the list of unparsed entities defined in this document
         *
         * @return an Iterator, whose items are of type String, containing the names of all
         *         unparsed entities defined in this document. If there are no unparsed entities or if the
         *         information is not available then an empty iterator is returned
         */
        public Iterator getUnparsedEntityNames()
        {
            // Emulate DotNetDocumentWrapper:
            var ls = Collections.emptyList();
            return ls.iterator();
        }

        /**
         * Ask whether the document contains any nodes whose type annotation is anything other than
         * UNTYPED
         *
         * @return true if the document contains elements whose type is other than UNTYPED
         */
        public bool isTyped()
        {
            // Emulate DotNetDocumentWrapper:
            return false;
        }
        /**
         * Get the element with a given ID, if any
         *
         * @param id        the required ID value
         * @param getParent true if running the element-with-id() function rather than the id()
         *                  function; the difference is that in the case of an element of type xs:ID, the parent of
         *                  the element should be returned, not the element itself.
         * @return a NodeInfo representing the element with the given ID, or null if there
         *         is no such element. This implementation does not necessarily conform to the
         *         rule that if an invalid document contains two elements with the same ID, the one
         *         that comes last should be returned.
         */
        public NodeInfo selectID(string id, bool getParent)
        {
            // XDocument has no equivalent of XmlDocument.GetElementById. To implement this function
            // requires DTD processing - we need to know which attributes are of xs:ID type as indicated
            // by DTD (or, in theory, some other form of schema).
            // This might be worth looking at: 
            //   System.Xml.Schema.Extensions.GetSchemaInfo( XAttribute )
            throw new NotImplementedException();            
        }
        /**
         * Get user data held in the document node. This retrieves properties previously set using
         * {@link #setUserData}
         *
         * @param key A string giving the name of the property to be retrieved.
         * @return the value of the property, or null if the property has not been defined.
         */
        public object getUserData(string str)
        {
            object result;
            _userData.TryGetValue(str, out result);
            return result;
        }
        /**
         * Set user data on the document node. The user data can be retrieved subsequently
         * using {@link #getUserData}
         *
         * @param key   A string giving the name of the property to be set. Clients are responsible
         *              for choosing a key that is likely to be unique. Must not be null. Keys used internally
         *              by Saxon are prefixed "saxon:".
         * @param value The value to be set for the property. May be null, which effectively
         *              removes the existing value for the property.
         */
        public void setUserData(string str, object obj)
        {
            if (obj == null)
                _userData.Remove(str);
            else
                _userData[str] = obj;
        }
        /**
         * Get the type annotation. Always XS_UNTYPED.
         */
        public override int getTypeAnnotation()
        {
            // Emulate DotNetDocumentWrapper:
            return StandardNames.XS_UNTYPED;
        }
        /**
         * Get the type annotation of this node, if any. The type annotation is represented as
         * SchemaType object.
         * <p/>
         * <p>Types derived from a DTD are not reflected in the result of this method.</p>
         *
         * @return For element and attribute nodes: the type annotation derived from schema
         *         validation (defaulting to xs:untyped and xs:untypedAtomic in the absence of schema
         *         validation). For comments, text nodes, processing instructions, and namespaces: null.
         *         For document nodes, either xs:untyped if the document has not been validated, or
         *         xs:anyType if it has.
         * @since 9.4
         */
        public override SchemaType getSchemaType()
        {
            // Emulate DotNetDocumentWrapper:
            return Untyped.getInstance();
        }
    }
}
