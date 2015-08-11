using System;

[Serializable]
public class SessionTimedOutException : System.Exception
{
    public SessionTimedOutException()
    {
    }
		
    public SessionTimedOutException(string message): base(message)
    {
    }

    public SessionTimedOutException(string message, Exception innerException) : base(message, innerException)
    {
    }
}