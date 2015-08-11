using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using oWord = Microsoft.Office.Interop.Word;

// http://forums.asp.net/t/392321.aspx/1


//if (Request.UserHostAddress.StartsWith("192.168"))
//{
//    localuser
//}


public class WordDocServerSidePrinter
{

    /// <summary>
    /// print Word
    /// </summary>
    /// <param name="wordfile">Fullpath and file name</param>
    /// <param name="printer">Printer name</param>
    public static void Print(string wordfile, string printer)
    {
        oWord.ApplicationClass word = new oWord.ApplicationClass();
        Type wordType = word.GetType();
        
        //Open WORD
        oWord.Documents docs = word.Documents;
        Type docsType = docs.GetType();
        object objDocName = wordfile;
        oWord.Document doc = (oWord.Document)docsType.InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, docs, new Object[] { objDocName, true, true });

        doc.Application.ActivePrinter = printer;

        //Print to File  
        //used doc.PrintOut() and Type.InvokeMember   
        Type docType = doc.GetType();
        object printFileName = wordfile + ".xps";
        docType.InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, doc, new object[] { false, false, oWord.WdPrintOutRange.wdPrintAllDocument, printFileName });
        
        //Exit WORD
        wordType.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, word, null);
    }

    public static void Print2(string wordfile, string printer = null)
    {
        oWord.Application wordApp = new oWord.Application();
        wordApp.Visible = false;

        wordApp.Documents.Open(wordfile);
        wordApp.DisplayAlerts = oWord.WdAlertLevel.wdAlertsNone;


        System.Drawing.Printing.PrinterSettings settings = new System.Drawing.Printing.PrinterSettings();

        if (printer == null) // print to all installed printers
        {
            foreach (string p in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                try
                {
                    settings.PrinterName = p;
                    wordApp.ActiveDocument.PrintOut(false);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, true);
                }
            }
        }
        else
        {
            settings.PrinterName = printer;
            wordApp.ActiveDocument.PrintOut(false);
        }

        wordApp.Quit(oWord.WdSaveOptions.wdDoNotSaveChanges);
    }

}