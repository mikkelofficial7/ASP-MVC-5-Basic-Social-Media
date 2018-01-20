using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Sentiment
    {
        [JsonProperty("positive")]
        public string positive { get; set; }
        [JsonProperty("negative")]
        public string negative { get; set; }
    }
}