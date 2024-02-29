using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Numerics;

namespace BankoCheater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Variabler til at definere Url stien og stien til ChromeDriveren
            string url = "https://mags-template.github.io/Banko/";
            string driverPath = @"C:\Users\kvist\Documents\GitHub\banko-snyd-MdotKvist\BankoCheater\chromedriver.exe";

            //Her indtaster man navn
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
            //Denne option gør at ChromeDriveren ikke venter på at få en ready state fra hjemme siden
            options.PageLoadStrategy = PageLoadStrategy.None;

            //Her fortæller vi programmet at hvergang vi skriver webdriver referer vi til ChromeDriver
            //Den ved hvor Driveren ligger på grund af den variable vi har angivet i () Som referer til vores eksiterende variabel
            using (IWebDriver webdriver = new ChromeDriver(driverPath))
            {
                //Her fortæller vi den at den skal navigere til vores URL
                //Den variable der ligger i () altså url referer til vores eksisterende url Variable
                webdriver.Navigate().GoToUrl(url);

                //Her starter vi et for loop som køre indtil vores variable: antalPlader er mindre end variablen i
                for (int i = 0; i < antalPlader; i++)
                {
                    //Her går ChromeDriveren så ind og finder et id i HTML'en som hedder "tekstboks"
                    //Det id gemmer den så i en IWebElement der hedder InputField
                    IWebElement inputField = webdriver.FindElement(By.Id("tekstboks"));
                    //Denne linje indtaster Brugernavn og i altså tallet vi er kommer til på pladen ind på hjemme siden
                    inputField.SendKeys(brugerNavn + i);

                    //Her går ChromeDriveren ind i HTML'en og finder et id der hedder "knap"
                    //Det id gemmer den så i en IWebElement variabel kaldet generateButton
                    IWebElement generateButton = webdriver.FindElement(By.Id("knap"));
                    //Den gemte variable generateButton Fortæller Click() funktionen hvad den skal trykke på
                    generateButton.Click();

                    //Her opretter vi en variabel/object (wait) ved hjælp af WebDriverWait libraryet,
                    //vi fortæller også at det er webdriver variable der skal vente i 10 sekunder 
                    WebDriverWait wait = new WebDriverWait(webdriver, TimeSpan.FromSeconds(10));
                    //her bruger vi wait og fortæller at vi skal vente indtil at elementet er synglig i vores html id p11
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("p11")));

                    //Her opretter vi object/variabel
                    Plade plade = new Plade
                    {
                        //her fortæller vi at i denne object/variabel skal vil tøje navn altså vores brugernavn
                        Navn = $"{brugerNavn}{i}",
                        //Her fortæller vi hvordan vores tre rækker skal indsættes i vores liste (vi skal springe ned til funktionen
                        //ParseToIntList med dataen der står i parantesen)
                        Række1 = ParseToIntList(webdriver.FindElement(By.Id("p11")).Text),
                        Række2 = ParseToIntList(webdriver.FindElement(By.Id("p12")).Text),
                        Række3 = ParseToIntList(webdriver.FindElement(By.Id("p13")).Text),
                        //Her fortæller vi at vi skal indsætte en bingo status og 3 bools
                        BingoStatus = new bool[3]
                    };

                    //Her tilføjer vi pladen ovenfor i listen plader
                    plader.Add(plade);
                    //Her sletter vi hvad der står i id"tekstboks" på hjemmesiden
                    inputField.Clear();
                }

                //Her Springer vi til en funktion der hedder GemPlader med dataen fra listen plader
                GemPlader(plader);
            }

            //Her springer vi til funktionen
            søgITal(plader);
        }

        //Det er vores funktion som laver en string kaldet numbers om til en listen har ikke noget navn
        //men den list der bliver lavet blive smidt direkte op til den liste hvor i vi kalder på funktionen
        static List<int> ParseToIntList(string numbers)
        {
            return numbers.Split(' ') //Deler Stringen op hvor der er mellemrum
                          .Select(n => int.TryParse(n, out int result) ? result : 0) //For hver opdelte string forsøger den at converter den til et helt tal, hvis den kan det returnere den tallet til result variablen. ellers returner vi 0
                          .Where(n => n != 0) //denne linje filtrer talene der ikke er nul fra (Den filtrer bokstaverne fra som den ikke kunne lave om til en int i forrige linje)
                          .ToList(); //Her pakker den tallene ind i en liste og retunere
        }

        //denne funktion gemmer pladerne i en json filen kalder plader, denne fil bliver ikke brugt at programmet
        //men er til for at hvis man kan lide at holde overblik over de plader man har hente fra hjemmesiden
        static void GemPlader(List<Plade> plader)
        {
            //Variabel der definere stien til json filen
            string sti = @"C:\Users\kvist\Documents\GitHub\banko-snyd-MdotKvist\BankoCheater\plader.json";
            //Rydder konsollen for det Som ChromeDriveren skriver ind i konsollen
            Console.Clear();

            //Denne linje fortæller programmet at den skal forsøge køre det som står inde i curlybrackets 
            try
            {
                //Denne linje converter Variablen plader om til en string kaldet json
                string json = JsonConvert.SerializeObject(plader, Formatting.Indented);
                //Her fortæller vi systemet at den skal skrive teksten ind i dokumentet
                System.IO.File.WriteAllText(sti, json);
                //Her skriver vi konsollen at pladerne er gemt korrekt
                Console.WriteLine("Pladerne er gemt korrekt!");
            }
            //hvis try ikke lykkedes skal den Skrive den besked det står i ConsoleWriteLine
            catch (Exception ex)
            {
                //Dette er beskeden til konsollen hvis første forsøg ikke lykkedes
                Console.WriteLine($"Fejl i forsøget på at gemme pladerne! {ex.Message}");
            }
        }

        //Vores søgITal function er den function som søger i vores liste kaldet plader
        static void søgITal(List<Plade> plader)
        {   
            //Her udskriver vi en masse streger i konsollen for at dele det op (øjen pleaser)
            Console.WriteLine("-------------------------------------------------------------------");
            //Her udskriver vi en besked i konsollen om at vi skal indtaste de tal der bliver råbt op og at vi kan skrive done for at afslutte programmet
            Console.WriteLine("Indtast de opråbte tal (eller skriv 'done' for at afslutte): ");
            //Her opretter vi en liste kaldet indtastedeTal i data typen int
            List<int> indtastedeTal = new List<int>();
            //Her opretter vi en string kaldet input
            string input;
            //Her opretter vi en bool kaldet allRowsBingo
            bool allRowsBingo = false;
            //Opretter vi en variable ved hjælp af classen variabel som fortæller programmet at denne variabel hænger sammen med classen plade
            Plade plade = null;
            
            //Dette While loop gør så vores Konsole readline køre så længe at der ikke blive indtastet done
            while ((input = Console.ReadLine()) != "done")
            {
                //Denne if statement køre hvis det er muligt at converter inputtet til int
                if (int.TryParse(input, out int tal))
                {
                    //Hvis variablen ikke indholde tal fra variablen tal
                    if (!indtastedeTal.Contains(tal))
                    {
                        //Her tilføjer vi variablen tal til den variabel kaldet indtastedeTal
                        indtastedeTal.Add(tal);
                        //Her rydder vi konsollen
                        Console.Clear();
                        //Her skriver vi de indtastete tal til konsollen
                        Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal));
                        //Hvis allRowsBingo er true
                        if (allRowsBingo)
                        {
                            //Skriv beskeden i konsollen hvis der er bingo på fuld plade
                            Console.WriteLine($"\nBingo på fuld plade! {plade.Navn}\n");
                        }
                    }
                    //Hvis variablen indTastedeTal indeholde variablen kaldet tal
                    else
                    {
                        //Ryd konsollen
                        Console.Clear();
                        //Udskriv i konsollen de tideligere tal
                        Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal));
                        //Udskirv besked i konsollen
                        Console.WriteLine("Tallet er allerede indtastet, prøv igen.");
                    }
                }
                //hvis variablen tal ikke kan laves om til int
                else
                {
                    //Udskriv besked i konsollen og forsæt programmet
                    Console.WriteLine("Ugyldigt input, prøv igen.");
                    continue;
                }

                //for hver aktuelPlade i plader
                foreach (var aktuelPlade in plader)
                {
                    //Her gemmen den data fra functionen CheckBingoRow men vi fortæller også hvilke variabler den skal bruge i funktionen
                    //det data gemmer den så i variablen bingorow
                    int bingoRow = CheckBingoRow(aktuelPlade, indtastedeTal);
                    //Her laver vi aktuelplade fra funktionen CheckBingRow om til plade for at vi kan bruge dataen i dette foreach loop
                    plade = aktuelPlade;
                    //Hvis BingoRow variablen ikke er -1
                    if (bingoRow != -1)
                    {
                        //Udskriv besked i konsollen
                        Console.WriteLine($"\nBingo på række {bingoRow + 1}! {plade.Navn} har bingo på række {bingoRow + 1}: {string.Join(",", plade.Rækker[bingoRow])}\n");

                        //Hvis BingoRow er falsk
                        if (!plade.BingoStatus[bingoRow])
                        {
                            //Udskriv Besked i konsollen
                            Console.WriteLine($"{plade.Navn} har bingo på række {bingoRow + 1}: {string.Join(",", plade.Rækker[bingoRow])}\n");
                            //Her bliver bingRow true
                            plade.BingoStatus[bingoRow] = true;
                        }

                        //Hvis alle index pladser i BingoStatus arrayet er sande
                        if (plade.BingoStatus.All(status => status))
                        {
                            //udskriv beskeden i konsollen
                            Console.WriteLine($"\nBingo på fuld plade! {plade.Navn} har bingo på alle tre rækker!\n");
                        }
                    }
                }
            }
        }


        // funktionen CheckBingoRow skal Bruge dataen som er indtastet i () ellers kan den ikke kaldes
        static int CheckBingoRow(Plade aktuelPlade, List<int> indtastedeTal)
        {
            //hvis variabel aktuelPLade er null
            if (aktuelPlade == null)
            {
                //return til aktuelPlade -1
                return -1;
            }
            //Hvis aktuelPlade er mindre end i
            for (int i = 0; i < aktuelPlade.Rækker.Count; i++)
            {
                //Hvis pladen ikke har bingo på den den angivet rækket [i]
                if (!aktuelPlade.BingoStatus[i] && aktuelPlade.Rækker[i].All(t => indtastedeTal.Contains(t)))
                {
                    //Her fortæller vi at bingostatus for den angivet rækker er true og retruner i
                    aktuelPlade.BingoStatus[i] = true;
                    return i;
                }
            }
            //Hvis if og for ikke finder sted retuner den -1
            return -1;
        }
    }

    class Plade
    {
        //Denne linje tilader at hente status for navn og tilader at skrive
        public string Navn { get; set; }
        //Denne linje tilader at hente status for Række1 og tilader at skrive
        public List<int> Række1 { get; set; }
        //Denne linje tilader at hente status for Række2 og tilader at skrive
        public List<int> Række2 { get; set; }
        //Denne linje tilader at hente status for Række3 og tilader at skrive
        public List<int> Række3 { get; set; }
        //Her fortæller vi at vi skal hente dataen for Række1-3 som en liste i data typen int det giver også adgang til alle rækker på en gang
        public List<List<int>> Rækker => new List<List<int>> { Række1, Række2, Række3 };
        //Denne linje tillader at vi kan læse og skrive dataen for bingostatus
        public bool[] BingoStatus { get; set; } // Tilføjet for at holde styr på bingo-status for hver række
    }
}
