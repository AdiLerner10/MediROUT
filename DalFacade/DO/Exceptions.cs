namespace DO;

/// <summary>
/// Does not exist exception type
/// </summary>
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

/// <summary>
/// already exists exception type
/// </summary>
[Serializable]
public class DalAlreadyExistException : Exception
{
    public DalAlreadyExistException(string? message) : base(message) { }
}

/// <summary>
/// function is null exception type
/// </summary>
[Serializable]
public class DalFunctionIsNull : Exception
{
    public DalFunctionIsNull(string? message) : base(message) { }
}

/// <summary>
/// unsuccessful casting exception type
/// </summary>
[Serializable]
public class DalUnSuccessfulCasting : Exception
{
    public DalUnSuccessfulCasting(string? message) : base(message) { }
}

/// <summary>
/// DalXMLFileLoad create exception type
/// </summary>
[Serializable]
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}
