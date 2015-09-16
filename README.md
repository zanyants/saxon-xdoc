# saxon-xdoc
Proof of concept Saxon .NET API XDM implementation for LINQ to XML (use Saxon .NET with wrapped XDocument)

*Note: This is a working proof of concept. However, a couple of things are not implemented, and it is not tested.*

### Summary

saxon-xdoc allows you to use XDocument (and related) objects directly with Saxon's .NET implementation without converting to XmlDocument. For example, you can use Saxon's XPath engine to select nodes from an XDocument, and access the actual underlying XNode objects selected.

### Proof of Concept

This project was developed rapidly as a proof of concept to determine if it was feasable to use Saxon .NET with the LINQ to XML object model without converting to and fro to another object model (Saxon .NET only supports XmlDocument internally by default). And yes, it is feasable, and performance with this first-draft, unoptimized proof of concept is similar to using Saxon .NET with XmlDocument. Further work will be put into this project if we move forwards from proof of concept to production code. In the mean time, the proof of concept may be of interest to others, so we've published it.

### Unsupported features

`public NodeInfo XDocumentWrapper::selectID(string id, bool getParent)`

XDocument has no equivalent of XmlDocument.GetElementById. To implement this function requires DTD processing - we need to know which attributes are of xs:ID type as indicated by DTD (or, in theory, some other form of schema). This might be possible, but is not of interest for our own use, and is unlikely that we will implement this feature.

### To do

`public SteppingNode XObjectWrapper<TXObject>::getSuccessorElement(SteppingNode anchor, string uri, string local)`

This method is currently not implemented, simply because it is non-trivial and was not requried to complete the proof of concept evaluation. Any XPath expressions that require this method will throw.
