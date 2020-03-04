using Formula1.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Utils;

namespace Formula1.Core
{
    /// <summary>
    /// Daten sind in XML-Dateien gespeichert und werden per Linq2XML
    /// in die Collections geladen.
    /// </summary>
    public static class ImportController
    {
        public static IDictionary<string, Driver> Drivers { get; set; }
        public static IDictionary<string, Team> Teams { get; set; }
        public static IEnumerable<Result> Results { get; set; }
        public static IEnumerable<Race> Races { get; set; }
        /// <summary>
        /// Daten der Rennen werden aus der
        /// XML-Datei ausgelesen und in die Races-Collection gespeichert.
        /// Grund: Races werden nicht aus den Results geladen, weil sonst die
        /// Rennen in der Zukunft fehlen
        /// </summary>
        public static IEnumerable<Race> LoadRacesFromRacesXml()
        {
            List<Race> races = new List<Race>();
            string racesPath = MyFile.GetFullNameInApplicationTree("Races.xml");
            XElement xElement = XDocument.Load(racesPath).Root;

            if(xElement != null)
            {
                races =
                    xElement.Elements("Race")
                    .Select(race => new Race
                    {
                        Number = (int)race.Attribute("round"),
                        Date = (DateTime)race.Element("Date"),
                        Country = (string)race.Element("Circuit")
                        ?.Element("Location")
                        ?.Element("Country")?.Value,
                        City = race.Element("Circuit")
                        ?.Element("Location")
                        ?.Element("Locality")?.Value
                    })
                    .ToList();
            }
            else
            {
                throw new ArgumentNullException(nameof(xElement));
            }
            return races;
        }

        /// <summary>
        /// Aus den Results werden alle Collections, außer Races gefüllt.
        /// Races wird extra behandelt, um auch Rennen ohne Results zu verwalten
        /// </summary>
        public static IEnumerable<Result> LoadResultsFromXmlIntoCollections()
        {
            Races = LoadRacesFromRacesXml();
            Drivers = new Dictionary<string, Driver>();
            Teams = new Dictionary<string, Team>();
            Results = new List<Result>();
            string resultsPath = MyFile.GetFullNameInApplicationTree("Results.xml");
            XElement xElement = XDocument.Load(resultsPath).Root;

            if(xElement != null)
            {
                Results = xElement.Elements("Race").Elements("ResultsList").Elements("Result")
                    .Select(result => new Result
                    {
                        Race = GetRace(result),
                        Driver = GetDriver(result),
                        Team = GetTeam(result),
                        Position = (int)result.Attribute("position"),
                        Points = (int)result.Attribute("points")
                    })
                    .ToList();
            }
            else
            {
                throw new ArgumentNullException(nameof(xElement));
            }
            return Results;
        }

        private static Team GetTeam(XElement result)
        {
            string teamName = result.Element("Constructor")?.Element("Name")?.Value;

            if(Teams.ContainsKey(teamName))
            {
                return Teams[teamName];
            }
            else
            {
                Teams[teamName] = new Team(teamName);
                return Teams[teamName];
            }
        }

        private static Driver GetDriver(XElement result)
        {
            string firstName = result.Element("Driver")?.Element("GivenName")?.Value;
            string lastName = result.Element("Driver")?.Element("FamilyName")?.Value;
            string fullName = $"{lastName} {firstName}";

            if(Drivers.ContainsKey(fullName))
            {
                return Drivers[fullName];
            }
            else
            {
                Drivers[fullName] = new Driver(fullName);
                return Drivers[fullName];
            }
        }

        private static Race GetRace(XElement result)
        {
            int raceNumber = (int)result.Parent?.Parent?.Attribute("round");

            return Races.SingleOrDefault(r => r.Number == raceNumber);
        }
    }
}