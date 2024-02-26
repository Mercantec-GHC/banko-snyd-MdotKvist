using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace BankoCheater
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string url = "https://mags-template.github.io/Banko/";
            string driverPath = @"C:\Users\kvist\Documents\GitHub\banko-snyd-MdotKvist\BankoCheater\chromedriver.exe";


            Console.WriteLine("Hvad er dit navn? ");
            string brugerNavn = Console.ReadLine();

            Console.WriteLine("Hvor mange plader vil du bruge ?");
            int antalPlader = Convert.ToInt32(Console.ReadLine());


            for (int i = 0; i < antalPlader; i++)
            {

            }



            using (IWebDriver webdriver = new ChromeDriver(driverPath))
            {

                webdriver.Navigate().GoToUrl(url);

                IWebElement inputField = webdriver.FindElement(By.Id("tekstboks"));

                inputField.SendKeys(brugerNavn);

                IWebElement generateButton = webdriver.FindElement(By.Id("knap"));
                generateButton.Click();

                System.Threading.Thread.Sleep(2000);

                IWebElement row1 = webdriver.FindElement(By.Id("p11"));
                IWebElement row2 = webdriver.FindElement(By.Id("p12"));
                IWebElement row3 = webdriver.FindElement(By.Id("p13"));

                string bingoNumbers1 = row1.Text.Trim();
                string bingoNumbers2 = row2.Text.Trim();
                string bingoNumbers3 = row3.Text.Trim();

                Console.WriteLine($"Navn: {brugerNavn}");
                Console.WriteLine($"Bingo tal: {bingoNumbers1}");
                Console.WriteLine($"Bingo tal: {bingoNumbers2}");
                Console.WriteLine($"Bingo tal: {bingoNumbers3}");
            }

        }
    }
}
