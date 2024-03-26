namespace TownOfUsReworked.Modules;

public class UnsupportedLanguageException : Exception
{
    public UnsupportedLanguageException(string message) : base($"Selected language is unsupported {message}") {}

    public UnsupportedLanguageException(string message, Exception innerException) : base($"Selected language is unsupported {message}", innerException) {}
}

public class IncorrectLengthException : Exception
{
    public IncorrectLengthException(string message) : base($"Length is out of bounds {message}") {}

    public IncorrectLengthException(string message, Exception innerException) : base($"Length is out of bounds {message}", innerException) {}
}