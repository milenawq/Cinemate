using Filmoteka.Models;
using System.Net;
using System.Text.Json;

namespace Filmoteka.Api;

public static class TmdbApi
{
    public static string ApiKey { get; set; } = "";

    private static readonly HttpClient client = new() { Timeout = TimeSpan.FromSeconds(10) };

    private static async Task<T> SendApi<T>(string path, string language = "ru-RU")
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new TmdbException(500, "API-ключ не настроен. Добавьте его!");

        string separator = path.Contains('?') ? "&" : "?";
        string url = $"https://api.themoviedb.org/3/{path}{separator}api_key={ApiKey}&language={language}";

        try
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new TmdbException(401, "TMDB не принял API-ключ. Проверьте, что ключ указан правильно."),
                    HttpStatusCode.NotFound => new TmdbException(404, "Фильм или страница не найдены."),
                    HttpStatusCode.TooManyRequests => new TmdbException(429, "Слишком много запросов к TMDB. Подождите немного и попробуйте снова."),
                    _ => new TmdbException(502, "Сервис TMDB сейчас недоступен. Попробуйте позже.")
                };
            }

            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json)
                ?? throw new TmdbException(502, "TMDB прислал пустой ответ.");
        }
        catch (HttpRequestException ex)
        {
            throw new TmdbException(503, "Не удалось подключиться к TMDB. Проверьте подключение к интернету.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new TmdbException(504, "TMDB слишком долго не отвечает. Попробуйте позже.", ex);
        }
        catch (JsonException ex)
        {
            throw new TmdbException(502, "Не удалось обработать ответ TMDB.", ex);
        }
    }

    public static async Task<GenreListResponse> GetGenreList()
    {
        return await SendApi<GenreListResponse>("genre/movie/list");
    }

    public static async Task<MovieListResponse> GetPopularMovies(int page)
    {
        return await SendApi<MovieListResponse>($"movie/popular?page={page}");
    }

    public static async Task<MovieListResponse> SearchMovies(string query, int page)
    {
        return await SendApi<MovieListResponse>($"search/movie?query={Uri.EscapeDataString(query)}&page={page}");
    }

    public static async Task<MovieListResponse> GetMoviesByGenre(int genreId, int page)
    {
        return await SendApi<MovieListResponse>($"discover/movie?with_genres={genreId}&page={page}");
    }

    public static async Task<Movie> GetMovieById(int id)
    {
        return await SendApi<Movie>($"movie/{id}");
    }

    public static async Task<MovieListResponse> GetSimilarMovies(int id)
    {
        return await SendApi<MovieListResponse>($"movie/{id}/similar");
    }

    public static async Task<MovieListResponse> GetNowPlayingMovies()
    {
        return await SendApi<MovieListResponse>("movie/now_playing");
    }

    public static async Task<string?> GetTrailerKey(int id, ILogger logger)
    {
        try
        {
            var videos = await SendApi<VideoResponse>($"movie/{id}/videos", "en-US");

            return videos.Results.Where(v => v.Site == "YouTube" && (v.Type == "Trailer" || v.Type == "Teaser"))
                .OrderBy(v => v.Type == "Trailer" ? 0 : 1)
                .ThenByDescending(v => v.Official)
                .FirstOrDefault()?.Key;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Не удалось получить трейлер фильма {MovieId}", id);
            return null;
        }
    }


}