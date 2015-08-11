using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class UserDatabaseMapper
{

    public UserDatabaseMapper(int id, string username, string dbname)
    {
        this.id = id;
        this.username = username;
        this.dbname = dbname;
    }

    private int id;
    public int ID
    {
        get { return this.id; }
        set { this.id = value; }
    }
    private string username;
    public string Username
    {
        get { return this.username; }
        set { this.username = value; }
    }
    private string dbname;
    public string DBName
    {
        get { return this.dbname; }
        set { this.dbname = value; }
    }
    public override string ToString()
    {
        return id.ToString() + " " + username.ToString() + " " + dbname.ToString();
    }

}