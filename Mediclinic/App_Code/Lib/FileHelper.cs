using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

public class FileHelper
{

    public static string GetTempFileName(string docname)
    {
        int index = docname.LastIndexOf('.');
        string part1 = docname.Substring(0, index) + "_";
        string part2 = docname.Substring(index);
        for (int i = 0; i < 1000; i++)
        {
            //string name = part1 + i.ToString().PadLeft(4, '0') + part2;
            string name = part1 + RandomString(8) + part2;  // randomly generate string (10^55 possibilities)
            if (!File.Exists(name))
                return name;
        }

        throw new CustomMessageException("No new files can be created.");
    }

    public static string GetTempDirectoryName(string dir)
    {
        if (!dir.EndsWith(@"\"))
            dir += @"\";

        for (int i = 0; i < 1000; i++)
        {
            string name = dir + RandomString(8);  // randomly generate string (10^55 possibilities)
            if (!Directory.Exists(name))
                return name + @"\";
        }

        throw new CustomMessageException("No new directories can be created.");
    }

    private static Random random = new Random((int)DateTime.Now.Ticks);
    private static string RandomString(int size)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < size; i++)
        {
            double rnd = 61.999 * random.NextDouble();
            char ch;
            if (rnd < 26)
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(rnd + 65)));
            else if (rnd < 52)
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(rnd - 26 + 97)));
            else
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(rnd - 52 + 48)));

            //ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }

        return builder.ToString();
    }


    public static void DeleteOldFiles(string dir, TimeSpan age)
    {
        string[] fileEntries = Directory.GetFiles(dir);
        DateTime maxAge = DateTime.Now.Subtract(age);
        foreach (string fileName in fileEntries)
        {
            FileInfo fi = new FileInfo(fileName);
            if (fi.LastWriteTime < maxAge)
                fi.Delete();
        }
    }

}