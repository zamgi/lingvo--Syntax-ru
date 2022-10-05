using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using lingvo.tokenizing;

namespace lingvo.syntax.webService
{
    /// <summary>
    /// 
    /// </summary>
	public sealed class ConcurrentFactory : IDisposable
	{
		private readonly SemaphoreSlim                      _Semaphore;
        private readonly ConcurrentStack< SyntaxProcessor > _Stack;

        public ConcurrentFactory( in SyntaxProcessorConfig config, IConfig opts )
		{
			var instanceCount = opts.CONCURRENT_FACTORY_INSTANCE_COUNT;
            if ( instanceCount <= 0 ) throw (new ArgumentException( nameof(instanceCount) ));
			Config = opts ?? throw (new ArgumentNullException( nameof(opts) ));

            _Semaphore = new SemaphoreSlim( instanceCount, instanceCount );
            _Stack     = new ConcurrentStack< SyntaxProcessor >();
            for ( int i = 0; i < instanceCount; i++ )
			{
                _Stack.Push( new SyntaxProcessor( config ) );
			}			
		}
        public void Dispose()
        {
            foreach ( var worker in _Stack )
            {
				worker.Dispose();
			}
			_Stack.Clear();
        }

		public IConfig Config { get; }

        public async Task< List< word_t[] > > Run_Details( string text, bool splitBySmiles )
        {
			await _Semaphore.WaitAsync().ConfigureAwait( false );
			var worker = default(SyntaxProcessor);
			var result = default(List< word_t[] >);
			try
			{
                worker = Pop( _Stack );
                result = worker.Run_Details( text, splitBySmiles );
			}
			finally
			{
                if ( worker != null )
				{
                    _Stack.Push( worker );
				}
				_Semaphore.Release();
			}
			return (result);
		}

        private static T Pop< T >( ConcurrentStack< T > stack ) => stack.TryPop( out var t ) ? t : default;
	}
}
