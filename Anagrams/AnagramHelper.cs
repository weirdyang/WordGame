using System.Diagnostics;

namespace wordgame.Anagrams
{
    public class AnagramHelper
    {
        public static readonly int WordOffSet = 3;

        const string Placeholder = "$";
        public static IEnumerable<T[]> Combinations<T>(IEnumerable<T> source)
        {
            if (null == source)
                throw new ArgumentNullException(nameof(source));

            T[] data = source.ToArray();

            return Enumerable
              .Range(0, 1 << (data.Length))
              .Select(index => data
                 .Where((v, i) => (index & (1 << i)) != 0)
                 .ToArray());
        }

        public static Dictionary<char, int> GetFrequencyTable(char[] chars)
        {
            var dictionary = new Dictionary<char, int>();
            foreach (char c in chars)
            { 
                if (!dictionary.ContainsKey(c))
                {
                    dictionary[c] = Array
                        .FindAll(chars, (item) => item == c).Length;
                }
            }
            return dictionary;
        }
        public static List<string> GetAnagramsWithFrequency(string word, IEnumerable<string>? possibleWords = null, string? path = null)
        {
            var timer = Stopwatch.StartNew();

            int limit = Limit(word);
            List<char[]> wordChars = 
                GetWordChars(word, ref possibleWords, path)
                .Where(c => c.Length >= limit)
                .ToList();

            var checker = new 
                Dictionary<string,Dictionary<char, int>>(StringComparer.InvariantCultureIgnoreCase);

            foreach (char[] ch in wordChars)
            {
                var current = string.Join("", ch);
                checker[current] = GetFrequencyTable(ch);
            }

            var test = possibleWords!
                .Where(w => w.Length >= limit)
                .Select(w =>
                {
                    var possibleChars = w.ToCharArray();
                    Array.Sort(possibleChars);
                    //var possible = GetFrequencyTable(possibleChars);
                    var isSame = checker.ContainsKey(string.Join("", possibleChars));
                    if (isSame)
                    {
                        return w;
                    }
                    return Placeholder;
                })
                .Where(x => x is not Placeholder)
                .Distinct()
                .ToList();

            timer.Stop();
            Console.WriteLine(string.Format("Frequency {0}", timer.ElapsedMilliseconds));
            return test;

        }
        public static List<string> GetAnagrams(string word,
                                               IEnumerable<string>? possibleWords = null,
                                               string? path = null)
        {
            var timer = Stopwatch.StartNew();

            List<char[]> wordChars = GetWordChars(word, ref possibleWords, path);
            wordChars = wordChars
                .Where(x => x.Length >= Limit(word))
                .ToList();


            var test = wordChars
                .Select(chars =>
                {
                    var anagrams = possibleWords!
                    .Where(x => x.Length == chars.Length)
                    .Select(x =>
                    {
                        var possible = x.ToCharArray();
                        Array.Sort(possible);
                        if (string.Join("", possible)
                        .Equals(string.Join("", chars),
                        StringComparison.InvariantCultureIgnoreCase))
                        {
                            return x;
                        }
                        return Placeholder;

                    })
                    .Where(word => word is not Placeholder)
                    .Distinct()
                    .ToList();
                    return anagrams;
                }).SelectMany(stringlist => stringlist)
            .Distinct()
            .ToList();
            timer.Stop();
            Console.WriteLine(string.Format("List {0}", timer.ElapsedMilliseconds));
            return test;
        }

        private static int Limit(string word)
        {
            return word.Length - 3;
        }

        private static List<char[]> GetWordChars(string word, ref IEnumerable<string>? possibleWords, string? path)
        {
            Validate(possibleWords, path);
            possibleWords = possibleWords ?? ReadFile(path!);

            List<char[]> wordChars = Combinations(word.ToCharArray()).ToList();
            wordChars.ForEach(x => Array.Sort(x));
            return wordChars;
        }

        private static void Validate(IEnumerable<string>? possibleWords, string? path)
        {
            if (possibleWords is null && path is null)
            {
                throw new ArgumentNullException("possible words can not be null if path is null");
            }
        }

        public static List<string> ReadFile(string path)
        {
            return File.ReadAllLines(path).Select(x => x.ToLower()).ToList();
        }
    }
}
