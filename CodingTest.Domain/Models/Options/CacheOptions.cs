using System;
using System.Collections.Generic;
using System.Text;

namespace CodingTest.Domain.Models.Options
{
    public class CacheOptions
    {
        public int BestNewsExpirationInSeconds { get; set; }
        public int SpecificNewsExpirationInSeconds { get; set; }
    }
}
