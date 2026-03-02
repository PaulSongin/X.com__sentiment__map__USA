using System;
using System.Collections.Generic;
using System.IO;
using лаба1.Models;

namespace лаба1.Services
{
    public static class TweetService
    {
        public static List<Tweet> LoadTweets(string topic)
        {
            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                topic + ".txt"
            );

            List<Tweet> tweets = new List<Tweet>();

            if (!File.Exists(path))
            {
                Console.WriteLine($"Ошибка: Файл {path} не найден!");
                return tweets;
            }

            foreach (var line in File.ReadLines(path))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    tweets.Add(TweetParser.Parse(line));
                }
            }

            return tweets;
        }

        public static void SaveResult(string topic, List<Tweet> tweets)
        {
            string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string outputPath = Path.Combine(outputDir, topic + "_result.txt");

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                foreach (var t in tweets)
                {
                    writer.WriteLine($"{t.Latitude}, {t.Longitude} -> {t.Sentiment}");
                }
            }
        }
    }
}