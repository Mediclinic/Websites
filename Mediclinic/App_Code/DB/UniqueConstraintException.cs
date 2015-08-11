using System;

[Serializable]
public class UniqueConstraintException : System.Exception
{
    public UniqueConstraintException()
    {
    }
		
    public UniqueConstraintException(string message): base(message)
    {
    }

    public UniqueConstraintException(string message, Exception innerException): base(message, innerException)
    {
    }
}