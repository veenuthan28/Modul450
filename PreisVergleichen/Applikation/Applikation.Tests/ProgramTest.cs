using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Applikation.Tests
{
    [TestClass]
    public class PriceComparison
    {
        [TestMethod]
        [DataRow("https://www.fust.ch")]
        [DataRow("https://www.microspot.ch")]
        [DataRow("https://www.interdiscount.ch")]
        [DataRow("https://www.digitec.ch")]
        [DataRow("https://www.galaxus.ch")]
        public void TestIfUrlExistsInFile(string expectedStart)
        {
            // Arrange
            string filePath = @"../../../../Applikation.console\Vergleichen\iphone15_20240109141712.txt";
            ;

            // Act
            var fileContent = File.ReadAllText(filePath);

            // Assert
            Assert.IsTrue(fileContent.Contains(expectedStart), $"Die erwartete URL '{expectedStart}' wurde nicht in der Datei gefunden.");
        }

    [TestMethod]
        public void TestIfEachUrlStartsWithHttpsAndContainsDotCh()
        {
            // Arrange
            string filePath = @"../../../../Applikation.console\Vergleichen\iphone15_20240109141712.txt";
            var fileContent = File.ReadAllLines(filePath);

            // Act und Assert
            foreach (var line in fileContent)
            {
                Assert.IsTrue(line.StartsWith("https://www") && line.Contains(".ch"), $"Eine URL in der Datei entspricht nicht den Kriterien. URL: {line}");
            }
        }

        [TestMethod]
        public void TestIfPriceExistsInFile()
        {
            // Arrange
            string filePath = @"../../../../Applikation.console\Vergleichen\iphone 12 pro max_20240112203629.txt";

            // Act
            var fileContent = File.ReadAllText(filePath);

            // Assert
            Assert.IsTrue(fileContent.Any(char.IsDigit), $"Der Preis wurde nicht in der Datei gefunden.");
        }
    }
}
