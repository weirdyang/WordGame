using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using wordgame.Contracts;

namespace wordgame.Services
{
    public class WordReader : IWordReader
    {
        private HashSet<string> _words = new(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<string>> _wordMap = new(StringComparer.Ordinal);
        private List<string> _dailyCandidates = [];
        private string? _dailyWord;

        public bool ReadFile(string path)
        {
            _words = File.ReadLines(path)
                .Select(Normalize)
                .Where(word => word.Length >= 3 && word.All(char.IsLetter))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _wordMap = _words
                .GroupBy(SortLetters)
                .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);

            _dailyCandidates = _words
                .Where(word => word.Length is >= 7 and <= 9)
                .OrderBy(word => word, StringComparer.Ordinal)
                .ToList();

            return _dailyCandidates.Count > 0;
        }

        public string GetWord(int minimumLength)
        {
            var choices = _words.Where(word => word.Length >= minimumLength).ToArray();
            if (choices.Length == 0) throw new InvalidOperationException("No eligible words are loaded.");
            return choices[Random.Shared.Next(choices.Length)];
        }

        public bool CheckWord(string sourceWord, string candidate)
        {
            var source = Normalize(sourceWord);
            var guess = Normalize(candidate);

            if (guess.Length < 3 || guess == source || !_words.Contains(guess))
                return false;

            var remaining = source.ToList();
            foreach (var letter in guess)
            {
                if (!remaining.Remove(letter)) return false;
            }

            return true;
        }

        public IReadOnlyList<string> GetAnagrams(string word)
        {
            var normalized = Normalize(word);
            var combinations = new HashSet<string>(StringComparer.Ordinal);

            for (var mask = 1; mask < (1 << normalized.Length); mask++)
            {
                var length = BitOperations.PopCount((uint)mask);
                if (length < 3 || length >= normalized.Length) continue;
                var letters = new char[length];
                var index = 0;
                for (var i = 0; i < normalized.Length; i++)
                    if ((mask & (1 << i)) != 0) letters[index++] = normalized[i];
                combinations.Add(SortLetters(new string(letters)));
            }

            return combinations
                .Where(_wordMap.ContainsKey)
                .SelectMany(key => _wordMap[key])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(candidate => candidate.Length)
                .ThenBy(candidate => candidate, StringComparer.Ordinal)
                .ToList();
        }

        public string RefreshWord()
        {
            if (_dailyCandidates.Count == 0) throw new InvalidOperationException("Words have not been loaded.");
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"word-hunt:{date:yyyy-MM-dd}"));
            var index = BitConverter.ToUInt32(bytes, 0) % (uint)_dailyCandidates.Count;
            _dailyWord = _dailyCandidates[(int)index];
            return _dailyWord;
        }

        public string CurrentWord() => _dailyWord ?? RefreshWord();
        private static string Normalize(string value) => value.Trim().ToLowerInvariant();
        private static string SortLetters(string value) => new(value.OrderBy(letter => letter).ToArray());
    }
}

