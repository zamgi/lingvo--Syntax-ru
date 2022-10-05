using System.Collections.Generic;

using lingvo.tokenizing;

namespace lingvo.postagger
{
    /// <summary>
    /// 
    /// </summary>
    internal readonly struct SearchResult
    {
        /// <summary>
        /// 
        /// </summary>
        public sealed class Comparer : IComparer< SearchResult >
        {
            public static Comparer Inst { get; } = new Comparer();
            private Comparer() { }
            public int Compare( SearchResult x, SearchResult y )
            {
                var d = y.Length - x.Length;
                if ( d != 0 )
                    return (d);

                return (y.StartIndex - x.StartIndex);
            }
        }

        public SearchResult( int startIndex, int length )
        {
            StartIndex = startIndex;
            Length     = length;
        }

        public int StartIndex { get; }
        public int Length     { get; }

        public override string ToString() => $"[{StartIndex}:{Length}]";
    }

    /// <summary>
    /// Class for searching string for one or multiple keywords using efficient Aho-Corasick search algorithm
    /// </summary>
    internal sealed class Searcher
    {
        /// <summary>
        /// Tree node representing character and its transition and failure function
        /// </summary>
        private sealed class TreeNode
        {
            /// <summary>
            /// 
            /// </summary>
            private sealed class StringsIEqualityComparer : IEqualityComparer< string[] >
            {
                public static StringsIEqualityComparer Inst { get; } = new StringsIEqualityComparer();
                private StringsIEqualityComparer() { }
                public bool Equals( string[] x, string[] y )
                {
                    var len = x.Length;
                    if ( len != y.Length )
                    {
                        return (false);
                    }

                    for ( int i = 0; i < len; i++ )
                    {
                        if ( !string.Equals( x[ i ], y[ i ] ) )
                        {
                            return (false);
                        }
                    }
                    return (true);
                }
                public int GetHashCode( string[] obj ) => obj.Length;
            }

            /// <summary>
            /// Build tree from specified keywords
            /// </summary>
            public static TreeNode BuildTree( IList< string[] > ngrams )
            {
                // Build keyword tree and transition function
                var root = new TreeNode( null, null );
                foreach ( var ngram in ngrams )
                {
                    // add pattern to tree
                    var node = root;
                    foreach ( string word in ngram )
                    {
                        var nodeNew = node.GetTransition( word );
                        if ( nodeNew == null )
                        {
                            nodeNew = new TreeNode( node, word );
                            node.AddTransition( nodeNew );
                        }
                        node = nodeNew;
                    }
                    node.AddNgram( ngram );
                }

                // Find failure functions
                var nodes = new List< TreeNode >();
                // level 1 nodes - fail to root node
                var transitions_root_nodes = root.Transitions;
                if ( transitions_root_nodes != null )
                {
                    nodes.Capacity = transitions_root_nodes.Count;

                    foreach ( var node in transitions_root_nodes )
                    {
                        node.Failure = root;
                        var transitions_nodes = node.Transitions;
                        if ( transitions_nodes != null )
                        {
                            foreach ( var trans in transitions_nodes )
                            {
                                nodes.Add( trans );
                            }
                        }
                    }
                }

                // other nodes - using BFS
                while ( nodes.Count != 0 )
                {
                    var newNodes = new List< TreeNode >( nodes.Count );
                    foreach ( var node in nodes )
                    {
                        var r = node.Parent.Failure;
                        var word = node.Word;

                        while ( (r != null) && !r.ContainsTransition( word ) )
                        {
                            r = r.Failure;
                        }
                        if ( r == null )
                        {
                            node.Failure = root;
                        }
                        else
                        {
                            node.Failure = r.GetTransition( word );
                            var failure_ngrams = node.Failure.Ngrams;
                            if ( failure_ngrams != null )
                            {
                                foreach ( var result in failure_ngrams )
                                {
                                    node.AddNgram( result );
                                }
                            }
                        }

                        // add child nodes to BFS list 
                        var transitions_nodes = node.Transitions;
                        if ( transitions_nodes != null )
                        {
                            foreach ( var child in transitions_nodes )
                            {
                                newNodes.Add( child );
                            }
                        }
                    }
                    nodes = newNodes;
                }
                root.Failure = root;

                return (root);
            }

            #region [.ctor() & methods.]
            /// <summary>
            /// Initialize tree node with specified character
            /// </summary>
            /// <param name="parent">Parent node</param>
            /// <param name="word">word</param>
            public TreeNode( TreeNode parent, string word )
            {
                Word   = word;
                Parent = parent;
            }

            /// <summary>
            /// Adds pattern ending in this node
            /// </summary>
            /// <param name="ngram">Pattern</param>
            public void AddNgram( string[] ngram )
            {
                if ( _Ngrams == null ) _Ngrams = new HashSet< string[] >( StringsIEqualityComparer.Inst );
                _Ngrams.Add( ngram );
            }

            /// <summary>
            /// Adds trabsition node
            /// </summary>
            /// <param name="node">Node</param>
            public void AddTransition( TreeNode node )
            {
                if ( _TransDict == null ) _TransDict = new Dictionary< string, TreeNode >();
                _TransDict.Add( node.Word, node );
            }

            /// <summary>
            /// Returns transition to specified character (if exists)
            /// </summary>
            /// <param name="c">Character</param>
            /// <returns>Returns TreeNode or null</returns>
            public TreeNode GetTransition( string word ) => (_TransDict != null) && _TransDict.TryGetValue( word, out var node ) ? node : null;

            /// <summary>
            /// Returns true if node contains transition to specified character
            /// </summary>
            /// <param name="c">Character</param>
            /// <returns>True if transition exists</returns>
            public bool ContainsTransition( string word ) => ((_TransDict != null) && _TransDict.ContainsKey( word ));
            #endregion

            #region [.properties.]
            private Dictionary< string, TreeNode > _TransDict;
            private HashSet< string[] > _Ngrams;

            /// <summary>
            /// Character
            /// </summary>
            public string Word { get; }

            /// <summary>
            /// Parent tree node
            /// </summary>
            public TreeNode Parent { get; }

            /// <summary>
            /// Failure function - descendant node
            /// </summary>
            public TreeNode Failure { get; internal set; }

            /// <summary>
            /// Transition function - list of descendant nodes
            /// </summary>
            public ICollection< TreeNode > Transitions => ((_TransDict != null) ? _TransDict.Values : null);

            /// <summary>
            /// Returns list of patterns ending by this letter
            /// </summary>
            public ICollection< string[] > Ngrams => _Ngrams;
            public bool HasNgrams => (_Ngrams != null);
            #endregion

            public override string ToString() => ((Word != null) ? ('\'' + Word + '\'') : "ROOT") +
                                                 ", transitions(descendants): " + ((_TransDict != null) ? _TransDict.Count : 0) + ", ngrams: " + ((_Ngrams != null) ? _Ngrams.Count : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        private struct Finder
        {
            private TreeNode _Root;
            private TreeNode _Node;
            public static Finder Create( TreeNode root ) => new Finder() { _Root = root, _Node = root };

            public TreeNode Find( string word )
            {
                TreeNode transNode;
                do
                {
                    transNode = _Node.GetTransition( word );
                    if ( _Node == _Root )
                    {
                        break;
                    }
                    if ( transNode == null )
                    {
                        _Node = _Node.Failure;
                    }
                }
                while ( transNode == null );
                if ( transNode != null )
                {
                    _Node = transNode;
                }
                return (_Node);
            }
        }


        #region [.ctor().]
        private TreeNode _Root;
        public Searcher( IList< string[] > ngrams ) => _Root = TreeNode.BuildTree( ngrams );
        #endregion

        #region [.public method's & properties.]
        public SearchResult? FindFirstIngnoreCase( List< word_t > words )
        {
            var ss = FindAllIngnoreCaseInternal( words );
            if ( ss != null )
            {
                return (ss.Min);
            }
            return (null);
        }
        public SearchResult? FindFirstSensitiveCase( List< word_t > words )
        {
            var ss = FindAllSensitiveCaseInternal( words );
            if ( ss != null )
            {
                return (ss.Min);
            }
            return (null);
        }
        public ICollection< SearchResult > FindAllIngnoreCase( List< word_t > words ) => FindAllIngnoreCaseInternal( words );
        public ICollection< SearchResult > FindAllSensitiveCase( List< word_t > words ) => FindAllSensitiveCaseInternal( words );

        private SortedSet< SearchResult > FindAllIngnoreCaseInternal( List< word_t > words )
        {
            var ss = default(SortedSet< SearchResult >);
            var finder = Finder.Create( _Root );

            for ( int index = 0, len = words.Count; index < len; index++ )
            {
                var valueUpper = words[ index ].valueUpper;
                if ( valueUpper == null )
                    continue;

                var node = finder.Find( valueUpper );
                if ( node.HasNgrams )
                {
                    if ( ss == null ) ss = new SortedSet< SearchResult >( SearchResult.Comparer.Inst );

                    foreach ( var ngram in node.Ngrams )
                    {
                        ss.Add( new SearchResult( index - ngram.Length + 1, ngram.Length ) );
                    }
                }
            }
            return (ss);
        }
        private SortedSet< SearchResult > FindAllSensitiveCaseInternal( List< word_t > words )
        {
            var ss = default(SortedSet< SearchResult >);
            var finder = Finder.Create( _Root );

            for ( int index = 0, len = words.Count; index < len; index++ )
            {
                var valueOriginal = words[ index ].valueOriginal;
                if ( valueOriginal == null )
                    continue;

                var node = finder.Find( valueOriginal );
                if ( node.HasNgrams )
                {
                    if ( ss == null ) ss = new SortedSet< SearchResult >( SearchResult.Comparer.Inst );

                    foreach ( var ngram in node.Ngrams )
                    {
                        ss.Add( new SearchResult( index - ngram.Length + 1, ngram.Length ) );
                    }
                }
            }
            return (ss);
        }
        #endregion

        public override string ToString() => $"[{_Root}]";
    }
}
