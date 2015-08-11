using System;

[Serializable]
public class HINXNoPatientOnInvoiceLineException : System.Exception
{
    public HINXNoPatientOnInvoiceLineException()
    {
    }
		
    public HINXNoPatientOnInvoiceLineException(string message): base(message)
    {
    }

    public HINXNoPatientOnInvoiceLineException(string message, Exception innerException) : base(message, innerException)
    {
    }
}