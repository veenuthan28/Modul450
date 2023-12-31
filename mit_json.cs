using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using System.IO;

class WebsiteConfig
{
    public string? Url { get; set; }
    public string? Xpath { get; set; }
}

class AppConfig
{
    public List<WebsiteConfig>? Websites { get; set; }
}

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

        try
        {
            // Laden der Konfiguration aus der JSON-Datei
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), "selenium_config.json");
            var config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(configPath));

            if (config == null || config.Websites == null || config.Websites.Count == 0)
            {
                Console.WriteLine("Fehler in der JSON-Konfiguration: Ungültiges Format oder leere Konfiguration.");
                return;
            }

            // Chrome-Optionen konfigurieren
            var options = new ChromeOptions();
            options.AddArguments("--incognito");
            options.AddArguments("--disable-extensions");
            options.AddArguments("--disable-popup-blocking");
            options.AddArguments("--ignore-certificate-errors");
            options.AddArguments("--ignore-ssl-errors");
            // Fügen Sie hier weitere Optionen hinzu, die Sie benötigen

            Console.WriteLine("Vor dem Öffnen des Browsers");
            using var driver = new ChromeDriver(options);
            Console.WriteLine("Nach dem Öffnen des Browsers");

            foreach (var website in config.Websites)
            {
                if (string.IsNullOrWhiteSpace(website.Url) || string.IsNullOrWhiteSpace(website.Xpath))
                {
                    Console.WriteLine("Url oder XPath ist null oder leer. Bitte überprüfen Sie die Konfigurationsdatei.");
                    continue;
                }

                // Fügen Sie den Suchbegriff direkt in die URL ein
                string url = website.Url + Uri.EscapeDataString(productName ?? "");
                try
                {
                    driver.Navigate().GoToUrl(url);
                    await Task.Delay(10000); // Anpassen basierend auf der Ladezeit der Webseite

                    // Preisinformation mit Selenium extrahieren
                    var price = driver.FindElement(By.XPath(website.Xpath)).Text.Trim();
                    Console.WriteLine($"{url} - Preis: {price}");
                }
                catch (Exception e)
                {
                    // Hier wird die Fehlermeldung unterdrückt, aber der Preis wird dennoch angezeigt
                    Console.WriteLine($"Ein Fehler ist aufgetreten beim Zugriff auf {url} oder beim Finden des Elements: {e.Message}");
                }
            }

            driver.Quit(); // Schließen Sie den Browser, wenn alle URLs abgearbeitet wurden
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
        }
    }
}
