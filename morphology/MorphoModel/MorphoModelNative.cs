using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using lingvo.core;

using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
    unsafe internal sealed class MorphoModelNative : MorphoModelBase, IMorphoModel
    {
        /// <summary>
        /// 
        /// </summary>
        private struct MorphoType_pair_t
        {
            public IntPtr           Name;
            public MorphoTypeNative MorphoType;
            public bool IsEmpty { [M(O.AggressiveInlining)] get => (Name == IntPtr.Zero); }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class CharIntPtr_EqualityComparer : IEqualityComparer< IntPtr >
        {
            public CharIntPtr_EqualityComparer() { }
            unsafe public bool Equals( IntPtr x, IntPtr y )
            {
                if ( x == y )
                    return (true);

                for ( char* x_ptr = (char*) x,
                            y_ptr = (char*) y; ; x_ptr++, y_ptr++ )
                {
                    var x_ch = *x_ptr;
                    if ( x_ch != *y_ptr )
                        return (false);
                    if ( x_ch == '\0' )
                        return (true);
                }
            }
            unsafe public int GetHashCode( IntPtr obj )
            {
                var ptr = (char*) obj;
                int n1 = 5381;
                int n2 = 5381;
                int n3;
                while ( (n3 = (int) (*(ushort*) ptr)) != 0 )
                {
                    n1 = ((n1 << 5) + n1 ^ n3);
                    n2 = ((n2 << 5) + n2 ^ n3);
                    ptr++;
                }
                return (n1 + n2 * 1566083941);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class PartOfSpeechToNativeStringMapper : IDisposable
        {
            private Dictionary< PartOfSpeechEnum, IntPtr > _Dictionary;
            public PartOfSpeechToNativeStringMapper()
            {
                _Dictionary = Enum.GetValues( typeof(PartOfSpeechEnum) )
                                  .Cast< PartOfSpeechEnum >()
                                  .ToDictionary( pos => pos, pos => Marshal.StringToHGlobalUni( pos.ToString() ) );
            }
            ~PartOfSpeechToNativeStringMapper() => DisposeNativeResources();
            public void Dispose()
            {
                DisposeNativeResources();
                GC.SuppressFinalize( this );
            }
            private void DisposeNativeResources()
            {
                if ( _Dictionary != null )
                {
                    foreach ( IntPtr ptr in _Dictionary.Values )
                    {
                        Marshal.FreeHGlobal( ptr );
                    }
                    _Dictionary = null;
                }
            }

            public IntPtr this[ PartOfSpeechEnum partOfSpeech ] { [M(O.AggressiveInlining)] get => _Dictionary[ partOfSpeech ]; }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class ModelLoader : IDisposable
        {
            #region [.private field's.]
            private const int                              ENDING_BUFFER_SIZE = 1024; //256;

            private static CharType*                       _CHARTYPE_MAP;
            private static char*                           _UPPER_INVARIANT_MAP;
            private static char*                           _LOWER_INVARIANT_MAP;

            private MorphoModelConfig                      _Config;
		    // морфо-типы
            private HashSet< IntPtr >                      _EndingHashSet;
            private Dictionary< IntPtr, MorphoTypeNative > _MorphoTypesDict;
            // список допустимых комбинаций морфо-аттрибутов в группах 
            private MorphoAttributeList                    _MorphoAttributeList;
            private PartOfSpeechList                       _PartOfSpeechList;
            private PartOfSpeechToNativeStringMapper       _PartOfSpeechToNativeStringMapper;
            private List< MorphoAttributePair >            _MorphoAttributePairs_Buffer;
            private Action< string, string >               _ModelLoadingErrorCallback;
            //---private GCHandle                               _LOWER_INVARIANT_MAP_GCHandle;
            //---private char*                                  _LOWER_INVARIANT_MAP;
            private char*                                  _ENDING_LOWER_BUFFER;
            private GCHandle                               _ENDING_LOWER_BUFFER_GCHandle;
            private char*                                  _ENDING_UPPER_BUFFER;
            private GCHandle                               _ENDING_UPPER_BUFFER_GCHandle;
            private EnumParser< MorphoAttributeEnum >      _EnumParserMorphoAttribute;
            private EnumParser< PartOfSpeechEnum    >      _EnumParserPartOfSpeech;
            // empty-native-string
            private IntPtr _EMPTY_STRING;

            //out-of-this-class created field & passed as params of 'Run'-method
            private TreeDictionary _TreeDictionary;
            private NativeMemAllocationMediator _NativeMemAllocator;
            #endregion

            #region [.ctor().]
            static ModelLoader()
            {
                _CHARTYPE_MAP        = xlat_Unsafe.Inst._CHARTYPE_MAP;
                _UPPER_INVARIANT_MAP = xlat_Unsafe.Inst._UPPER_INVARIANT_MAP;
                _LOWER_INVARIANT_MAP = xlat_Unsafe.Inst._LOWER_INVARIANT_MAP;
            }

            public static void RunLoad( in MorphoModelConfig config, TreeDictionary treeDictionary, NativeMemAllocationMediator nativeMemAllocationMediator )
            {
                using ( var loader = new ModelLoader( config, treeDictionary, nativeMemAllocationMediator ) )
                {
                    loader.RunLoad();
                }
            }
            public ModelLoader( in MorphoModelConfig config, TreeDictionary treeDictionary, NativeMemAllocationMediator nativeMemAllocationMediator )
            {
                const int ENDING_DICTIONARY_CAPACITY           = 350003; //prime
                const int MORPHOTYPES_DICTIONARY_CAPACITY      = 4001;   //prime
                const int MORPHOATTRIBUTEPAIRS_BUFFER_CAPACITY = 100;

                _TreeDictionary     = treeDictionary;
                _NativeMemAllocator = nativeMemAllocationMediator;
                //-----------------------------------------------//                
                TreeDictionary.BeginLoad();
                _Config                           = config;
                var comparer = new CharIntPtr_EqualityComparer();
                _EndingHashSet                    = new HashSet< IntPtr >( ENDING_DICTIONARY_CAPACITY, comparer );
                _MorphoTypesDict                  = new Dictionary< IntPtr, MorphoTypeNative >( MORPHOTYPES_DICTIONARY_CAPACITY, comparer );
                _MorphoAttributeList              = new MorphoAttributeList();
                _PartOfSpeechList                 = new PartOfSpeechList();
                _PartOfSpeechToNativeStringMapper = new PartOfSpeechToNativeStringMapper();
                _MorphoAttributePairs_Buffer      = new List< MorphoAttributePair >( MORPHOATTRIBUTEPAIRS_BUFFER_CAPACITY );
                _EnumParserMorphoAttribute        = new EnumParser< MorphoAttributeEnum >();
                _EnumParserPartOfSpeech           = new EnumParser< PartOfSpeechEnum >();

                _EMPTY_STRING = _NativeMemAllocator.AllocAndCopy( string.Empty );
                _EndingHashSet.Add( _EMPTY_STRING );

                #region [._ModelLoadingErrorCallback.]
                if ( config.ModelLoadingErrorCallback == null )
                {
                    _ModelLoadingErrorCallback = (s1, s2) => { };
                }
                else
                {
                    _ModelLoadingErrorCallback = config.ModelLoadingErrorCallback;
                }
                #endregion
                #region comm.[._LOWER_INVARIANT_MAP.]
                //var lower_invariant_map       = xlat.Create_LOWER_INVARIANT_MAP();
                //_LOWER_INVARIANT_MAP_GCHandle = GCHandle.Alloc( lower_invariant_map, GCHandleType.Pinned );
                //_LOWER_INVARIANT_MAP          = (char*) _LOWER_INVARIANT_MAP_GCHandle.AddrOfPinnedObject().ToPointer();
	            #endregion
                #region [._ENDING_LOWER_BUFFER.]
                var ending_lower_buffer       = new char[ ENDING_BUFFER_SIZE ];
                _ENDING_LOWER_BUFFER_GCHandle = GCHandle.Alloc( ending_lower_buffer, GCHandleType.Pinned );
                _ENDING_LOWER_BUFFER          = (char*) _ENDING_LOWER_BUFFER_GCHandle.AddrOfPinnedObject().ToPointer();
                #endregion
                #region [._ENDING_UPPER_BUFFER.]
                var ending_upper_buffer       = new char[ ENDING_BUFFER_SIZE ];
                _ENDING_UPPER_BUFFER_GCHandle = GCHandle.Alloc( ending_upper_buffer, GCHandleType.Pinned );
                _ENDING_UPPER_BUFFER          = (char*) _ENDING_UPPER_BUFFER_GCHandle.AddrOfPinnedObject().ToPointer();
                #endregion
            }
            ~ModelLoader() => DisposeNativeResources();
            public void Dispose()
            {
                DisposeNativeResources();
                GC.SuppressFinalize( this );
            }
            private void DisposeNativeResources()
            {
                _TreeDictionary = null;
                //-----------------------------------------------//                
                TreeDictionary.EndLoad();
                //---_LOWER_INVARIANT_MAP_GCHandle.Free(); _LOWER_INVARIANT_MAP = null;
                _ENDING_LOWER_BUFFER_GCHandle.Free(); _ENDING_LOWER_BUFFER = null;
                _ENDING_UPPER_BUFFER_GCHandle.Free(); _ENDING_UPPER_BUFFER = null;
                _EndingHashSet                = null;
                _ModelLoadingErrorCallback    = null;
                _MorphoTypesDict              = null;
                _MorphoAttributeList          = null;
                _PartOfSpeechList             = null;
                _PartOfSpeechToNativeStringMapper.Dispose(); _PartOfSpeechToNativeStringMapper = null;
                _MorphoAttributePairs_Buffer  = null;                
                _EnumParserMorphoAttribute    = null;
                _EnumParserPartOfSpeech       = null;
                _Config                       = default;
            }
            #endregion

            public void RunLoad()
            {
#if DEBUG
                var sw = new Stopwatch();
# endif
                foreach ( var morphoTypesFilename in _Config.MorphoTypesFilenames )
                {
                    var filename = GetFullFilename( _Config.BaseDirectory, morphoTypesFilename );
#if DEBUG
                    sw.Restart(); 
#endif
                    ReadMorphoTypes( filename );
#if DEBUG
                    sw.Stop(); Console.WriteLine( $"morphology: '{filename}', elapsed: {sw.Elapsed}" ); 
#endif
                }

                foreach ( var properNamesFilename in _Config.ProperNamesFilenames )
                {
                    var filename = GetFullFilename( _Config.BaseDirectory, properNamesFilename );
#if DEBUG
                    sw.Restart(); 
#endif
                    ReadWords( filename, MorphoAttributeEnum.Proper );
#if DEBUG
                    sw.Stop(); Console.WriteLine( $"morphology: '{filename}', elapsed: {sw.Elapsed}" ); 
#endif
                }

                foreach ( var commonFilename in _Config.CommonFilenames )
                {
                    var filename = GetFullFilename( _Config.BaseDirectory, commonFilename );
#if DEBUG
                    sw.Restart(); 
#endif
                    ReadWords( filename, MorphoAttributeEnum.Common );
#if DEBUG
                    sw.Stop(); Console.WriteLine( $"morphology: '{filename}', elapsed: {sw.Elapsed}" ); 
#endif
                }

#if DEBUG
                sw.Restart(); 
#endif
                _TreeDictionary.Trim();
#if DEBUG
                sw.Stop(); Console.WriteLine( $"morphology: _TreeDictionary.Trim(), elapsed: {sw.Elapsed}" ); 
#endif
            }

		    /// <summary>
            /// чтение файла морфотипов, filename - полный путь к файлу
		    /// </summary>
            private void ReadMorphoTypes( string filename )
		    {
                const int MORPHOFORMS_CAPACITY = 150;

                var lines = ReadFile( filename );
                var morphoForms = new List< MorphoFormNative >( MORPHOFORMS_CAPACITY );

                var morphoForm = default(MorphoFormNative);
                var morphoTypePair = default(MorphoType_pair_t);
                var morphoTypePairLast = default(MorphoType_pair_t);
			    foreach ( var line in lines )
			    {
                    //if ( line == "Pronoun, MORPHO_TYPE:что" )
                    //  System.Diagnostics.Debugger.Break();

                    fixed ( char* lineBase = line )
                    {
                        /// новый морфо-тип
                        if ( TryCreateMorphoTypePair( lineBase, line.Length, ref morphoTypePair ) ) 
				        {
                            if ( !morphoTypePairLast.IsEmpty )
                            {
                                morphoTypePairLast.MorphoType.SetMorphoForms( morphoForms );

                                AddMorphoType2Dict( in morphoTypePairLast );
                            }
                            morphoForms.Clear();
                    
                            morphoTypePairLast = morphoTypePair;
				        }
				        else 
                        /// словоформа в последнем морфо-типе
                        if ( !morphoTypePairLast.IsEmpty )
				        {
                            if ( TryCreateMorphoForm( morphoTypePairLast.MorphoType, lineBase, ref morphoForm ) )
                            {
                                morphoForms.Add( morphoForm );
                            }
                            else
                            {
                                _ModelLoadingErrorCallback( "Wrong line format", line ); //throw (new MorphoFormatException());
                            }
                        }
                        else
                        {
                            _ModelLoadingErrorCallback( "Null MorphoType", line ); //throw (new NullPointerException());
                        }
                    }
			    }
                /// последний морфо-тип
                if ( !morphoTypePairLast.IsEmpty )
                {
                    morphoTypePairLast.MorphoType.SetMorphoForms( morphoForms );
                    AddMorphoType2Dict( in morphoTypePairLast );
                }
		    }

		    /// чтение файла со словами
		    /// path - полный путь к файлу
		    /// nounType - тип существи тельного
            private void ReadWords( string filename, MorphoAttributeEnum nounType )
		    {
                var lines = ReadFile( filename );

                var plw = default(ParsedLineWords_unsafe);

			    foreach ( var line in lines )
                {
                   fixed ( char* lineBase = line )
                   {
                        if ( !TryParseLineWords( lineBase, ref plw ) )
                        {
                            _ModelLoadingErrorCallback( "Wrong line format", line ); //throw (new MorphoFormatException());
                            continue;
                        }

				        if ( !_MorphoTypesDict.TryGetValue( (IntPtr) plw.MorphoTypeName, out var morphoType ) )
                        {
                            _ModelLoadingErrorCallback( "Unknown morpho-type", line ); //throw new UnknownMorphoTypeException();
                            continue;
                        }

                        if ( !StringsHelper.IsEqual( (IntPtr) plw.PartOfSpeech, _PartOfSpeechToNativeStringMapper[ morphoType.PartOfSpeech ] ) )
                        {
                            _ModelLoadingErrorCallback( "Wrong part-of-speech", line ); //throw new WrongPartOfSpeechException();
                            continue;
                        }
                        
                        if ( morphoType.HasMorphoForms )
                        {
                            #region [.alloc native-memory for _base-of-word.]
                            var len = plw.WordLength - StringsHelper.GetLength( morphoType.FirstEnding );
                            len = ((0 <= len) ? len : plw.WordLength);

                            IntPtr lineBasePtr;
                            if ( 0 < len )
                            {
                                *(lineBase + len) = '\0';
                                lineBasePtr = new IntPtr( lineBase );

                                if ( _EndingHashSet.TryGetValue( lineBasePtr, out var existsPtr ) )
                                {
                                    lineBasePtr = existsPtr;
                                }
                                else
                                {
                                    lineBasePtr = _NativeMemAllocator.AllocAndCopy( lineBase, len );
                                    var suc = _EndingHashSet.Add( lineBasePtr );
                                    Debug.Assert( suc );
                                }
                            }
                            else
                            {
                                lineBasePtr = _EMPTY_STRING;
                            }
                            #endregion

                            if ( (morphoType.MorphoAttributeGroup & MorphoAttributeGroupEnum.NounType) == MorphoAttributeGroupEnum.NounType )
                            {
                                if ( !_MorphoAttributeList.TryGetMorphoAttributePair( MorphoAttributeGroupEnum.NounType, nounType, ref _TempMorphoAttributePair ) )
                                {
                                    throw (new MorphoFormatException());
                                }

                                _TreeDictionary.AddWord( (char*) lineBasePtr, morphoType, in _TempMorphoAttributePair );
                            }
                            else
                            {
                                _TreeDictionary.AddWord( (char*) lineBasePtr, morphoType );
                            }
                        }
                    }
			    }
		    }
            /*private void ReadWordsParallely( string filename, MorphoAttributeEnum nounType )
		    {
                const int LINES_CAPACITY = 200000;
                
                var lines = new List< string >( LINES_CAPACITY );
                var seq = ReadFile( filename );
                foreach ( var line in seq )
                {
                    lines.Add( line );
                }

                //var sw = new Stopwatch();
                using ( var rwls_EndingDictionary = new ReaderWriterLockSlim() )
                using ( var rwls_TreeDictionary   = new ReaderWriterLockSlim() )
                {
                    var partitioner = Partitioner.Create( 0, lines.Count, lines.Count / Environment.ProcessorCount + 1 );

                    Parallel.ForEach( partitioner, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },
                    (t) =>
                    {
                        var plw = default(ParsedLineWords);

                        for ( var i = t.Item1; i < t.Item2; i++ )
                        {
                            fixed ( char* lineBase = lines[ i ] )
                            {
                                if ( !ParseLineWords( lineBase, ref plw ) )
                                {
                                    _ModelLoadingErrorCallback( "Wrong line format", lines[ i ] ); //throw (new MorphoFormatException());
                                    continue;
                                }

                                MorphoTypeNative morphoType = GetMorphoTypeByName( (IntPtr) plw.MorphoTypeName );
                                if ( morphoType == null )
                                {
                                    _ModelLoadingErrorCallback( "Unknown morpho-type", lines[ i ] ); //throw new UnknownMorphoTypeException();
                                    continue;
                                }

                                if ( !StringsHelper.IsEqual( (IntPtr) plw.PartOfSpeech, _PartOfSpeechToNativeStringMapper[ morphoType.PartOfSpeech ] ) )
                                {
                                    _ModelLoadingErrorCallback( "Wrong part-of-speech", lines[ i ] ); //throw new WrongPartOfSpeechException();
                                    continue;
                                }

                                if ( morphoType.HasMorphoForms )
                                {
                                    var nounTypePair = default( MorphoAttributePair? );
                                    if ( (morphoType.MorphoAttributeGroup & MorphoAttributeGroupEnum.NounType) == MorphoAttributeGroupEnum.NounType )
                                    {
                                        nounTypePair = _MorphoAttributeList.GetMorphoAttributePair_2( MorphoAttributeGroupEnum.NounType, nounType );
                                    }

                                    var len = plw.WordLength - StringsHelper.GetLength( morphoType.FirstEnding );

                                    #region [.alloc native-memory for _base-of-word.]
                                    len = ((0 <= len) ? len : plw.WordLength);

                                    IntPtr basePtr;
                                    if ( 0 < len )
                                    {
                                        *(lineBase + len) = '\0';
                                        basePtr = new IntPtr( lineBase );

                                        IntPtr existsPtr;
                                        rwls_EndingDictionary.EnterReadLock();
                                        var exists = _EndingDictionary.TryGetValue( basePtr, out existsPtr );
                                        rwls_EndingDictionary.ExitReadLock();
                                        if ( exists )
                                        {
                                            basePtr = existsPtr;
                                        }
                                        else
                                        {                                            
                                            AllocHGlobalAndCopy( lineBase, len, out basePtr );

                                            rwls_EndingDictionary.EnterWriteLock();
                                            _EndingDictionary.Add( basePtr, basePtr );
                                            rwls_EndingDictionary.ExitWriteLock();
                                        }    
                                    }
                                    else
                                    {
                                        basePtr = _EMPTY_STRING;
                                    }
                                    #endregion

                                    //sw.Start();
                                    rwls_TreeDictionary.EnterWriteLock();
                                    _TreeDictionary.AddWord( (char*) basePtr, morphoType, ref nounTypePair );
                                    rwls_TreeDictionary.ExitWriteLock();
                                    //sw.Stop();
                                }
                            }
                        }
                    } );
                }                
                //Console.WriteLine( "(elapsed:" + sw.Elapsed + ", " + filename + ')' );
		    }*/

            /// <summary>
            /// сохранение морфотипа
            /// </summary>
            [M(O.AggressiveInlining)] private void AddMorphoType2Dict( in MorphoType_pair_t morphoTypePair )
            {
                if ( _MorphoTypesDict.ContainsKey( morphoTypePair.Name ) )
                {
                    _ModelLoadingErrorCallback( "Duplicated morpho-type", StringsHelper.ToString( morphoTypePair.Name ) );
                }
                else
                {
                    _MorphoTypesDict.Add( morphoTypePair.Name, morphoTypePair.MorphoType );
                }
            }

            /// <summary>
            /// создание морфоформы из строки
            /// </summary>
            private MorphoAttributePair _TempMorphoAttributePair;
            [M(O.AggressiveInlining)] private bool TryCreateMorphoForm( MorphoTypeNative morphoType, char* lineBase, ref MorphoFormNative morphoForm )
		    {
                #region [.find index-of-COLON & check on length.]
                var index = IndexOf( lineBase, COLON );
                if ( (index == -1) || (ENDING_BUFFER_SIZE <= index) )
                {
                    _ModelLoadingErrorCallback( "Index of COLON is undefined or length the line is too long", StringsHelper.ToString( lineBase ) );
                    return (false);
                }
                #endregion

                #region [.fill '_ENDING_LOWER_BUFFER'.]
                var i = 0;
                for ( char* ptr = lineBase; i < index; ptr++, i++ )
                {                    
                    var ch = *ptr;
                    if ( (_CHARTYPE_MAP[ ch ] & CharType.IsWhiteSpace) == CharType.IsWhiteSpace )
                        break;
                    _ENDING_LOWER_BUFFER[ i ] = _LOWER_INVARIANT_MAP[ ch ];
                }
                _ENDING_LOWER_BUFFER[ i ] = '\0';
                #endregion

                #region [.fill '_MorphoAttributePairs_Buffer'.]
                _MorphoAttributePairs_Buffer.Clear();
                for ( char* ptr = lineBase + index + 1; ; ptr++ )
                {
                    var ch = *ptr;
                    if ( ch == '\0' )
                        break;

                    if ( (_CHARTYPE_MAP[ ch ] & CharType.IsLetter) != CharType.IsLetter )
                        continue;

                    var len = 0;
                    for ( ; ; ptr++ )
                    {
                        ch = *ptr;
                        if ( ch == '\0' )
                            break;
                        var ct = _CHARTYPE_MAP[ ch ];
                        if ( (ct & CharType.IsLetter) != CharType.IsLetter &&
                                (ct & CharType.IsDigit)  != CharType.IsDigit )
                        {
                            break;
                        }
                        len++;
                    }
                    if ( len != 0 )
                    {
                        var morphoAttribute = default(MorphoAttributeEnum);
                        if ( _EnumParserMorphoAttribute.TryParse( ptr - len, len, ref morphoAttribute ) )
                        {
                            if ( _MorphoAttributeList.TryGetMorphoAttributePair( morphoType.MorphoAttributeGroup, morphoAttribute, ref _TempMorphoAttributePair ) )
                            {
                                _MorphoAttributePairs_Buffer.Add( _TempMorphoAttributePair );
                            }
#if DEBUG
                            //*
                            //TOO MANY ERRORS AFTER last (2016.12.28) getting morpho-dcitionaries from 'lingvo-[ilook]'
                            else
                            {
                                var attr = new string( ptr - len, 0, len );
                                _ModelLoadingErrorCallback( "Error in morpho-attribute: '" + attr + '\'', StringsHelper.ToString( lineBase ) );
                            }
                            //*/
#endif
                        }
                        else
                        {
                            var attr = new string( ptr - len, 0, len );
                            _ModelLoadingErrorCallback( "Unknown morpho-attribute: '" + attr + '\'', StringsHelper.ToString( lineBase ) );
                        }
                    }

                    if (ch == '\0')
                        break;
                }
                #endregion

                #region [.alloc native-memory for ending-of-word.]
                IntPtr endingPtr;
                IntPtr endingUpperPtr;
                if ( (i == 1) && (_ENDING_LOWER_BUFFER[ 0 ] == UNDERLINE) )
                {
                    //this is a '_EMPTY_STRING'. it is already in '_EndingDictionary'
                    endingPtr      = _EMPTY_STRING;
                    endingUpperPtr = _EMPTY_STRING;
                }
                else
                {
                    #region [.v2.]
                    #region [.ending-in-original-case.]
                    endingPtr = new IntPtr( _ENDING_LOWER_BUFFER );
                    if ( _EndingHashSet.TryGetValue( endingPtr, out var existsPtr ) )
                    {
                        endingPtr = existsPtr;
                    }
                    else
                    {
                        endingPtr = _NativeMemAllocator.AllocAndCopy( _ENDING_LOWER_BUFFER, index );
                        var suc = _EndingHashSet.Add( endingPtr );
                        Debug.Assert( suc );
                    }
                    #endregion

                    #region [.ending-in-upper-case.]
                    StringsHelper.ToUpperInvariant( _ENDING_LOWER_BUFFER, _ENDING_UPPER_BUFFER );

                    endingUpperPtr = new IntPtr( _ENDING_UPPER_BUFFER );
                    if ( _EndingHashSet.TryGetValue( endingUpperPtr, out existsPtr ) )
                    {
                        endingUpperPtr = existsPtr;
                    }
                    else
                    {
                        endingUpperPtr = _NativeMemAllocator.AllocAndCopy( _ENDING_UPPER_BUFFER, index );
                        var suc = _EndingHashSet.Add( endingUpperPtr );
                        Debug.Assert( suc );
                    } 
                    #endregion
                    #endregion
                }
                #endregion

                morphoForm = new MorphoFormNative( (char*) endingPtr, (char*) endingUpperPtr, _MorphoAttributePairs_Buffer );
                return (true);
            }

		    /// <summary>
            /// создание морфотипа из строки
		    /// </summary>
            [M(O.AggressiveInlining)] private bool TryCreateMorphoTypePair( char* lineBase, int lineLength, ref MorphoType_pair_t morphoTypePair )
		    {
                var index1 = IndexOf( lineBase, COMMA );
                if ( index1 == -1 )
                {
                    return (false);
                }
                var index2 = IndexAfter_MORPHO_TYPE( lineBase + index1 + 1 );
                if ( index2 == -1 )
                {
                    return (false);
                }

                var pos = default(PartOfSpeechEnum);
                if ( _EnumParserPartOfSpeech.TryParse( lineBase, index1, ref pos ) )
                {
                    var startIndex = index1 + 1 + index2 + 1;

                    var namePtr = _NativeMemAllocator.AllocAndCopy( lineBase + startIndex, lineLength - startIndex );

                    var morphoType = new MorphoTypeNative( _PartOfSpeechList.GetPartOfSpeech( pos ) );
                    morphoTypePair = new MorphoType_pair_t()
                    {
                        Name       = namePtr,
                        MorphoType = morphoType,
                    };
                    return (true);
                }
                else
                {
                    var pos_txt = StringsHelper.ToString( lineBase, index1 );
                    _ModelLoadingErrorCallback( "Unknown part-of-speech: '" + pos_txt + '\'', StringsHelper.ToString( lineBase ) );
                }
                return (false);
		    }
              
            [M(O.AggressiveInlining)] private static int IndexAfter_MORPHO_TYPE( char* ptr )
            {
                var index = IndexOf( ptr, COLON );
                if ( index == -1 )
                    return (-1);

                for ( char* p = ptr + index - 1; ptr <= p; p-- )
                {
                    if ( (_CHARTYPE_MAP[ *p ] & CharType.IsWhiteSpace) == CharType.IsWhiteSpace )
                        continue;

                    //"MORPHO_TYPE"
                    if ( *(p--) != 'E' ) break;
                    if ( *(p--) != 'P' ) break;
                    if ( *(p--) != 'Y' ) break;
                    if ( *(p--) != 'T' ) break;
                    if ( *(p--) != '_' ) break;
                    if ( *(p--) != 'O' ) break;
                    if ( *(p--) != 'H' ) break;
                    if ( *(p--) != 'P' ) break;
                    if ( *(p--) != 'R' ) break;
                    if ( *(p--) != 'O' ) break;
                    if ( *(p--) != 'M' ) break;
                    if ( (_CHARTYPE_MAP[ *p ] & CharType.IsWhiteSpace) != CharType.IsWhiteSpace ) break;

                    return (index);
                }

                return (-1);
            }
        }

        #region [.private field's.]
        // словарь слов
        private TreeDictionary _TreeDictionary;
        private NativeMemAllocationMediator _NativeMemAllocator;
        #endregion

        #region [.ctor().]
        public MorphoModelNative( in MorphoModelConfig config ) : base( config )
        {
            _TreeDictionary = new TreeDictionary();
            _NativeMemAllocator = new NativeMemAllocationMediator( nativeBlockAllocSize: 4096 * 100 );

            ModelLoader.RunLoad( in config, _TreeDictionary, _NativeMemAllocator );
        }
        public void Dispose() => _NativeMemAllocator.Dispose();
        #endregion

        #region [.IMorphoModel.]
        public bool TryGetWordFormMorphologies( string wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum mode ) => _TreeDictionary.TryGetWordFormMorphologies( wordUpper, result, mode );
        public bool TryGetWordFormMorphologies( char* wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum mode ) => _TreeDictionary.TryGetWordFormMorphologies( wordUpper, result, mode );
        public bool TryGetWordForms( string wordUpper, ICollection< WordForm_t > result ) => _TreeDictionary.TryGetWordForms( wordUpper, result );
        #endregion
    }
}