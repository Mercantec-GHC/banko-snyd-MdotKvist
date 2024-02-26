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

            Console.WriteLine("Hvor mange plader vil du bruge?");
            int antalPlader = Convert.ToInt32(Console.ReadLine());

            List<string[]> plader = new List<string[]>();

            using (IWebDriver webdriver = new ChromeDriver(driverPath))
            {
                webdriver.Navigate().GoToUrl(url);

                for (int i = 0; i < antalPlader; i++)
                {
                    Console.WriteLine($"Navn: {brugerNavn}{i}");
                    IWebElement inputField = webdriver.FindElement(By.Id("tekstboks"));
                    inputField.SendKeys(brugerNavn + i);

                    IWebElement generateButton = webdriver.FindElement(By.Id("knap"));
                    generateButton.Click();

                    System.Threading.Thread.Sleep(0);

                    IWebElement row1 = webdriver.FindElement(By.Id("p11"));
                    IWebElement row2 = webdriver.FindElement(By.Id("p12"));
                    IWebElement row3 = webdriver.FindElement(By.Id("p13"));

                    string bingoNumbers1 = row1.Text.Trim();
                    string bingoNumbers2 = row2.Text.Trim();
                    string bingoNumbers3 = row3.Text.Trim();

                    Console.WriteLine($"Tal for plade {i + 1}: {bingoNumbers1}, {bingoNumbers2}, {bingoNumbers3}");

                    string plade = $"{bingoNumbers1}, {bingoNumbers2}, {bingoNumbers3}";

                    string[] pladeTal = { bingoNumbers1, bingoNumbers2, bingoNumbers3 };
                    plader.Add(pladeTal);

                    // Clear input field for the next iteration
                    inputField.Clear();
                }
            }
            // Bed brugeren om at indtaste de råbte tal
            Console.WriteLine("Indtast de råbte tal (adskilt med komma): ");
            string indtastedeTalInput = Console.ReadLine();
            string[] indtastedeTal = indtastedeTalInput.Split(',');

            // Tjek for bingo på hver plade
            for (int i = 0; i < plader.Count; i++)
            {
                if (CheckForBingo(plader[i], indtastedeTal))
                {
                    Console.WriteLine($"Bingo på plade {i + 1}!");
                }
            }
        }

        static bool CheckForBingo(string[] pladeTal, string[] indtastedeTal)
        {
            // Tjek om alle tal på pladen findes i de indtastede tal
            foreach (string tal in pladeTal)
            {
                if (!Array.Exists(indtastedeTal, element => element.Trim() == tal.Trim()))
                {
                    Console.WriteLine("Der er desværrer ikke Bingo!");
                    return false; // Et tal på pladen er ikke i de indtastede tal, der er ikke bingo
                }
            }
            Console.WriteLine("BINGO!");
            // Hvis vi når her, betyder det at alle tal på pladen blev fundet i de indtastede tal, så der er bingo
            return true;
        }
    }
}

 //// Udskriv de oprettede arrays
            //for (int i = 0; i < plader.Count; i++)
            //{
            //    Console.WriteLine($"Plade {i + 1} tal:");
            //    foreach (var tal in plader[i])
            //    {
            //        Console.Write($"{tal}, ");
            //    }
            //    Console.WriteLine();
            //}