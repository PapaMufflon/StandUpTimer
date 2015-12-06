using System;

namespace StandUpTimer.Common
{
    internal class Contract
    {
        public static void Requires<T>(bool predicate) where T : Exception, new() 
        {
            if (!predicate)
            {
                throw new T();
            }
        }
    }
}