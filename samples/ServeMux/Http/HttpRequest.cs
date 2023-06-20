namespace ServeMux.Http;

public class HttpRequest : IHttpRequest
{
    public string Path { get; init; } = default!;

    public string Body { get; init; } = default!;
}
