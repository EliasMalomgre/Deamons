using System;
using System.Collections.Generic;
using System.Linq;
using Stemtest.BL;
using Stemtest.BL.Domain.Sessie;
using Stemtest.BL.Domain.Test;

namespace Stemtest.UI.CA
{
    public class PartijSpelCA
    {
        private static bool quit = false;
        private static GameManager _gameManager = new GameManager();
        private static int userId = 69;
        private static int leerlingId = 0;
        private static string gekozenPartij;

        public static void Main(string[] args)
        {
            _gameManager.StartSessie("PTest", SoortSpel.PARTIJSPEL, new List<string> {"Vlaams Belang", "Groen"}, "203A",
                userId);

            ShowStart();
            while (!quit)
            {
                leerlingId++;
                _gameManager.BeginSpel(userId, leerlingId);
                ShowMenu();
                SelecteerPartij();
                BeginSpel();
            }
        }

        private static void BeginSpel()
        {
            Test test = _gameManager.GetTest(userId);
            int aantalStellingen = test.stellingen.Count;

            for (int i = 0; i < aantalStellingen; i++)
            {
                Console.Clear();
                Console.WriteLine("Stelling {0} van de {1}", i + 1, aantalStellingen);
                Console.WriteLine("{0}", test.stellingen[i].tekst);
                Console.WriteLine("{0}      {1}", test.stellingen[i].antwoorden[0].mening,
                    test.stellingen[i].antwoorden[1].mening);
                Console.WriteLine("\n\n\n");
                bool inValidAction = false;
                do
                {
                    inValidAction = false;
                    Console.Write("Antwoord met J/N - druk op W om alle woordverklaringen te tonen:");
                    string antwoord = Console.ReadLine();
                    string argumentering = "";
                    if (!antwoord.Equals("W"))
                    {
                        Console.Write("Beargumenteer je antwoord:");
                        argumentering = Console.ReadLine();
                        if (argumentering == "")
                        {
                            argumentering = "Geen mening";
                        }
                    }

                    switch (antwoord)
                    {
                        case "J":
                        case "j":
                        case "ja":
                        case "Ja":
                            _gameManager.BeantwoordStelling(argumentering, leerlingId, userId, 0);
                            break;
                        case "N":
                        case "n":
                        case "Nee":
                        case "nee":
                            _gameManager.BeantwoordStelling(argumentering, leerlingId, userId, 1);
                            break;
                        case "W":
                        case "w":
                            foreach (var woordverklaring in _gameManager.GetVerklaringen(userId, i + 1))
                            {
                                Console.WriteLine(woordverklaring.woord + ": " + woordverklaring.verklaring);
                            }

                            inValidAction = true;
                            break;
                        default:
                            Console.WriteLine("Geen geldige keuze!");
                            inValidAction = true;
                            break;
                    }
                } while (inValidAction);
            }

            Console.Clear();
            Console.WriteLine("Gefeliciteerd, je hebt het partijspel uitgespeeld");
            Console.WriteLine("Voor je partij {0} heb je een score van {1} behaald", gekozenPartij,
                _gameManager.berekenScore(userId, leerlingId));
            Console.WriteLine("\n\n\n\n");
            Console.Write("Druk op \"1\" als je je foute vragen wilt overlopen" +
                          "\nDruk op \"2\" om alle antwoorden te overlopen" +
                          "\nDruk op \"0\" als je verder wilt gaan" +
                          "\nKeuze:");

            string keuze = Console.ReadLine();
            int cijfer;
            
            if (Int32.TryParse(keuze, out cijfer))
            {
                switch (cijfer)
                {
                    case 1:
                        FoutenOverlopen(test);
                        break;
                    case 2:
                        VragenOverlopen(test);
                        break;
                    case 0:
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }
        }


        private static void FoutenOverlopen(Test test)
        {
            List<Antwoord> fouteAntwoorden = _gameManager.GetFouteAntwoorden(userId, leerlingId);
            int laatsteStellingId = fouteAntwoorden.Last().stellingId;


            foreach (Antwoord foutAntwoord in fouteAntwoorden)
            {
                Console.Clear();
                Console.WriteLine("Stelling :{0}",
                    test.stellingen.Find(s => s.stellingID == foutAntwoord.stellingId).tekst);
                Console.WriteLine("\nJouw antwoord: {0}", foutAntwoord.mening);
                Console.WriteLine("Jouw argumentering: {0}\n", foutAntwoord.Argumentering);
                Antwoord partijAntwoord =
                    _gameManager.GetPartijAntwoord(userId, foutAntwoord.stellingId, gekozenPartij);
                Console.WriteLine("Het antwoord van {0}: {1}", gekozenPartij, partijAntwoord.mening);
                Console.WriteLine("Uitleg: {0}", partijAntwoord.Argumentering);

                if (foutAntwoord.stellingId == laatsteStellingId)
                {
                    Console.WriteLine("\n Dat waren al je foute antwoorden!");
                    Console.WriteLine("\n Druk op <Enter> om terug te gaan naar het hoofdmenu");
                    Console.ReadLine();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("\n Druk op <Enter> om je volgende fout te bekijken");
                    Console.ReadLine();
                }
            }
        }

        private static void VragenOverlopen(Test test)
        {
            List<Antwoord> leerlingAntwoorden = _gameManager.GetAntwoorden(userId, leerlingId);
            List<Antwoord> partijAntwoorden = _gameManager.GetPartijAntwoorden(userId, gekozenPartij);
            Console.Clear();
            for (int i = 0; i < partijAntwoorden.Count; i++)
            {
                Console.WriteLine("{0}) {1} jouw antwoord: {2}", i + 1, test.stellingen[i].tekst,
                    leerlingAntwoorden[i].Correct ? "correct" : "fout");
            }

            bool doorgaan = true;
            do
            {
                Console.WriteLine("\nWelke stelling wil je bekijken?" +
                              "\nOm af te sluiten druk je op \"0\"" +
                              "\nKeuze:");
            string keuze = Console.ReadLine();
            int cijfer;
            
                if (Int32.TryParse(keuze, out cijfer)) 
                {
                    if (cijfer == 0)
                    {
                        doorgaan = false;
                    }

                    else if (cijfer > partijAntwoorden.Count)
                    {
                     Console.WriteLine("\n Dit cijfer komt niet overeen met een stelling.");
                    }
                    else
                    {
                        doorgaan = false;
                        antwoordDetail(test, leerlingAntwoorden[cijfer-1], partijAntwoorden[cijfer-1]);
                    }
            }
            
            } while (doorgaan);
        }

        private static void antwoordDetail(Test test, Antwoord antwoord, Antwoord partijAntwoord)
        {
            Console.Clear();
            Console.WriteLine("Stelling :{0}", test.stellingen.Find(s => s.stellingID == antwoord.stellingId).tekst);
            Console.WriteLine("\nJouw antwoord: {0}", antwoord.mening);
            Console.WriteLine("Jouw argumentering: {0}\n", antwoord.Argumentering);
            Console.WriteLine("Het antwoord van {0}: {1}", gekozenPartij, partijAntwoord.mening);
            Console.WriteLine("Uitleg: {0}", partijAntwoord.Argumentering);
            Console.WriteLine("Druk op <Enter> om terug te gaan" +
                              "\nOm af te sluiten druk op 0");
            bool doorgaan = true;
            do
            {
                string keuze = Console.ReadLine();
                int cijfer;
                if (Int32.TryParse(keuze, out cijfer))
                {
                    if (cijfer == 0)
                    {
                        doorgaan = false;
                    }
                    
                }
                else
                {
                    doorgaan = false;
                    VragenOverlopen(test);
                }
            } while (doorgaan);
            
        }


        private static string VraagArgumentering()
        {
            Console.Write("Heb je nog een argument?:");
            string argumentatie = Console.ReadLine();
            if (argumentatie.Equals(""))
            {
                argumentatie = "geen argumentatie meegegeven";
            }

            return argumentatie;
        }

        private static void SelecteerPartij()
        {
            bool inValidAction = false;
            do
            {
                inValidAction = false;
                Console.Write("Keuze: ");
                string input = Console.ReadLine();
                int action;

                if (Int32.TryParse(input, out action))
                {
                    switch (action)
                    {
                        case 1:
                            _gameManager.SelecteerPartij(leerlingId, userId, "Vlaams Belang");
                            gekozenPartij = "Vlaams Belang";
                            break;
                        case 2:
                            _gameManager.SelecteerPartij(leerlingId, userId, "Groen");
                            gekozenPartij = "Groen";
                            break;
                        case 0:
                            Environment.Exit(0);
                            return;
                        default:
                            Console.WriteLine("Geen geldige keuze!");
                            inValidAction = true;
                            break;
                    }
                }
            } while (inValidAction);
        }

        private static void ShowStart()
        {
            Console.WriteLine("=====================================");
            Console.WriteLine("=== DE STEMTEST - HET PARTIJSPEL ===");
            Console.WriteLine("=====================================");
            Console.WriteLine("Welkom bij het partij spel! \n" +
                              "In dit spel ga je een partij kiezen en zul je verschillende stellingen te zien krijgen.\n" +
                              "Probeer de mening op elke stelling van de partijen te raden en verdien zo punten.");
            Console.WriteLine();
        }

        private static void ShowMenu()
        {
            Console.WriteLine("Kies een Partij");
            Console.WriteLine("1) Vlaams Belang");
            Console.WriteLine("2) Groen");
            Console.WriteLine("0) Afsluiten");
        }
    }
}