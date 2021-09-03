using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CharitySaleBackEnd.Models;
using CharitySaleBackEnd.Services;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CharitySaleBackEndTest
{
    class ProductItemTest
    {
        [Test]
        public void ProductItemDeserializeJSON()
        {
            // Test Products deserialized correct
            /* Content of configuration file:
             
                [
                    {
                    "categoryName": "Bake",
                    "name": "Coca-Cola",
                    "price": 0.89,
                    "count": 12
                    }
                ]

                On upload converts by browser to data string:
                data:application/json;base64,Ww0KICB7DQ...AgfQ0KXQ==
             */

            // Arrange
            string jsonFile = "data:application/json;base64,Ww0KICB7DQogICAgImNhdGVnb3J5TmFtZSI6ICJCYWtlIiwNCiAgICAibmFtZSI6ICJDb2NhLUNvbGEiLA0KICAgICJwcmljZSI6IDAuODksDQogICAgImNvdW50IjogMTINCiAgfQ0KXQ==";
            var dataFile = new DataFile(jsonFile);

            // Act
            Products result = new();
            try { result = JsonConvert.DeserializeObject<Products>(dataFile.FileContent); }
            catch (Exception) { }

            // Assert
            Assert.AreEqual("Bake", result.ElementAt(0).CategoryName);
            Assert.AreEqual("Coca-Cola", result.ElementAt(0).Name);
            Assert.AreEqual(0.89, result.ElementAt(0).Price);
            Assert.AreEqual(12, result.ElementAt(0).Count);
            Assert.AreEqual(true, result.ElementAt(0).IsValidProduct());
        }

        [Test]
        public void ProductItemDeserializeXML()
        {
            // Test Products deserialized correct
            /* Content of configuration file:
             
                <?xml version="1.0" encoding="utf-8" ?>
                <Products>
                    <ProductItem>
                        <CategoryName>Games</CategoryName>
                        <Name>Ball</Name>
                        <Price>35.99</Price>
                        <Count>3</Count>
                    </ProductItem>
                </Products>

                On upload converts by browser to data string:
                data:text/xml;base64,PD94bWwgdmVyc2l...kdWN0cz4=
             */

            // Arrange
            string jsonFile = "data:text/xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiID8+DQo8UHJvZHVjdHM+DQogICAgPFByb2R1Y3RJdGVtPg0KICAgICAgICA8Q2F0ZWdvcnlOYW1lPkdhbWVzPC9DYXRlZ29yeU5hbWU+DQogICAgICAgIDxOYW1lPkJhbGw8L05hbWU+DQogICAgICAgIDxQcmljZT4zNS45OTwvUHJpY2U+DQogICAgICAgIDxDb3VudD4zPC9Db3VudD4NCiAgICA8L1Byb2R1Y3RJdGVtPg0KPC9Qcm9kdWN0cz4=";
            var dataFile = new DataFile(jsonFile);

            // Act
            Products result = new();
            Serializer serializer = new Serializer();
            try { result = serializer.Deserialize<Products>(dataFile.FileContent); }
            catch (Exception) { }

            // Assert
            Assert.AreEqual("Games", result.ElementAt(0).CategoryName);
            Assert.AreEqual("Ball", result.ElementAt(0).Name);
            Assert.AreEqual(35.99, result.ElementAt(0).Price);
            Assert.AreEqual(3, result.ElementAt(0).Count);
            Assert.AreEqual(true, result.ElementAt(0).IsValidProduct());
        }
    }
}
