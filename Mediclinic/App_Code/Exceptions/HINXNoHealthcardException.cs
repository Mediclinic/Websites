using System;

[Serializable]
public class HINXNoHealthcardException : System.Exception
{
    public HINXNoHealthcardException()
    {
    }
		
    public HINXNoHealthcardException(string message): base(message)
    {
    }

    public HINXNoHealthcardException(string message, Exception innerException) : base(message, innerException)
    {
    }
}