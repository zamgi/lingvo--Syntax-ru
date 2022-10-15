using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using lingvo.morphology;
using lingvo.postagger;
using lingvo.tokenizing;

namespace lingvo.syntax
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Program
    {
        private static async Task Main( string[] args )
        {
            try
            {
                using var env = await SyntaxEnvironment.CreateAsync().CAX();

                Run_1( env );
                //Run_2( "C:\\" );
            }
            catch ( Exception ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( ex );
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine( Environment.NewLine + "[.....finita fusking comedy.....]" );
            Console.ReadLine();
        }

        private static void Run_1( SyntaxEnvironment env )
        {
            using var syntaxProcessor = env.CreateSyntaxProcessor();

            var text = "Напомню, что, как правило, поисковые системы работают с так называемым обратным индексом, отличной метафорой которого будет алфавитный указатель в конце книги: все использованные термины приведены в нормальной форме и упорядочены лексикографически — проще говоря, по алфавиту, и после каждого указан номер страницы, на которой этот термин встречается. Разница только в том, что такая координатная информация в поисковиках, как правило, значительно подробнее. Например, корпоративный поиск МойОфис (рабочее название — baalbek), для каждого появления слова в документе хранит, кроме порядкового номера, ещё и его грамматическую форму и привязку к разметке.";
            var sents = syntaxProcessor.Run_Details( text, splitBySmiles: true );

            sents.Print2Console( text );
        }
        private static void Run_2( SyntaxEnvironment env, string path )
        {
            using var syntaxProcessor = env.CreateSyntaxProcessor();

            var n = 0;
            foreach ( var fn in EnumerateAllFiles( path ) )
            {
                var text = File.ReadAllText( fn );

                var sents = syntaxProcessor.Run_Details( text, splitBySmiles: true );

                Console_Write( $"{++n}.) ", ConsoleColor.DarkGray );
                sents.Print2Console( text );
            }
        }

        private static void Print2Console( this List< word_t[] > sents, string text )
        {
            Console.Write( $"text: " );
            Console_WriteLine( $"'{text.Cut().Norm()}'", ConsoleColor.DarkGray );
            if ( sents.Any() )
            {
                var max_width = Console.WindowWidth - 2;
                foreach ( var words in sents )
                {
                    var sum_width = 0;
                    var ts = words.Select( w => {
                        var sr = w.syntaxRoleType.ToText();
                        var max_len = Math.Max( sr.Length, w.valueOriginal.Length );                        
                        return (valueOriginal: w.valueOriginal.PadRight( max_len ), syntaxRoleType: sr.PadRight( max_len ));
                    })
                    .TakeWhile( t => {
                        sum_width += t.valueOriginal.Length + 1;
                        return (sum_width < max_width);
                    })
                    .ToList();

                    var postfix = ((ts.Count < words.Length) ? "..." : null);
                    Console.WriteLine( "  " + string.Join( " ", ts.Select( t => t.valueOriginal  ) ) + postfix );
                    Console_WriteLine( "  " + string.Join( " ", ts.Select( t => t.syntaxRoleType ) ) + postfix, ConsoleColor.Magenta );
                    Console.WriteLine();
                }
                //Console.WriteLine( "  " + string.Join( "\r\n  ", sents.Select( words => string.Join( " ", words.Select( w => $"{w.valueOriginal}:[{w.syntaxRoleType.ToText()}]" ) ) ) ) );
            }
            else
            {
                Console_WriteLine( "  [no any words found]", ConsoleColor.DarkRed );
            }
            Console.WriteLine();
        }

        private static IEnumerable< string > EnumerateAllFiles( string path, string searchPattern = "*.txt" )
        {
            try
            {
                var seq = Directory.EnumerateDirectories( path ).SafeWalk()
                                   .SelectMany( _path => EnumerateAllFiles( _path ) );
                return (seq.Concat( Directory.EnumerateFiles( path, searchPattern )/*.SafeWalk()*/ ));
            }
            catch ( Exception ex )
            {
                Debug.WriteLine( ex.GetType().Name + ": '" + ex.Message + '\'' );
                return (Enumerable.Empty< string >());
            }
        }

        private static void Console_Write( string msg, ConsoleColor color )
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write( msg );
            Console.ForegroundColor = fc;
        }
        private static void Console_WriteLine( string msg, ConsoleColor color )
        {
            var fc = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine( msg );
            Console.ForegroundColor = fc;
        }

        private static ConfiguredTaskAwaitable< T > CAX< T >( this Task< T > t ) => t.ConfigureAwait( false );
        private static ConfiguredTaskAwaitable CAX( this Task t ) => t.ConfigureAwait( false );
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class Extensions
    {
        public static string Cut( this string s, int max_len = 150 ) => (max_len < s.Length) ? s.Substring( 0, max_len ) + "..." : s;
        public static string Norm( this string s ) => s.Replace( '\n', ' ' ).Replace( '\r', ' ' ).Replace( '\t', ' ' ).Replace( "  ", " " );
        public static bool IsNullOrEmpty( this string value ) => string.IsNullOrEmpty( value );
        public static IEnumerable< T > SafeWalk< T >( this IEnumerable< T > source )
        {
            using ( var enumerator = source.GetEnumerator() )
            {
                for ( ; ; )
                {
                    try
                    {
                        if ( !enumerator.MoveNext() )
                            break;
                    }
                    catch ( Exception ex )
                    {
                        Debug.WriteLine( ex.GetType().Name + ": '" + ex.Message + '\'' );
                        continue;
                    }

                    yield return (enumerator.Current);
                }
            }
        }
    }
}
