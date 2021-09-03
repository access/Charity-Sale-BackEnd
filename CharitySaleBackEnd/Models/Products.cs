/// ==========================================
///  Title:     Charity-Sale-BackEnd
///  Author:    Jevgeni Kostenko
///  Date:      29.08.2021
/// ==========================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace CharitySaleBackEnd.Models
{
    [XmlRoot("Products")]
    public class Products : List<ProductItem>
    {
    }
}
