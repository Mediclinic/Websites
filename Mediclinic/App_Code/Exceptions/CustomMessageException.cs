using System;

[Serializable]
public class CustomMessageException : System.Exception
{
    public bool HidePage = false;

    public CustomMessageException(bool hidePage = false)
    {
        this.HidePage = hidePage;
    }
		
    public CustomMessageException(string message, bool hidePage=false): base(message)
    {
        this.HidePage = hidePage;
    }

    public CustomMessageException(string message, Exception innerException, bool hidePage = false) : base(message, innerException)
    {
        this.HidePage = hidePage;
    }
}