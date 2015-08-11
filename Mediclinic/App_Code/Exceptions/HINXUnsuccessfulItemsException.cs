using System;

[Serializable]
public class HINXUnsuccessfulItemsException : System.Exception
{
    public HINXUnsuccessfulItemsException()
    {
    }
		
    public HINXUnsuccessfulItemsException(string message): base(message)
    {
    }

    public HINXUnsuccessfulItemsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}