namespace wordgame.Contracts
{
    public interface IWordReader
    {
        Dictionary<string, bool> GetQuestionWords(string word, int number = 10);

        Dictionary<string, bool> GetAnagramWords(string word, int number = 10);
        bool ReadFile(string path);
        string GetWord(int length);
        bool CheckWord(string word, string toCheck);
        string RefreshWord();
        string CurrentWord();

        List<string> GetAnagrams(string word);
    }
}
