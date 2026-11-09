namespace Filmoteka.Api;

public class TmdbException : Exception
{
    public int StatusCode { get; }

    public TmdbException(int statusCode, string message, Exception? inner = null) : base(message, inner)
    {
        StatusCode = statusCode;
    }
}
