namespace X4.CatalogFileLib.Exceptions;


[Serializable]
public abstract class CatalogFileLibException : Exception
{
    protected CatalogFileLibException()
    {
    }

    protected CatalogFileLibException(string message) : base(message)
    {
    }

    protected CatalogFileLibException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected CatalogFileLibException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}