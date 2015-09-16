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

using javax.xml.transform;
using net.sf.saxon.evpull;
using net.sf.saxon.om;
using net.sf.saxon.pattern;
using net.sf.saxon.tree.iter;
using net.sf.saxon.tree.util;
using net.sf.saxon.tree.wrapper;
using System;
using System.Xml.Linq;

namespace ZanyAnts.Saxon.XDoc
{
    public abstract class XObjectWrapper : AbstractNodeWrapper, SteppingNode, NodeInfo, Source, Item, Sequence, PullEvent, SiblingCountingNode
    {
        public virtual SteppingNode getFirstChild()
        {
            return null;
        }
        public abstract SteppingNode getNextSibling();
        public abstract SteppingNode getPreviousSibling();
        /**
         * Get the index position of this node among its siblings (starting from 0).
         * In the case of a text node that maps to several adjacent siblings in the DOM,
         * the numbering actually refers to the position of the underlying DOM nodes;
         * thus the sibling position for the text node is that of the first DOM node
         * to which it relates, and the numbering of subsequent XPath nodes is not necessarily
         * consecutive.
         */
        public abstract int getSiblingPosition();
        public abstract SteppingNode getSuccessorElement(SteppingNode sn, string str1, string str2);
        SteppingNode SteppingNode.getParent()
        {
            return (SteppingNode)getParent();
        }
        public static XObjectWrapper MakeWrapper(XDocument obj)
        {
            return new XDocumentWrapper(obj);
        }
        public static XObjectWrapper MakeWrapper(XAttribute obj)
        {
            return new XAttributeWrapper(obj);
        }
        public static XObjectWrapper MakeWrapper(XCData obj)
        {
            return new XCDataWrapper(obj);
        }
        public static XObjectWrapper MakeWrapper(XComment obj)
        {
            return new XCommentWrapper(obj);
        }
        public static XObjectWrapper MakeWrapper(XElement obj)
        {
            return new XElementWrapper(obj);
        }
        public static XObjectWrapper MakeWrapper(XProcessingInstruction obj)
        {
            return new XProcessingInstructionWrapper(obj);
        }
        public static XObjectWrapper MakeWrapper(XText obj)
        {
            if (obj is XCData)
                return MakeWrapper((XCData)obj);
            else
                return new XTextWrapper(obj);
        }
        public static XObjectWrapper MakeWrapper(XObject obj)
        {
            if (obj is XAttribute)
                return MakeWrapper((XAttribute)obj);

            if (obj is XComment)
                return MakeWrapper((XComment)obj);

            if (obj is XProcessingInstruction)
                return MakeWrapper((XProcessingInstruction)obj);

            if (obj is XText)
                return MakeWrapper((XText)obj);

            if (obj is XElement)
                return MakeWrapper((XElement)obj);

            if (obj is XDocument)
                return MakeWrapper((XDocument)obj);

            throw new NotSupportedException(obj.GetType().FullName);
        }
    }
    public abstract class XObjectWrapper<TXObject> : XObjectWrapper
        where TXObject : XObject
    {
        protected TXObject _node;
        int? _namecode;

        /**
             * Find the next matching element in document order; that is, the first child element
             * with the required name if there is one; otherwise the next sibling element
             * if there is one; otherwise the next sibling element of the parent, grandparent, etc, up to the anchor element.
             *
             * @param anchor the root of the tree within which navigation is confined
             * @param uri    the required namespace URI, or null if any namespace is acceptable
             * @param local  the required local name, or null if any local name is acceptable
             * @return the next element after this one in document order, with the given URI and local name
             *         if specified, or null if this is the last node in the document, or the last node
             *         within the subtree being navigated
             */
        override public SteppingNode getSuccessorElement(SteppingNode anchor, string uri, string local)
        {
            // TODO
            throw new NotImplementedException();
        }
        public override bool isSameNodeInfo(NodeInfo other)
        {
            // .NET reference equality can be used instead of the less efficient base implementation.
            return ReferenceEquals((other as XObjectWrapper)?.getUnderlyingNode(), _node);
        }
        /**
         * The equals() method compares nodes for identity. It is defined to give the same result
         * as isSameNodeInfo().
         *
         * @param other the node to be compared with this node
         * @return true if this NodeInfo object and the supplied NodeInfo object represent
         *         the same node in the tree.
         * @since 8.7 Previously, the effect of the equals() method was not defined. Callers
         *        should therefore be aware that third party implementations of the NodeInfo interface may
         *        not implement the correct semantics. It is safer to use isSameNodeInfo() for this reason.
         *        The equals() method has been defined because it is useful in contexts such as a Java Set or HashMap.
         */
        public override bool equals(object other)
        {
            return isSameNodeInfo(other as NodeInfo);
        }
        /**
         * The hashCode() method obeys the contract for hashCode(): that is, if two objects are equal
         * (represent the same node) then they must have the same hashCode()
         *
         * @since 8.7 Previously, the effect of the equals() and hashCode() methods was not defined. Callers
         *        should therefore be aware that third party implementations of the NodeInfo interface may
         *        not implement the correct semantics.
         */
        override public int hashCode()
        {
            return _node.GetHashCode();
        }
        public override object getUnderlyingNode()
        {
            return _node;
        }        
        public override DocumentInfo getDocumentRoot()
        {
            return _node.Document?.Annotation<XDocumentWrapper>();
        }
        abstract public override int getNodeKind();
        public override NodeInfo getParent()
        {
            return _node.Parent?.Annotation<XObjectWrapper>();
        }
        /**
         * Get the prefix of the name of the node. This is defined only for elements and attributes.
         * If the node has no prefix, or for other kinds of node, return a zero-length string.
         * This implementation simply returns the prefix defined in the DOM model; this is not strictly
         * accurate in all cases, but is good enough for the purpose.
         *
         * @return The prefix of the name of the node.
         */
        public override string getPrefix()
        {
            return string.Empty;
        }
        /**
         * Get the URI part of the name of this node. This is the URI corresponding to the
         * prefix, or the URI of the default namespace if appropriate.
         *
         * @return The URI of the namespace of this node. For an unnamed node,
         *         or for a node with an empty prefix, return an empty
         *         string.
         */
        public override string getURI()
        {
            return string.Empty;
        }
        /**
         * Get the local part of the name of this node. This is the name after the ":" if any.
         *
         * @return the local part of the name. For an unnamed node, returns null, except for
         *         un unnamed namespace node, which returns "".
         */
        public override string getLocalPart()
        {
            return null;
        }
        // This method is overridden in derived classes where the operation is meaningful.
        protected override AxisIterator iterateAttributes(NodeTest nt)
        {            
            throw new NotSupportedException();
        }
        // This method is overridden in derived classes where the operation is meaningful.
        protected override AxisIterator iterateChildren(NodeTest nt)
        {
            throw new NotSupportedException();
        }
        // This method is overridden in derived classes where the operation is meaningful.
        protected override AxisIterator iterateDescendants(NodeTest nt, bool b)
        {
            throw new NotSupportedException();
        }
        // This method is overridden in derived classes where the operation is meaningful.
        protected override AxisIterator iterateSiblings(NodeTest nt, bool b)
        {
            throw new NotSupportedException();
        }

        public override int compareOrder(NodeInfo other)
        {
            // Emulate DotNetNodeWrapper:
            if (other is SiblingCountingNode)
            {
                return Navigator.compareOrder(this, (SiblingCountingNode)other);
            }
            else
            {
                return -other.compareOrder(this);
            }
        }

        public override void generateId(FastStringBuffer buffer)
        {
            // Emulate DotNetNodeWrapper:
            Navigator.appendSequentialKey(this, buffer, true);
        }
        virtual protected int GenerateNameCode()
        {
            return -1;
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
        override public int getNameCode()
        {
            if (!_namecode.HasValue)
                _namecode = GenerateNameCode();

            return _namecode.Value;           
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
            return string.Empty;
        }
        public override bool hasChildNodes()
        {
            return false;
        }
        /**
         * Copy this node to a given outputter (deep copy)
         */
        override public void copy(net.sf.saxon.@event.Receiver output, int copyOptions, int locationId)
        {
            net.sf.saxon.@event.Receiver r = new net.sf.saxon.@event.NamespaceReducer(output);
            Navigator.copy(this, r, copyOptions, locationId);
        }
        public override string getBaseURI()
        {
            return _node.BaseUri;
        }
    }
}
