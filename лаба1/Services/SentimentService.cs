using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace лаба1.Services
{
    public static class SentimentService
    {
        private static Dictionary<string, double> _sentiments;
        private static int _maxPhraseLength = 0; 
        static SentimentService()
        {
            _sentiments = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            LoadDictionary();
        }

        private static void LoadDictionary()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "sentiments.csv");

            if (!File.Exists(path))
            {
                Console.WriteLine("ВНИМАНИЕ: Файл sentiments.csv не найден в папке Data. Сентимент будет равен 0.");
                return;
            }

            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                int lastCommaIndex = line.LastIndexOf(',');
                if (lastCommaIndex == -1) continue;

                string phrase = line.Substring(0, lastCommaIndex).Trim();
                string scorePart = line.Substring(lastCommaIndex + 1).Trim();

                if (double.TryParse(scorePart, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double score))
                {
                    if (!_sentiments.ContainsKey(phrase))
                    {
                        _sentiments.Add(phrase, score);

                        int wordCount = phrase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (wordCount > _maxPhraseLength)
                        {
                            _maxPhraseLength = wordCount;
                        }
                    }
                }
            }
            Console.WriteLine($"Словарь тональности загружен: {_sentiments.Count} фраз.");
        }

        public static double Calculate(string text)
        {
            if (_sentiments.Count == 0 || string.IsNullOrWhiteSpace(text)) return 0;

            var words = Regex.Replace(text.ToLower(), @"[^\w\s']", "")
                             .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            double totalScore = 0;
            int i = 0;

            while (i < words.Length)
            {
                bool matchFound = false;
                for (int length = _maxPhraseLength; length >= 1; length--)
                {
                    if (i + length > words.Length) continue;

                    string candidate = string.Join(" ", words, i, length);

                    if (_sentiments.TryGetValue(candidate, out double score))
                    {
                        totalScore += score;
                        i += length;
                        matchFound = true;
                        break;
                    }
                }

                if (!matchFound)
                {
                    i++;
                }
            }

            return totalScore;
        }
    }
}