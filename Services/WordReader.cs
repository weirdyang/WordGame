using wordgame.Anagrams;
using wordgame.Contracts;

namespace wordgame.Services
{
    public class WordReader : IWordReader
    {
        private IReadOnlyCollection<string>? _words;
        private string? _dailyWord;
        private Random random = new Random();
        private Dictionary<string, List<string>>? _wordMap;
        public List<string> GetAnagrams(string word)
        {
            return AnagramHelper.GetAnagramsWithFrequency(word, _wordMap!);
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
            _words = File
                .ReadAllLines(path)
                .Select(x => x.ToLower())
                .ToArray();
            _wordMap = BuildHashMap(_words);
            return _words.Count > 0;
        }
        private string SortString(string input)
        {
            char[] characters = input.ToArray();
            Array.Sort(characters);
            return new string(characters);
        }
        private Dictionary<string, List<string>> BuildHashMap(IEnumerable<string> words)
        {
            var wordList = words.ToList();
            var dictionary = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
            for (int i = 0; i < words.Count(); i++)
            {
                var key = SortString(wordList[i]);
                if(dictionary.TryGetValue(key, out var value))
                {
                    dictionary[key] = value.Append(wordList[i]).ToList();
                    continue;
                }
                dictionary[key] = [wordList[i]];
            }
            return dictionary;
        }
        public string RefreshWord()
        {
            _dailyWord = GetWord(10);
            return _dailyWord;
        }
        public Dictionary<string, bool> GetQuestionWords(string word, int number = 10)
        {
            return Random
                .Shared
                .GetItems(_words!.ToArray(), number)
                .ToDictionary(item => item, value => CheckWord(word, value));
        }
        public Dictionary<string, bool> GetAnagramWords(string word, int number = 10)
        {
            var possibleAnagrams = GetAnagrams(word);
            possibleAnagrams.Remove(word);
            var anagrams = possibleAnagrams.ToArray();
            var random = new Random();
            var anagramsNumber = anagrams.Length >= 10 ? random.Next(5, 8) : anagrams.Length;
            var selectedAnagrams = Random.Shared.GetItems<string>(anagrams, anagramsNumber);
            Console.WriteLine(word);
            Console.WriteLine(string.Join(", ", selectedAnagrams));
            var wordLimit = AnagramHelper.Limit(word);

            var selectedWords = Random.Shared
                .GetItems(_words!
                .Where(x => x.Length <= word.Length
                && !selectedAnagrams.Contains(x, StringComparer.InvariantCultureIgnoreCase)).ToArray(),
                number - anagramsNumber)
                .ToDictionary(item => item, value => CheckWord(word, value));

            foreach ( var item in selectedAnagrams)
            {
                selectedWords[item] = true;
            }
            return selectedWords;

        }
        public string CurrentWord()
        {
            return _dailyWord!;
        }
    }
}
