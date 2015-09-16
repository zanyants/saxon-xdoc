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

using java.lang;
using net.sf.saxon.tree.util;
using System.Linq;
using System.Xml.Linq;
using SaxonType = net.sf.saxon.type.Type;

namespace ZanyAnts.Saxon.XDoc
{
    public class XAttributeWrapper : XObjectWrapper<XAttribute>
    {
        internal XAttributeWrapper( XAttribute obj)
        {
            _node = obj;
        }
        public override int getNodeKind()
        {
            return _node.IsNamespaceDeclaration ? SaxonType.NAMESPACE : SaxonType.ATTRIBUTE;
        }
        override public SteppingNode getNextSibling()
        {
            var n = _node.NextAttribute;

            while (n != null && n.IsNamespaceDeclaration != _node.IsNamespaceDeclaration)
                n = n.NextAttribute;
            
            return n?.Annotation<XObjectWrapper>();
        }

        override public SteppingNode getPreviousSibling()
        {
            var n = _node.PreviousAttribute;

            while (n != null && n.IsNamespaceDeclaration != _node.IsNamespaceDeclaration)
                n = n.PreviousAttribute;

            return n?.Annotation<XObjectWrapper>();
        }

        /**
 * Get the index position of this node among its siblings (starting from 0)
 *
 * @return 0 for the first child, 1 for the second child, etc.
 */
public override int getSiblingPosition()
        {
            // TODO: scope for optimisation
            return _node.Parent.Attributes().TakeWhile(a => a != _node).Count(a => a.IsNamespaceDeclaration == _node.IsNamespaceDeclaration);
        }
        public override CharSequence getStringValueCS()
        {
            return _node.Value;
        }
        public override string getPrefix()
        {
            return (_node.IsNamespaceDeclaration || _node.Parent == null) ? string.Empty : _node.Parent.GetPrefixOfNamespace(_node.Name.Namespace);
        }

        public override string getURI()
        {
            return _node.Name.NamespaceName;
        }
        /**
         * Get the local part of the name of this node. This is the name after the ":" if any.
         *
         * @return the local part of the name. For an unnamed node, returns null, except for
         *         un unnamed namespace node, which returns "".
         */
        public override string getLocalPart()
        {
            return _node.Name.LocalName;
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
            if (_node.IsNamespaceDeclaration)
                return string.Empty;

            var prefix = getPrefix();
            var sb = new System.Text.StringBuilder();
            if ( !string.IsNullOrWhiteSpace(prefix))
            {
                sb.Append(prefix);
                sb.Append(":");
            }
            sb.Append(_node.Name.LocalName);
            return sb.ToString();           
        }
    }
}
