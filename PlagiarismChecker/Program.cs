// Ignore Spelling: Pdf

using System.Text.RegularExpressions;
using Microsoft.Playwright;

public static class BillFetcher
{
    public static async Task<string> GetRenderedBillText(string billId)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

        var page = await browser.NewPageAsync();
        await page.GotoAsync($"https://www.wyoleg.gov/Legislation/2025/{billId}?format=legislation");

        var billTextElement = page.Locator("div.compactLineHeight.htmlString").Filter(new()
        {
            HasText = "HOUSE BILL NO."
        });
        
        string billText = await billTextElement.InnerTextAsync();

        return billText;
    }

    public static string CleanBillText(string raw)
    {
        var text = Regex.Replace(raw, @"^.*?(?=HOUSE BILL NO\.)", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"\(END\).*$", "", RegexOptions.Singleline);

        return text;
    }

    public static async Task Main(string[] args)
    {

        string billText = await BillFetcher.GetRenderedBillText("HB0001");
        
        
        billText = BillFetcher.CleanBillText(billText);
        Console.WriteLine(billText);
    }
}
