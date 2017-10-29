//////////////////////////////////////////////
// Fork by : Derek Tremblay (derektremblay666@gmail.com)
// Original code : https://stackoverflow.com/questions/11447529/convert-an-object-to-an-xml-string
//////////////////////////////////////////////

using System.Xml.Serialization;

namespace WpfHexaEditor.Core
{
    internal class SerializeHelper
    {
        /// <summary>
        /// Serialize an object
        /// </summary>
        public static string Serialize<T>(T dataToSerialize)
        {
            if (dataToSerialize == null) return null;

            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
        }

        /// <summary>
        /// Deserialize an object
        /// </summary>
        public static T Deserialize<T>(string xmlText)
        {
            if (string.IsNullOrWhiteSpace(xmlText)) return default(T);

            using (var stringReader = new System.IO.StringReader(xmlText))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T) serializer.Deserialize(stringReader);
            }
        }
    }
}
