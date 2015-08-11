using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class FileSystemItemCS
{
	public FileSystemItemCS(FileInfo file)
	{
        this.Name           = file.Name;
        this.FullName       = file.FullName;
        this.Size           = file.Length;
        this.CreationTime   = file.CreationTime;
        this.LastAccessTime = file.LastAccessTime;
        this.LastWriteTime  = file.LastWriteTime;
        this.IsFolder       = false;
	}

    public FileSystemItemCS(DirectoryInfo folder)
    {
        this.Name           = folder.Name;
        this.FullName       = folder.FullName;
        this.Size           = null;
        this.CreationTime   = folder.CreationTime;
        this.LastAccessTime = folder.LastAccessTime;
        this.LastWriteTime  = folder.LastWriteTime;
        this.IsFolder       = true;
    }

    public FileSystemItemCS()
    {
        this.Name           = string.Empty;
        this.FullName       = string.Empty;
        this.Size           = null;
        this.CreationTime   = DateTime.MinValue;
        this.LastAccessTime = DateTime.MinValue;
        this.LastWriteTime  = DateTime.MinValue;
        this.IsFolder       = false;
    }

    public string   Name           { get; set; }
    public string   FullName       { get; set; }
    public long?    Size           { get; set; }
    public DateTime CreationTime   { get; set; }
    public DateTime LastAccessTime { get; set; }
    public DateTime LastWriteTime  { get; set; }
    public bool     IsFolder       { get; set; }
    public string   FileSystemType { get { return this.IsFolder ? "Folder" : "File"; } }

}