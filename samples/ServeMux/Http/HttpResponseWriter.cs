using System.Text;

namespace ServeMux.Http;

public class HttpResponseWriter : IHttpResponseWriter
{
    public StringBuilder Buffer { get; } = new();

    public void Write(string message)
    {
        Buffer.Append(message);
    }
}
