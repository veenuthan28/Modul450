using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Net.WebRequestMethods;

namespace Applikation.Tests
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void ConstructedUrl_ShouldMatchExpectedFormat()
        {
            // Arrange
            string productName = "Iphone 15";
            string baseUrl = "https://www.fust.ch/de/search.html?searchtext=";
            string expectedUrl = baseUrl + Uri.EscapeDataString(productName);

            // Act
            string actualUrl = "https://www.fust.ch/de/search.html?searchtext=iphone%2015";

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }
    }
}
