using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;

public class WebsiteConfig
{
    public string? Url { get; set; }
    public string? Xpath { get; set; }
    public decimal Price { get; set; }

}

public class AppConfig
{
    public List<WebsiteConfig>? Websites { get; set; }
}

public class PriceComparison
{
    public static async Task Main()
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
            var configPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "selenium_config.json");
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

            List<string> outputLines = new List<string>();

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
                    var priceElement = driver.FindElement(By.XPath(website.Xpath));
                    string priceStr = priceElement.Text.Trim();

                    if (TryParsePrice(priceStr, out double price))
                    {
                        string outputLine = $"{url} - Preis: {price.ToString("C", CultureInfo.GetCultureInfo("de-CH"))}";
                        Console.WriteLine(outputLine);
                        outputLines.Add(outputLine);
                    }
                    else
                    {
                        Console.WriteLine($"Ein Fehler ist aufgetreten beim Extrahieren des Preises aus {url}: {priceStr}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ein Fehler ist aufgetreten beim Zugriff auf {url} oder beim Finden des Elements: {e.Message}");
                }
            }

            // Schließen Sie den Browser, wenn alle URLs abgearbeitet wurden
            driver.Quit();

            // Sortieren Sie die Preisinformationen
            var sortedOutputLines = outputLines.OrderBy(line => GetPriceFromLine(line)).ToList();

            // Ordner "vergleichen" erstellen, falls er nicht existiert
            string folderPath = @"C:\Users\veenu\OneDrive - GIBZ\GIBZ\dev\Infa2b\Informatik Module\M450\PreisVergleichen\Applikation\Applikation.console\Vergleichen\";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            // Methode zum Speichern der Ausgabe in eine Datei aufrufen
            string fileName = $"{ productName}_{DateTime.Now:yyyyMMddHHmmss}.txt";
            SaveOutputToFile(sortedOutputLines, Path.Combine(folderPath, fileName));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
        }
    }
    public static List<WebsiteConfig> LoadConfiguration(string configPath)
    {
        try
        {
            string configFileContent = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<AppConfig>(configFileContent);
            return config?.Websites ?? new List<WebsiteConfig>();
        }
        catch (Exception ex)
        {
            // Logge den Fehler oder handle ihn entsprechend
            Console.WriteLine($"Fehler beim Laden der Konfigurationsdatei: {ex.Message}");
            return new List<WebsiteConfig>();
        }
    }

    public static bool TryParsePrice(string priceStr, out double price)
    {
        // Entfernen Sie das Währungssymbol und das Tausendertrennzeichen, ersetzen Sie das Dezimalkomma durch einen Punkt
        priceStr = priceStr.Replace("CHF", "").Replace(".–", "").Trim();
        if (double.TryParse(priceStr, NumberStyles.Currency, CultureInfo.GetCultureInfo("de-CH"), out price))
        {
            return true;
        }
        return false;
    }

    public static double GetPriceFromLine(string line)
    {
        int index = line.IndexOf("Preis:");
        if (index >= 0)
        {
            string priceStr = line.Substring(index + 6).Trim();
            if (double.TryParse(priceStr, NumberStyles.Currency, CultureInfo.GetCultureInfo("de-CH"), out double price))
            {
                return price;
            }
        }
        return double.MaxValue; // Fallback, wenn der Preis nicht gefunden wurde
    }

    // Methode zum Speichern der Ausgabe in eine Datei
    public static void SaveOutputToFile(List<string> outputLines, string filePath)
    {
        try
        {
            File.WriteAllLines(filePath, outputLines);
            Console.WriteLine($"Die Ausgabe wurde in '{filePath}' gespeichert.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ein Fehler ist beim Speichern der Ausgabe aufgetreten: {e.Message}");
        }
    }
}