﻿using System.Collections.Generic;

namespace Stemtest.BL.Domain.Sessie
{
    public class StellingResultaat
    {
        public int StellingId { get; set; }
        public int aantalEens { get; set; }
        public int aantalOneens { get; set; }
        public List<string> arugmenten { get; set; }
    }
}