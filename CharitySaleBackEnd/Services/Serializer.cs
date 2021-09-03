/// ==========================================
///  Title:     Charity-Sale-BackEnd
///  Author:    Jevgeni Kostenko
///  Date:      30.08.2021
/// ==========================================

using System.IO;
using System.Xml.Serialization;

namespace CharitySaleBackEnd.Services
{
    public class Serializer
    {
        public T Deserialize<T>(string xmlSource) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xmlSource))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }
    }
}
