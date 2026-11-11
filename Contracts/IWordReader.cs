namespace wordgame.Contracts
{
    public interface IWordReader
    {
        bool ReadFile(string path);
        string GetWord(int minimumLength);
        bool CheckWord(string sourceWord, string candidate);
        string RefreshWord();
        string CurrentWord();
        IReadOnlyList<string> GetAnagrams(string word);
    }
}

