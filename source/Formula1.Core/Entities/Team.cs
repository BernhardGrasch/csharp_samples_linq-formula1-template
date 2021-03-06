﻿using Formula1.Core.Contracts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Formula1.Core.Entities
{
    public class Team : ICompetitor
    {
        public string Name { get; }

        public Team(string name)
        {
            Name = name;
        
        
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
