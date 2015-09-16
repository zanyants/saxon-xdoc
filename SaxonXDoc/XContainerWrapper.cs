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

using net.sf.saxon.pattern;
using net.sf.saxon.tree.iter;
using net.sf.saxon.tree.util;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ZanyAnts.Saxon.XDoc
{
    public abstract class XContainerWrapper<TXContainer> : XNodeWrapper<TXContainer>
        where TXContainer : XContainer
    {
        sealed override public SteppingNode getFirstChild()
        {            
            return _node.FirstNode?.Annotation<XObjectWrapper>();
        }
        protected override AxisIterator iterateChildren(NodeTest nt)
        {
            AxisIterator iter = new AxisIteratorAdapter(_node.Nodes());
            if (nt != AnyNodeTest.getInstance())
                iter = new Navigator.AxisFilter(iter, nt);
            return iter;
        }
        protected override AxisIterator iterateDescendants(NodeTest nt, bool includeSelf)
        {
            IEnumerable<XNode> collection = includeSelf ? Enumerable.Repeat(_node, 1).Concat(_node.DescendantNodes()) : _node.DescendantNodes();
            AxisIterator iter = new AxisIteratorAdapter(collection);
            if (nt != AnyNodeTest.getInstance())
                iter = new Navigator.AxisFilter(iter, nt);
            return iter;
        }
        public override bool hasChildNodes()
        {
            return _node.Nodes().Any();
        }
    }
}
