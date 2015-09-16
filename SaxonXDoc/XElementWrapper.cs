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

using net.sf.saxon.om;
using net.sf.saxon.pattern;
using net.sf.saxon.tree.iter;
using net.sf.saxon.tree.util;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SaxonType = net.sf.saxon.type.Type;

namespace ZanyAnts.Saxon.XDoc
{
    public sealed class XElementWrapper : XContainerWrapper<XElement>
    {
        internal XElementWrapper(XElement obj)
        {
            _node = obj;
        }
        public override int getNodeKind()
        {
            return SaxonType.ELEMENT;
        }
        public override string getPrefix()
        {
            return _node.GetPrefixOfNamespace(_node.Name.Namespace);
        }
        public override string getURI()
        {
            return _node.Name.Namespace.NamespaceName;
        }
        public override string getLocalPart()
        {
            return _node.Name.LocalName;
        }
        protected override AxisIterator iterateAttributes(NodeTest nt)
        {
            AxisIterator iter = new AxisIteratorAdapter(_node.Attributes());
            if (nt != AnyNodeTest.getInstance())
                iter = new Navigator.AxisFilter(iter, nt);
            return iter;
        }
        /**
         * Get name code. The name code is a coded form of the node name: two nodes
         * with the same name code have the same namespace URI, the same local name,
         * and the same prefix. By masking the name code with &0xfffff, you get a
         * fingerprint: two nodes with the same fingerprint have the same local name
         * and namespace URI.
         *
         * @see net.sf.saxon.om.NamePool#allocate allocate
         */
        protected override int GenerateNameCode()
        {
            return getNamePool().allocate(getPrefix() ?? string.Empty, getURI(), getLocalPart());
        }
        /**
         * Get the display name of this node. For elements and attributes this is [prefix:]localname.
         * For unnamed nodes, it is an empty string.
         *
         * @return The display name of this node.
         *         For a node with no name, return an empty string.
         */
        override public string getDisplayName()
        {
            var prefix = getPrefix();
            var sb = new System.Text.StringBuilder();
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                sb.Append(prefix);
                sb.Append(":");
            }
            sb.Append(_node.Name.LocalName);
            return sb.ToString();
        }
        /**
         * Get the string value of a given attribute of this node
         *
         * @param uri   the namespace URI of the attribute name. Supply the empty string for an attribute
         *              that is in no namespace
         * @param local the local part of the attribute name.
         * @return the attribute value if it exists, or null if it does not exist. Always returns null
         *         if this node is not an element.
         * @since 9.4
         */
        override public string getAttributeValue(/*@NotNull*/ string uri, /*@NotNull*/ string local)
        {
            return _node.Attribute(XName.Get(local, uri))?.Value;
        }
        /**
                    * Get all namespace declarations and undeclarations defined on this element.
                    *
                    * @param buffer If this is non-null, and the result array fits in this buffer, then the result
                    *               may overwrite the contents of this array, to avoid the cost of allocating a new array on the heap.                    
                    *         <p>For a node other than an element, the method returns null.</p>
                    */
        public override NamespaceBinding[] getDeclaredNamespaces(NamespaceBinding[] buffer)
        {
            // Comment and logic paraphrased from DotNetNodeWrapper:
            // Note: in a DOM created by the XML parser, all namespaces are present as attribute nodes. But
            // in a DOM created programmatically, this is not necessarily the case. So we need to add
            // namespace bindings for the namespace of the element and any attributes
            HashSet<NamespaceBinding> bindings = new HashSet<NamespaceBinding>();

            foreach ( var attr in _node.Attributes())
            {
                if (attr.IsNamespaceDeclaration)
                {
                    if (attr.Name.LocalName == "xmlns")
                        bindings.Add(new NamespaceBinding(string.Empty, attr.Value));
                    else
                        bindings.Add(new NamespaceBinding(attr.Name.LocalName, attr.Value));
                }
                else
                {
                    if (!string.IsNullOrEmpty(attr.Name.NamespaceName))
                        bindings.Add(new NamespaceBinding(_node.GetPrefixOfNamespace(attr.Name.Namespace), attr.Name.NamespaceName));
                }
            }

            if (!string.IsNullOrEmpty(_node.Name.NamespaceName))
                bindings.Add(new NamespaceBinding(_node.GetPrefixOfNamespace(_node.Name.Namespace), _node.Name.NamespaceName));

            return bindings.ToArray();
        }
    }
}