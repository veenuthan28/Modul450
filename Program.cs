using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
class PriceComparison
{
    static async Task Main()
    {
        Console.WriteLine("Geben Sie den Namen des Produkts ein, das Sie suchen möchten:");
        string? productName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(productName))
        {
            Console.WriteLine("Der Produktname darf nicht leer sein.");
            return;
        }

        // Chrome-Optionen konfigurieren
        var options = new ChromeOptions();
        options.AddArguments("--incognito");
        options.AddArguments("--disable-extensions");
        options.AddArguments("--disable-popup-blocking");
        // Fügen Sie hier weitere Optionen hinzu, die Sie benötigen

        using var driver = new ChromeDriver(options);
        foreach (var url in new[]
        {
            $"https://www.digitec.ch/de/search?q={productName}",
            $"https://www.galaxus.ch/de/search?q={productName}",
            $"https://www.amazon.com/s?k={productName}",
            $"https://www.microcenter.com/search?q={productName}",
        })
        {
            try
            {
                driver.Navigate().GoToUrl(url);
                await Task.Delay(10000); // Warten Sie, bis die Seite geladen ist.

                string xpathExpression = url.Contains("digitec.ch") ? "//*[@id=\"productListingContainer\"]/div[4]/article[1]/div[4]/span/span" :
                                        url.Contains("galaxus.ch") ? "//*[@id=\"productListingContainer\"]/div[4]/article[1]/div[4]" :
                                        url.Contains("amazon.com") ? "//*[@id=\"search\"]/div[1]/div[1]/div/span[1]/div[1]/div[4]/div/div/span/div/div/div/div[2]/div/div/div[3]/div[1]/div/div[3]/div/span[2]" :
                                        url.Contains("microcenter.com") ? "//*[@id=\"pricing\"]" : "";

                if (string.IsNullOrEmpty(xpathExpression))
                {
                    Console.WriteLine($"{url} - XPath-Ausdruck nicht definiert.");
                    continue;
                }

                // Preisinformation mit Selenium extrahieren
                var productNode = driver.FindElement(By.XPath(xpathExpression));
                var price = productNode != null ? productNode.Text.Trim() : "Preis nicht gefunden.";
                Console.WriteLine($"{url} - Preis: {price}");
            }
            catch (WebDriverException e)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten beim Zugriff auf {url}: {e.Message}");
            }
        }

        driver.Quit(); // Schließen Sie den Browser, wenn alle URLs abgearbeitet wurden
    }
}
