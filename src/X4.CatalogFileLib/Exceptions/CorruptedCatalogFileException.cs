namespace X4.CatalogFileLib.Exceptions;

public class CorruptedCatalogFileException : CatalogFileLibException
{
    public CorruptedCatalogFileException()
    {
    }

    public CorruptedCatalogFileException(string message) : base(message)
    {
    }

    public CorruptedCatalogFileException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public CorruptedCatalogFileException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}