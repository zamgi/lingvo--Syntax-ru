using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using lingvo.tokenizing;

namespace lingvo.syntax
{
    /// <summary>
    /// 
    /// </summary>
	internal sealed class ConcurrentFactory
	{
		private readonly Semaphore                          _Semaphore;
        private readonly ConcurrentStack< SyntaxProcessor > _Stack;

        public ConcurrentFactory( in SyntaxProcessorConfig config, int instanceCount )
		{
            if ( instanceCount <= 0 ) throw (new ArgumentException("instanceCount"));

            _Semaphore = new Semaphore( instanceCount, instanceCount );
            _Stack = new ConcurrentStack< SyntaxProcessor >();
            for ( int i = 0; i < instanceCount; i++ )
			{
                _Stack.Push( new SyntaxProcessor( config ) );
			}
		}

        public word_t[] Run( string text, bool splitBySmiles )
		{
			_Semaphore.WaitOne();
			var worker = default(SyntaxProcessor);
			try
			{
                worker = Pop( _Stack );
                if ( worker == null )
                {
                    for ( var i = 0; ; i++ )
                    {
                        worker = Pop( _Stack );
                        if ( worker != null )
                            break;

                        Thread.Sleep( 25 );

                        if ( 10000 <= i )
                            throw (new InvalidOperationException( this.GetType().Name + ": no (fusking) worker item in queue" ));
                    }
                }

                var result = worker.Run( text, splitBySmiles ).ToArray();
                return (result);
			}
			finally
			{
				if ( worker != null )
				{
					_Stack.Push( worker );
				}
				_Semaphore.Release();
			}

            throw (new InvalidOperationException( this.GetType().Name + ": nothing to return (fusking)" ));
		}
        public List< word_t[] > Run_Details( string text, bool splitBySmiles )
		{
			_Semaphore.WaitOne();
			var worker = default(SyntaxProcessor);
			try
			{
                worker = Pop( _Stack );
                if ( worker == null )
                {
                    for ( var i = 0; ; i++ )
                    {
                        worker = Pop( _Stack );
                        if ( worker != null )
                            break;

                        Thread.Sleep( 25 );

                        if ( 10000 <= i )
                            throw (new InvalidOperationException( this.GetType().Name + ": no (fusking) worker item in queue" ));
                    }
                }

                var result = worker.Run_Details( text, splitBySmiles ).ToList();
                return (result);
			}
			finally
			{
				if ( worker != null )
				{
					_Stack.Push( worker );
				}
				_Semaphore.Release();
			}

            throw (new InvalidOperationException( this.GetType().Name + ": nothing to return (fusking)" ));
		}

        private static T Pop<T>( ConcurrentStack< T > stack ) => stack.TryPop( out var t ) ? t : default;
	}
}
