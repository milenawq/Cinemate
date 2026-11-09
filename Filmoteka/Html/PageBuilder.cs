using Filmoteka.Models;
using System.Globalization;
using System.Net;
using System.Text;

namespace Filmoteka.Html
{
    public static class PageBuilder
    {
        public static string BuildPage(string body, string title = "Cinemate", string query = "", bool showLayout = true)
        {
            string header = showLayout ? GetHeaderSection(query) : "";
            string footer = showLayout ? GetFooterSection() : "";

            return $"""
                <!DOCTYPE html>
                <html lang="ru">
                <head>
                    <meta charset="utf-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
                    {GetStyleSection(title)}
                </head>
                <body class="body">
                    {header}
                    {body}
                    {footer}
                    {GetScriptSection()}
                </body>
                </html>
                """;
        }

        private static string GetStyleSection(string title)
        {
            return $"""
                <link rel="stylesheet" href="/css/bootstrap.min.css">
                <link rel="stylesheet" href="/css/splide.min.css">
                <link rel="stylesheet" href="/css/slimselect.css">
                <link rel="stylesheet" href="/css/plyr.css">
                <link rel="stylesheet" href="/css/main.css">
                <link rel="stylesheet" href="/css/mystyle.css">
                
                <link rel="stylesheet" href="/webfont/tabler-icons.min.css">
                
                <link rel="icon" type="image/png" href="/icon/favicon-32x32.png" sizes="32x32">
                <link rel="apple-touch-icon" href="/icon/favicon-32x32.png">
                
                <meta name="description" content="Cinemate: популярные фильмы, поиск и категории">
                <title>{WebUtility.HtmlEncode(title)}</title> 
                """;
        }

        private static string GetHeaderSection(string query)
        {
            return $"""
                <header class="header">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <div class="header__content">
                                    <a href="/" class="header__logo">
                                        <span class="logo-text"><span>CINE</span>MATE</span>
                                    </a>
                
                                    <ul class="header__nav">
                                        <li class="header__nav-item">
                                            <a href="/" class="header__nav-link">Главная</a>
                                        </li>
                                    </ul>
                
                                    <div class="header__auth">
                                        <form action="/" method="get" class="header__search">
                                            <input class="header__search-input" type="text" name="query"
                                                   value="{WebUtility.HtmlEncode(query)}" placeholder="Поиск фильмов...">
                                            <button class="header__search-button" type="submit">
                                                <i class="ti ti-search"></i>
                                            </button>
                                            <button class="header__search-close" type="button">
                                                <i class="ti ti-x"></i>
                                            </button>
                                        </form>
                
                                        <button class="header__search-btn" type="button">
                                            <i class="ti ti-search"></i>
                                        </button>
                                    </div>
                
                                    <button class="header__btn" type="button">
                                        <span></span>
                                        <span></span>
                                        <span></span>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </header> 
                """;
        }

        private static string GetFooterSection()
        {
            return """
                <footer class="footer">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <div class="footer__content">
                                    <a href="/" class="footer__logo">
                                        <span class="logo-text"><span>CINE</span>MATE</span>
                                    </a>
                
                                    <span class="footer__copyright">
                                        Cinemate <br>
                                        Данные и изображения: <a href="https://www.themoviedb.org/" target="_blank" rel="noopener">TMDB</a>.
                                        Этот продукт использует TMDB API, но не одобрен и не сертифицирован TMDB.
                                    </span>
                
                                    <button class="footer__back" type="button">
                                        <i class="ti ti-arrow-narrow-up"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </footer>
                """;
        }

        private static string GetScriptSection()
        {
            return """
                   <script src="/js/bootstrap.bundle.min.js"></script>
                   <script src="/js/splide.min.js"></script>
                   <script src="/js/slimselect.min.js"></script>
                   <script src="/js/smooth-scrollbar.js"></script>
                   <script src="/js/plyr.min.js"></script>
                   <script src="/js/main.js"></script>
                """;
        }

        private static string GetMovieCard(Movie movie, bool forCarousel = false)
        {
            string poster = GetPosterUrl(movie.PosterPath, "w342");
            string title = WebUtility.HtmlEncode(movie.Title);
            string rate = movie.VoteAverage.ToString("0.0", CultureInfo.InvariantCulture);
            string rateClass = GetRateClass(movie.VoteAverage);
            string year = movie.ReleaseDate is { Length: >= 4 } ? movie.ReleaseDate[..4] : "";
            string itemClass = forCarousel ? "item item--hero" : "item";

            string item = $"""
                <div class="{itemClass}">
                    <div class="item__cover">
                        <img src="{poster}" alt="{title}">
                        <a href="/movie/{movie.Id}" class="item__play"><i class="ti ti-eye"></i></a>
                        <span class="item__rate {rateClass}">{rate}</span>
                    </div>
                    <div class="item__content">
                        <h3 class="item__title"><a href="/movie/{movie.Id}">{title}</a></h3>
                        <span class="item__category"><a href="/movie/{movie.Id}">{year}</a></span>
                    </div>
                </div>
                """;

            return forCarousel ? $"""<li class="splide__slide">{item}</li>"""
                : $"""<div class="col-6 col-sm-4 col-lg-3 col-xl-2">{item}</div>""";
        }

        public static string GetCarouselSection(List<Movie> movies)
        {
            if (movies.Count == 0) return "";

            var slides = new StringBuilder();
            foreach (Movie m in movies.Take(10))
                slides.Append(GetMovieCard(m, forCarousel: true));

            return $"""
                <section class="home">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <h2 class="home__title"><b>СЕЙЧАС</b> В КИНО</h2>
                            </div>
                            <div class="col-12">
                                <div class="home__carousel splide splide--home">
                                    <div class="splide__arrows">
                                        <button class="splide__arrow splide__arrow--prev" type="button"><i class="ti ti-chevron-left"></i></button>
                                        <button class="splide__arrow splide__arrow--next" type="button"><i class="ti ti-chevron-right"></i></button>
                                    </div>
                                    <div class="splide__track">
                                        <ul class="splide__list">
                                            {slides}
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </section> 
                """;
        }

        public static string GetCatalogPage(string title, MovieListResponse data, List<Genre> genres, string query = "", int? categoryId = null)
        {
            var cards = new StringBuilder();
            foreach (Movie movie in data.Results)
            {
                cards.Append(GetMovieCard(movie));
            }

            int totalPages = Math.Min(data.TotalPages, 500);
            string pagination = GetPagination(data.Page, totalPages, query, categoryId);

            return $"""
                <section class="section section--first">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <div class="section__wrap">
                                    <h1 class="section__title section__title--head">{WebUtility.HtmlEncode(title)}</h1>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>

                {GetGenreFilter(genres, categoryId)}

                <div class="section section--catalog">
                    <div class="container">
                        <div class="row">
                            {cards}
                            {pagination}
                        </div>
                    </div>
                </div>
                """;
        }

        private static string GetGenreFilter(List<Genre> genres, int? categoryId)
        {
            var options = new StringBuilder();
            options.Append("""<option value="/">Все жанры</option>""");

            foreach (Genre genre in genres)
            {
                string name = char.ToUpper(genre.Name[0]) + genre.Name[1..];
                string selected = genre.Id == categoryId ? " selected" : "";
                options.Append($"""<option value="/?categoryId={genre.Id}"{selected}>{WebUtility.HtmlEncode(name)}</option>""");
            }

            return $"""
                <div class="filter">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <div class="filter__content">
                                    <div class="filter__items">
                                        <select class="filter__select" id="filter__genre" onchange="location.href = this.value">
                                            {options}
                                        </select>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div> 
                """;
        }

        private static string GetPageUrl(int page, string query = "", int? categoryId = null)
        {
            var url = $"/?page={page}";
            if (!string.IsNullOrEmpty(query))
                url += $"&query={Uri.EscapeDataString(query)}";
            if (categoryId is not null)
                url += $"&categoryId={categoryId}";
            return url;
        }

        private static string GetPagination(int currentPage, int totalPages, string query = "", int? categoryId = null)
        {
            if (totalPages <= 1)
                return "";

            // проходим по пяти номерам текузей страницы и добавляем в SortedSet чтобы избежать дубликатов и отсортировать
            var pages = new SortedSet<int> { 1, totalPages };
            for (int i = currentPage - 2; i <= currentPage + 2; i++)
            {
                if (i >= 1 && i <= totalPages)
                    pages.Add(i);
            }

            var st = new StringBuilder();

            // добавляем кнопку назад, но не для страницы 1
            if (currentPage > 1)
                st.Append($"""<li class="paginator__item paginator__item--prev"><a href="{GetPageUrl(currentPage - 1, query, categoryId)}"><i class="ti ti-chevron-left"></i></a></li>""");

            // выводим номера по порядку и ставим ... где между соседними номерами пропущены страницы
            int previous = 0;
            foreach (int number in pages)
            {
                if (number - previous > 1)
                    st.Append("""<li class="paginator__item"><span>...</span></li>""");

                string active = number == currentPage ? " paginator__item--active" : "";
                st.Append($"""<li class="paginator__item{active}"><a href="{GetPageUrl(number, query, categoryId)}">{number}</a></li>""");

                previous = number;
            }

            // стрелка вперед кроме последней страницы
            if (currentPage < totalPages)
                st.Append($"""<li class="paginator__item paginator__item--next"><a href="{GetPageUrl(currentPage + 1, query, categoryId)}"><i class="ti ti-chevron-right"></i></a></li>""");

            // для мобильной версии стрелки
            string mobilePrev = currentPage > 1
                ? $"""<li><a href="{GetPageUrl(currentPage - 1, query, categoryId)}"><i class="ti ti-chevron-left"></i><span>Назад</span></a></li>"""
                : "";
            string mobileNext = currentPage < totalPages
                ? $"""<li><a href="{GetPageUrl(currentPage + 1, query, categoryId)}"><span>Далее</span><i class="ti ti-chevron-right"></i></a></li>"""
                : "";

            return $"""
                <div class="col-12">
                    <div class="paginator-mob">
                        <span class="paginator-mob__pages">{currentPage} из {totalPages}</span>
                        <ul class="paginator-mob__nav">
                            {mobilePrev}
                            {mobileNext}
                        </ul>
                    </div>
                    <ul class="paginator">
                        {st}
                    </ul>
                </div> 
                """;
        }

        private static string GetPosterUrl(string? path, string size) =>
    path is null ? "/img/no-poster.svg" : $"https://image.tmdb.org/t/p/{size}{path}";

        private static string GetRateClass(double rate) =>
            rate >= 8 ? "item__rate--green" : rate >= 5 ? "item__rate--yellow" : "item__rate--red";

        public static string GetMoviePage(Movie movie, List<Movie> similar, string? trailerKey)
        {
            string title = WebUtility.HtmlEncode(movie.Title);
            string rate = movie.VoteAverage.ToString("0.0", CultureInfo.InvariantCulture);

            var meta = new StringBuilder();

            if (movie.Genres.Count > 0)
            {
                string genreLinks = string.Join(" ", movie.Genres.Select(g =>
                    $"""<a href="/?categoryId={g.Id}">{WebUtility.HtmlEncode(g.Name)}</a>"""));
                meta.Append($"<li><span>Жанр:</span> {genreLinks}</li>");
            }

            if (movie.ReleaseDate is { Length: >= 4 })
                meta.Append($"<li><span>Премьера:</span> {movie.ReleaseDate[..4]}</li>");

            if (movie.Runtime is > 0)
                meta.Append($"<li><span>Длительность:</span> {movie.Runtime} мин</li>");

            if (movie.ProductionCountries.Count > 0)
            {
                string countries = string.Join(", ", movie.ProductionCountries.Select(c => WebUtility.HtmlEncode(GetCountryName(c))));
                meta.Append($"<li><span>Страна:</span> {countries}</li>");
            }

            string overview = string.IsNullOrWhiteSpace(movie.Overview) ? "Описание отсутствует." : WebUtility.HtmlEncode(movie.Overview);

            string tagline = string.IsNullOrWhiteSpace(movie.Tagline) ? "" : $"<p><i>{WebUtility.HtmlEncode(movie.Tagline)}</i></p>";

            string player = "";
            if (!string.IsNullOrEmpty(trailerKey))
            {
                player = $"""
                    <div class="col-12 col-xl-6">
                        <div id="player" data-plyr-provider="youtube" data-plyr-embed-id="{WebUtility.HtmlEncode(trailerKey)}"></div>
                    </div> 
                    """;
            }
            else if (movie.BackdropPath is not null)
            {
                player = $"""
                    <div class="col-12 col-xl-6">
                        <img src="https://image.tmdb.org/t/p/w780{movie.BackdropPath}" alt="{title}" class="details__backdrop">
                    </div>
                    """;
            }

            string similarBlock = "";
            if (similar.Count > 0)
            {
                var cards = new StringBuilder();
                foreach (Movie m in similar.Take(5))
                    cards.Append(GetMovieCard(m));

                similarBlock = $"""
                    <section class="section section--border">
                        <div class="container">
                            <div class="row">
                                <div class="col-12">
                                    <div class="section__title-wrap">
                                        <h2 class="section__title">Похожие фильмы</h2>
                                    </div>
                                </div>
                                {cards}
                            </div>
                        </div>
                    </section>
                    """;
            }

            return $"""
                <section class="section section--details">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <h1 class="section__title section__title--head">{title}</h1>
                            </div>
                
                            <div class="col-12 col-xl-6">
                                <div class="item item--details">
                                    <div class="row">
                                        <div class="col-12 col-sm-5 col-md-5 col-lg-4 col-xl-6 col-xxl-5">
                                            <div class="item__cover">
                                                <img src="{GetPosterUrl(movie.PosterPath, "w500")}" alt="{title}">
                                                <span class="item__rate {GetRateClass(movie.VoteAverage)}">{rate}</span>
                                            </div>
                                        </div>
                                        <div class="col-12 col-md-7 col-lg-8 col-xl-6 col-xxl-7">
                                            <div class="item__content">
                                                <ul class="item__meta">
                                                    {meta}
                                                </ul>
                                                <div class="item__description">
                                                    {tagline}
                                                    <p>{overview}</p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            {player}
                        </div>
                    </div>
                </section>
                {similarBlock} 
                """;
        }

        private static readonly Dictionary<string, string> CountryNames = new()
        {
            ["US"] = "США",
            ["GB"] = "Великобритания",
            ["FR"] = "Франция",
            ["DE"] = "Германия",
            ["IT"] = "Италия",
            ["ES"] = "Испания",
            ["CA"] = "Канада",
            ["AU"] = "Австралия",
            ["JP"] = "Япония",
            ["KR"] = "Южная Корея",
            ["CN"] = "Китай",
            ["IN"] = "Индия",
            ["RU"] = "Россия",
            ["UA"] = "Украина",
            ["PL"] = "Польша",
            ["SE"] = "Швеция",
            ["NZ"] = "Новая Зеландия",
            ["MX"] = "Мексика",
            ["BR"] = "Бразилия",
            ["HK"] = "Гонконг",
            ["IE"] = "Ирландия",
            ["BE"] = "Бельгия",
            ["DK"] = "Дания",
            ["NO"] = "Норвегия",
            ["CZ"] = "Чехия",
            ["TR"] = "Турция",
            ["AR"] = "Аргентина",
            ["NL"] = "Нидерланды",
            ["CH"] = "Швейцария",
            ["AT"] = "Австрия",
            ["FI"] = "Финляндия",
            ["TH"] = "Таиланд",
            ["TW"] = "Тайвань"
        };

        private static string GetCountryName(ProductionCountry country) =>
            CountryNames.TryGetValue(country.Iso31661, out string? name) ? name : country.Name;

        public static string GetErrorPage(int statusCode, string message)
        {
            return $"""
                <div class="page-404 error-page">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <div class="page-404__wrap">
                                    <div class="page-404__content">
                                        <h1 class="page-404__title">{statusCode}</h1>
                                        <p class="page-404__text">{WebUtility.HtmlEncode(message)}</p>
                                        <a href="/" class="page-404__btn">На главную</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                """;
        }

    }
}
