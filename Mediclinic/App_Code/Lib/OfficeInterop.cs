using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

using Microsoft.Office;
using Word = Microsoft.Office.Interop.Word;  // Add ref - COM tab - "Microsoft Office 14.0 Object Library"
using Microsoft.Office.Interop.Word;

/*
 * Don't use this file
 * Instead use file:  "Business Objects/WordMailMerge.cs"
 * 
 * (tho maybe the methods at the end are used... dunno
 */


namespace OfficeInterop
{
    // Adding a field in word:
    // "Quick Parts" => "Field..." => "MergeField"
    public class MailMerge
    {
        public static bool MergeDataWithWordTemplate(string sourceTemplatePath, string outputDocPath, DataSet sourceData, bool isDoubleSidedPrinting, string[] extraPages, out string errorString)
        {
            #region Declares
            errorString = string.Empty;
            Object oMissing = System.Reflection.Missing.Value; //null value
            Object oTrue = true;
            Object oFalse = false;
            Object oPageBreak = Word.WdBreakType.wdPageBreak;
            Object oTemplatePath = sourceTemplatePath;
            Object oOutputPath = outputDocPath;
            Object oOutputPathTemp = outputDocPath.Substring(0, outputDocPath.IndexOf(".doc")) + "_temp.doc";
            Object sectionStart = (Object)Microsoft.Office.Interop.Word.WdSectionStart.wdSectionNewPage;

            Word.Application oWord = null;
            Word.Document oWordDoc = null; //the document to load into word application
            Word.Document oFinalWordDoc = null; //the document to load into word application
            #endregion

            try
            {
                Logger.LogMailMerge(false, "");
                Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() merging document: " + sourceTemplatePath);

                oWord = new Word.Application(); //starts an instance of word
                oWord.Visible = false; //don't show the UI

                //create an empty document that we will insert all the merge docs into
                oFinalWordDoc = oWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //for each record in the dataset
                int count = 1;
                foreach (DataRow dr in sourceData.Tables[0].Rows)
                {
                    Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() adding a document for this record");

                    //insert a document for each record
                    oWordDoc = oWord.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);
                    if (oWordDoc.Fields.Count == 0)
                    {
                        Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() No template fields found in document:" + sourceTemplatePath);
                        return false;
                    }

                    foreach (Word.Field myMergeField in oWordDoc.Fields)
                    {
                        Word.Range rngFieldCode = myMergeField.Code;
                        String fieldText = rngFieldCode.Text;
                    }

                    oWordDoc.Activate(); //make current

                    // Perform mail merge field
                    foreach (Word.Field myMergeField in oWordDoc.Fields)
                    {
                        Word.Range rngFieldCode = myMergeField.Code;
                        String fieldText = rngFieldCode.Text;

                        // ONLY GET THE MAILMERGE FIELDS
                        if (fieldText.StartsWith(" MERGEFIELD") && fieldText.IndexOf("\\") != -1)
                        {
                            // THE TEXT COMES IN THE FORMAT OF
                            // MERGEFIELD MyFieldName \\* MERGEFORMAT
                            // THIS HAS TO BE EDITED TO GET ONLY THE FIELDNAME "MyFieldName"
                            Int32 endMerge = fieldText.IndexOf("\\");
                            Int32 fieldNameLength = fieldText.Length - endMerge;
                            String fieldName = fieldText.Substring(11, endMerge - 11);
                            //Logger.LogMailMerge("fieldName:" + fieldName);

                            // GIVES THE FIELDNAMES AS THE USER HAD ENTERED IN .dot FILE
                            // field names with spaces in them have quotes on either end, so strip those
                            fieldName = fieldName.Trim().Replace("\"", "");
                            Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() found word template field: " + fieldName);

                            //find a matching dataset column
                            foreach (DataColumn col in sourceData.Tables[0].Columns)
                            {

                                // google:   c# word document merge table

                                if (col.ColumnName.StartsWith("tbl_"))
                                {

                                    // Fill the table with data.
                                    Word.Table tbl = oWordDoc.Tables[1];
                                    DirectoryInfo di = new DirectoryInfo("C:\\");

                                    // Start with row 2.
                                    int i = 2;

                                    Object beforeRow = Type.Missing;
                                    foreach (FileInfo fi in di.GetFiles())
                                    {
                                        tbl.Rows.Add(ref beforeRow);
                                        tbl.Cell(i, 1).Range.Text = fi.Name;
                                        tbl.Cell(i, 2).Range.Text =
                                          string.Format("{0:N0}", fi.Length);
                                        tbl.Cell(i, 3).Range.Text =
                                            string.Format("{0:g}", fi.LastWriteTime);
                                        i++;
                                    } 

                                    continue;
                                }


                                string key = col.ColumnName;
                                string value = dr[key].ToString();

                                // **** FIELD REPLACEMENT IMPLEMENTATION GOES HERE ****//
                                // THE PROGRAMMER CAN HAVE HIS OWN IMPLEMENTATIONS HERE
                                if (fieldName == key)
                                {
                                    Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() setting value: " + value);

                                    myMergeField.Select();
                                    oWord.Selection.TypeText(value);
                                }
                            }
                        }
                    }


                    if (isDoubleSidedPrinting) // make sure so far, even nbr of pages
                    {
                        int nPages = oWordDoc.ComputeStatistics(Word.WdStatistic.wdStatisticPages, ref oMissing);
                        if (nPages % 2 == 1)
                        {
                            Word.Paragraph paragraph = oWordDoc.Content.Paragraphs.Add(ref oMissing);
                            paragraph.Range.InsertParagraphBefore();
                            paragraph.Range.Text = string.Empty;
                            paragraph.Range.InsertBreak(ref oPageBreak);
                            paragraph.Range.InsertParagraphAfter();
                        }
                    }



                    Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() adding pages");

                    if (extraPages != null)
                    {
                        foreach (string text in extraPages)
                        {
                            Word.Paragraph paragraph;

                            paragraph = oWordDoc.Content.Paragraphs.Add(ref oMissing);
                            paragraph.Range.InsertParagraphBefore();
                            paragraph.Range.Text = string.Empty;
                            paragraph.Range.InsertBreak(ref oPageBreak);
                            paragraph.Range.InsertParagraphAfter();

                            paragraph = oWordDoc.Content.Paragraphs.Add(ref oMissing);
                            paragraph.Range.InsertParagraphBefore();
                            paragraph.Range.Text = text;
                            paragraph.Range.InsertParagraphAfter();


                            if (isDoubleSidedPrinting) // make sure so far, even nbr of pages
                            {
                                int nPages = oWordDoc.ComputeStatistics(Word.WdStatistic.wdStatisticPages, ref oMissing);
                                if (nPages % 2 == 1)
                                {
                                    paragraph = oWordDoc.Content.Paragraphs.Add(ref oMissing);
                                    paragraph.Range.InsertParagraphBefore();
                                    paragraph.Range.Text = string.Empty;
                                    paragraph.Range.InsertBreak(ref oPageBreak);
                                    paragraph.Range.InsertParagraphAfter();
                                }
                            }

                        }
                    }


                    Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() saving the doc");

                    //SAVE THE DOCUMENT as temp
                    oWordDoc.SaveAs(ref oOutputPathTemp, ref oMissing, ref oMissing, ref oMissing
                        , ref oMissing, ref oMissing, ref oMissing, ref oMissing
                        , ref oMissing, ref oMissing, ref oMissing, ref oMissing
                        , ref oMissing, ref oMissing, ref oMissing, ref oMissing);



                    Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() closing the doc");

                    //CLOSE THE DOCUMENT
                    oWordDoc.Close(ref oFalse, ref oMissing, ref oMissing);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oWordDoc);
                    oWordDoc = null;

                    //NOW ADD THE NEW DOC TO THE MAIN DOC
                    oFinalWordDoc.Activate(); //make current
                    oWord.Selection.InsertFile(oOutputPathTemp.ToString(), ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                    if (count < sourceData.Tables[0].Rows.Count)
                        oWord.Selection.InsertBreak(ref sectionStart);

                    count++;
                }

                //SAVE THE FINAL DOC
                oFinalWordDoc.SaveAs(ref oOutputPath, ref oMissing, ref oMissing, ref oMissing
                        , ref oMissing, ref oMissing, ref oMissing, ref oMissing
                        , ref oMissing, ref oMissing, ref oMissing, ref oMissing
                        , ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //CLOSE THE FINAL DOC
                oFinalWordDoc.Close(ref oFalse, ref oMissing, ref oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oFinalWordDoc);
                oFinalWordDoc = null;

                //now delete the temp file
                File.Delete(oOutputPathTemp.ToString());

                Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() Merge complete, printing document");

                return true;
            }
            catch (System.Exception ex)
            {
                errorString = ex.Message;
                Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() Error: " + Environment.NewLine + Environment.NewLine + ex.Message);
            }
            finally
            {
                //RELEASE WORD ITSELF
                oWord.Quit(ref oMissing, ref oMissing, ref oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oWord);
                oWord = null;

                GC.Collect();
            }

            return false;
        }

        public static bool MergeTable(int tableNbr, bool startAtRow2, string sourcePath, string destPath, string[,] tblInfo , out string errorString)
        {

            // Tables:
            // http://msdn.microsoft.com/en-us/library/office/aa192483%28v=office.11%29.aspx


            #region Declares
            errorString = string.Empty;
            Object oMissing = System.Reflection.Missing.Value; //null value
            Object oTrue = true;
            Object oFalse = false;
            Object oPageBreak = Word.WdBreakType.wdPageBreak;
            Object oSourcePath = sourcePath;
            Object oOutputPath = destPath;
            int posExtDot = oOutputPath.ToString().LastIndexOf('.');
            Object oOutputPathTemp = oOutputPath.ToString().Substring(0, posExtDot) + "_temp" + oOutputPath.ToString().Substring(posExtDot);

            Object sectionStart = (Object)Microsoft.Office.Interop.Word.WdSectionStart.wdSectionNewPage;

            Word.Application oWord = null;
            Word.Document oWordDoc = null; //the document to load into word application
            Word.Document oFinalWordDoc = null; //the document to load into word application
            #endregion

            try
            {
                if (tableNbr < 1)
                    throw new Exception("tableNbr must be at least 1");

                Logger.LogMailMerge(false, "");
                Logger.LogMailMerge(false, "MailMergeWebApp:MergeInvoice() merging document: " + oSourcePath);

                oWord = new Word.Application(); //starts an instance of word
                oWord.Visible = false; //don't show the UI

                //create an empty document that we will insert all the merge docs into
                oFinalWordDoc = oWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //insert a document for each record
                oWordDoc = oWord.Documents.Add(ref oSourcePath, ref oMissing, ref oMissing, ref oMissing);

                //make current
                oWordDoc.Activate();




                Logger.LogMailMerge(false, "MailMergeWebApp:MergeInvoice() adding to table: " + oSourcePath);

                // Fill the table with data.
                Word.Table tbl = oWordDoc.Tables[tableNbr];   // starts at "1"
                Object beforeRow = Type.Missing;

                for (int row = 0; row < tblInfo.GetLength(0); row++)
                {
                    tbl.Rows.Add(ref beforeRow);
                    for (int col = 0; col < tblInfo.GetLength(1); col++)
                    {
                        tbl.Cell(row+1+ (startAtRow2 ? 1 : 0), col + 1).Range.Text = tblInfo[row, col];  // first cell is (1,1), not (0,0) -- if start on 2nd row after headings, use (2,1)
                        tbl.Cell(row+1+ (startAtRow2 ? 1 : 0), col + 1).Range.Font.Bold = 0;
                        tbl.Cell(row+1+ (startAtRow2 ? 1 : 0), col + 1).Range.Font.Underline = Word.WdUnderline.wdUnderlineNone;
                    }
                }








                Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() saving the doc");

                //SAVE THE DOCUMENT as temp
                oWordDoc.SaveAs(ref oOutputPathTemp, ref oMissing, ref oMissing, ref oMissing
                    , ref oMissing, ref oMissing, ref oMissing, ref oMissing
                    , ref oMissing, ref oMissing, ref oMissing, ref oMissing
                    , ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() closing the doc");

                //CLOSE THE DOCUMENT
                oWordDoc.Close(ref oFalse, ref oMissing, ref oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oWordDoc);
                oWordDoc = null;

                //NOW ADD THE NEW DOC TO THE MAIN DOC
                oFinalWordDoc.Activate(); //make current
                oWord.Selection.InsertFile(oOutputPathTemp.ToString(), ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //SAVE THE FINAL DOC
                oFinalWordDoc.SaveAs(ref oOutputPath, ref oMissing, ref oMissing, ref oMissing
                        , ref oMissing, ref oMissing, ref oMissing, ref oMissing
                        , ref oMissing, ref oMissing, ref oMissing, ref oMissing
                        , ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //CLOSE THE FINAL DOC
                oFinalWordDoc.Close(ref oFalse, ref oMissing, ref oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oFinalWordDoc);
                oFinalWordDoc = null;

                //now delete the temp file
                File.Delete(oOutputPathTemp.ToString());

                Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() Merge complete, printing document");

                return true;
            }
            catch (System.Exception ex)
            {
                errorString = ex.Message;
                Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() Error: " + Environment.NewLine + Environment.NewLine + ex.Message);
            }
            finally
            {
                //RELEASE WORD ITSELF
                oWord.Quit(ref oMissing, ref oMissing, ref oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oWord);
                oWord = null;

                GC.Collect();
            }

            return false;
        }
    
    }


    public class MultiDocumentMerger
    {
        public static bool Merge(string[] filesToMerge, string outputFilename, bool insertPageBreaks, out string errorString)
        {
            return Merge(filesToMerge, outputFilename, insertPageBreaks, null, out errorString);
        }

        public static bool Merge(string[] filesToMerge, string outputFilename, bool insertPageBreaks, string documentTemplate, out string errorString)
        {
            errorString = string.Empty;
            object defaultTemplate = documentTemplate;
            object missing = System.Type.Missing;
            object oTrue = true;
            object oFalse = false;
            //object pageBreak = Word.WdBreakType.wdPageBreak;
            object pageBreak = Word.WdBreakType.wdSectionBreakNextPage;
            object outputFile = outputFilename;


            if (filesToMerge.Length == 0)
                return true;


            for (int i = 0; i < filesToMerge.Length - 1; i++)
            {
                if (Path.GetExtension(filesToMerge[i]).ToUpper() == ".PDF")
                {
                    errorString = "MultiDocumentMerger.Merge() should be given word docs to merge, but pdf file sent in.";
                    return false;
                }
            }


            if (filesToMerge.Length == 1)
            {
                if (Path.GetExtension(outputFilename).ToUpper() == ".PDF")
                {
                    string _errorString = null;
                    FormatConverter.WordToPDF(filesToMerge[0], outputFilename, out _errorString);
                    if (_errorString != string.Empty)
                    {
                        errorString = _errorString;
                        return false;
                    }
                }
                else
                {
                    System.IO.File.Copy(filesToMerge[0], outputFilename);
                }

                return true;
            }



            // Create  a new Word application
            Word._Application wordApplication = new Word.Application();

            try
            {
                // Create a new file based on our template
                object template = (documentTemplate == null) ? missing : defaultTemplate;

                // this doesnt keep the header info and the formatting (and background images) are all screwed up
                //Word._Document wordDocument = wordApplication.Documents.Add(ref template, ref missing, ref missing, ref missing);
                
                // for some reason this is placed at the end of the documents, so put the last doc in, then below add first up to 2nd last doc
                object lastFile = filesToMerge[filesToMerge.Length-1];
                Word._Document wordDocument = wordApplication.Documents.Add(ref lastFile, ref missing, ref missing, ref missing);

                // Make a Word selection object.
                Word.Selection selection = wordApplication.Selection;
                
                // Loop thru each of the Word documents
                for (int i = 0; i < filesToMerge.Length-1; i++)
                {
                    // Insert the files to our template
                    selection.InsertFile(
                                              filesToMerge[i]
                                            , ref missing
                                            , ref missing
                                            , ref missing
                                            , ref missing);

                    //Do we want page breaks added after each documents?
                    if (i < (filesToMerge.Length - 1) && insertPageBreaks)
                        selection.InsertBreak(ref pageBreak);
                }

                // Save the document to it's output file.
                object fileFormat = System.IO.Path.GetExtension(outputFile.ToString()).ToUpper() == ".PDF" ? Word.WdSaveFormat.wdFormatPDF : missing;
                wordDocument.SaveAs(
                              ref outputFile
                            , ref fileFormat
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing
                            , ref missing);

                // Clean up!
                wordDocument.Close(ref oFalse, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDocument);
                wordDocument = null;


                return true;
            }
            catch (System.Exception ex)
            {
                errorString = ex.Message;
            }
            finally
            {
                //RELEASE WORD ITSELF
                wordApplication.Quit(ref missing, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApplication);
                wordApplication = null;

                GC.Collect();
            }

            return false;


        }
    }

    public class FormatConverter
    {

        public static bool WordToPDF(string inFileName, string outFileName, out string errorString)
        {
            #region Declares
            errorString = string.Empty;
            Object oMissing = System.Reflection.Missing.Value; //null value
            Object oTrue = true;
            Object oFalse = false;
            Object oPageBreak = Word.WdBreakType.wdPageBreak;
            Object oOutputPath = outFileName;
            Object oSourcePath = inFileName;
            Object sectionStart = (Object)Microsoft.Office.Interop.Word.WdSectionStart.wdSectionNewPage;

            Word.Application oWord = null;
            #endregion


            try
            {
                oWord = new Word.Application(); //starts an instance of word
                oWord.Visible = false; //don't show the UI

                // Use the dummy value as a placeholder for optional arguments
                Document doc = oWord.Documents.Open(ref oSourcePath, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                doc.Activate();

                object fileFormat = WdSaveFormat.wdFormatPDF;

                // Save document into PDF Format
                doc.SaveAs(
                         ref oOutputPath,
                         ref fileFormat,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing,
                         ref oMissing);


                // Close the Word document, but leave the Word application open.
                // doc has to be cast to type _Document so that it will find the
                // correct Close method.                
                object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
                ((_Document)doc).Close(ref saveChanges, ref oMissing, ref oMissing);
                doc = null;



                return true;
            }
            catch (System.Exception ex)
            {
                errorString = ex.Message;
                Logger.LogMailMerge(false, "MailMergeWebApp:MailMerge() Error: " + Environment.NewLine + Environment.NewLine + ex.Message);
            }
            finally
            {
                //RELEASE WORD ITSELF
                oWord.Quit(ref oMissing, ref oMissing, ref oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oWord);
                oWord = null;

                GC.Collect();
            }

            return false;
        }

    }

}
