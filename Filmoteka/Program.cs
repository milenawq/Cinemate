using Filmoteka.Api;
using Filmoteka.Html;
using Filmoteka.Models;

var builder = WebApplication.CreateBuilder(args);
TmdbApi.ApiKey = builder.Configuration["TMDB:ApiKey"] ?? "";
var app = builder.Build();

app.UseStaticFiles();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (TmdbException ex)
    {
        await WriteErrorPage(context, ex.StatusCode, ex.Message);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Необработанная ошибка");
        await WriteErrorPage(context, 500, "Что-то пошло не так. Попробуйте позже.");
    }
});

app.MapGet("/", async (ILogger <Program> logger, int page = 1, string? query = null, int? categoryId = null) =>
{
    page = Math.Clamp(page, 1, 500);
    string search = query?.Trim() ?? "";

    var genres = (await TmdbApi.GetGenreList()).Genres;

    MovieListResponse data;
    string title;

    if (search != "")
    {
        data = await TmdbApi.SearchMovies(search, page);
        title = $"Результаты поиска: {search}";
    }
    else if (categoryId is not null)
    {
        data = await TmdbApi.GetMoviesByGenre(categoryId.Value, page);
        string name = genres.FirstOrDefault(g => g.Id == categoryId)?.Name ?? "Фильмы";
        title = char.ToUpper(name[0]) + name[1..];
    }
    else
    {
        data = await TmdbApi.GetPopularMovies(page);
        title = "Популярные фильмы";
    }

    int? activeCategory = search == "" ? categoryId : null;

    string carousel = "";
    if (page == 1 && search == "" && categoryId is null)
    {
        try
        {
            var nowPlaying = await TmdbApi.GetNowPlayingMovies();
            carousel = PageBuilder.GetCarouselSection(nowPlaying.Results);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Не удалось загрузить карусель");
        }
    }

    string body = carousel + PageBuilder.GetCatalogPage(title, data, genres, search, activeCategory);
    string html = PageBuilder.BuildPage(body, title, search);

    return Results.Content(html, "text/html; charset=utf-8");
});

app.MapGet("/movie/{id:int}", async (int id, ILogger<Program> logger) =>
{
    var movie = await TmdbApi.GetMovieById(id);
    var trailerKey = await TmdbApi.GetTrailerKey(id, logger);

    var similar = new List<Movie>();
    try
    {
        similar = (await TmdbApi.GetSimilarMovies(id)).Results;
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Не удалось загрузить схожие фильмы для {MovieId}", id);
    }

    string body = PageBuilder.GetMoviePage(movie, similar, trailerKey);
    string html = PageBuilder.BuildPage(body, movie.Title);

    return Results.Content(html, "text/html; charset=utf-8");
});

app.MapFallback(async context =>
    await WriteErrorPage(context, 404, "Такой страницы не существует."));

app.Run();

static async Task WriteErrorPage(HttpContext context, int statusCode, string message)
{
    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "text/html; charset=utf-8";

    string body = PageBuilder.GetErrorPage(statusCode, message);
    string html = PageBuilder.BuildPage(body, $"Ошибка {statusCode}", showLayout: false);

    await context.Response.WriteAsync(html);
}

