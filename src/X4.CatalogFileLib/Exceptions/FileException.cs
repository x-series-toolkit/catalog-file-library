namespace X4.CatalogFileLib.Exceptions;

public abstract class FileException : CatalogFileLibException
{
    private readonly string? _path;

    protected FileException()
    {
    }

    protected FileException(string message, string path) : base(message)
    {
        _path = path;
    }

    protected FileException(string message, string path, Exception inner)
        : base(message, inner)
    {
        _path = path;
    }

    protected FileException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        
        if (_path != null)
        {
            info.AddValue("CatalogPath", _path, typeof(string));
        }
    }
    
    public override string Message
    {
        get
        {
            string s = base.Message;
            if (!string.IsNullOrEmpty(_path))
            {
                s += $" Path: {_path}";
            }

            return s;
        }
    }
}