using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace TestTaskServer
{
    public class WordDictionary
    {
        private readonly AppDbContext context;
        private readonly int maxPredictions;
        public WordDictionary(AppDbContext context, int maxPredictions = 5)
        {
            this.context = context;
            this.maxPredictions = maxPredictions;
        }

        public void Create(IEnumerable<Word> words)
        {
            Clear();
            context.Words.AddRange(words);
            context.SaveChanges();
        }

        public void Update(IEnumerable<Word> words)
        {
            var wordList = words.ToList();
            var wordValuesSet = new HashSet<string>(wordList.Select(x => x.Value));
            var existingWords = context.Words
                .Where(x => wordValuesSet.Contains(x.Value));
            foreach (var existingWord in existingWords)
                existingWord.Frequency += wordList.First(x => x.Value == existingWord.Value).Frequency;
            context.UpdateRange(existingWords);
            var existingWordsValuesSet = new HashSet<string>(existingWords.Select(x => x.Value));
            foreach (var newWord in wordList.Where(x => !existingWordsValuesSet.Contains(x.Value)))
                context.Add(newWord);
            context.SaveChanges();
        }

        public IEnumerable<Word> GetPredictions(string prefix)
        {
            return context.Words
                .Where(x => x.Value.StartsWith(prefix))
                .OrderByDescending(x => x.Frequency)
                .ThenBy(x => x.Value)
                .Take(maxPredictions);
        }

        public void Clear()
        {
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Words\"");
        }
    }
}
