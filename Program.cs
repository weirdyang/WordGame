using Microsoft.AspNetCore.Mvc;
using wordgame.Contracts;
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
            var app = builder.Build();

            var wordReader = app.Services.GetRequiredService<IWordReader>();
            wordReader.ReadFile(Path.Combine(builder.Environment.WebRootPath, "assets", "words.txt"));
            wordReader.RefreshWord();
            app.MapRazorPages();

            app.MapGet("/api/puzzle", (IWordReader reader) =>
            {
                var word = reader.CurrentWord();
                return Results.Ok(new { word, solutionCount = reader.GetAnagrams(word).Count, date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd") });
            });
            app.MapGet("/api/check", (IWordReader reader, [FromQuery(Name = "c")] string? candidate) =>
            {
                if (string.IsNullOrWhiteSpace(candidate)) return Results.BadRequest(new { message = "Enter a word first." });
                var normalized = candidate.Trim().ToLowerInvariant();
                if (!reader.CheckWord(reader.CurrentWord(), normalized)) return Results.NotFound(new { message = "That word is not in today's puzzle." });
                return Results.Ok(new { word = normalized, points = Math.Max(1, normalized.Length - 2) });
            });
            app.MapGet("/api/solutions", (IWordReader reader) => Results.Ok(reader.GetAnagrams(reader.CurrentWord())));
            app.UseStaticFiles();
            app.Run();
        }
    }
}

