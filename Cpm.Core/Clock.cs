using System;

namespace Cpm.Core
{
    public class Clock : IDisposable
    {
        private static DateTime? _nowForTest;

        public static DateTime Now => _nowForTest ?? DateTime.Now;

        private static readonly object ClockLock = new object();

        public static IDisposable NowIs(DateTime dateTime)
        {
            lock (ClockLock)
            {
                if (_nowForTest.HasValue)
                {
                    throw new InvalidOperationException("The clock is already overriden");
                }
                _nowForTest = dateTime;
            }
            return new Clock();
        }

        public void Dispose()
        {
            _nowForTest = null;
        }
    };
}
