using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace TestTaskServer
{
    internal class Program
    {
        private static IEnumerable<Word> GetWordsFromFile(string filePath,
            int minWordLength = 3, int maxWordLength = 15, int minFrequency = 3)
        {
            var separators = new char[] { ' ', '.', ',' };
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            var words = new Dictionary<string, int>();
            using (var file = new StreamReader(filePath))
            {
                var regex = new Regex(@"[^\w -]");
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    var wordList = regex.Replace(line, " ")
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => x.Length >= minWordLength && x.Length <= maxWordLength)
                        .ToList()
                        .Select(x => x.ToLower());

                    foreach (var word in wordList)
                        if (words.ContainsKey(word))
                            words[word]++;
                        else
                            words[word] = 1;
                }
            }

            return words.Where(x => x.Value >= minFrequency).Select(kv => new Word
            {
                Value = kv.Key,
                Frequency = kv.Value
            });
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2)
                throw new Exception("Arguments: connectionString port");
            else
            {
                var context = new AppDbContext(args[0]);
                var wordDictionary = new WordDictionary(context);
                var server = new Server(wordDictionary, Int32.Parse(args[1]));
                var serverThread = new Thread(() => server.Run());
                serverThread.Start();
                ListenToConsole(wordDictionary);
            }
        }

        private static void ListenToConsole(WordDictionary wordDictionary)
        {
            while (true)
            {
                Console.Write(">");
                var input = Console.ReadLine().Split(' ');
                if (input.Length == 0)
                    break;
                switch (input[0])
                {
                    case "CREATE":
                        if (input.Length < 2)
                            throw new Exception("File path needed");
                        wordDictionary.Create(GetWordsFromFile(input[1]));
                        break;
                    case "UPDATE":
                        if (input.Length < 2)
                            throw new Exception("File path needed");
                        wordDictionary.Update(GetWordsFromFile(input[1]));
                        break;
                    case "CLEAR":
                        wordDictionary.Clear();
                        break;
                    default:
                        throw new Exception("Unknown command");
                }
            }
        }
    }
}
