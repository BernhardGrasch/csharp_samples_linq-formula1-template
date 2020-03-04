using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula1.Core.Entities;

namespace Formula1.Core
{
    public class ResultCalculator
    {
        /// <summary>
        /// Berechnet aus den Ergebnissen den aktuellen WM-Stand für die Fahrer
        /// </summary>
        /// <returns>DTO für die Fahrerergebnisse</returns>
        public static IEnumerable<TotalResultDto<Driver>> GetDriverWmTable()
        {
            var driverResults = ImportController.LoadResultsFromXmlIntoCollections()
                .GroupBy(g => g.Driver)
                .OrderByDescending(t => t.Sum(t => t.Points))
                .Select((t, pos) => new TotalResultDto<Driver>
                {
                    Competitor = t.Key,
                    Position = pos + 1,
                    Points = t.Sum(t => t.Points)
                });

            return driverResults;
        }

        /// <summary>
        /// Berechnet aus den Ergebnissen den aktuellen WM-Stand für die Teams
        /// </summary>
        /// <returns>DTO für die Teamergebnisse</returns>
        public static IEnumerable<TotalResultDto<Team>> GetTeamWmTable()
        {
            var teamResults = ImportController.LoadResultsFromXmlIntoCollections()
                .GroupBy(g => g.Team)
                .OrderByDescending(t => t.Sum(t => t.Points))
                .Select((t, pos) => new TotalResultDto<Team>
                {
                    Competitor = t.Key,
                    Position = pos + 1,
                    Points = t.Sum(t => t.Points)
                });

            return teamResults;
        }
    }
}



