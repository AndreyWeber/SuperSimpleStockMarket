namespace SuperSimpleStockMarket.Common;

public static class Throw
{
    /// <summary>
    /// Throws <see cref="ArgumentNullException" /> with if <paramref name="argVal" />
    /// is equal to null.
    /// </summary>
    /// <param name="argName">Argument name</param>
    /// <param name="argVal">Argument value</param>
    /// <param name="message">Exception message</param>
    public static void IfNull(String argName, Object argVal, String message = "")
    {
        if (argVal == null)
        {
            throw new ArgumentNullException(
                argName,
                String.IsNullOrWhiteSpace(message)
                    ? "Argument cannot be null"
                    : message
            );
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException" /> with if <paramref name="argVal" />
    /// is null or white space / empty string.
    /// </summary>
    /// <param name="argName">Argument name</param>
    /// <param name="argVal">Argument value</param>
    /// <param name="message">Exception message</param>
    public static void IfNullOrWhiteSpace(String argName, String argVal, String message = "")
    {
        if (String.IsNullOrWhiteSpace(argVal))
        {
            throw new ArgumentException(
                argName,
                String.IsNullOrWhiteSpace(message)
                    ? "Argument cannot be null or empty"
                    : message
            );
        }
    }
}
