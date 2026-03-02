using System;
using System.IO;
using System.Linq;
using лаба1.Services;

namespace лаба1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите тему:");

            string topic = Console.ReadLine();

            
            var tweets = TweetService.LoadTweets(topic);
            Console.WriteLine($"Загружено: {tweets.Count}");

            if (tweets.Count == 0)
            {
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Анализ тональности текста...");
            foreach (var tweet in tweets)
            {
                tweet.Sentiment = SentimentService.Calculate(tweet.Text);
            }

            var mapPoints = tweets
                .Select(t => new
                {
                    Latitude = t.Latitude,
                    Longitude = t.Longitude,
                    Sentiment = t.Sentiment 
                })
                .ToList();

            TweetService.SaveResult(topic, tweets);
            Console.WriteLine("Текстовый файл сохранён в папке Output!");

            Console.WriteLine("Запуск отрисовки карты...");
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "maping.exe");

            if (File.Exists(exePath))
            {
                PythonBridge.RunMapingExe(mapPoints, exePath);
                Console.WriteLine("Карта успешно закрыта!");
            }
            else
            {
                Console.WriteLine($"Файл карты не найден: {exePath}");
                Console.WriteLine("Положите maping.exe и states.json в папку: " + AppDomain.CurrentDomain.BaseDirectory);
            }

            Console.ReadLine();
        }
    }
}
