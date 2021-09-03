using CharitySaleBackEnd.Services;
using NUnit.Framework;

namespace CharitySaleBackEndTest
{
    public class DataFileTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateDataFile()
        {
            // Test for uploaded data for bulk load configuration to database
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
            string fileExt = dataFile.FileExtension.ToUpper();
            bool fileSourceIsCorrect = dataFile.FileSourceIsCorrect;

            // Assert
            Assert.AreEqual("JSON", fileExt);
            Assert.AreEqual(true, fileSourceIsCorrect);
        }


    }
}