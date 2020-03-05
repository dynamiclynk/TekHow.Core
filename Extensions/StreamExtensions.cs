using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TekHow.Core.Extensions
{
    public static class StreamExtensions
    {
        public static T To<T>(this Stream inStream, bool disposeStream = false)
        {

            try
            {
                var binForm = new BinaryFormatter();

                var retVal = (T)binForm.Deserialize(inStream);

                return retVal;
            }
            finally
            {
                if (disposeStream)
                {
                    inStream?.Dispose();
                }
            }
        }
    }
}
