using System;

[Serializable]
public class ForeignKeyConstraintException :  System.Exception
{
    public ForeignKeyConstraintException()
    {
    }
		
    public ForeignKeyConstraintException(string message): base(message)
    {
    }

    public ForeignKeyConstraintException(string message, Exception innerException): base(message, innerException)
    {
    }
}