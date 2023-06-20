namespace ServeMux.Http;

public interface IHttpRequest
{
    string Path { get; }
    string Body { get; }
}
