using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics; 
#endif
using System.Globalization;
using System.IO;
using System.Text;

using lingvo.core;

namespace lingvo.postagger
{
    /// <summary>
    /// 
    /// </summary>
    public struct MorphoAmbiguityResolverConfig
    {
        public MorphoAmbiguityResolverConfig( string modelFilename, string templateFilename_5g, string templateFilename_3g )
        {
            ModelFilename       = modelFilename;
            TemplateFilename_5g = templateFilename_5g;
            TemplateFilename_3g = templateFilename_3g;
        }

        public string ModelFilename       { get; set; }
        public string TemplateFilename_5g { get; set; }
        public string TemplateFilename_3g { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal sealed class ByteIntPtr_IEqualityComparer : IEqualityComparer< IntPtr >
    {
        public ByteIntPtr_IEqualityComparer() { }
        unsafe private static int getLength( byte* ptr )
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
        private NativeMemAllocationMediator _NativeMemAllocator;
        public MorphoAmbiguityResolverModel( in MorphoAmbiguityResolverConfig config )
        {
            config                    .ThrowIfNull("config");
            config.ModelFilename      .ThrowIfNullOrWhiteSpace("ModelFilename");
            config.TemplateFilename_5g.ThrowIfNullOrWhiteSpace("TemplateFilename_5g");
            config.TemplateFilename_3g.ThrowIfNullOrWhiteSpace("TemplateFilename_3g");

            Config = config;

            _NativeMemAllocator = new NativeMemAllocationMediator( nativeBlockAllocSize: 1024 * 512 );
            DictionaryBytes = LoadModelBytes( config.ModelFilename, _NativeMemAllocator );
        }
        ~MorphoAmbiguityResolverModel() => DisposeNativeResources();
        public void Dispose()
        {
            DisposeNativeResources();
            GC.SuppressFinalize( this );
        }
        private void DisposeNativeResources() => _NativeMemAllocator.Dispose();

        unsafe private struct key_t
        {
            public char* ptr;
            public int   length;
            public override string ToString() => StringsHelper.ToString( ptr, length ); //new string( key.ptr, 0, key.length );
        }
        unsafe private static Dictionary< IntPtr, float > LoadModelBytes( string modelFilename, NativeMemAllocationMediator nativeMemAllocator )
        {
            var NF = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            var NS = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign;
            var f  = default(float);

            var dict = new Dictionary< IntPtr, float >( 500_000, new ByteIntPtr_IEqualityComparer() );

            const int BYTE_BUFFER_SZIE = 4096;
            byte* bytesBuffer = stackalloc byte[ BYTE_BUFFER_SZIE ];

            var encoding = Encoding.UTF8;

            using ( var sr = new StreamReader( modelFilename ) )
            {
                for ( var line = sr.ReadLine(); line != null; line = sr.ReadLine() )
                {
                    var key = default(key_t);

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
                                f = float.Parse( value, NS, NF ); //its need for replace custom impl!
                                key = new key_t() { ptr = _base, length = (int) (ptr - _base) };

                                break;
                            }
                        }
                        #endregion
                    }
                    if ( key.ptr == ((char*) 0) )
                    {
                        throw (new InvalidDataException("Invalid data foramt: '" + line + '\'' ));
                    }
#if DEBUG
                    //Debug.Assert( key.Length * sizeof(char) <= BYTE_BUFFER_SZIE );
                    Debug.Assert( encoding.GetByteCount( key.ptr, key.length ) <= BYTE_BUFFER_SZIE );
#endif                        
                    var byteCount = encoding.GetBytes( key.ptr, key.length, bytesBuffer, BYTE_BUFFER_SZIE );

                    var bytesPtr = nativeMemAllocator.Alloc( byteCount );
                    Buffer.MemoryCopy( bytesBuffer, (void*) bytesPtr, byteCount, byteCount );

                    //Debug.Assert( encoding.GetString( (byte*) bytesPtr, byteCount ) == key.ToString() );

                    dict.Add( bytesPtr, f );
                }
            }

            return (dict);
        }

        public MorphoAmbiguityResolverConfig Config { get; }
        public Dictionary< IntPtr, float > DictionaryBytes { get; }
    }
}
