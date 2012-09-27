/*

Copyright dotNetRDF Project 2009-12
dotnetrdf-develop@lists.sf.net

------------------------------------------------------------------------

This file is part of dotNetRDF.

dotNetRDF is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

dotNetRDF is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with dotNetRDF.  If not, see <http://www.gnu.org/licenses/>.

------------------------------------------------------------------------

dotNetRDF may alternatively be used under the LGPL or MIT License

http://www.gnu.org/licenses/lgpl.html
http://www.opensource.org/licenses/mit-license.php

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using VDS.RDF.Query;

namespace VDS.RDF
{
    /// <summary>
    /// Class for representing RDF Graphs
    /// </summary>
    /// <threadsafety instance="false">Safe for multi-threaded read-only access but unsafe if one/more threads may modify the Graph by using the <see cref="Graph.Assert">Assert</see>, <see cref="Graph.Retract">Retract</see> or <see cref="BaseGraph.Merge">Merge</see> methods</threadsafety>
#if !SILVERLIGHT
    [Serializable,XmlRoot(ElementName="graph")]
#endif
    public class Graph 
        : BaseGraph
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of a Graph
        /// </summary>
        public Graph() 
            : base() { }

        /// <summary>
        /// Creates a new instance of a Graph with an optionally empty Namespace Map
        /// </summary>
        /// <param name="emptyNamespaceMap">Whether the Namespace Map should be empty</param>
        public Graph(bool emptyNamespaceMap)
            : this()
        {
            if (emptyNamespaceMap) this._nsmapper.Clear();
        }

        /// <summary>
        /// Creates a new instance of a Graph using the given Triple Collection
        /// </summary>
        /// <param name="tripleCollection">Triple Collection</param>
        public Graph(BaseTripleCollection tripleCollection)
            : base(tripleCollection) { }

        /// <summary>
        /// Creates a new instance of a Graph using the given Triple Collection and an optionally empty Namespace Map
        /// </summary>
        /// <param name="tripleCollection">Triple Collection</param>
        /// <param name="emptyNamespaceMap">Whether the Namespace Map should be empty</param>
        public Graph(BaseTripleCollection tripleCollection, bool emptyNamespaceMap)
            : base(tripleCollection)
        {
            if (emptyNamespaceMap) this._nsmapper.Clear();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Deserialization Constructor
        /// </summary>
        /// <param name="info">Serialization Information</param>
        /// <param name="context">Streaming Context</param>
        protected Graph(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif

        #endregion

        #region Triple Assertion & Retraction

        /// <summary>
        /// Asserts a Triple in the Graph
        /// </summary>
        /// <param name="t">The Triple to add to the Graph</param>
        public override bool Assert(Triple t)
        {
            //Add to Triples Collection
            if (this._triples.Add(t))
            {
                this.RaiseTripleAsserted(t);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Asserts a List of Triples in the graph
        /// </summary>
        /// <param name="ts">List of Triples in the form of an IEnumerable</param>
        public override bool Assert(IEnumerable<Triple> ts)
        {
            bool asserted = false;
            foreach (Triple t in ts)
            {
                asserted = this.Assert(t) || asserted;
            }
            return asserted;
        }

        /// <summary>
        /// Retracts a Triple from the Graph
        /// </summary>
        /// <param name="t">Triple to Retract</param>
        /// <remarks>Current implementation may have some defunct Nodes left in the Graph as only the Triple is retracted</remarks>
        public override bool Retract(Triple t)
        {
            if (this._triples.Delete(t))
            {
                this.RaiseTripleRetracted(t);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retracts a enumeration of Triples from the graph
        /// </summary>
        /// <param name="ts">Enumeration of Triples to retract</param>
        public override bool Retract(IEnumerable<Triple> ts)
        {
            bool retracted = false;
            foreach (Triple t in ts)
            {
                retracted = this.Retract(t) || retracted;
            }
            return retracted;
        }

        #endregion

        #region Node Selection

        /// <summary>
        /// Returns the UriNode with the given Uri if it exists
        /// </summary>
        /// <param name="uri">The Uri of the Node to select</param>
        /// <returns>Either the UriNode Or null if no Node with the given Uri exists</returns>
        public override IUriNode GetUriNode(Uri uri)
        {
            IUriNode test = new UriNode(this, uri);
            IEnumerable<IUriNode> us = from u in this.Nodes.UriNodes()
                                          where u.Equals(test)
                                          select u;
            return us.FirstOrDefault();
        }

        /// <summary>
        /// Returns the UriNode with the given QName if it exists
        /// </summary>
        /// <param name="qname">The QName of the Node to select</param>
        /// <returns></returns>
        public override IUriNode GetUriNode(String qname)
        {
            IUriNode test = new UriNode(this, qname);
            IEnumerable<IUriNode> us = from u in this.Nodes.UriNodes()
                                      where u.Equals(test)
                                      select u;
            return us.FirstOrDefault();
        }

        /// <summary>
        /// Returns the LiteralNode with the given Value if it exists
        /// </summary>
        /// <param name="literal">The literal value of the Node to select</param>
        /// <returns>Either the LiteralNode Or null if no Node with the given Value exists</returns>
        /// <remarks>The LiteralNode in the Graph must have no Language or DataType set</remarks>
        public override ILiteralNode GetLiteralNode(String literal)
        {
            ILiteralNode test = new LiteralNode(this, literal);
            IEnumerable<ILiteralNode> ls = from l in this.Nodes.LiteralNodes()
                                          where l.Equals(test)
                                          select l;
            return ls.FirstOrDefault();
        }

        /// <summary>
        /// Returns the LiteralNode with the given Value in the given Language if it exists
        /// </summary>
        /// <param name="literal">The literal value of the Node to select</param>
        /// <param name="langspec">The Language Specifier for the Node to select</param>
        /// <returns>Either the LiteralNode Or null if no Node with the given Value and Language Specifier exists</returns>
        public override ILiteralNode GetLiteralNode(String literal, String langspec)
        {
            ILiteralNode test = new LiteralNode(this, literal, langspec);
            IEnumerable<ILiteralNode> ls = from l in this.Nodes.LiteralNodes()
                                          where l.Equals(test)
                                          select l;
            return ls.FirstOrDefault();
        }

        /// <summary>
        /// Returns the LiteralNode with the given Value and given Data Type if it exists
        /// </summary>
        /// <param name="literal">The literal value of the Node to select</param>
        /// <param name="datatype">The Uri for the Data Type of the Literal to select</param>
        /// <returns>Either the LiteralNode Or null if no Node with the given Value and Data Type exists</returns>
        public override ILiteralNode GetLiteralNode(String literal, Uri datatype)
        {
            ILiteralNode test = new LiteralNode(this, literal, datatype);
            IEnumerable<ILiteralNode> ls = from l in this.Nodes.LiteralNodes()
                                          where l.Equals(test)
                                          select l;
            return ls.FirstOrDefault();
        }

        /// <summary>
        /// Returns the Blank Node with the given Identifier
        /// </summary>
        /// <param name="nodeId">The Identifier of the Blank Node to select</param>
        /// <returns>Either the Blank Node or null if no Node with the given Identifier exists</returns>
        public override IBlankNode GetBlankNode(String nodeId)
        {
            IEnumerable<IBlankNode> bs = from b in this.Nodes.BlankNodes()
                                        where b.InternalID.Equals(nodeId)
                                        select b;

            return bs.FirstOrDefault();
        }

        #endregion

        #region Triple Selection

        /// <summary>
        /// Gets all the Triples involving the given Node
        /// </summary>
        /// <param name="n">The Node to find Triples involving</param>
        /// <returns>Zero/More Triples</returns>
        public override IEnumerable<Triple> GetTriples(INode n)
        {
            return this.GetTriplesWithSubject(n).Concat(this.GetTriplesWithPredicate(n)).Concat(this.GetTriplesWithObject(n));
        }

        /// <summary>
        /// Gets all the Triples involving the given Uri
        /// </summary>
        /// <param name="uri">The Uri to find Triples involving</param>
        /// <returns>Zero/More Triples</returns>
        public override IEnumerable<Triple> GetTriples(Uri uri)
        {
            IEnumerable<Triple> ts = from t in this._triples
                                     where t.Involves(uri)
                                     select t;

            return ts;
        }


        /// <summary>
        /// Gets all the Triples with the given Node as the Subject
        /// </summary>
        /// <param name="n">The Node to find Triples with it as the Subject</param>
        /// <returns>Zero/More Triples</returns>
        public override IEnumerable<Triple> GetTriplesWithSubject(INode n)
        {
            return this._triples.WithSubject(n);
        }

        /// <summary>
        /// Gets all the Triples with the given Uri as the Subject
        /// </summary>
        /// <param name="u">The Uri to find Triples with it as the Subject</param>
        /// <returns>Zero/More Triples</returns>
        public override IEnumerable<Triple> GetTriplesWithSubject(Uri u)
        {
            return this._triples.WithSubject(this.CreateUriNode(u));
        }

        /// <summary>
        /// Gets all the Triples with the given Node as the Predicate
        /// </summary>
        /// <param name="n">The Node to find Triples with it as the Predicate</param>
        /// <returns></returns>
        public override IEnumerable<Triple> GetTriplesWithPredicate(INode n)
        {
            return this._triples.WithPredicate(n);
        }

        /// <summary>
        /// Gets all the Triples with the given Uri as the Predicate
        /// </summary>
        /// <param name="u">The Uri to find Triples with it as the Predicate</param>
        /// <returns>Zero/More Triples</returns>
        public override IEnumerable<Triple> GetTriplesWithPredicate(Uri u)
        {
            return this._triples.WithPredicate(this.CreateUriNode(u));
        }

        /// <summary>
        /// Gets all the Triples with the given Node as the Object
        /// </summary>
        /// <param name="n">The Node to find Triples with it as the Object</param>
        /// <returns></returns>
        public override IEnumerable<Triple> GetTriplesWithObject(INode n)
        {
            return this._triples.WithObject(n);
        }

        /// <summary>
        /// Gets all the Triples with the given Uri as the Object
        /// </summary>
        /// <param name="u">The Uri to find Triples with it as the Object</param>
        /// <returns>Zero/More Triples</returns>
        public override IEnumerable<Triple> GetTriplesWithObject(Uri u)
        {
            return this._triples.WithObject(this.CreateUriNode(u));
        }

        /// <summary>
        /// Selects all Triples with the given Subject and Predicate
        /// </summary>
        /// <param name="subj">Subject</param>
        /// <param name="pred">Predicate</param>
        /// <returns></returns>
        public override IEnumerable<Triple> GetTriplesWithSubjectPredicate(INode subj, INode pred)
        {
            return this._triples.WithSubjectPredicate(subj, pred);
        }

        /// <summary>
        /// Selects all Triples with the given Subject and Object
        /// </summary>
        /// <param name="subj">Subject</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        public override IEnumerable<Triple> GetTriplesWithSubjectObject(INode subj, INode obj)
        {
            return this._triples.WithSubjectObject(subj, obj);
        }

        /// <summary>
        /// Selects all Triples with the given Predicate and Object
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        public override IEnumerable<Triple> GetTriplesWithPredicateObject(INode pred, INode obj)
        {
            return this._triples.WithPredicateObject(pred, obj);
        }

        #endregion
    }

    /// <summary>
    /// Class for representing RDF Graphs when you don't want Indexing
    /// </summary>
    /// <remarks>
    /// Gives better load performance but poorer lookup performance
    /// </remarks>
    public class NonIndexedGraph : Graph
    {
        /// <summary>
        /// Creates a new Graph which is not indexed
        /// </summary>
        public NonIndexedGraph()
            : base(new TripleCollection()) { }
    }
}