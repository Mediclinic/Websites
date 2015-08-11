using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Office;
using Word = Microsoft.Office.Interop.Word;  // Add ref - COM tab - "Microsoft Office 14.0 Object Library"
using Microsoft.Office.Interop.Word;

using System.Data;
using System.Collections;

public class WordMailMerger
{

    public static bool Merge(string sourceTemplatePath, string outputDocPath, DataSet sourceData, string[,] tblInfo, int tableNbr, bool startAtRow2, bool isDoubleSidedPrinting, string[] extraPages, bool addExtraPagesAtEnd, string invlinkAtEndOfDoc, out string errorString)
    {
        errorString = "";
        object oMissing = System.Reflection.Missing.Value;
        object oFalse = false;
        object oTrue = true;
        //object oPageBreak = Word.WdBreakType.wdPageBreak;
        object oPageBreak = Word.WdBreakType.wdSectionBreakNextPage;
        object oSourceTemplatePath = sourceTemplatePath;
        object oOutputDocPath = outputDocPath;
        object sectionStart = (object)Microsoft.Office.Interop.Word.WdSectionStart.wdSectionNewPage;


        ApplicationClass myWordApp = null;    // our application
        Document         myWordDoc = null;    // our document

        try
        {

            myWordApp = new ApplicationClass();
            myWordDoc = new Document();

            myWordApp.Visible = false;  // tell word not to show itself


            if (oSourceTemplatePath != null)
            {
                // load the template and check how many fields there are to replace
                myWordDoc = myWordApp.Documents.Add(            // load the template into a document workspace
                                      ref oSourceTemplatePath,  // and reference it through our myWordDoc
                                      ref oMissing,
                                      ref oMissing,
                                      ref oMissing);
            }
            else
            {
                myWordDoc = myWordApp.Documents.Add(             // add new document
                                      ref oMissing,
                                      ref oMissing,
                                      ref oMissing,
                                      ref oMissing);
            }


            if (sourceData != null)
            {
                // iterate through the fields collection and update
                foreach (Field myMergeField in myWordDoc.Fields)
                {

                    Word.Range rngFieldCode = myMergeField.Code;
                    String fieldText = rngFieldCode.Text;

                    // only get the mailmerge fields
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
                        Log("Found word template field: " + fieldName);

                        //find a matching dataset column
                        foreach (DataColumn col in sourceData.Tables[0].Columns)
                        {
                            string key = col.ColumnName;
                            string value = sourceData.Tables[0].Rows[0][key].ToString();

                            // **** FIELD REPLACEMENT IMPLEMENTATION GOES HERE ****//
                            // THE PROGRAMMER CAN HAVE HIS OWN IMPLEMENTATIONS HERE
                            if (fieldName == key)
                            {
                                Log("Setting value: " + value);

                                myMergeField.Select();
                                myWordApp.Selection.TypeText(value);
                            }
                        }
                    }
                }
            }



            if (tblInfo != null)
            {

                Log("MailMergeWebApp:MergeInvoice() adding to table: " + sourceTemplatePath);

                // Fill the table with data.
                Word.Table tbl = myWordDoc.Tables[tableNbr];   // starts at "1"
                Object beforeRow = Type.Missing;

                for (int row = 0; row < tblInfo.GetLength(0); row++)
                {
                    tbl.Rows.Add(ref beforeRow);
                    for (int col = 0; col < tblInfo.GetLength(1); col++)
                    {
                        bool isAlignRight   = false;
                        bool isBold         = false;
                        bool isUnderline    = false;
                        bool invpaymentlink = false;
                        if (tblInfo[row, col].StartsWith("<align=right>") && tblInfo[row, col].EndsWith("</align>"))
                        {
                            tblInfo[row, col] = tblInfo[row, col].Substring(13, tblInfo[row, col].Length - 21);
                            isAlignRight = true;
                        }
                        if (tblInfo[row, col].StartsWith("<b>") && tblInfo[row, col].EndsWith("</b>"))
                        {
                            tblInfo[row, col] = tblInfo[row, col].Substring(3, tblInfo[row, col].Length-7);
                            isBold = true;
                        }
                        if (tblInfo[row, col].StartsWith("<u>") && tblInfo[row, col].EndsWith("</u>"))
                        {
                            tblInfo[row, col] = tblInfo[row, col].Substring(3, tblInfo[row, col].Length - 7);
                            isUnderline = true;
                        }
                        if (tblInfo[row, col].StartsWith("<invpaymentlink>") && tblInfo[row, col].EndsWith("</invpaymentlink>"))
                        {
                            tblInfo[row, col] = tblInfo[row, col].Substring(16, tblInfo[row, col].Length - 33);
                            invpaymentlink = true;
                        }


                        if (tbl.Rows.Count >= row + 1 + (startAtRow2 ? 1 : 0) && tbl.Columns.Count >= col + 1)
                        {
                            if (invpaymentlink)
                            {
                                //Word.Selection wrdSelection = myWordApp.Selection;
                                //string StrToAdd = "If you would like to pay online, please ";
                                //wrdSelection.TypeText(StrToAdd);

                                Word.Range range = tbl.Cell(row + 1 + (startAtRow2 ? 1 : 0), col + 1).Range;
                                Word.Hyperlink hl = myWordDoc.Hyperlinks.Add(range, tblInfo[row, col], ref oMissing, ref oMissing, "Web Pay", ref oMissing);
                            }
                            else
                            {
                                tbl.Cell(row + 1 + (startAtRow2 ? 1 : 0), col + 1).Range.Text = tblInfo[row, col];  // first cell is (1,1), not (0,0) -- if start on 2nd row after headings, use (2,1)
                                tbl.Cell(row + 1 + (startAtRow2 ? 1 : 0), col + 1).Range.Font.Bold = isBold ? 1 : 0;
                                tbl.Cell(row + 1 + (startAtRow2 ? 1 : 0), col + 1).Range.Font.Underline = isUnderline ? Word.WdUnderline.wdUnderlineWords : Word.WdUnderline.wdUnderlineNone;
                                tbl.Cell(row + 1 + (startAtRow2 ? 1 : 0), col + 1).Range.ParagraphFormat.Alignment = isAlignRight ? WdParagraphAlignment.wdAlignParagraphRight : WdParagraphAlignment.wdAlignParagraphLeft;

                            }
                        }

                    }
                }


            }


            if (invlinkAtEndOfDoc != null)
            {
                foreach (Field myMergeField in myWordDoc.Fields)
                {
                    Word.Range rngFieldCode = myMergeField.Code;
                    String fieldText = rngFieldCode.Text;

                    // only get the mailmerge fields
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
                        Log("Found word template field: " + fieldName);

                        if (fieldName == "inv_pay_online_link")
                        {
                            myMergeField.Select();

                            //invlinkAtEndOfDoc = invlinkAtEndOfDoc.Replace("https://portal.mediclinic.com.au", "http://localhost:88");

                            Word.Selection wrdSelection = myWordApp.Selection;
                            string StrToAdd = "If you would like to pay online, please ";
                            wrdSelection.TypeText(StrToAdd);

                            Word.Range range = myWordApp.Selection.Range;
                            Word.Hyperlink hl = myWordDoc.Hyperlinks.Add(range, invlinkAtEndOfDoc, ref oMissing, ref oMissing, "Click Here", ref oMissing);

                        }

                    }
                }



                if (false)
                {
                    /*
                    Word.Selection wrdSelection = myWordApp.Selection;;

                    // Go to the end of the document.
                    Object oConst1 = Word.WdGoToItem.wdGoToLine;
                    Object oConst2 = Word.WdGoToDirection.wdGoToLast;
                    //myWordApp.Selection.GoTo(ref oConst1, ref oConst2, ref oMissing, ref oMissing);

                    myWordApp.Selection.TypeParagraph();
                    myWordApp.Selection.TypeParagraph();

                    // Create a string and insert it into the document.
                    string StrToAdd = "If you would like to pay online, please follow this link: ";
                    wrdSelection.TypeText(StrToAdd);

                    // Insert a hyperlink to the Web page.
                    Object oAddress = invlinkAtEndOfDoc;
                    Object oRange = wrdSelection.Range;
                    wrdSelection.Hyperlinks.Add(oRange, ref oAddress, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                    string newText = "......";
                    wrdSelection.TypeText(newText);
                    */

                    // Go to the end of the document.
                    // oConst1 = Word.WdGoToItem.wdGoToLine;
                    // oConst2 = Word.WdGoToDirection.wdGoToLast;
                    //myWordApp.Selection.GoTo(ref oConst1, ref oConst2, ref oMissing, ref oMissing);
                    //myWordDoc.GoTo(ref oConst1, ref oConst2, ref oMissing, ref oMissing);



                    // now make start to point to the end of the content of the first document
                    //Object end = myWordApp.ActiveDocument.Content.End - 1; 
                    // create another range object with the new value for start
                    //Range range = myWordDoc.Range(ref end, ref oMissing);



                    //invlinkAtEndOfDoc = invlinkAtEndOfDoc.Replace("https://portal.mediclinic.com.au", "http://localhost:88");

                    //Word.Selection wrdSelection = myWordApp.Selection;
                    //string StrToAdd = "If you would like to pay online, please ";
                    //wrdSelection.TypeText(StrToAdd);

                    //Word.Range range = myWordApp.Selection.Range;
                    //Word.Hyperlink hl = myWordDoc.Hyperlinks.Add(range, invlinkAtEndOfDoc, ref oMissing, ref oMissing, "Click Here", ref oMissing);

                }

            }


            if (oSourceTemplatePath != null && addExtraPagesAtEnd && isDoubleSidedPrinting) // make sure so far, even nbr of pages
            {
                int nPages = myWordDoc.ComputeStatistics(Word.WdStatistic.wdStatisticPages, ref oMissing);
                if (nPages % 2 == 1)
                {
                    Word.Paragraph paragraph = myWordDoc.Content.Paragraphs.Add(ref oMissing);
                    paragraph.Range.InsertParagraphBefore();
                    paragraph.Range.Text = string.Empty;
                    paragraph.Range.InsertBreak(ref oPageBreak);
                    paragraph.Range.InsertParagraphAfter();
                }
            }


            Log("Adding pages");

            if (extraPages != null)
            {
                for(int i=0; i<extraPages.Length; i++)
                {
                    string text = Environment.NewLine + Environment.NewLine +
                                  Environment.NewLine + Environment.NewLine +
                                  Environment.NewLine + Environment.NewLine + extraPages[i];

                    Word.Paragraph paragraph;

                    //if (oSourceTemplatePath != null || i > 0)  // if not blank doc OR not first extra page -- add page break first
                    //{
                    //    paragraph = myWordDoc.Content.Paragraphs.Add(ref oMissing);
                    //    paragraph.Range.InsertParagraphBefore();
                    //    paragraph.Range.Text = string.Empty;
                    //    paragraph.Range.InsertBreak(ref oPageBreak);
                    //    paragraph.Range.InsertParagraphAfter();
                    //}

                    //paragraph = myWordDoc.Content.Paragraphs.Add(ref oMissing);
                    //paragraph.Range.InsertParagraphBefore();
                    //paragraph.Range.Text = text;
                    //paragraph.Range.InsertParagraphAfter();

                    paragraph = myWordDoc.Content.Paragraphs.Add(ref oMissing);
                    if ((oSourceTemplatePath != null && addExtraPagesAtEnd) || i > 0)  // if not blank doc OR not first extra page -- add page break first
                        paragraph.Range.InsertBreak(ref oPageBreak);
                    paragraph.Range.InsertBefore(text);
                }
            }


            // but put it at the end of all notes
            if (isDoubleSidedPrinting) // make sure so far, even nbr of pages
            {
                int nPages = myWordDoc.ComputeStatistics(Word.WdStatistic.wdStatisticPages, ref oMissing);
                if (nPages % 2 == 1)
                {
                    Word.Paragraph paragraph = myWordDoc.Content.Paragraphs.Add(ref oMissing);
                    paragraph.Range.InsertParagraphBefore();
                    paragraph.Range.Text = string.Empty;
                    paragraph.Range.InsertBreak(ref oPageBreak);
                    paragraph.Range.InsertParagraphAfter();
                }
            }


            object fileFormat = System.IO.Path.GetExtension(oOutputDocPath.ToString()).ToUpper() == ".PDF" ? Word.WdSaveFormat.wdFormatPDF : oMissing;
            myWordDoc.SaveAs(
                     ref oOutputDocPath,
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



            //CLOSE THE FINAL DOC
            myWordDoc.Close(ref oFalse, ref oMissing, ref oMissing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(myWordDoc);
            myWordDoc = null;


            Log("Merge complete");

            return true;
        }
        catch (System.Exception ex)
        {
            errorString = ex.ToString(); //.Message;
        }
        finally
        {
            //RELEASE WORD ITSELF
            try{ myWordApp.Quit(ref oFalse, ref oMissing, ref oMissing);             } catch (Exception) { }
            try{ System.Runtime.InteropServices.Marshal.ReleaseComObject(myWordApp); } catch (Exception) { }
            try{ myWordApp = null;                                                   } catch (Exception) { }
            GC.Collect();
        }

        return false;
    }


    protected static void Log(string text)
    {
        Logger.LogMailMerge(false, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ": " + text);
    }

}