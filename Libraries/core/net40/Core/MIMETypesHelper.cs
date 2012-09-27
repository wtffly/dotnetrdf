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
using System.IO;
using System.Linq;
using System.Text;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Update;
using VDS.RDF.Writing;

namespace VDS.RDF
{
    /// <summary>
    /// Helper Class containing arrays of MIME Types for the various RDF Concrete Syntaxes and Content Negotation Methods
    /// </summary>
    /// <remarks>
    /// The first type in each array is the canonical type that should be used
    /// </remarks>
    public class MimeTypesHelper
    {
        /// <summary>
        /// Constant for Valid MIME Types
        /// </summary>
        internal const String ValidMimeTypePattern = @"[\w\-]+/\w+(-\w+)*";

        /// <summary>
        /// Constant for W3C File Formats Namespace
        /// </summary>
        private const String W3CFormatsNamespace = "http://www.w3.org/ns/formats/";

        #region Initialisation of MIME Type Definitions

        /// <summary>
        /// List of MIME Type Definition
        /// </summary>
        private static List<MimeTypeDefinition> _mimeTypes;
        /// <summary>
        /// Whether MIME Type Definitions have been initialised
        /// </summary>
        private static bool _init = false;
        private static Object _initLock = new Graph();

        /// <summary>
        /// Initialises the MIME Type definitions
        /// </summary>
        private static void Init()
        {
            lock (_initLock)
            {
                if (!_init)
                {
                    _mimeTypes = new List<MimeTypeDefinition>();

                    //Define NTriples
                    MimeTypeDefinition ntriples = new MimeTypeDefinition("NTriples", W3CFormatsNamespace + "N-Triples", NTriples, new String[] { DefaultNTriplesExtension }, typeof(NTriplesParser), null, null, typeof(NTriplesWriter), null, null);
#if !SILVERLIGHT
                    ntriples.Encoding = Encoding.ASCII;
#endif
                    _mimeTypes.Add(ntriples);
#if !NO_COMPRESSION
                    MimeTypeDefinition ntriplesGZipped = new MimeTypeDefinition("GZipped NTriples", NTriples, new String[] { DefaultNTriplesExtension + "." + DefaultGZipExtension }, typeof(GZippedNTriplesParser), null, null, typeof(GZippedNTriplesWriter), null, null);
#if !SILVERLIGHT
                    ntriplesGZipped.Encoding = Encoding.ASCII;
#endif
                    _mimeTypes.Add(ntriplesGZipped);
#endif

                    //Define Turtle
                    _mimeTypes.Add(new MimeTypeDefinition("Turtle", W3CFormatsNamespace + "Turtle", Turtle, new String[] { DefaultTurtleExtension }, typeof(TurtleParser), null, null, typeof(CompressingTurtleWriter), null, null));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped Turtle", Turtle, new String[] { DefaultTurtleExtension + "." + DefaultGZipExtension }, typeof(GZippedTurtleParser), null, null, typeof(GZippedTurtleWriter), null, null));
#endif

                    //Define Notation 3
                    _mimeTypes.Add(new MimeTypeDefinition("Notation 3", W3CFormatsNamespace + "N3", Notation3, new String[] { DefaultNotation3Extension }, typeof(Notation3Parser), null, null, typeof(Notation3Writer), null, null));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped Notation 3", Notation3, new String[] { DefaultNotation3Extension + "." + DefaultGZipExtension }, typeof(GZippedNotation3Parser), null, null, typeof(GZippedNotation3Writer), null, null));
#endif

                    //Define NQuads
                    _mimeTypes.Add(new MimeTypeDefinition("NQuads", NQuads, new String[] { DefaultNQuadsExtension }, null, typeof(NQuadsParser), null, null, typeof(NQuadsWriter), null));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped NQuads", NQuads, new String[] { DefaultNQuadsExtension + "." + DefaultGZipExtension }, null, typeof(GZippedNQuadsParser), null, null, typeof(GZippedNQuadsWriter), null));
#endif

                    //Define TriG
                    _mimeTypes.Add(new MimeTypeDefinition("TriG", TriG, new String[] { DefaultTriGExtension }, null, typeof(TriGParser), null, null, typeof(TriGWriter), null));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped TriG", TriG, new String[] { DefaultTriGExtension + "." + DefaultGZipExtension }, null, typeof(GZippedTriGParser), null, null, typeof(GZippedTriGWriter), null));
#endif

                    //Define TriX
                    _mimeTypes.Add(new MimeTypeDefinition("TriX", TriX, new String[] { DefaultTriXExtension }, null, typeof(TriXParser), null, null, typeof(TriXWriter), null));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped TriX", TriX, new String[] { DefaultTriXExtension + "." + DefaultGZipExtension }, null, typeof(GZippedTriXParser), null, null, typeof(GZippedTriXWriter), null));
#endif

                    //Define SPARQL Results XML
                    _mimeTypes.Add(new MimeTypeDefinition("SPARQL Results XML", W3CFormatsNamespace + "SPARQL_Results_XML", SparqlResultsXml, new String[] { DefaultSparqlXmlExtension }, null, null, typeof(SparqlXmlParser), null, null, typeof(SparqlXmlWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped SPARQL Results XML", SparqlResultsXml, new String[] { DefaultSparqlXmlExtension + "." + DefaultGZipExtension }, null, null, typeof(GZippedSparqlXmlParser), null, null, typeof(GZippedSparqlXmlWriter)));
#endif

                    //Define SPARQL Results JSON
                    _mimeTypes.Add(new MimeTypeDefinition("SPARQL Results JSON", W3CFormatsNamespace + "SPARQL_Results_JSON", SparqlResultsJson, new String[] { DefaultSparqlJsonExtension, DefaultJsonExtension }, null, null, typeof(SparqlJsonParser), null, null, typeof(SparqlJsonWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped SPARQL Results JSON", SparqlResultsJson, new String[] { DefaultSparqlJsonExtension + "." + DefaultGZipExtension, DefaultJsonExtension + "." + DefaultGZipExtension }, null, null, typeof(GZippedSparqlJsonParser), null, null, typeof(GZippedSparqlJsonWriter)));
#endif

                    //Define SPARQL Boolean
                    _mimeTypes.Add(new MimeTypeDefinition("SPARQL Boolean Result", SparqlResultsBoolean, Enumerable.Empty<String>(), null, null, typeof(SparqlBooleanParser), null, null, null));

                    //Define RDF/XML - include SPARQL Parsers to support servers that send back incorrect MIME Type for SPARQL XML Results
                    //We define this after SPARQL Results XML to ensure we favour the correct MIME type for it
                    _mimeTypes.Add(new MimeTypeDefinition("RDF/XML", W3CFormatsNamespace + "RDF_XML", RdfXml, new String[] { DefaultRdfXmlExtension, "owl" }, typeof(RdfXmlParser), null, typeof(SparqlXmlParser), typeof(RdfXmlWriter), null, typeof(SparqlXmlWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped RDF/XML", RdfXml, new String[] { DefaultRdfXmlExtension + "." + DefaultGZipExtension }, typeof(GZippedRdfXmlParser), null, null, typeof(GZippedRdfXmlWriter), null, null));
#endif

                    //Define RDF/JSON - include SPARQL Parsers to support servers that send back incorrect MIME Type for SPARQL JSON Results
                    //We define this after SPARQL Results JSON to ensure we favour the correct MIME type for it
                    _mimeTypes.Add(new MimeTypeDefinition("RDF/JSON", Json, new String[] { DefaultRdfJsonExtension, DefaultJsonExtension }, typeof(RdfJsonParser), null, typeof(SparqlJsonParser), typeof(RdfJsonWriter), null, typeof(SparqlJsonWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped RDF/JSON", Json, new String[] { DefaultRdfJsonExtension + "." + DefaultGZipExtension, DefaultJsonExtension + "." + DefaultGZipExtension }, typeof(GZippedRdfJsonParser), null, null, typeof(GZippedRdfJsonWriter), null, null));
#endif

                    //Define CSV
                    _mimeTypes.Add(new MimeTypeDefinition("CSV", Csv, new String[] { DefaultCsvExtension }, null, null, typeof(SparqlCsvParser), typeof(CsvWriter), typeof(CsvStoreWriter), typeof(SparqlCsvWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped SPARQL CSV", Csv, new String[] { DefaultCsvExtension + "." + DefaultGZipExtension }, null, null, typeof(GZippedSparqlCsvParser), null, null, typeof(GZippedSparqlCsvWriter)));
#endif

                    //Define TSV
                    _mimeTypes.Add(new MimeTypeDefinition("TSV", Tsv, new String[] { DefaultTsvExtension }, null, null, typeof(SparqlTsvParser), typeof(TsvWriter), typeof(TsvStoreWriter), typeof(SparqlTsvWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped TSV", Tsv, new String[] { DefaultTsvExtension + "." + DefaultGZipExtension }, null, null, typeof(GZippedSparqlTsvParser), null, null, typeof(GZippedSparqlTsvWriter)));
#endif

                    //Define HTML
                    _mimeTypes.Add(new MimeTypeDefinition("HTML", W3CFormatsNamespace + "RDFa", Html, new String[] { DefaultHtmlExtension, DefaultXHtmlExtension, ".htm" }, typeof(RdfAParser), null, null, typeof(HtmlWriter), null, typeof(SparqlHtmlWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new MimeTypeDefinition("GZipped HTML", Html, new String[] { DefaultHtmlExtension + "." + DefaultGZipExtension, DefaultXHtmlExtension + "." + DefaultGZipExtension, ".htm." + DefaultGZipExtension }, typeof(GZippedRdfAParser), null, null, typeof(GZippedRdfAWriter), null, null));
#endif

                    //Define GraphViz DOT
                    _mimeTypes.Add(new MimeTypeDefinition("GraphViz DOT", new String[] { "text/vnd.graphviz" }, new String[] { ".gv", ".dot" }, null, null, null, typeof(GraphVizWriter), null, null));

                    //Define SPARQL Query
                    MimeTypeDefinition qDef = new MimeTypeDefinition("SPARQL Query", new String[] { SparqlQuery }, new String[] { DefaultSparqlQueryExtension });
                    qDef.SetObjectParserType<SparqlQuery>(typeof(SparqlQueryParser));
                    _mimeTypes.Add(qDef);

                    //Define SPARQL Update
                    MimeTypeDefinition uDef = new MimeTypeDefinition("SPARQL Update", new String[] { SparqlUpdate }, new String[] { DefaultSparqlUpdateExtension });
                    uDef.SetObjectParserType<SparqlUpdateCommandSet>(typeof(SparqlUpdateParser));
                    _mimeTypes.Add(uDef);

                    _init = true;
                }
            }
         }

        #endregion

        #region Definition, Parser and Writer Management

        /// <summary>
        /// Resets the MIME Type Definitions (the associations between file extensions, MIME types and their respective parsers and writers) to the library defaults
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be useful if you've altered the definitions and caused something to stop working as a result
        /// </para>
        /// </remarks>
        public static void ResetDefinitions()
        {
            if (_init)
            {
                _init = false;
                _mimeTypes.Clear();
                Init();
            }
        }

        /// <summary>
        /// Gets the available MIME Type Definitions
        /// </summary>
        public static IEnumerable<MimeTypeDefinition> Definitions
        {
            get
            {
                if (!_init) Init();

                return _mimeTypes;
            }
        }

        /// <summary>
        /// Adds a new MIME Type Definition
        /// </summary>
        /// <param name="definition">MIME Type Definition</param>
        public static void AddDefinition(MimeTypeDefinition definition)
        {
            if (!_init) Init();
            if (definition == null) throw new ArgumentNullException("definition");
            _mimeTypes.Add(definition);
        }

        /// <summary>
        /// Registers a parser as the default RDF Parser for all the given MIME types and updates relevant definitions to include the MIME types and file extensions
        /// </summary>
        /// <param name="parser">RDF Parser</param>
        /// <param name="mimeTypes">MIME Types</param>
        /// <param name="fileExtensions">File Extensions</param>
        public static void RegisterParser(IRdfReader parser, IEnumerable<String> mimeTypes, IEnumerable<String> fileExtensions)
        {
            if (!_init) Init();

            if (!mimeTypes.Any()) throw new RdfException("Cannot register a parser without specifying at least 1 MIME Type");

            //Get any existing defintions that are to be altered
            IEnumerable<MimeTypeDefinition> existing = GetDefinitions(mimeTypes);
            foreach (MimeTypeDefinition def in existing)
            {
                foreach (String type in mimeTypes)
                {
                    def.AddMimeType(type);
                }
                foreach (String ext in fileExtensions)
                {
                    def.AddFileExtension(ext);
                }
                def.RdfParserType = parser.GetType();
            }

            //Create any new defintions
            IEnumerable<String> newTypes = mimeTypes.Where(t => !GetDefinitions(t).Any());
            if (newTypes.Any())
            {
                MimeTypeDefinition newDef = new MimeTypeDefinition(String.Empty, newTypes, fileExtensions);
                newDef.RdfParserType = parser.GetType();
                AddDefinition(newDef);
            }
        }

        /// <summary>
        /// Registers a parser as the default RDF Dataset Parser for all the given MIME types and updates relevant definitions to include the MIME types and file extensions
        /// </summary>
        /// <param name="parser">RDF Dataset Parser</param>
        /// <param name="mimeTypes">MIME Types</param>
        /// <param name="fileExtensions">File Extensions</param>
        public static void RegisterParser(IStoreReader parser, IEnumerable<String> mimeTypes, IEnumerable<String> fileExtensions)
        {
            if (!_init) Init();

            if (!mimeTypes.Any()) throw new RdfException("Cannot register a parser without specifying at least 1 MIME Type");

            //Get any existing defintions that are to be altered
            IEnumerable<MimeTypeDefinition> existing = GetDefinitions(mimeTypes);
            foreach (MimeTypeDefinition def in existing)
            {
                foreach (String type in mimeTypes)
                {
                    def.AddMimeType(type);
                }
                foreach (String ext in fileExtensions)
                {
                    def.AddFileExtension(ext);
                }
                def.RdfDatasetParserType = parser.GetType();
            }

            //Create any new defintions
            IEnumerable<String> newTypes = mimeTypes.Where(t => !GetDefinitions(t).Any());
            if (newTypes.Any())
            {
                MimeTypeDefinition newDef = new MimeTypeDefinition(String.Empty, newTypes, fileExtensions);
                newDef.RdfDatasetParserType = parser.GetType();
                AddDefinition(newDef);
            }
        }

        /// <summary>
        /// Registers a parser as the default SPARQL Rsults Parser for all the given MIME types and updates relevant definitions to include the MIME types and file extensions
        /// </summary>
        /// <param name="parser">SPARQL Results Parser</param>
        /// <param name="mimeTypes">MIME Types</param>
        /// <param name="fileExtensions">File Extensions</param>
        public static void RegisterParser(ISparqlResultsReader parser, IEnumerable<String> mimeTypes, IEnumerable<String> fileExtensions)
        {
            if (!_init) Init();

            if (!mimeTypes.Any()) throw new RdfException("Cannot register a parser without specifying at least 1 MIME Type");

            //Get any existing defintions that are to be altered
            IEnumerable<MimeTypeDefinition> existing = GetDefinitions(mimeTypes);
            foreach (MimeTypeDefinition def in existing)
            {
                foreach (String type in mimeTypes)
                {
                    def.AddMimeType(type);
                }
                foreach (String ext in fileExtensions)
                {
                    def.AddFileExtension(ext);
                }
                def.SparqlResultsParserType = parser.GetType();
            }

            //Create any new defintions
            IEnumerable<String> newTypes = mimeTypes.Where(t => !GetDefinitions(t).Any());
            if (newTypes.Any())
            {
                MimeTypeDefinition newDef = new MimeTypeDefinition(String.Empty, newTypes, fileExtensions);
                newDef.SparqlResultsParserType = parser.GetType();
                AddDefinition(newDef);
            }
        }

        /// <summary>
        /// Registers a writer as the default RDF Writer for all the given MIME types and updates relevant definitions to include the MIME types and file extensions
        /// </summary>
        /// <param name="writer">RDF Writer</param>
        /// <param name="mimeTypes">MIME Types</param>
        /// <param name="fileExtensions">File Extensions</param>
        public static void RegisterWriter(IRdfWriter writer, IEnumerable<String> mimeTypes, IEnumerable<String> fileExtensions)
        {
            if (!_init) Init();

            if (!mimeTypes.Any()) throw new RdfException("Cannot register a writer without specifying at least 1 MIME Type");

            //Get any existing defintions that are to be altered
            IEnumerable<MimeTypeDefinition> existing = GetDefinitions(mimeTypes);
            foreach (MimeTypeDefinition def in existing)
            {
                foreach (String type in mimeTypes)
                {
                    def.AddMimeType(type);
                }
                foreach (String ext in fileExtensions)
                {
                    def.AddFileExtension(ext);
                }
                def.RdfWriterType = writer.GetType();
            }

            //Create any new defintions
            IEnumerable<String> newTypes = mimeTypes.Where(t => !GetDefinitions(t).Any());
            if (newTypes.Any())
            {
                MimeTypeDefinition newDef = new MimeTypeDefinition(String.Empty, newTypes, fileExtensions);
                newDef.RdfWriterType = writer.GetType();
                AddDefinition(newDef);
            }
        }

        /// <summary>
        /// Registers a writer as the default RDF Dataset Writer for all the given MIME types and updates relevant definitions to include the MIME types and file extensions
        /// </summary>
        /// <param name="writer">RDF Dataset Writer</param>
        /// <param name="mimeTypes">MIME Types</param>
        /// <param name="fileExtensions">File Extensions</param>
        public static void RegisterWriter(IStoreWriter writer, IEnumerable<String> mimeTypes, IEnumerable<String> fileExtensions)
        {
            if (!_init) Init();

            if (!mimeTypes.Any()) throw new RdfException("Cannot register a writer without specifying at least 1 MIME Type");

            //Get any existing defintions that are to be altered
            IEnumerable<MimeTypeDefinition> existing = GetDefinitions(mimeTypes);
            foreach (MimeTypeDefinition def in existing)
            {
                foreach (String type in mimeTypes)
                {
                    def.AddMimeType(type);
                }
                foreach (String ext in fileExtensions)
                {
                    def.AddFileExtension(ext);
                }
                def.RdfDatasetWriterType = writer.GetType();
            }

            //Create any new defintions
            IEnumerable<String> newTypes = mimeTypes.Where(t => !GetDefinitions(t).Any());
            if (newTypes.Any())
            {
                MimeTypeDefinition newDef = new MimeTypeDefinition(String.Empty, newTypes, fileExtensions);
                newDef.RdfDatasetWriterType = writer.GetType();
                AddDefinition(newDef);
            }
        }

        /// <summary>
        /// Registers a writer as the default SPARQL Results Writer for all the given MIME types and updates relevant definitions to include the MIME types and file extensions
        /// </summary>
        /// <param name="writer">SPARQL Results Writer</param>
        /// <param name="mimeTypes">MIME Types</param>
        /// <param name="fileExtensions">File Extensions</param>
        public static void RegisterWriter(ISparqlResultsWriter writer, IEnumerable<String> mimeTypes, IEnumerable<String> fileExtensions)
        {
            if (!_init) Init();

            if (!mimeTypes.Any()) throw new RdfException("Cannot register a writer without specifying at least 1 MIME Type");

            //Get any existing defintions that are to be altered
            IEnumerable<MimeTypeDefinition> existing = GetDefinitions(mimeTypes);
            foreach (MimeTypeDefinition def in existing)
            {
                foreach (String type in mimeTypes)
                {
                    def.AddMimeType(type);
                }
                foreach (String ext in fileExtensions)
                {
                    def.AddFileExtension(ext);
                }
                def.SparqlResultsWriterType = writer.GetType();
            }

            //Create any new defintions
            IEnumerable<String> newTypes = mimeTypes.Where(t => !GetDefinitions(t).Any());
            if (newTypes.Any())
            {
                MimeTypeDefinition newDef = new MimeTypeDefinition(String.Empty, newTypes, fileExtensions);
                newDef.SparqlResultsWriterType = writer.GetType();
                AddDefinition(newDef);
            }
        }

        /// <summary>
        /// Gets all MIME Type definitions which support the given MIME Type
        /// </summary>
        /// <param name="mimeType">MIME Type</param>
        /// <returns></returns>
        public static IEnumerable<MimeTypeDefinition> GetDefinitions(String mimeType)
        {
            if (!_init) Init();

            if (mimeType == null) return Enumerable.Empty<MimeTypeDefinition>();

            return (from definition in MimeTypesHelper.Definitions
                    where definition.SupportsMimeType(mimeType)
                    select definition);
        }

        /// <summary>
        /// Gets all MIME Type definitions which support the given MIME Types
        /// </summary>
        /// <param name="mimeTypes">MIME Types</param>
        /// <returns></returns>
        public static IEnumerable<MimeTypeDefinition> GetDefinitions(IEnumerable<String> mimeTypes)
        {
            if (!_init) Init();

            return (from mimeType in mimeTypes
                    from definition in MimeTypesHelper.GetDefinitions(mimeType.Contains(";") ? mimeType.Substring(0, mimeType.IndexOf(';')) : mimeType)
                    select definition).Distinct();
        }

        /// <summary>
        /// Gets all MIME Types definitions which are associated with a given file extension
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <returns></returns>
        public static IEnumerable<MimeTypeDefinition> GetDefinitionsByFileExtension(String fileExt)
        {
            if (fileExt == null) return Enumerable.Empty<MimeTypeDefinition>();

            //Remove leading "."
            if (fileExt.StartsWith("."))
            {
                fileExt = fileExt.Substring(1);
            }

            if (!_init) Init();
            return (from def in MimeTypesHelper.Definitions
                    where def.FileExtensions.Contains(fileExt)
                    select def);
        }

#endregion

        #region MIME Type Constants

        /// <summary>
        /// MIME Type for accept any content Type
        /// </summary>
        public const String Any = "*/*";

        /// <summary>
        /// MIME Type for URL Encoded WWW Form Content used when POSTing over HTTP
        /// </summary>
        public const String WWWFormURLEncoded = "application/x-www-form-urlencoded";

        /// <summary>
        /// MIME Type for Multipart Form Data
        /// </summary>
        public const String FormMultipart = "multipart/form-data";

        /// <summary>
        /// MIME Types for Turtle
        /// </summary>
        internal static string[] Turtle = { "text/turtle", "application/x-turtle", "application/turtle" };

        /// <summary>
        /// MIME Types for RDF/XML
        /// </summary>
        internal static string[] RdfXml = { "application/rdf+xml", "text/xml", "application/xml" };

        /// <summary>
        /// MIME Types for Notation 3
        /// </summary>
        internal static string[] Notation3 = { "text/n3", "text/rdf+n3" };

        /// <summary>
        /// MIME Types for NTriples
        /// </summary>
        internal static string[] NTriples = { "text/plain", "text/ntriples", "text/ntriples+turtle", "application/rdf-triples", "application/x-ntriples" };

        /// <summary>
        /// MIME Types for NQuads
        /// </summary>
        internal static string[] NQuads = { "text/x-nquads" };

        /// <summary>
        /// MIME Types for TriG
        /// </summary>
        internal static string[] TriG = { "application/x-trig" };

        /// <summary>
        /// MIME Types for TriX
        /// </summary>
        internal static string[] TriX = { "application/trix" };

        /// <summary>
        /// MIME Types for RDF/JSON
        /// </summary>
        internal static string[] Json = { "application/json", "text/json" };

        /// <summary>
        /// MIME Types for SPARQL Result Sets
        /// </summary>
        internal static string[] SparqlResults = { "application/sparql-results+xml", "application/sparql-results+json" };

        /// <summary>
        /// MIME Types for SPARQL Results XML
        /// </summary>
        internal static string[] SparqlResultsXml = { "application/sparql-results+xml" };

        /// <summary>
        /// MIME Types for SPARQL Results JSON
        /// </summary>
        internal static string[] SparqlResultsJson = { "application/sparql-results+json" };

        /// <summary>
        /// MIME Types for SPARQL Boolean Result
        /// </summary>
        internal static string[] SparqlResultsBoolean = { "text/boolean" };

        /// <summary>
        /// MIME Types for CSV
        /// </summary>
        internal static string[] Csv = { "text/csv", "text/comma-separated-values" };

        /// <summary>
        /// MIME Types for TSV
        /// </summary>
        internal static string[] Tsv = { "text/tab-separated-values" };

        /// <summary>
        /// MIME Types for HTML
        /// </summary>
        internal static string[] Html = { "text/html", "application/xhtml+xml" };

        /// <summary>
        /// MIME Type for SPARQL Queries
        /// </summary>
        public const String SparqlQuery = "application/sparql-query";

        /// <summary>
        /// MIME Type for SPARQL Updates
        /// </summary>
        public const String SparqlUpdate = "application/sparql-update";

        #endregion

        #region File Extension Constants

        /// <summary>
        /// Default File Extension for Turtle Files
        /// </summary>
        public const String DefaultTurtleExtension = "ttl";
        /// <summary>
        /// Default File Extension for RDF/XML
        /// </summary>
        public const String DefaultRdfXmlExtension = "rdf";
        /// <summary>
        /// Default File Extension for Notation 3
        /// </summary>
        public const String DefaultNotation3Extension = "n3";
        /// <summary>
        /// Default File Extension for NTriples
        /// </summary>
        public const String DefaultNTriplesExtension = "nt";
        /// <summary>
        /// Default File Extension for Json formats
        /// </summary>
        public const String DefaultJsonExtension = "json";
        /// <summary>
        /// Default File Extension for RDF/JSON
        /// </summary>
        public const String DefaultRdfJsonExtension = "rj";
        /// <summary>
        /// Default File Extension for SPARQL XML Results Format
        /// </summary>
        public const String DefaultSparqlXmlExtension = "srx";
        /// <summary>
        /// Default File Extension for SPARQL JSON Results Format
        /// </summary>
        public const String DefaultSparqlJsonExtension = "srj";
        /// <summary>
        /// Default File Extension for TriG
        /// </summary>
        public const String DefaultTriGExtension = "trig";
        /// <summary>
        /// Default File Extension for NQuads
        /// </summary>
        public const String DefaultNQuadsExtension = "nq";
        /// <summary>
        /// Default File Extension for TriX
        /// </summary>
        public const String DefaultTriXExtension = "xml";
        /// <summary>
        /// Default File Extension for CSV
        /// </summary>
        public const String DefaultCsvExtension = "csv";
        /// <summary>
        /// Default File Extension for TSV
        /// </summary>
        public const String DefaultTsvExtension = "tsv";
        /// <summary>
        /// Default File Extension for HTML
        /// </summary>
        public const String DefaultHtmlExtension = "html";
        /// <summary>
        /// Default File Extension for XHTML
        /// </summary>
        public const String DefaultXHtmlExtension = "xhtml";
        /// <summary>
        /// Default File Extension for SPARQL Queries
        /// </summary>
        public const String DefaultSparqlQueryExtension = "rq";
        /// <summary>
        /// Default File Extension for SPARQL Updates
        /// </summary>
        public const String DefaultSparqlUpdateExtension = "ru";
        /// <summary>
        /// Default File Extension for GZip
        /// </summary>
        public const String DefaultGZipExtension = "gz";

        /// <summary>
        /// Extensions which are considered stackable
        /// </summary>
        private static String[] AllowedStackableExtensions = new String[] 
        {
            DefaultGZipExtension
        };

        #endregion

        #region HTTP Header Properties

        /// <summary>
        /// Builds the String for the HTTP Accept Header that should be used when you want to ask for content in RDF formats (except Sparql Results)
        /// </summary>
        /// <returns></returns>
        public static String HttpAcceptHeader
        {
            get
            {
                if (!_init) Init();

                StringBuilder output = new StringBuilder();

                foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
                {
                    if (definition.CanParseRdf)
                    {
                        output.Append(String.Join(",", definition.MimeTypes.ToArray()));
                        output.Append(',');
                    }
                }
                if (output[output.Length - 1] == ',') output.Remove(output.Length - 1, 1);
                output.Append(",*/*;q=0.5");

                return output.ToString();
            }
        }

        /// <summary>
        /// Builds the String for the HTTP Accept Header that should be used for querying Sparql Endpoints where the response will be a SPARQL Result Set format
        /// </summary>
        /// <returns></returns>
        public static String HttpSparqlAcceptHeader
        {
            get
            {
                if (!_init) Init();

                StringBuilder output = new StringBuilder();

                foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
                {
                    if (definition.CanParseSparqlResults)
                    {
                        output.Append(String.Join(",", definition.MimeTypes.ToArray()));
                        output.Append(',');
                    }
                }
                if (output[output.Length - 1] == ',') output.Remove(output.Length - 1, 1);

                return output.ToString();
            }
        }

        /// <summary>
        /// Builds the String for the HTTP Accept Header that should be used for making HTTP Requests where the returned data may be RDF or a SPARQL Result Set
        /// </summary>
        /// <returns></returns>
        public static String HttpRdfOrSparqlAcceptHeader
        {
            get
            {
                if (!_init) Init();

                StringBuilder output = new StringBuilder();

                foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
                {
                    if (definition.CanParseRdf || definition.CanParseSparqlResults)
                    {
                        output.Append(String.Join(",", definition.MimeTypes.ToArray()));
                        output.Append(',');
                    }
                }
                if (output[output.Length - 1] == ',') output.Remove(output.Length - 1, 1);
                output.Append(",*/*;q=0.5");

                return output.ToString();
            }
        }

        /// <summary>
        /// Builds the String for the HTTP Accept Header that should be used for making HTTP Requests where the returned data will be an RDF dataset
        /// </summary>
        public static String HttpRdfDatasetAcceptHeader
        {
            get
            {
                if (!_init) Init();

                StringBuilder output = new StringBuilder();

                foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
                {
                    if (definition.CanParseRdfDatasets)
                    {
                        output.Append(String.Join(",", definition.MimeTypes.ToArray()));
                        output.Append(',');
                    }
                }
                if (output[output.Length - 1] == ',') output.Remove(output.Length - 1, 1);

                return output.ToString();
            }
        }

        /// <summary>
        /// Builds the String for the HTTP Accept Header that should be used for making HTTP Requests where the returned data may be RDF or an RDF dataset
        /// </summary>
        public static String HttpRdfOrDatasetAcceptHeader
        {
            get
            {
                if (!_init) Init();

                StringBuilder output = new StringBuilder();

                foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
                {
                    if (definition.CanParseRdf || definition.CanParseRdfDatasets)
                    {
                        output.Append(String.Join(",", definition.MimeTypes.ToArray()));
                        output.Append(',');
                    }
                }
                if (output[output.Length - 1] == ',') output.Remove(output.Length - 1, 1);
                output.Append(";q=1.0,*/*;q=0.5");

                return output.ToString();
            }
        }

        /// <summary>
        /// Creates a Custom HTTP Accept Header containing the given selection of MIME Types
        /// </summary>
        /// <param name="mimeTypes">Enumeration of MIME Types to use</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// <strong>Note:</strong> No validation is done on MIME Types so it is possible to generated a malformed header using this function
        /// </para>
        /// </remarks>
        public static String CustomHttpAcceptHeader(IEnumerable<String> mimeTypes)
        {
            return String.Join(",", mimeTypes.ToArray());
        }

        /// <summary>
        /// Creates a Custom HTTP Accept Header containing the given selection of MIME Types where those MIME Types also appear in the list of supported Types
        /// </summary>
        /// <param name="mimeTypes">Enumeration of MIME Types to use</param>
        /// <param name="supportedTypes">Enumeration of supported MIME Types</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// <strong>Note:</strong> No validation is done on MIME Types so it is possible to generated a malformed header using this function
        /// </para>
        /// <para>
        /// Use this function when you wish to generate a Custom Accept Header where the URI to which you are making requests supports a set range of URIs (given in the <paramref name="mimeTypes"/> parameter) where that range of types may exceed the range of types actually supported by the library or your response processing code.
        /// </para>
        /// </remarks>
        public static String CustomHttpAcceptHeader(IEnumerable<String> mimeTypes, IEnumerable<String> supportedTypes)
        {
            StringBuilder output = new StringBuilder();
            HashSet<String> supported = new HashSet<string>(supportedTypes);

            foreach (String type in mimeTypes)
            {
                if (supported.Contains(type))
                {
                    if (output.Length > 0) output.Append(",");
                    output.Append(type);
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Creates a Custom HTTP Accept Header containing only the Accept Types supported by a specific parser
        /// </summary>
        /// <param name="parser">RDF Parser</param>
        /// <returns></returns>
        public static String CustomHttpAcceptHeader(IRdfReader parser)
        {
            if (!_init) Init();

            Type requiredType = parser.GetType();
            foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
            {
                if (requiredType.Equals(definition.RdfParserType))
                {
                    return String.Join(",", definition.MimeTypes.ToArray());
                }
            }
            return MimeTypesHelper.HttpAcceptHeader;
        }

        /// <summary>
        /// Creates a Custom HTTP Accept Header containing only the Accept Types supported by a specific parser
        /// </summary>
        /// <param name="parser">RDF Parser</param>
        /// <returns></returns>
        public static String CustomHttpAcceptHeader(IStoreReader parser)
        {
            if (!_init) Init();

            Type requiredType = parser.GetType();
            foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
            {
                if (requiredType.Equals(definition.RdfDatasetParserType))
                {
                    return String.Join(",", definition.MimeTypes.ToArray());
                }
            }
            return MimeTypesHelper.HttpRdfDatasetAcceptHeader;
        }

        /// <summary>
        /// Gets the Enumeration of supported MIME Types for RDF Graphs
        /// </summary>
        public static IEnumerable<String> SupportedRdfMimeTypes
        {
            get
            {
                if (!_init) Init();

                return (from definition in MimeTypesHelper.Definitions
                        where definition.CanParseRdf
                        from mimeType in definition.MimeTypes
                        select mimeType);
            }
        }

        /// <summary>
        /// Gets the Enumeration of supported MIME Types for RDF Datasets
        /// </summary>
        public static IEnumerable<String> SupportedRdfDatasetMimeTypes
        {
            get
            {
                if (!_init) Init();

                return (from definition in MimeTypesHelper.Definitions
                        where definition.CanParseRdfDatasets
                        from mimeType in definition.MimeTypes
                        select mimeType);
            }
        }

        /// <summary>
        /// Gets the Enumeration of supported MIME Types for SPARQL Results
        /// </summary>
        public static IEnumerable<String> SupportedSparqlMimeTypes
        {
            get
            {
                if (!_init) Init();

                return (from definition in MimeTypesHelper.Definitions
                        where definition.CanParseSparqlResults
                        from mimeType in definition.MimeTypes
                        select mimeType);
            }
        }

        /// <summary>
        /// Gets the Enumeration of supported MIME Types for RDF Graphs or SPARQL Results
        /// </summary>
        public static IEnumerable<String> SupportedRdfOrSparqlMimeTypes
        {
            get
            {
                if (!_init) Init();

                return (from definition in MimeTypesHelper.Definitions
                        where definition.CanParseRdf || definition.CanParseSparqlResults
                        from mimeType in definition.MimeTypes
                        select mimeType);
            }
        }

        #endregion

        #region File Filters

        /// <summary>
        /// Generates a Filename Filter that can be used with any .Net application and includes all formats that dotNetRDF is aware of
        /// </summary>
        /// <returns></returns>
        public static String GetFilenameFilter()
        {
            return MimeTypesHelper.GetFilenameFilter(true, true, true, true, true, true);
        }

        /// <summary>
        /// Generates a Filename Filter that can be used with any .Net application and includes a user dictated subset of the formats that dotNetRDF is aware of
        /// </summary>
        /// <param name="rdf">Allow RDF Graph formats (e.g. Turtle)</param>
        /// <param name="rdfDatasets">Allow RDF Dataset formats (e.g. NQuads)</param>
        /// <param name="sparqlResults">Allow SPARQL Results formats (e.g. SPARQL Results XML)</param>
        /// <param name="sparqlQuery">Allow SPARQL Query (i.e. .rq files)</param>
        /// <param name="sparqlUpdate">Allow SPARQL Update (i.e. .ru files)</param>
        /// <param name="allFiles">Allow All Files (i.e. */*)</param>
        /// <returns></returns>
        public static String GetFilenameFilter(bool rdf, bool rdfDatasets, bool sparqlResults, bool sparqlQuery, bool sparqlUpdate, bool allFiles)
        {
            if (!_init) Init();

            String filter = String.Empty;
            List<String> exts = new List<string>();

            foreach (MimeTypeDefinition def in MimeTypesHelper.Definitions)
            {
                if ((rdf && (def.CanParseRdf || def.CanWriteRdf)) 
                    || (rdfDatasets && (def.CanParseRdfDatasets || def.CanWriteRdfDatasets)) 
                    || (sparqlResults && (def.CanParseSparqlResults || def.CanWriteSparqlResults)) 
                    || (sparqlQuery && def.CanParseObject<SparqlQuery>()) 
                    || (sparqlUpdate && def.CanParseObject<SparqlUpdateCommandSet>()))
                {
                    exts.AddRange(def.FileExtensions/*.Where(e => !e.Contains("."))*/);
                    filter += def.SyntaxName + " Files|*." + String.Join(";*.", def.FileExtensions.ToArray()) + "|";
                }
            }
            //Add an All Supported Formats option as first option
            filter = "All Supported Files|*." + String.Join(";*.", exts.ToArray()) + "|" + filter;

            if (allFiles)
            {
                filter += "All Files|*.*";
            }
            else
            {
                filter = filter.Substring(0, filter.Length - 1);
            }

            return filter;
        }

        #endregion

        #region Reader and Writer Selection

        private static void ApplyWriterOptions(Object writer)
        {
            if (writer is ICompressingWriter)
            {
                ((ICompressingWriter)writer).CompressionLevel = Options.DefaultCompressionLevel;
            }
        }

        /// <summary>
        /// Selects an appropriate <see cref="IRdfWriter">IRdfWriter</see> based on the given MIME Types
        /// </summary>
        /// <param name="ctypes">MIME Types</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// This method does not take account of any quality/charset preference parameters included in the Accept Header
        /// </para>
        /// <para>
        /// For writers which support <see cref="ICompressingWriter">ICompressingWriter</see> they will be instantiated with the Compression Level specified by <see cref="Options.DefaultCompressionLevel">Options.DefaultCompressionLevel</see>
        /// </para>
        /// </remarks>
        public static IRdfWriter GetWriter(IEnumerable<String> ctypes)
        {
            String temp;
            return GetWriter(ctypes, out temp);
        }

        /// <summary>
        /// Selects an appropriate <see cref="IRdfWriter">IRdfWriter</see> based on the given MIME Types
        /// </summary>
        /// <param name="ctypes">MIME Types</param>
        /// <param name="contentType">The Content Type header that should be sent in the Response to the Request</param>
        /// <returns></returns>
        public static IRdfWriter GetWriter(IEnumerable<String> ctypes, out String contentType)
        {
            String type;

            if (ctypes != null)
            {
                foreach (String ctype in ctypes)
                {
                    //Strip off the Charset/Quality if specified
                    if (ctype.Contains(";"))
                    {
                        type = ctype.Substring(0, ctype.IndexOf(";"));
                    }
                    else
                    {
                        type = ctype;
                    }
                    type = type.ToLowerInvariant();

                    //See if there are any MIME Type Definitions for this MIME Type
                    foreach (MimeTypeDefinition definition in MimeTypesHelper.GetDefinitions(type))
                    {
                        //If so return the Writer from the first match found
                        if (definition.CanWriteRdf)
                        {
                            IRdfWriter writer = definition.GetRdfWriter();
                            MimeTypesHelper.ApplyWriterOptions(writer);
                            contentType = definition.CanonicalMimeType;
                            return writer;
                        }
                    }
                }
            }

            //Default to NTriples
            contentType = MimeTypesHelper.NTriples[0];
            return new NTriplesWriter();
        }

        /// <summary>
        /// Selects an appropriate <see cref="IRdfWriter">IRdfWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="acceptHeader">Value of the HTTP Accept Header</param>
        /// <param name="contentType">The Content Type header that should be sent in the Response to the Request</param>
        /// <returns>A Writer for a Content Type the client accepts and the Content Type that should be sent to the client</returns>
        /// <remarks>
        /// <para>
        /// This method does not take account of any quality/charset preference parameters included in the Accept Header
        /// </para>
        /// <para>
        /// For writers which support <see cref="ICompressingWriter">ICompressingWriter</see> they will be instantiated with the Compression Level specified by <see cref="Options.DefaultCompressionLevel">Options.DefaultCompressionLevel</see>
        /// </para>
        /// </remarks>
        public static IRdfWriter GetWriter(String acceptHeader, out String contentType)
        {
            String[] ctypes;

            //Parse Accept Header into a String Array
            if (acceptHeader != null)
            {
                acceptHeader = acceptHeader.Trim();
                if (acceptHeader.Contains(","))
                {
                    ctypes = acceptHeader.Split(',');
                }
                else
                {
                    ctypes = new String[] { acceptHeader };
                }
            }
            else
            {
                ctypes = new String[] { };
            }

            return GetWriter(ctypes, out contentType);
        }

        /// <summary>
        /// Selects an appropriate <see cref="IRdfWriter">IRdfWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="acceptHeader">Value of the HTTP Accept Header</param>
        /// <returns>A Writer for a Content Type the client accepts</returns>
        /// <remarks>This method does not take account of any quality/charset preference parameters included in the Accept Header</remarks>
        public static IRdfWriter GetWriter(String acceptHeader)
        {
            String temp;
            return GetWriter(acceptHeader, out temp);
        }

        /// <summary>
        /// Selects a RDF writer based on the file extension
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <exception cref="RdfWriterSelectionException">Thrown if no writers are associated with the given file extension</exception>
        /// <returns></returns>
        public static IRdfWriter GetWriterByFileExtension(String fileExt)
        {
            String temp;
            return MimeTypesHelper.GetWriterByFileExtension(fileExt, out temp);
        }

        /// <summary>
        /// Selects a RDF writer based on the file extension
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <param name="contentType">Content Type of the chosen writer</param>
        /// <exception cref="RdfWriterSelectionException">Thrown if no writers are associated with the given file extension</exception>
        /// <returns></returns>
        public static IRdfWriter GetWriterByFileExtension(String fileExt, out String contentType)
        {
            if (fileExt == null) throw new ArgumentNullException("fileExt", "File extension cannot be null");

            //See if there are any MIME Type Definition for the file extension
            foreach (MimeTypeDefinition definition in MimeTypesHelper.GetDefinitionsByFileExtension(fileExt))
            {
                //If so return the Writer from the first match found
                if (definition.CanWriteRdf)
                {
                    IRdfWriter writer = definition.GetRdfWriter();
                    MimeTypesHelper.ApplyWriterOptions(writer);
                    contentType = definition.CanonicalMimeType;
                    return writer;
                }
            }

            //Error if unable to select
            contentType = null;
            throw new RdfWriterSelectionException("Unable to select a RDF writer, no writers are associated with the file extension '" + fileExt + "'");
        }

        /// <summary>
        /// Selects an appropriate <see cref="IRdfReader">IRdfReader</see> based on the given MIME Types
        /// </summary>
        /// <param name="ctypes">MIME TYpes</param>
        /// <returns></returns>
        public static IRdfReader GetParser(IEnumerable<String> ctypes)
        {
            if (ctypes != null)
            {
                foreach (String ctype in ctypes)
                {
                    try
                    {
                        IRdfReader parser = GetParser(ctype);
                        return parser;
                    }
                    catch (RdfParserSelectionException)
                    {
                        //Ignore
                    }
                }
            }

            String types = (ctypes == null) ? String.Empty : String.Join(",", ctypes.ToArray());
            throw new RdfParserSelectionException("The Library does not contain any Parsers for RDF Graphs in any of the following MIME Types: " + types);
        }

        /// <summary>
        /// Selects an appropriate <see cref="IRdfReader">IRdfReader</see> based on the HTTP Content-Type header from a HTTP Response
        /// </summary>
        /// <param name="contentType">Value of the HTTP Content-Type Header</param>
        /// <returns></returns>
        public static IRdfReader GetParser(String contentType)
        {
            //Strip off Charset specifier of the Content Type if any
            if (contentType.Contains(";"))
            {
                contentType = contentType.Substring(0, contentType.IndexOf(";"));
            }
            contentType = contentType.ToLowerInvariant();

            foreach (MimeTypeDefinition definition in MimeTypesHelper.GetDefinitions(contentType))
            {
                if (definition.CanParseRdf)
                {
                    return definition.GetRdfParser();
                }
            }

            throw new RdfParserSelectionException("The Library does not contain a Parser for RDF Graphs in the format '" + contentType + "'");
        }

        /// <summary>
        /// Selects a RDF Parser based on the file extension
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <returns></returns>
        public static IRdfReader GetParserByFileExtension(String fileExt)
        {
            if (fileExt == null) throw new ArgumentNullException("fileExt", "File extension cannot be null");

            foreach (MimeTypeDefinition def in MimeTypesHelper.GetDefinitionsByFileExtension(fileExt))
            {
                if (def.CanParseRdf)
                {
                    IRdfReader parser = def.GetRdfParser();
                    return parser;
                }
            }

            throw new RdfParserSelectionException("The Library does not contain any Parsers for RDF Graphs associated with the File Extension '" + fileExt + "'");
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsReader">ISparqlResultsReader</see> based on the HTTP Content-Type header from a HTTP Response
        /// </summary>
        /// <param name="contentType">Value of the HTTP Content-Type Header</param>
        /// <returns></returns>
        public static ISparqlResultsReader GetSparqlParser(String contentType)
        {
            //Strip off Charset specifier of the Content Type if any
            if (contentType.Contains(";"))
            {
                contentType = contentType.Substring(0, contentType.IndexOf(";"));
            }
            contentType = contentType.ToLowerInvariant();

            foreach (MimeTypeDefinition definition in MimeTypesHelper.GetDefinitions(contentType))
            {
                if (definition.CanParseSparqlResults)
                {
                    return definition.GetSparqlResultsParser();
                }
            }

            throw new RdfParserSelectionException("The Library does not contain a Parser for SPARQL Results in the format '" + contentType + "'");
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsReader">ISparqlResultsReader</see> based on the HTTP Content-Type header from a HTTP Response
        /// </summary>
        /// <param name="contentType">Value of the HTTP Content-Type Header</param>
        /// <param name="allowPlainTextResults">Whether you allow Sparql Boolean results in text/plain format (Boolean results in text/boolean are handled properly but text/plain results can be conflated with CONSTRUCT/DESCRIBE results in NTriples format)</param>
        /// <returns></returns>
        public static ISparqlResultsReader GetSparqlParser(String contentType, bool allowPlainTextResults)
        {
            //Strip off Charset specifier of the Content Type if any
            if (contentType.Contains(";"))
            {
                contentType = contentType.Substring(0, contentType.IndexOf(";"));
            }
            contentType = contentType.ToLowerInvariant();

            try
            {
                return GetSparqlParser(contentType);
            }
            catch (RdfParserSelectionException)
            {
                if (allowPlainTextResults && (contentType.Equals("text/plain") || contentType.Equals("text/boolean")))
                {
                    return new SparqlBooleanParser();
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Selects a SPARQL Parser based on the file extension
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <returns></returns>
        public static ISparqlResultsReader GetSparqlParserByFileExtension(String fileExt)
        {
            if (fileExt == null) throw new ArgumentNullException("fileExt", "File Extension cannot be null");

            foreach (MimeTypeDefinition def in MimeTypesHelper.GetDefinitionsByFileExtension(fileExt))
            {
                if (def.CanParseSparqlResults)
                {
                    return def.GetSparqlResultsParser();
                }
            }

            throw new RdfParserSelectionException("The Library does not contain a Parser for SPARQL Results associated with the file extension '" + fileExt + "'");
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsWriter">ISparqlResultsWriter</see> based on the given MIME Types
        /// </summary>
        /// <param name="ctypes">MIME Types</param>
        /// <returns>A Writer for a Content Type the client accepts</returns>
        /// <remarks>This method does not take account of any quality/charset preference parameters included in the Accept Header</remarks>
        public static ISparqlResultsWriter GetSparqlWriter(IEnumerable<String> ctypes)
        {
            String temp;
            return GetSparqlWriter(ctypes, out temp);
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsWriter">ISparqlResultsWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="ctypes">String array of accepted Content Types</param>
        /// <param name="contentType">The Content Type header that should be sent in the Response to the Request</param>
        /// <returns>A Writer for a Content Type the client accepts and the Content Type that should be sent to the client</returns>
        /// <remarks>This method does not take account of any quality/charset preference parameters included in the Accept Header</remarks>
        public static ISparqlResultsWriter GetSparqlWriter(IEnumerable<String> ctypes, out String contentType)
        {
            String type;

            foreach (String ctype in ctypes)
            {
                //Strip off the Charset/Quality if specified
                if (ctype.Contains(";"))
                {
                    type = ctype.Substring(0, ctype.IndexOf(";"));
                }
                else
                {
                    type = ctype;
                }
                type = type.ToLowerInvariant();

                foreach (MimeTypeDefinition definition in MimeTypesHelper.GetDefinitions(type))
                {
                    if (definition.CanWriteSparqlResults)
                    {
                        contentType = definition.CanonicalMimeType;
                        return definition.GetSparqlResultsWriter();
                    }
                }
            }

            //Default to SPARQL XML Output
            contentType = MimeTypesHelper.SparqlResultsXml[0];
            return new SparqlXmlWriter();
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsWriter">ISparqlResultsWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="acceptHeader">Value of the HTTP Accept Header</param>
        /// <param name="contentType">The Content Type header that should be sent in the Response to the Request</param>
        /// <returns>A Writer for a Content Type the client accepts and the Content Type that should be sent to the client</returns>
        /// <remarks>This method does not take account of any quality/charset preference parameters included in the Accept Header</remarks>
        public static ISparqlResultsWriter GetSparqlWriter(String acceptHeader, out String contentType)
        {
            String[] ctypes;

            //Parse Accept Header into a String Array
            acceptHeader = acceptHeader.Trim();
            if (acceptHeader.Contains(","))
            {
                ctypes = acceptHeader.Split(',');
            }
            else
            {
                ctypes = new String[] { acceptHeader };
            }

            return GetSparqlWriter(ctypes, out contentType);
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsWriter">ISparqlResultsWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="acceptHeader">Value of the HTTP Accept Header</param>
        /// <returns>A Writer for a Content Type the client accepts</returns>
        /// <remarks>This method does not take account of any quality/charset preference parameters included in the Accept Header</remarks>
        public static ISparqlResultsWriter GetSparqlWriter(String acceptHeader)
        {
            String temp;
            return GetSparqlWriter(acceptHeader, out temp);
        }

        /// <summary>
        /// Selects an appropriate <see cref="IStoreReader">IStoreReader</see> based on the HTTP Content-Type header from a HTTP Response
        /// </summary>
        /// <param name="contentType">Value of the HTTP Content-Type Header</param>
        /// <returns></returns>
        public static IStoreReader GetStoreParser(String contentType)
        {
            //Strip off Charset specifier of the Content Type if any
            if (contentType.Contains(";"))
            {
                contentType = contentType.Substring(0, contentType.IndexOf(";"));
            }
            contentType = contentType.ToLowerInvariant();

            foreach (MimeTypeDefinition definition in MimeTypesHelper.GetDefinitions(contentType))
            {
                if (definition.CanParseRdfDatasets)
                {
                    return definition.GetRdfDatasetParser();
                }
            }

            throw new RdfParserSelectionException("The Library does not contain a Parser for RDF datasets in the format '" + contentType + "'");
        }

        /// <summary>
        /// Selects an appropriate <see cref="IStoreWriter">IStoreWriter</see> based on the given MIME Types
        /// </summary>
        /// <param name="ctypes">MIME Types</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// This method does not take account of any quality/charset preference parameters included in the Accept Header
        /// </para>
        /// <para>
        /// For writers which support <see cref="ICompressingWriter">ICompressingWriter</see> they will be instantiated with the Compression Level specified by <see cref="Options.DefaultCompressionLevel">Options.DefaultCompressionLevel</see>
        /// </para>
        /// </remarks>
        public static IStoreWriter GetStoreWriter(IEnumerable<String> ctypes)
        {
            String temp;
            return GetStoreWriter(ctypes, out temp);
        }

        /// <summary>
        /// Selects an appropriate <see cref="IStoreWriter">IStoreWriter</see> based on the given MIME Types
        /// </summary>
        /// <param name="ctypes">MIME Types</param>
        /// <param name="contentType">The Content Type header that should be sent in the Response to the Request</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// This method does not take account of any quality/charset preference parameters included in the Accept Header
        /// </para>
        /// <para>
        /// For writers which support <see cref="ICompressingWriter">ICompressingWriter</see> they will be instantiated with the Compression Level specified by <see cref="Options.DefaultCompressionLevel">Options.DefaultCompressionLevel</see>
        /// </para>
        /// </remarks>
        public static IStoreWriter GetStoreWriter(IEnumerable<String> ctypes, out String contentType)
        {
            String type;
            foreach (String ctype in ctypes)
            {
                //Strip off the Charset/Quality if specified
                if (ctype.Contains(";"))
                {
                    type = ctype.Substring(0, ctype.IndexOf(";"));
                }
                else
                {
                    type = ctype;
                }
                type = type.ToLowerInvariant();

                foreach (MimeTypeDefinition definition in MimeTypesHelper.GetDefinitions(type))
                {
                    if (definition.CanWriteRdfDatasets)
                    {
                        contentType = definition.CanonicalMimeType;
                        return definition.GetRdfDatasetWriter();
                    }
                }
            }

            throw new RdfWriterSelectionException("The Library does not contain a writer which can output RDF datasets in a format supported by the Client");
        }

        /// <summary>
        /// Selects an appropriate <see cref="IStoreWriter">IStoreWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="acceptHeader">Value of the HTTP Accept Header</param>
        /// <param name="contentType">The Content Type header that should be sent in the Response to the Request</param>
        /// <returns>A Writer for a Content Type the client accepts and the Content Type that should be sent to the client</returns>
        /// <remarks>This method does not take account of any quality/charset preference parameters included in the Accept Header</remarks>
        public static IStoreWriter GetStoreWriter(String acceptHeader, out String contentType)
        {
            String[] ctypes;

            //Parse Accept Header into a String Array
            acceptHeader = acceptHeader.Trim();
            if (acceptHeader.Contains(","))
            {
                ctypes = acceptHeader.Split(',');
            }
            else
            {
                ctypes = new String[] { acceptHeader };
            }

            return GetStoreWriter(ctypes, out contentType);
        }

        /// <summary>
        /// Selects an appropriate <see cref="IStoreWriter">IStoreWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="acceptHeader">Value of the HTTP Accept Header</param>
        /// <returns>A Writer for a Content Type the client accepts</returns>
        /// <remarks>This method does not take account of any quality/charset preference parameters included in the Accept Header</remarks>
        public static IStoreWriter GetStoreWriter(String acceptHeader)
        {
            String temp;
            return GetStoreWriter(acceptHeader, out temp);
        }

        #endregion

        #region MIME Type Selection

        /// <summary>
        /// Selects the appropriate MIME Type for the given File Extension if the File Extension is a standard extension for an RDF format
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <returns></returns>
        [Obsolete("This method is deprecated, please use GetDefinitionsForExtension() to find relevant definitions and extract the MIME types from there", false)]
        public static String GetMimeType(String fileExt)
        {
            //Remove leading "."
            if (fileExt.StartsWith("."))
            {
                fileExt = fileExt.Substring(1);
            }

            if (!_init) Init();
            foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
            {
                if (definition.FileExtensions.Contains(fileExt))
                {
                    return definition.CanonicalMimeType;
                }
            }

            //Unknown File Extension
            throw new RdfParserSelectionException("Unable to determine the appropriate MIME Type for the File Extension '" + fileExt + "' as this is not a standard extension for an RDF format");
        }

        /// <summary>
        /// Gets all the MIME Types associated with a given File Extension
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <returns></returns>
        [Obsolete("This method is deprecated, please use GetDefinitionsForExtension() to find relevant definitions and extract the MIME types from there", false)]
        public static IEnumerable<String> GetMimeTypes(String fileExt)
        {
            //Remove leading "."
            if (fileExt.StartsWith("."))
            {
                fileExt = fileExt.Substring(1);
            }

            if (!_init) Init();
            List<String> types = new List<string>();
            foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
            {
                if (definition.FileExtensions.Contains(fileExt))
                {
                    types.AddRange(definition.MimeTypes);
                }
            }

            if (types.Count > 0) return types;

            //Unknown File Extension
            throw new RdfParserSelectionException("Unable to determine the appropriate MIME Type for the File Extension '" + fileExt + "' as this is not a standard extension for an RDF format");

        }

        #endregion

        #region File Extension Selection

        /// <summary>
        /// Gets the true file extension for a filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// This is an alternative to using <see cref="Path.GetExtension()"/> which is designed to take into account known extensions which are used in conjunction with other extensions and mask the true extension, for example <strong>.gz</strong>
        /// </para>
        /// <para>
        /// Consider the filename <strong>example.ttl.gz</strong>, obtaining the extension the standard way gives only <strong>.gz</strong> which is unhelpful since it doesn't actually tell us the underlying format of the data only that it is GZipped and if it is GZipped we almost certainly want to stream the data rather than read all into memory and heuristically detect the actual format.  Instead we'd like to get <strong>.ttl.gz</strong> as the file extension which is much more useful and this is what this function does.
        /// </para>
        /// <para>
        /// <strong>Important:</strong> This method does not blindly return double extensions whenever they are present (since they may simply by period characters in the filename and not double extensions at all) rather it returns double extensions only when the standard extension is an extension is known to be used with double extensions e.g. <strong>.gz</strong>
        /// </para>
        /// </remarks>
        public static String GetTrueFileExtension(String filename)
        {
            String actualFilename = Path.GetFileName(filename);
            int extIndex = actualFilename.IndexOf('.');

            //If no extension(s) return standard method
            if (extIndex == -1) return Path.GetExtension(filename);

            //Otherwise get the detected extension and then check for double extensions
            String stdExt = Path.GetExtension(actualFilename);

            //Only proceed to do double extension checking if the extension is known to be stackable
            if (!AllowedStackableExtensions.Contains(stdExt.Substring(1))) return stdExt;

            int stdIndex = actualFilename.Length - stdExt.Length;

            //If the indexes match then the standard method returned the only extension present
            if (extIndex == stdIndex) return stdExt;

            //Otherwise we have a double extension
            actualFilename = actualFilename.Substring(0, stdIndex);
            String realExt = Path.GetExtension(actualFilename);

            return realExt + stdExt;
        }

        /// <summary>
        /// Selects the appropriate File Extension for the given MIME Type
        /// </summary>
        /// <param name="mimeType">MIME Type</param>
        /// <returns></returns>
        public static String GetFileExtension(String mimeType)
        {
            //Strip off Charset specifier of the Content Type if any
            if (mimeType.Contains(";"))
            {
                mimeType = mimeType.Substring(0, mimeType.IndexOf(";"));
            }

            if (!_init) Init();
            foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
            {
                if (definition.MimeTypes.Contains(mimeType))
                {
                    return definition.CanonicalFileExtension;
                }
            }

            throw new RdfException("Unable to determine the appropriate File Extension for the MIME Type '" + mimeType + "'");
        }

        /// <summary>
        /// Selects the appropriate File Extension for the given RDF Writer
        /// </summary>
        /// <param name="writer">RDF Writer</param>
        /// <returns></returns>
        public static String GetFileExtension(IRdfWriter writer)
        {
            if (!_init) Init();
            Type requiredType = writer.GetType();
            foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
            {
                if (requiredType.Equals(definition.RdfWriterType))
                {
                    return definition.CanonicalFileExtension;
                }
            }

            throw new RdfException("Unable to determine the appropriate File Extension for the RDF Writer '" + writer.GetType().ToString() + "'");
        }

        /// <summary>
        /// Selects the appropriate File Extension for the given Store Writer
        /// </summary>
        /// <param name="writer">Store Writer</param>
        /// <returns></returns>
        public static String GetFileExtension(IStoreWriter writer)
        {
            if (!_init) Init();
            Type requiredType = writer.GetType();
            foreach (MimeTypeDefinition definition in MimeTypesHelper.Definitions)
            {
                if (requiredType.Equals(definition.RdfDatasetWriterType))
                {
                    return definition.CanonicalFileExtension;
                }
            }
                
            throw new RdfException("Unable to determine the appropriate File Extension for the Store Writer '" + writer.GetType().ToString() + "'");
        }

        #endregion
   }
}