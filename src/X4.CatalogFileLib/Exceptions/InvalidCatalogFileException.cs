namespace X4.CatalogFileLib.Exceptions;

public class InvalidCatalogFileException : CatalogFileLibException
{
    public InvalidCatalogFileException()
    {
    }

    public InvalidCatalogFileException(string message) : base(message)
    {
    }

    public InvalidCatalogFileException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public InvalidCatalogFileException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}