namespace CqrsAccount.Domain
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class exists because ReactiveDomain forces its version of Rx on us, even though
    /// the bits of it we use do not even use Rx.
    /// </summary>
    sealed class CompositeDisposable : List<IDisposable>, IDisposable
    {
        public void Dispose()
        {
            foreach (var disp in this)
            {
                try
                {
                    disp?.Dispose();
                }
                catch
                {
                    // Ignore
                }
            }
        }
    }
}