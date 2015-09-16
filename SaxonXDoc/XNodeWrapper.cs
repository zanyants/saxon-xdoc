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
using net.sf.saxon.pattern;
using net.sf.saxon.tree.iter;
using net.sf.saxon.tree.util;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ZanyAnts.Saxon.XDoc
{
    public abstract class XNodeWrapper<TXNode> : XObjectWrapper<TXNode>
            where TXNode : XNode
    {
        sealed override public SteppingNode getNextSibling()
        {
            return _node.NextNode?.Annotation<XObjectWrapper>();
        }
        sealed override public SteppingNode getPreviousSibling()
        {
            return _node.PreviousNode?.Annotation<XObjectWrapper>();
        }
        /**
         * Get the index position of this node among its siblings (starting from 0)
         *
         * @return 0 for the first child, 1 for the second child, etc.
         */
        sealed override public int getSiblingPosition()
        {
            // TODO: scope for optimisation
            return _node.NodesBeforeSelf().Count();
        }
        protected override AxisIterator iterateSiblings(NodeTest nt, bool forwards)
        {
            AxisIterator iter = new AxisIteratorAdapter(forwards ? _node.NodesAfterSelf() : _node.NodesBeforeSelf());
            if (nt != AnyNodeTest.getInstance())
                iter = new Navigator.AxisFilter(iter, nt);
            return iter;
        }
        public override CharSequence getStringValueCS()
        {
            return _node.CreateNavigator().Value;
        }
    }

}
