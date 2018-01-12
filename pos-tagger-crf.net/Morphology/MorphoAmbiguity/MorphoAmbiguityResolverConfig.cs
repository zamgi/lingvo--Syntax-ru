using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using lingvo.core;

namespace lingvo.postagger
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MorphoAmbiguityResolverConfig
    {
        public MorphoAmbiguityResolverConfig()
        {
        }
        public MorphoAmbiguityResolverConfig( string modelFilename, string templateFilename_5g, string templateFilename_3g )
        {
            ModelFilename       = modelFilename;
            TemplateFilename_5g = templateFilename_5g;
            TemplateFilename_3g = templateFilename_3g;
        }

        public string ModelFilename
        {
            get;
            set;
        }
        public string TemplateFilename_5g
        {
            get;
            set;
        }
        public string TemplateFilename_3g
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal sealed class ByteIntPtr_IEqualityComparer : IEqualityComparer< IntPtr >
    {
        public ByteIntPtr_IEqualityComparer()
        {
        }

        unsafe private int getLength( byte* ptr )
        {
            for ( var i = 0; ; i++ )
            {
                if ( *ptr == 0 )
                    return (i);
                ptr++;
            }
        }

        unsafe public bool Equals( IntPtr x, IntPtr y )
        {
            var x_ptr = (byte*) x.ToPointer();
            var y_ptr = (byte*) y.ToPointer();

            /*
            var x_len = getLength( x_ptr );
            var y_len = getLength( y_ptr );
            System.Diagnostics.Debug.Assert( x_len < 50, "50 < x_len" );
            System.Diagnostics.Debug.Assert( y_len < 50, "50 < y_len" );
            */

            //if ( x_ptr == y_ptr )
            //    return (true);

            for ( ; ; x_ptr++, y_ptr++)
            {
                var x_ch = *x_ptr;
                var y_ch = *y_ptr;

                if ( x_ch != y_ch )
                    return (false);
                if ( x_ch == '\0' )
                    return (true);
            }
        }
        unsafe public int GetHashCode( IntPtr obj )
        {
            byte* ptr = (byte*) obj.ToPointer();
            int num = 5381;
            int num2 = num;
            byte* ptr2 = ptr;
            int num3;
            while ( (num3 = (int) (*(byte*) ptr2)) != 0 )
            {
                num = ((num << 5) + num ^ num3);
                num2 = ((num2 << 5) + num2 ^ num3);
                ptr2++;
            }
            return (num + num2 * 1566083941);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class MorphoAmbiguityResolverModel : IDisposable
    {
        public MorphoAmbiguityResolverModel( MorphoAmbiguityResolverConfig config )
        {
            config                    .ThrowIfNull("config");
            config.ModelFilename      .ThrowIfNullOrWhiteSpace("ModelFilename");
            config.TemplateFilename_5g.ThrowIfNullOrWhiteSpace("TemplateFilename_5g");
            config.TemplateFilename_3g.ThrowIfNullOrWhiteSpace("TemplateFilename_3g");

            Config = config;

            /*Dictionary      = LoadModel     ( config.ModelFilename );*/
            DictionaryBytes = LoadModelBytes( config.ModelFilename );
        }

        ~MorphoAmbiguityResolverModel()
        {
            DisposeNativeResources();
        }
        public void Dispose()
        {
            DisposeNativeResources();

            GC.SuppressFinalize( this );
        }
        private void DisposeNativeResources()
        {
            if ( DictionaryBytes != null )
            {
                foreach ( var p in DictionaryBytes )
                {
                    Marshal.FreeHGlobal( p.Key );
                }
                DictionaryBytes = null;
            }
        }

        /*unsafe private static Dictionary< string, float > LoadModel( string modelFilename )
        {
            var NF = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            var NS = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign;
            var key = default(string);
            var f   = default(float);

            var dict = new Dictionary< string, float >( 200000 );

            using ( var sr = new StreamReader( modelFilename ) )
            {
                for ( var line = sr.ReadLine(); line != null; line = sr.ReadLine() )
                {
                    key = default( string );

                    fixed ( char* _base = line )
                    {
                        var ptr = _base;
                        for ( ; ; ptr++ )
                        {
                            var ch = *ptr;
                            if ( ch == '\0' )
                                break;

                            if ( ch == '\t' )
                            {                                
                                var value = new string( ptr + 1 );
                                f   = float.Parse( value, NS, NF );
                                key = new string( _base, 0, (int) (ptr - _base) );

                                break;
                            }
                        }
                    }

                    if ( key == default(string) )
                    {
                        throw (new InvalidDataException("Invalid data foramt: '" + line + '\'' ));
                    }

                    dict.Add( key, f );
                }
            }

            return (dict);
        }*/
        unsafe private static Dictionary< IntPtr, float > LoadModelBytes( string modelFilename )
        {
            var NF = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            var NS = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign;
            var key = default(string);
            var f   = default(float);

            var dict = new Dictionary< IntPtr, float >( 500000, new ByteIntPtr_IEqualityComparer() );

            using ( var sr = new StreamReader( modelFilename ) )
            {
                for ( var line = sr.ReadLine(); line != null; line = sr.ReadLine() )
                {
                    key = default(string);

                    fixed ( char* _base = line )
                    {
                        #region [.move from head.]
                        /*
                        var ptr = _base;
                        for ( ; ; ptr++ )
                        {
                            var ch = *ptr;
                            if ( ch == '\0' )
                                break;

                            if ( ch == '\t' )
                            {                                
                                var value = new string( ptr + 1 );
                                f   = float.Parse( value, NS, NF );
                                key = new string( _base, 0, (int) (ptr - _base) );

                                break;
                            }
                        }
                        */
                        #endregion

                        #region [.move from tail.]
                        var ptr = _base + line.Length - 1;
                        for ( ; _base <= ptr; ptr-- )
                        {
                            if ( *ptr == '\t' )
                            {
                                *(ptr++) = '\0';
                                var value = new string( ptr );
                                f = float.Parse( value, NS, NF );
                                key = new string( _base, 0, (int) (ptr - _base) );

                                break;
                            }
                        }
                        #endregion
                    }

                    if ( key == default(string) )
                    {
                        throw (new InvalidDataException("Invalid data foramt: '" + line + '\'' ));
                    }

                    //--- var bytes = Encoding.UTF8.GetBytes( key + '\0' );
                    var bytes = Encoding.UTF8.GetBytes( key );
                    var bytesPtr = Marshal.AllocHGlobal( bytes.Length );
                    Marshal.Copy( bytes, 0, bytesPtr, bytes.Length );

                    dict.Add( bytesPtr, f );
                }
            }

            return (dict);
        }

        public MorphoAmbiguityResolverConfig Config
        {
            get;
            private set;
        }
        /*public Dictionary< string, float >   Dictionary
        {
            get;
            private set;
        }*/
        public Dictionary< IntPtr, float >   DictionaryBytes
        {
            get;
            private set;
        }
    }
}
