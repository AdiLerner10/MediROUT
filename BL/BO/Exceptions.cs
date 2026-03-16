namespace BO;

/// <summary>
/// Not strong password exception
/// </summary>
[Serializable]
public class BlNotStrongPasswordException : Exception
{
    public BlNotStrongPasswordException(string? message) : base(message) { }
    public BlNotStrongPasswordException(string message, Exception innerException)
        : base(message, innerException) { }

}


/// <summary>
/// Does not exist exception type
/// </summary>
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException) { }

}

/// <summary>
/// already exists exception type
/// </summary>
[Serializable]
public class BlAlreadyExistException : Exception
{
    public BlAlreadyExistException(string? message) : base(message) { }
    public BlAlreadyExistException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// function is null exception type
/// </summary>
[Serializable]
public class BlMemberIsNull : Exception
{
    public BlMemberIsNull(string? message) : base(message) { }
    public BlMemberIsNull(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// unsuccessful casting exception type
/// </summary>
[Serializable]
public class BlUnSuccessfulCasting : Exception
{
    public BlUnSuccessfulCasting(string? message) : base(message) { }
    public BlUnSuccessfulCasting(string message, Exception innerException)
    : base(message, innerException) { }
}

/// <summary>
/// unsuccessful casting exception type
/// </summary>
[Serializable]
public class BlInvalidCredentials : Exception
{
    public BlInvalidCredentials(string? message) : base(message) { }
    public BlInvalidCredentials(string message, Exception innerException)
   : base(message, innerException) { }
}

/// <summary>
/// unsuccessful casting exception type
/// </summary>
[Serializable]
public class BlIllegalRequestException : Exception
{
    public BlIllegalRequestException(string? message) : base(message) { }
    public BlIllegalRequestException(string message, Exception innerException)
   : base(message, innerException) { }
}

/// <summary>
/// unsuccessful casting exception type
/// </summary>
[Serializable]
public class blEmailFailedException : Exception
{
    public blEmailFailedException(string? message) : base(message) { }
    public blEmailFailedException(string message, Exception innerException)
   : base(message, innerException) { }
}

/// <summary>
/// null sent function exception type
/// </summary>
[Serializable]
public class blFunctionIsNull : Exception
{
    public blFunctionIsNull(string? message) : base(message) { }
    public blFunctionIsNull(string message, Exception innerException)
   : base(message, innerException) { }
}


/// <summary>
/// unsucessful castig exception type
/// </summary>
[Serializable]
public class blUnSuccessfulCasting : Exception
{
    public blUnSuccessfulCasting(string? message) : base(message) { }
    public blUnSuccessfulCasting(string message, Exception innerException)
   : base(message, innerException) { }
}

/// <summary>
/// unsucessful castig exception type
/// </summary>
[Serializable]
public class blInvalidInputException : Exception
{
    public blInvalidInputException(string? message) : base(message) { }
    public blInvalidInputException(string message, Exception innerException)
   : base(message, innerException) { }
}

/// <summary>
/// Not Available castig exception type
/// </summary>
[Serializable]
public class BLTemporaryNotAvailableException
 : Exception
{
    public BLTemporaryNotAvailableException(string? message) : base(message) { }
    public BLTemporaryNotAvailableException(string message, Exception innerException)
   : base(message, innerException) { }
}

