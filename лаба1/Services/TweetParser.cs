using System;
using System.Globalization;
using лаба1.Models;

namespace лаба1.Services
{
    public static class TweetParser
    {
        public static Tweet Parse(string input)
        {
            var parts = input.Split('\t');

            string coordPart = parts[0].Trim('[', ']');
            var coords = coordPart.Split(',');

            double lat = double.Parse(coords[0], CultureInfo.InvariantCulture);
            double lon = double.Parse(coords[1], CultureInfo.InvariantCulture);

            DateTime createdAt = DateTime.Parse(parts[2]);
            string text = parts[3];

            return new Tweet
            {
                Latitude = lat,
                Longitude = lon,
                CreatedAt = createdAt,
                Text = text,
                Sentiment = 0
            };
        }
    }
}