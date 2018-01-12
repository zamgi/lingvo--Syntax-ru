using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using lingvo.sentsplitting;
using lingvo.tokenizing;

namespace lingvo.postagger
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PosTaggerResourcesModel
    {
        private static readonly string[] PRETEXTS = new[] { "ОТ", "ДО", "НА", "С", "ПО", "ИЗ" };
        public const string UNION_AND = "И";

        static PosTaggerResourcesModel()
        {
            Pretexts = new HashSet< string >( PRETEXTS );
        }

        public PosTaggerResourcesModel( string posTaggerResourcesXmlFilename )
        {
            var SPLIT_BY_SPACE = new[] { ' ' };
            var xdoc = XDocument.Load( posTaggerResourcesXmlFilename );

            //-1-
            var numbers = from xe in xdoc.Root.Element( "numbers" ).Elements()
                            let v = xe.Value.Trim().ToUpperInvariant()
                            where !string.IsNullOrEmpty( v )
                          select v;
            Numbers = new HashSet< string >( numbers );

            //-2-
            var phrases = from xe in xdoc.Root.Element( "phrases-rus" ).Elements()
                            let v = xe.Value.Trim().ToUpperInvariant()
                            where !string.IsNullOrEmpty( v )
                            let words = v.Split( SPLIT_BY_SPACE, StringSplitOptions.RemoveEmptyEntries )
                          select words;
            PhrasesSearcher = new AhoCorasick( phrases.ToList() );

            //-3-
            AbbreviationsSearcher = CreateAbbreviationSearcher( "abbreviations", xdoc, posTaggerResourcesXmlFilename );

            //-4-
            var abbreviations = from xe in xdoc.Root.Element( "abbreviations" ).Elements()
                                  let v = xe.Value.Trim().ToUpperInvariant().Replace( " ", string.Empty )
                                  where !string.IsNullOrEmpty( v )
                                select v;
            Abbreviations = new HashSet< string >( abbreviations );
        }

        private static AhoCorasick CreateAbbreviationSearcher( 
            string elementName, XDocument xdoc, string posTaggerResourcesXmlFilename )
        {
            const char DOT = '.';
            var SPLIT_BY_DOT = new[] { DOT };

            var words = (from xe in xdoc.Root.Element( elementName ).Elements()
                            let v = xe.Value.Trim()//-регистрозависимый!!!-.ToUpperInvariant()
                            where !string.IsNullOrEmpty( v )
                         select v
                        ).ToArray();
            var abbreviations = new List< string[] >();

            foreach ( var word in words )
            {
                var words_by_dot = word.Split( SPLIT_BY_DOT, StringSplitOptions.RemoveEmptyEntries );
                for ( int i = 0, len = words_by_dot.Length; i < len; i++ )
                {
                    var w = words_by_dot[ i ].Trim();
                    if ( string.IsNullOrEmpty( w ) )
                        throw (new InvalidDataException( "Wrong data in <abbreviation> section => [empty subitem], word: '" + word + "', pos-tagger-resources-xml-file: '" + posTaggerResourcesXmlFilename + '\'' ));

                    words_by_dot[ i ] = w + DOT;
                }

                var result = open_abbreviation_permutations( words_by_dot, true );
                //if ( result == null )
                    //throw (new InvalidDataException( "Wrong data in <abbreviation> section => [more then 4 subitems in abbreviation], word: '" + word + "', pos-tagger-resources-xml-file: '" + posTaggerResourcesXmlFilename + '\'' ));

                abbreviations.AddRange( result );
            }

            var abbreviationSearcher = new AhoCorasick( abbreviations );
            return (abbreviationSearcher);
        }

        private static List< string[] > open_abbreviation_permutations( string[] words_by_dot, bool getMoreThenOneSubitem )
        {
            switch ( words_by_dot.Length )
            {
                case 1:
                {
                    var str1 = words_by_dot[ 0 ];

                    var lst = new List< string[] >();
                    if ( !getMoreThenOneSubitem )
                    {
                        lst.Add( new[] { str1 } );
                    }
                    return (lst);
                }
                case 2:
                {
                    var str1 = words_by_dot[ 0 ];
                    var str2 = words_by_dot[ 1 ];

                    var lst = new List< string[] >();
                    if ( !getMoreThenOneSubitem )
                    {
                        lst.Add( new[] { str1 + str2 } );
                    }
                    lst.Add( new[] { str1,  str2 } );
                    return (lst);
                }
                default:
                {
                    var result = recurrent( 0, words_by_dot );

                    var lst = (from r in result
                                let a = r.ToArray()
                                where (!getMoreThenOneSubitem || (getMoreThenOneSubitem && (1 < a.Length)))
                               select a
                              ).ToList();
                    return (lst);
                }
            }
        }
        private static IEnumerable< IEnumerable< string > > recurrent( int startIndex, string[] strings )
        {
            if ( strings.Length - startIndex == 3 )
            {
                var str1 = strings[ startIndex     ];
                var str2 = strings[ startIndex + 1 ];
                var str3 = strings[ startIndex + 2 ];
                var result = three( str1, str2, str3 );
                foreach ( var r in result )
                {
                    yield return (r);
                }
            }
            else
            {
                var str = strings[ startIndex ];

                var result = recurrent( startIndex + 1, strings );
                foreach ( var r in result )
                {
                    yield return (Enumerable.Repeat( str, 1 ).Concat( r ));

                    var a = r.ToArray();
                    a[ 0 ] = str + a[ 0 ];
                    yield return (a);
                }
            }
        }
        private static IEnumerable< IEnumerable< string > > three( string str1, string str2, string str3 )
        {
            yield return (Enumerable.Repeat( str1 + str2 + str3, 1 ));
            yield return (Enumerable.Repeat( str1, 1 ).Concat( Enumerable.Repeat( str2, 1 ) ).Concat( Enumerable.Repeat( str3, 1 ) ));
            yield return (Enumerable.Repeat( str1 + str2, 1 ).Concat( Enumerable.Repeat( str3, 1 ) ));
            yield return (Enumerable.Repeat( str1, 1 ).Concat( Enumerable.Repeat( str2 + str3, 1 ) ));
        }

        internal HashSet< string > Numbers
        {
            get;
            private set;
        }
        internal HashSet< string > Abbreviations
        {
            get;
            private set;
        }
        internal AhoCorasick       PhrasesSearcher
        {
            get;
            private set;
        }
        internal AhoCorasick       AbbreviationsSearcher
        {
            get;
            private set;
        }
        internal static HashSet< string > Pretexts
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class PosTaggerProcessorConfig
    {
        public PosTaggerProcessorConfig( string tokenizerResourcesXmlFilename, 
                                         string posTaggerResourcesXmlFilename,
                                         LanguageTypeEnum   languageType,                                          
                                         SentSplitterConfig sentSplitterConfig )
        {
            Model           = new PosTaggerResourcesModel( posTaggerResourcesXmlFilename );
            TokenizerConfig = new TokenizerConfig( tokenizerResourcesXmlFilename )
            {
                TokenizeMode                       = TokenizeMode.PosTagger,
                LanguageType                       = languageType,
                SentSplitterConfig                 = sentSplitterConfig,
                PosTaggerInputTypeProcessorFactory = new PosTaggerInputTypeProcessorFactory( Model, languageType ),
            };
        }

        public PosTaggerProcessorConfig( TokenizerConfig tokenizerConfig, 
                                         string posTaggerResourcesXmlFilename )
        {
            Model           = new PosTaggerResourcesModel( posTaggerResourcesXmlFilename );
            TokenizerConfig = tokenizerConfig;

            //set pos-tagger specially
            TokenizerConfig.TokenizeMode |= TokenizeMode.PosTagger;
            if ( TokenizerConfig.PosTaggerInputTypeProcessorFactory == null )
            {
                TokenizerConfig.PosTaggerInputTypeProcessorFactory = new PosTaggerInputTypeProcessorFactory( Model, TokenizerConfig.LanguageType );
            }
        }

        public string                  ModelFilename
        {
            get;
            set;
        }
        public string                  TemplateFilename
        {
            get;
            set;
        }
        public TokenizerConfig         TokenizerConfig
        {
            get;
            private set;
        }
        public PosTaggerResourcesModel Model
        {
            get;
            private set;
        }

        public LanguageTypeEnum LanguageType
        {
            get { return (TokenizerConfig.LanguageType); }
        }
    }
}
