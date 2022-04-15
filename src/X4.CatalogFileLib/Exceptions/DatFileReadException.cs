namespace X4.CatalogFileLib.Exceptions;

public class DatFileReadException : FileException
{
    public DatFileReadException()
    {
    }
    
    public DatFileReadException(string message, string path) : base(message, path)
    {
    }

    public DatFileReadException(string message, string path, Exception inner)
        : base(message, path, inner)
    {
    }

    public DatFileReadException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}