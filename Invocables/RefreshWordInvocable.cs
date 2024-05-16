using Coravel.Invocable;
using wordgame.Contracts;

namespace wordgame.Invocables
{
    public class RefreshWordInvocable : IInvocable
    {
        private readonly IWordReader _wordReader;

        public RefreshWordInvocable(IWordReader wordReader)
        {
            _wordReader = wordReader ?? throw new ArgumentNullException(nameof(wordReader));
        }

        public Task Invoke()
        {
            _wordReader.RefreshWord();
            return Task.CompletedTask;
        }
    }
}
