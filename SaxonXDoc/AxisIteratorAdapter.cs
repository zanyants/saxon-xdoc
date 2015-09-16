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
using net.sf.saxon.tree.iter;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ZanyAnts.Saxon.XDoc
{
    /// <summary>
    /// Adapts an <see cref="IEnumerable{T}"/> of <see cref="XObject"/> as an <see cref="AxisIterator"/>
    /// </summary>
    sealed public class AxisIteratorAdapter : AxisIterator, LookaheadIterator
    {
        IEnumerable<XObject> _collection;
        IEnumerator<XObject> _enumerator;
        bool _next;
        public AxisIteratorAdapter(IEnumerable<XObject> collection)
        {
            _collection = collection;
            _enumerator = _collection.GetEnumerator();
            _next = _enumerator.MoveNext();
        }

        public void close()
        {            
        }

        public AxisIterator getAnother()
        {
            return new AxisIteratorAdapter(_collection);
        }
        public int getProperties()
        {
            return SequenceIterator.LOOKAHEAD;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool hasNext()
        {
            return _next;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NodeInfo next()
        {
            if (!_next)
                return null;
            var ret = _enumerator.Current;
            _next = _enumerator.MoveNext();
            return ret?.Annotation<XObjectWrapper>();
        }
        SequenceIterator SequenceIterator.getAnother()
        {
            return getAnother();
        }

        UnfailingIterator UnfailingIterator.getAnother()
        {
            return getAnother();
        }

        Item SequenceIterator.next()
        {
            return next();
        }

        Item UnfailingIterator.next()
        {
            return next();
        }
    }
}
