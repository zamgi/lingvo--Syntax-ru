using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using lingvo.core;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
    public enum WordFormMorphologyModeEnum
    {
        Default,                              //try get buth - starts with lower & upper
        FirstStartsWithUpperAfterLowerLetter, //first try get starts with lower, if not exists get starts with upper
        FirstStartsWithLowerAfterUpperLetter, //first try get starts with upper, if not exists get starts with lower
        StartsWithUpperLetter,                //try get starts with upper
        StartsWithLowerLetter,                //try get starts with lower
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMorphoModel : IDisposable
    {
        bool TryGetWordFormMorphologies( string wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode );
        unsafe bool TryGetWordFormMorphologies( char*  wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode );
        bool TryGetWordForms( string wordUpper, ICollection< WordForm_t > result );
    }

    /// <summary>
    /// 
    /// </summary>
    internal abstract class MorphoModelBase
    {
        /// нулевое окончание
        protected const string EMPTY_ENDING = "_";
        /// регулярное выражение для префикса морфотипа
        protected Regex MORPHOTYPE_PREFIX_REGEXP    { get; private set; }
        /// разделитель морфоатрибутов
        protected char[] MORPHO_ATTRIBUTE_SEPARATOR { get; private set; }
        /// разделитель в словаре слов
        protected char[] WORDS_DICTIONARY_SEPARATOR { get; private set; }

        protected const char UNDERLINE     = '_';
        protected const char TABULATION    = '\t';
        protected const char COMMA         = ',';
        protected const char COLON         = ':';
        protected const string MORPHO_TYPE = "MORPHO_TYPE";

        protected MorphoModelBase( in MorphoModelConfig config ) => CheckConfig( in config );
        protected void Initialization()
        {
            MORPHOTYPE_PREFIX_REGEXP   = new Regex( "(([A-Za-z]+), MORPHO_TYPE:)" );
            MORPHO_ATTRIBUTE_SEPARATOR = new[] { COMMA };
            WORDS_DICTIONARY_SEPARATOR = new[] { TABULATION };                        
        }
        protected void UnInitialization()
        {
            MORPHOTYPE_PREFIX_REGEXP   = null;
            MORPHO_ATTRIBUTE_SEPARATOR = null;
            WORDS_DICTIONARY_SEPARATOR = null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected struct ParsedLineWords
        {
            public int    WordLength;
            public string MorphoTypeName;
            public string PartOfSpeech;
        }
        protected static bool TryParseLineWords( string line, ref ParsedLineWords plw )
        {
            var index1 = line.IndexOf( TABULATION );
            if ( index1 == -1 )
                return (false);

            var index2 = line.IndexOf( TABULATION, index1 + 1 );
            if ( index2 == -1 )
                return (false);

            var index3 = line.IndexOf( TABULATION, index2 + 1 );
            var partOfSpeech = (index3 == -1) ? line.Substring( index2 + 1 ) : line.Substring( index2 + 1, index3 - (index2 + 1) );

            plw.WordLength     = index1;
            //plw.Word           = line.Substring( 0, index1 );            
            plw.MorphoTypeName = line.Substring( index1 + 1, index2 - (index1 + 1) );
            plw.PartOfSpeech   = partOfSpeech;

            return (true);
        }

        unsafe protected struct ParsedLineWords_unsafe
        {
            public int   WordLength;
            public char* MorphoTypeName;
            public char* PartOfSpeech;

            public override string ToString() => ("MorphoTypeName: '" + StringsHelper.ToString( MorphoTypeName ) + 
                                                  "', PartOfSpeech: '" + StringsHelper.ToString( PartOfSpeech ) + '\'');
        }
        unsafe protected static bool TryParseLineWords( char* lineBase, ref ParsedLineWords_unsafe plw )
        {
            var index1 = IndexOf( lineBase, TABULATION );
            if ( index1 == -1 )
                return (false);

            var morphoTypeName = (lineBase + index1 + 1);
            var index2 = IndexOf( morphoTypeName, TABULATION );
            if ( index2 == -1 )
                return (false);

            var partOfSpeech = (morphoTypeName + index2 + 1);
            var index3 = IndexOf( partOfSpeech, TABULATION );

            //---*(line + index1) = '\0'; //end-of-Word
            *(morphoTypeName + index2) = '\0'; //end-of-MorphoTypeName
            if ( index3 != -1 )      
            {
                *(partOfSpeech + index3) = '\0'; //end-of-PartOfSpeech || till-end-of-string
            }

            plw.WordLength     = index1;
            plw.MorphoTypeName = morphoTypeName;
            plw.PartOfSpeech   = partOfSpeech;

            return (true);
        }

        unsafe protected static int IndexOf( char* ptr, char searchChar )
        {
            for ( int i = 0; ; ptr++, i++ )
            {
                var ch = *ptr;
                if ( ch == '\0' )
                    return (-1);

                if ( ch == searchChar )
                    return (i);
            }
        }

		/// чтение файла
		/// path - полный путь к файлу
		/// pLines [out] - вектор считанных строк
        protected static IEnumerable< string > ReadFile( string path )
		{
			using ( var sr = new StreamReader( path ) )
			{
				for ( var line = sr.ReadLine(); line != null; line = sr.ReadLine() )
                {
                    line = line.Trim();
                    if ( line.Length != 0 /*!string.IsNullOrEmpty( line )*/ )
                    {
                        yield return (line);
                    }
                }
            }
		}
        protected static string GetFullFilename( string folder, string filename )
        {
            if ( folder == null )
                folder = string.Empty;

            var fullFilename = folder.TrimEnd( '/', ' ' ) + '/' + filename.TrimStart( '/', ' ' );
            return (fullFilename);
            //return (Path.Combine( folder.TrimEnd( '/' ), filename.TrimStart( '/' ) ));
        }

        private static void CheckConfig( in MorphoModelConfig config )
        {
            config.ThrowIfNull( "config" );
            config.MorphoTypesFilenames.ThrowIfNullOrWhiteSpaceAnyElement( "config.MorphoTypesFilenames" );
            config.ProperNamesFilenames.ThrowIfNullOrWhiteSpaceAnyElement( "config.ProperNamesFilenames" );
            config.CommonFilenames     .ThrowIfNullOrWhiteSpaceAnyElement( "config.CommonFilenames" );
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MorphoModelFactory
    {
        public static IMorphoModel Create( in MorphoModelConfig config )
        {
            config.ThrowIfNull( nameof(config) );

            switch ( config.TreeDictionaryType )
            {
                case MorphoModelConfig.TreeDictionaryTypeEnum.Classic:
                case MorphoModelConfig.TreeDictionaryTypeEnum.FastMemPlenty:
                    return (new MorphoModel( config ));

                case MorphoModelConfig.TreeDictionaryTypeEnum.Native:
                    return (new MorphoModelNative( config ));

                default:
                    throw (new ArgumentException( config.TreeDictionaryType.ToString() ));
            }
        }
    }
}
