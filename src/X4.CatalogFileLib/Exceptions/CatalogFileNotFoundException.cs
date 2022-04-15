namespace X4.CatalogFileLib.Exceptions;

public class CatalogFileNotFoundException : FileException
{
    public CatalogFileNotFoundException()
    {
    }

    public CatalogFileNotFoundException(string message, string path) 
        : base(message, path)
    {
    }

    public CatalogFileNotFoundException(string message, string path, Exception inner)
        : base(message, path, inner)
    {
    }

    protected CatalogFileNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}