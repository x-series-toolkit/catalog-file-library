namespace X4.CatalogFileLib.Exceptions;

public class DatFileNotFoundException : FileException
{
    public DatFileNotFoundException()
    {
    }
    
    public DatFileNotFoundException(string message, string path) : base(message, path)
    {
    }

    public DatFileNotFoundException(string message, string path, Exception inner)
        : base(message, path, inner)
    {
    }

    public DatFileNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}