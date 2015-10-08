using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Redemption
{
    public static class Helper
    {
        public static void Retry(Action action, TimeSpan retryInterval, int retryCount = 3)
        {
            Retry<object>(() => 
            {
                action();
                return null;
            }, retryInterval, retryCount);
        }

        public static T Retry<T>(Func<T> action, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval);
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            
            foreach(var exception in exceptions)
                Logger.WriteLine(exception.Message);

            return default(T);
        }
    }
}
