using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace BankoCheater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Variabler til at definere Url stien og stien til ChromeDriveren
            string url = "https://mags-template.github.io/Banko/";
            string driverPath = @"C:\Users\kvist\Documents\GitHub\banko-snyd-MdotKvist\BankoCheater\chromedriver.exe";

            //Indtast navn her
            Console.WriteLine("Hvad er dit navn? ");
            string brugerNavn = Console.ReadLine();

            //Indtast hvor mange plader du vil bruge her
            Console.WriteLine("Hvor mange plader vil du bruge?");
            //Convert input til Int(64) og gem i variabelen antalPlader
            long antalPlader = Convert.ToInt64(Console.ReadLine());

            //Laver en liste Ved navn plader
            List<Plade> plader = new List<Plade>();

            //Laver en variable der gemmer indstillinger i den automatiseret ChromeBrowser
            ChromeOptions options = new ChromeOptions();
            options.PageLoadStrategy = PageLoadStrategy.None;

            using (IWebDriver webdriver = new ChromeDriver(driverPath))
            {

                webdriver.Navigate().GoToUrl(url);

                for (int i = 0; i < antalPlader; i++)
                {
                    IWebElement inputField = webdriver.FindElement(By.Id("tekstboks"));
                    inputField.SendKeys(brugerNavn + i);

                    IWebElement generateButton = webdriver.FindElement(By.Id("knap"));
                    generateButton.Click();

                    WebDriverWait wait = new WebDriverWait(webdriver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("p11"))); // Vent til elementet er synligt

                    Plade plade = new Plade
                    {
                        Navn = $"{brugerNavn}{i}",
                        Række1 = ParseToIntList(webdriver.FindElement(By.Id("p11")).Text),
                        Række2 = ParseToIntList(webdriver.FindElement(By.Id("p12")).Text),
                        Række3 = ParseToIntList(webdriver.FindElement(By.Id("p13")).Text),
                        BingoStatus = new bool[3]
                    };

                    plader.Add(plade);
                    inputField.Clear();
                }

                GemPlader(plader);
            }

            søgITal(plader);
        }

        static List<int> ParseToIntList(string numbers)
        {
            return numbers.Split(' ') //Deler Stringen op hvor der er mellemrum
                          .Select(n => int.TryParse(n, out int result) ? result : 0) //For hver opdelte string forsøger den at converter den til et helt tal, hvis den kan det returnere den tallet til result variablen. ellers returner vi 0
                          .Where(n => n != 0) //denne linje filtrer talene der ikke er nul fra (Den filtrer bokstaverne fra som den ikke kunne lave om til en int i forrige linje)
                          .ToList(); //Her pakker den tallene ind i en liste og retunere
        }

        //denne funktion gemmer pladerne i json filen
        static void GemPlader(List<Plade> plader)
        {
            string sti = @"C:\Users\kvist\Documents\GitHub\banko-snyd-MdotKvist\BankoCheater\plader.json";
            Console.Clear();

            try
            {
                string json = JsonConvert.SerializeObject(plader, Formatting.Indented);
                System.IO.File.WriteAllText(sti, json);
                Console.WriteLine("Pladerne er gemt korrekt!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl i forsøget på at gemme pladerne! {ex.Message}");
            }
        }

        static void søgITal(List<Plade> plader)
        {
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("Indtast de opråbte tal (eller skriv 'done' for at afslutte): ");
            List<int> indtastedeTal = new List<int>();
            string input;
            string tidligereTal = "";

            while ((input = Console.ReadLine()) != "done")
            {
                if (int.TryParse(input, out int tal))
                {
                    if (!indtastedeTal.Contains(tal)) // Check if the list already contains the number
                    {
                        indtastedeTal.Add(tal); // Add the number if it's not already in the list
                        Console.Clear();
                        Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal)); // Directly use the list to show previous numbers
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal)); // Directly use the list to show previous numbers

                        Console.WriteLine("Tallet er allerede indtastet, prøv igen.");
                    }

                    Console.WriteLine(tidligereTal.TrimEnd());
                }
                else
                {
                    Console.WriteLine("Ugyldigt input, prøv igen.");
                    continue; // Skip rest of the loop and start from the beginning
                }

                foreach (var plade in plader)
                {
                    int bingoRow = CheckBingoRow(plade, indtastedeTal);

                    if (bingoRow != -1)
                    {
                        Console.WriteLine($"\nBingo på række {bingoRow + 1}! {plade.Navn} har bingo på række {bingoRow + 1}: {string.Join(",", plade.Rækker[bingoRow])}\n");

                        bool allRowsBingo = plade.BingoStatus.All(status => status);
                        if (allRowsBingo)
                        {
                            Console.WriteLine($"\nBingo på fuld plade! {plade.Navn} har bingo på alle tre rækker!\n");
                        }
                    }
                }
            }
        }


        static int CheckBingoRow(Plade plade, List<int> indtastedeTal)
        {
            for (int i = 0; i < plade.Rækker.Count; i++)
            {
                if (!plade.BingoStatus[i] && plade.Rækker[i].All(t => indtastedeTal.Contains(t)))
                {
                    plade.BingoStatus[i] = true;
                    return i;
                }
            }
            return -1;
        }
    }

    class Plade
    {
        public string Navn { get; set; }
        public List<int> Række1 { get; set; }
        public List<int> Række2 { get; set; }
        public List<int> Række3 { get; set; }
        public List<List<int>> Rækker => new List<List<int>> { Række1, Række2, Række3 };
        public bool[] BingoStatus { get; set; } // Tilføjet for at holde styr på bingo-status for hver række
    }
}





//using System;
//using System.Collections.Generic;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using Newtonsoft.Json;

//namespace BankoCheater
//{
//    internal class Program
//    {
//        static void Main(string[] args)
//        {
//            //Variabler til at definere Url stien og stien til ChromeDriveren
//            string url = "https://mags-template.github.io/Banko/";
//            string driverPath = @"C:\Users\kvist\Documents\GitHub\banko-snyd-MdotKvist\BankoCheater\chromedriver.exe";

//            //Indtast navn her
//            Console.WriteLine("Hvad er dit navn? ");
//            string brugerNavn = Console.ReadLine();

//            //Indtast hvor mange plader du vil bruge her
//            Console.WriteLine("Hvor mange plader vil du bruge?");
//            //Convert input til Int(32) og gem i variabelen antalPlader
//            int antalPlader = Convert.ToInt32(Console.ReadLine());

//            //Laver en liste Ved navn plader
//            List<Plade> plader = new List<Plade>();

//            //Her fortæller vi programmet at hvergang vi skriver webdriver referer vi til ChromeDriver
//            //Den ved hvor Driveren ligger på grund af den variable vi har angivet i () Som referer til vores eksiterende variabel
//            using (IWebDriver webdriver = new ChromeDriver(driverPath))
//            {
//                //Her fortæller vi den at den skal navigere til vores URL
//                //Den variable der ligger i () altså url referer til vores eksisterende url Variable
//                webdriver.Navigate().GoToUrl(url);
//                //Console.Clear() sletter den tekst der er i konsollen
//                Console.Clear();

//                //Her starter vi et for loop som køre indtil vores variable: antalPlader er mindre end variablen i
//                for (int i = 0; i < antalPlader; i++)
//                {
//                    //Her skriver konsollen navn: også det brugerNavn der blev tastet ind tideligere
//                    //Der udover tilføjer vi variable i for at vi kan se i konsollen hvilken plader der er tale om 
//                    Console.WriteLine($"Navn: {brugerNavn}{i}");
//                    //her går ChromeDriveren så ind og finder et id i HTML'en som hedder "tekstboks"
//                    //Det id gemmer den så i en IWebElement der hedder InputField
//                    IWebElement inputField = webdriver.FindElement(By.Id("tekstboks"));
//                    //Denne linje indtaster Brugernavn og i altså tallet vi er kommer til på pladen ind på hjemme siden
//                    inputField.SendKeys(brugerNavn + i);
//                    //Her går ChromeDriveren ind i HTML'en og finder et id der hedder "knap"
//                    //Det id gemmer den så i en IWebElement variavle kaldet generateButton
//                    IWebElement generateButton = webdriver.FindElement(By.Id("knap"));
//                    //Den gemte variable generateButton Fortæller Click() funktionen hvad den skal trykke på
//                    generateButton.Click();

//                    //Denne linje får Programmet til at vente i 0 sekunder
//                    //Nullet i ()'erne kan ændres hvis hjemmesiden ikke kan nå at generer tallene inden programmet henter tallene
//                    System.Threading.Thread.Sleep(0);

//                    //I de 3 næste linjer finder ChromeDriveren de 3 id'ere "p11-p13" og gemmer id'erne i 3 variable row1-row3
//                    IWebElement row1 = webdriver.FindElement(By.Id("p11"));
//                    IWebElement row2 = webdriver.FindElement(By.Id("p12"));
//                    IWebElement row3 = webdriver.FindElement(By.Id("p13"));

//                    //Her trimmer vi teksten fra de 3 variabler i linjerne oven over og gemmer det i 3 string variabler bingoNumbersRow1-3
//                    string bingoNumbersRow1 = row1.Text.Trim();
//                    string bingoNumbersRow2 = row2.Text.Trim();
//                    string bingoNumbersRow3 = row3.Text.Trim();

//                    //Udskriv i konsollen hver gang den har kørt en plade igennem
//                    Console.WriteLine($"Tal for plade {i + 1}");
//                    Console.WriteLine($"Række 1: {bingoNumbersRow1}");
//                    Console.WriteLine($"Række 2: {bingoNumbersRow2}");
//                    Console.WriteLine($"Række 3: {bingoNumbersRow3}");

//                    //Her laver den Variablerne bingoNumbersRow1-3 om til int og gemmer det i en liste kaldet row1-3Numbers
//                    List<int> row1Numbers = ParseToIntList(bingoNumbersRow1);
//                    List<int> row2Numbers = ParseToIntList(bingoNumbersRow2);
//                    List<int> row3Numbers = ParseToIntList(bingoNumbersRow3);

//                    // Tilføj plade til listen
//                    Plade plade = new Plade
//                    {
//                        Navn = brugerNavn + i,
//                        Række1 = row1Numbers,
//                        Række2 = row2Numbers,
//                        Række3 = row3Numbers
//                    };
//                    plader.Add(plade);

//                    // Clear input field for the next iteration
//                    inputField.Clear();

//                    // Gem pladerne i en JSON-fil
//                    GemPlader(plader);
//                }

//                // Spørg brugeren om de opråbte tal og tjek for bingo
//                søgITal(plader);
//            }
//        }


//        static List<int> ParseToIntList(string numbers)
//        {
//            List<int> intList = new List<int>();
//            string[] numberStrings = numbers.Split(' ');
//            foreach (var numString in numberStrings)
//            {
//                int number;
//                if (int.TryParse(numString, out number))
//                {
//                    intList.Add(number);
//                }
//                else
//                {
//                    Console.WriteLine("fejl i konvertering!");
//                }
//            }
//            return intList;
//        }

//        static void GemPlader(List<Plade> plader)
//        {
//            string sti = @"C:\Users\kvist\Documents\GitHub\banko-snyd-MdotKvist\BankoCheater\plader.json";
//            try
//            {
//                string json = JsonConvert.SerializeObject(plader, Formatting.Indented);
//                System.IO.File.WriteAllText(sti, json);
//                Console.WriteLine("Pladen er gemt korrekt!");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Fejl i forsøget på at gemme pladerne! {ex.Message}");
//            }
//        }

//        static void søgITal(List<Plade> plader)
//        {
//            Console.WriteLine("Indtast de opråbte tal (eller skriv 'done' for at afslutte): ");
//            string input = Console.ReadLine();

//            List<int> indtastedeTal = new List<int>();

//            while (input.ToLower() != "done")
//            {
//                int søgeTal;
//                if (!int.TryParse(input, out søgeTal))
//                {
//                    Console.WriteLine("Ugyldigt input. Indtast venligst et tal eller 'done' for at afslutte: ");
//                    input = Console.ReadLine();
//                    continue;
//                }

//                indtastedeTal.Add(søgeTal);

//                // Tjek om de indtastede tal udgør en hel række på pladen
//                foreach (var plade in plader)
//                {
//                    List<int> bingoRækker = ErBingo(plade, indtastedeTal);

//                    foreach (var bingoRække in bingoRækker)
//                    {
//                        Console.WriteLine($"HALLOOOO De indtastede tal udgør en hel række {bingoRække} på pladen {plade.Navn}");
//                    }
//                }

//                input = Console.ReadLine();
//            }
//        }

//        // Metode til at kontrollere, om de indtastede tal udgør en hel række på pladen
//        static List<int> ErBingo(Plade plade, List<int> indtastedeTal)
//        {
//            List<int>[] rækker = { plade.Række1, plade.Række2, plade.Række3 };
//            List<int> bingoRækker = new List<int>();

//            // Tjek for hver række
//            foreach (var række in rækker)
//            {
//                // Hvis antallet af indtastede tal i rækken er 5, tilføj rækkens nummer til listen over bingo-rækker
//                if (indtastedeTal.Count(t => række.Contains(t)) == 5)
//                    bingoRækker.Add(rækker.ToList().IndexOf(række) + 1); // Tilføj 1 for at få rækkens nummer, da index starter fra 0
//            }

//            return bingoRækker;
//        }



//    }

//    class Plade
//    {
//        public string Navn { get; set; }
//        public List<int> Række1 { get; set; }
//        public List<int> Række2 { get; set; }
//        public List<int> Række3 { get; set; }
//    }
//}
