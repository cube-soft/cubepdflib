using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CubePdf.Data.Extensions
{
    public static class CopyExtensions
    {
        public static T Copy<T>(this T target)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, target);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
