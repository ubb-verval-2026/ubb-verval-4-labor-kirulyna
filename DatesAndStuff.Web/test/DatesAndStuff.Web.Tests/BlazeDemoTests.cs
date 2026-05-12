using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using FluentAssertions;
using OpenQA.Selenium.Support.UI;

namespace DatesAndStuff.Web.Tests;

[TestFixture]
public class BlazeDemoTests
{
    private IWebDriver driver;

    [SetUp]
    public void Setup()
    {
        ChromeOptions options = new ChromeOptions();
        options.BinaryLocation = "/usr/bin/brave-browser"; 
    
        driver = new ChromeDriver(options);
    }

    [TearDown]
    public void Teardown()
    {
        driver.Quit();
        driver.Dispose();
    }

    [Test]
    public void MexicoCityToDublin_ShouldHaveAtLeastThreeFlights()
    {
        driver.Navigate().GoToUrl("https://blazedemo.com");
        
        var fromPort = new SelectElement(driver.FindElement(By.Name("fromPort")));
        fromPort.SelectByValue("Mexico City");
        
        var toPort = new SelectElement(driver.FindElement(By.Name("toPort")));
        toPort.SelectByValue("Dublin");
        
        driver.FindElement(By.CssSelector("input[type='submit']")).Click();

        var rows = driver.FindElements(By.CssSelector("table.table tbody tr"));
        
        rows.Count.Should().BeGreaterThanOrEqualTo(3, "legalabb 3 jaratot vartunk Mexico City es Dublin kozott.");

        double priceLimit = 300.0; 
        
        foreach (var row in rows)
        {
            var priceText = row.FindElement(By.XPath("./td[6]")).Text.Replace("$", "");
            
            if (double.TryParse(priceText, out double price) && price < priceLimit)
            {
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, "olcsout_dublin.png");
                
                ss.SaveAsFile(filePath);
                break; 
            }
        }
    }
}