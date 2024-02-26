using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;

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

            List<Plade> plader = new List<Plade>();

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

                    string bingoNumbersRow1 = row1.Text.Trim();
                    string bingoNumbersRow2 = row2.Text.Trim();
                    string bingoNumbersRow3 = row3.Text.Trim();

                    Console.WriteLine($"Tal for plade {i + 1}");
                    Console.WriteLine($"Række 1: {bingoNumbersRow1}");
                    Console.WriteLine($"Række 2: {bingoNumbersRow2}");
                    Console.WriteLine($"Række 3: {bingoNumbersRow3}");

                    // Parse strengene til en liste af heltal
                    List<int> row1Numbers = ParseToIntList(bingoNumbersRow1);
                    List<int> row2Numbers = ParseToIntList(bingoNumbersRow2);
                    List<int> row3Numbers = ParseToIntList(bingoNumbersRow3);

                    // Tilføj plade til listen
                    Plade plade = new Plade
                    {
                        Navn = brugerNavn + i,
                        Række1 = row1Numbers,
                        Række2 = row2Numbers,
                        Række3 = row3Numbers
                    };
                    plader.Add(plade);

                    // Clear input field for the next iteration
                    inputField.Clear();

                    // Gem pladerne i en JSON-fil
                    GemPlader(plader);
                }

                // Spørg brugeren om de opråbte tal og tjek for bingo
                søgITal(plader);
            }
        }


        static List<int> ParseToIntList(string numbers)
        {
            List<int> intList = new List<int>();
            string[] numberStrings = numbers.Split(' ');
            foreach (var numString in numberStrings)
            {
                int number;
                if (int.TryParse(numString, out number))
                {
                    intList.Add(number);
                }
                else
                {
                    Console.WriteLine("fejl i convertering!");
                }
            }
            return intList;
        }

        static void GemPlader(List<Plade> plader)
        {
            string sti = @"C:\Users\kvist\Documents\GitHub\banko-snyd-MdotKvist\BankoCheater\plader.json";
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
            Console.WriteLine("Indtast de opråbte tal (eller skriv 'done' for at afslutte): ");
            string input = Console.ReadLine();

            List<int> indtastedeTal = new List<int>();

            while (input.ToLower() != "done")
            {
                int søgeTal;
                if (!int.TryParse(input, out søgeTal))
                {
                    Console.WriteLine("Ugyldigt input. Indtast venligst et tal eller 'done' for at afslutte: ");
                    input = Console.ReadLine();
                    continue;
                }

                indtastedeTal.Add(søgeTal);

                // Tjek om de indtastede tal udgør en hel række på pladen
                foreach (var plade in plader)
                {
                    if (ErBingo(plade, indtastedeTal))
                    {
                        Console.WriteLine($"Bingo! De indtastede tal udgør en hel række på pladen for {plade.Navn}");
                        break; // Bingo blev fundet, så gå videre til næste iteration
                    }
                }

                // Indtast det næste tal eller afslut
                Console.WriteLine("Indtast det næste opråbte tal (eller skriv 'done' for at afslutte): ");
                input = Console.ReadLine();
            }
        }

        // Metode til at kontrollere, om de indtastede tal udgør en hel række på pladen
        static bool ErBingo(Plade plade, List<int> indtastedeTal)
        {
            List<int>[] rækker = { plade.Række1, plade.Række2, plade.Række3 };

            // Tjek for hver række
            foreach (var række in rækker)
            {
                // Hvis antallet af indtastede tal i rækken ikke er 5, så gå videre til næste række
                if (indtastedeTal.Count(t => række.Contains(t)) != 5)
                    return false;
            }

            // Hvis alle rækker har fem tal, returner sandt
            return true;
        }



    }

    class Plade
    {
        public string Navn { get; set; }
        public List<int> Række1 { get; set; }
        public List<int> Række2 { get; set; }
        public List<int> Række3 { get; set; }
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
//            string url = "https://mags-template.github.io/Banko/";
//            string driverPath = @"C:\Users\kvist\Documents\GitHub\banko-snyd-MdotKvist\BankoCheater\chromedriver.exe";

//            Console.WriteLine("Hvad er dit navn? ");
//            string brugerNavn = Console.ReadLine();

//            Console.WriteLine("Hvor mange plader vil du bruge?");
//            int antalPlader = Convert.ToInt32(Console.ReadLine());

//            List<Plade> plader = new List<Plade>();

//            using (IWebDriver webdriver = new ChromeDriver(driverPath))
//            {
//                webdriver.Navigate().GoToUrl(url);

//                for (int i = 0; i < antalPlader; i++)
//                {
//                    Console.WriteLine($"Navn: {brugerNavn}{i}");
//                    IWebElement inputField = webdriver.FindElement(By.Id("tekstboks"));
//                    inputField.SendKeys(brugerNavn + i);

//                    IWebElement generateButton = webdriver.FindElement(By.Id("knap"));
//                    generateButton.Click();

//                    System.Threading.Thread.Sleep(0);

//                    IWebElement row1 = webdriver.FindElement(By.Id("p11"));
//                    IWebElement row2 = webdriver.FindElement(By.Id("p12"));
//                    IWebElement row3 = webdriver.FindElement(By.Id("p13"));

//                    string bingoNumbersRow1 = row1.Text.Trim();
//                    string bingoNumbersRow2 = row2.Text.Trim();
//                    string bingoNumbersRow3 = row3.Text.Trim();

//                    Console.WriteLine($"Tal for plade {i + 1}");
//                    Console.WriteLine($"Række 1: {bingoNumbersRow1}");
//                    Console.WriteLine($"Række 2: {bingoNumbersRow2}");
//                    Console.WriteLine($"Række 3: {bingoNumbersRow3}");

//                    // Parse strengene til en liste af heltal
//                    List<int> row1Numbers = ParseToIntList(bingoNumbersRow1);
//                    List<int> row2Numbers = ParseToIntList(bingoNumbersRow2);
//                    List<int> row3Numbers = ParseToIntList(bingoNumbersRow3);

//                    // Tilføj plade til listen
//                    Plade plade = new Plade
//                    {
//                        Navn = brugerNavn,
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
//                    Console.WriteLine("fejl i convertering!");
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
//                Console.WriteLine("Pladerne er gemt korrekt!");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Fejl i forsøget på at gemme pladerne! {ex.Message}");
//            }
//        }

//        static void søgITal(List<Plade> plader)
//        {




//            //// Indtast det opråbte tal
//            //Console.WriteLine("Indtast det opråbte tal (eller skriv 'done' for at afslutte): ");
//            //string input = Console.ReadLine();

//            //// Fortsæt, indtil brugeren indtaster "done"
//            //while (input.ToLower() != "done")
//            //{
//            //    int søgeTal;
//            //    if (!int.TryParse(input, out søgeTal))
//            //    {
//            //        Console.WriteLine("Ugyldigt input. Indtast venligst et tal eller 'done' for at afslutte: ");
//            //        input = Console.ReadLine();
//            //        continue;
//            //    }

//            //    // Tjek hver plade for bingo
//            //     foreach (var plade in plader)
//            //    {
//            //        if (ErBingo(plade, søgeTal))
//            //        {
//            //            Console.WriteLine($"Bingo! Tallet {søgeTal} udfylder en hel række på pladen for {plade.Navn}");
//            //            break; // Vi har fundet bingo på denne plade, så gå videre til næste tal
//            //        }
//            //    }

//            //    // Indtast det næste tal eller afslut
//            //    Console.WriteLine("Indtast det næste opråbte tal (eller skriv 'done' for at afslutte): ");
//            //    input = Console.ReadLine();

//            //        Console.Write("fejl!");
//            //    break;
//            //}
//        }

//        // Metode til at kontrollere, om et tal udfylder en hel række på en plade
//        static bool ErBingo(Plade plade, int opråbtTal)
//        {
//            //string[] rækker = { plade.Række1, plade.Række2, plade.Række3 };

//            foreach (var række in rækker)
//            {
//                string[] tal = række.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
//                bool fuldRække = true;

//                foreach (var t in tal)
//                {
//                    int talPåPladen;
//                    if (!int.TryParse(t, out talPåPladen))
//                    {
//                        Console.WriteLine($"Uventet fejl: Kunne ikke konvertere tal '{t}' på pladen til en numerisk værdi.");
//                        continue;
//                    }

//                    if (talPåPladen != opråbtTal)
//                    {
//                        fuldRække = false;
//                        break;
//                    }
//                }

//                if (fuldRække)
//                    return true;
//            }

//            return false;
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
