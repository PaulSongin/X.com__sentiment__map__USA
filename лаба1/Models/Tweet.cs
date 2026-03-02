using System;

namespace лаба1.Models
{
    public class Tweet
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
        public double Sentiment { get; set; }
    }
}