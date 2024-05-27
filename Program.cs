using Coravel;
using Microsoft.AspNetCore.Mvc;
using wordgame.Anagrams;
using wordgame.Contracts;
using wordgame.Invocables;
using wordgame.Services;

namespace wordgame
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<IWordReader, WordReader>();
            builder.Services.AddRazorPages();
            builder.Services.AddScheduler();
            var app = builder.Build();
            app.Services.UseScheduler(ss =>
            {
                ss.Schedule<RefreshWordInvocable>()
                .DailyAtHour(12)
                .PreventOverlapping(nameof(RefreshWordInvocable));
            });
            app.MapRazorPages();
            app.Services
                .GetRequiredService<IWordReader>()
                .ReadFile(Path.Combine(
                        builder.Environment.WebRootPath,
                        "assets",
                        "words.txt"));

            app.Services
                .GetRequiredService<IWordReader>()
                .RefreshWord();

            app.MapGet("/api/text", () =>
            {
                return File.ReadAllLines(
                    Path.Combine(
                        builder.Environment.WebRootPath,
                        "assets",
                        "words.txt"));
            });

            app.MapGet("/api/random",
                (IWordReader wordReader,
                [FromQuery(Name = "l")] int length) =>
                {
                    return Results.Ok(wordReader.GetWord(length));
                });

            app.MapGet("/api/check",
                (IWordReader wordReader,
                [FromQuery(Name = "w")] string word,
                [FromQuery(Name = "c")] string check) =>
                {
                    return wordReader.CheckWord(word, check)
                    ? Results.Ok("word exists")
                    : Results.NotFound("word does not exist");
                });

            app.UseStaticFiles();
            app.Run();
        }
    }
}
