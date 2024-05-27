using wordgame.Anagrams;
using wordgame.Contracts;

namespace wordgame.Services
{
    public class WordReader : IWordReader
    {
        private IReadOnlyCollection<string>? _words;
        private string? _dailyWord;
        private Random random = new Random();

        public List<string> GetAnagrams(string word)
        {
            Console.WriteLine(string.Join(", ", AnagramHelper.GetAnagrams(word, _words)));
            Console.WriteLine("***");
            Console.WriteLine(string.Join(", ", AnagramHelper.GetAnagramsWithFrequency(word, _words)));
            Console.WriteLine("***");
            return new List<string> { word };
        }
        public bool CheckWord(string word, string toCheck)
        {
            if (toCheck.Length < 3)
            {
                return false;
            }
            var chars = word
                .ToUpperInvariant()
                .ToCharArray()
                .ToList();
            var checkChars = toCheck
                .ToUpperInvariant()
                .ToCharArray();

            foreach (char c in checkChars)
            {
                if (!chars.Contains(c))
                {
                    return false;
                }
                // remove to prevent usage again
                chars.Remove(c);
            }
            return _words!.Contains(toCheck.ToUpperInvariant());
        }

        public string GetWord(int length)
        {
            var considered = _words!
                .Where(w => w.Length >= length)
                .ToList();

            return considered[random.Next(considered.Count)];
        }

        public bool ReadFile(string path)
        {
            _words = File.ReadAllLines(path);
            return _words.Count > 0;
        }

        public string RefreshWord()
        {
            _dailyWord = GetWord(10);
            return _dailyWord;
        }
        public Dictionary<string, bool> GetQuestionWords(string word, int number = 10)
        {
            return Random.Shared.GetItems(_words!.ToArray(), number)
                .ToDictionary(item => item, value => CheckWord(word, value));
        }
        public string CurrentWord()
        {
            return _dailyWord!;
        }
    }
}
