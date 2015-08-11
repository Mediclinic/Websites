using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

public partial class PatientScannedFileUploadsV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);
            lblUploadMessage.Text = string.Empty;


            if (!IsPostBack)
            {
                Utilities.SetNoCache(Response);

                Session.Remove("letter_sortexpression");
                Session.Remove("letter_data");

                FormParamType formParamType = GetFormParamType();
                if (formParamType == FormParamType.Patient)
                {
                    Patient    patient             = GetFormPatient();
                    IDandDescr medicalSserviceType = GetFormMedicalSserviceType();

                    if (patient == null)
                    {
                        spn_manage_files.Visible = false;
                        throw new CustomMessageException("Invalid patient ID");
                    }
                    lblHeading.Text = "Scanned Files For PT: " + patient.Person.FullnameWithoutMiddlename;

                    bool refresh_on_close = Request.QueryString["refresh_on_close"] != null && Request.QueryString["refresh_on_close"] == "1";
                    if (refresh_on_close)
                    {
                        btnClose.OnClientClick = "window.opener.location.href = window.opener.location.href;self.close();";

                        // make sure if user clicks "x" to close the window, this value is passed on so the other page gets this value passed on too
                        if (refresh_on_close) // refresh parent when parent opened this as tab
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "on_close_window", "<script language=javascript>window.onbeforeunload = function(){ window.opener.location.href = window.opener.location.href; }</script>");
                    }


                    string scannedDocsDir = patient.GetScannedDocsDirectory();
                    if (scannedDocsDir.EndsWith("\\")) scannedDocsDir = scannedDocsDir.Substring(0, scannedDocsDir.Length-1);
                    this.HomeFolder = scannedDocsDir;

                    if (!Directory.Exists(scannedDocsDir))
                        CreateDirectory(scannedDocsDir);

                    if (string.IsNullOrEmpty(this.HomeFolder) || !Directory.Exists(GetFullyQualifiedFolderPath(this.HomeFolder)))
                        throw new ArgumentException(String.Format("The HomeFolder setting '{0}' is not set or is invalid", this.HomeFolder));

                    this.CurrentFolder = medicalSserviceType == null ? this.HomeFolder : this.HomeFolder + "\\" + "__" + medicalSserviceType.ID + "__";

                    EnsureGPFoldersExist(patient, scannedDocsDir);
                    FillScannedDocGrid();
                }
                else if (formParamType == FormParamType.Booking)
                {
                    Booking booking = GetFormBooking();

                    if (booking == null)
                    {
                        spn_manage_files.Visible = false;
                        throw new CustomMessageException("Invalid booking ID");
                    }
                    page_title.Style["margin"]      = "6px 0 10px 0 !important";
                    page_title.Style["line-height"] = "15px !important";
                    lblHeading.Text = "<span style=\"font-size:70%;\">Scanned Files For " + (booking.Patient == null ? "Booking" : booking.Patient.Person.FullnameWithoutMiddlename) + "<br />" + booking.DateStart.ToString(@"dd MMM yyyy H:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + "<br />" + booking.Organisation.Name + "</span>";

                    string scannedDocsDir = booking.GetScannedDocsDirectory();
                    if (scannedDocsDir.EndsWith("\\")) scannedDocsDir = scannedDocsDir.Substring(0, scannedDocsDir.Length-1);
                    this.HomeFolder = scannedDocsDir;

                    if (!Directory.Exists(scannedDocsDir))
                        CreateDirectory(scannedDocsDir);

                    if (string.IsNullOrEmpty(this.HomeFolder) || !Directory.Exists(GetFullyQualifiedFolderPath(this.HomeFolder)))
                        throw new ArgumentException(String.Format("The HomeFolder setting '{0}' is not set or is invalid", this.HomeFolder));

                    this.CurrentFolder = this.HomeFolder;

                    FillScannedDocGrid();
                }
                else // formParamType == FormParamType.None
                {
                    throw new CustomMessageException("Invalid URL - no PT ID or BK ID");
                }

            }

        }
        catch (CustomMessageException ex)
        {
            if (IsPostBack) SetErrorMessage(ex.Message);
            else HideTableAndSetErrorMessage(ex.Message);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            if (IsPostBack) SetErrorMessage("Connection to network files currently unavailable.");
            else HideTableAndSetErrorMessage("Connection to network files currently unavailable.");

        }
        catch (Exception ex)
        {
            if (IsPostBack) SetErrorMessage("", ex.ToString());
            else HideTableAndSetErrorMessage("", ex.ToString());
        }
    }

    protected void EnsureGPFoldersExist(Patient patient, string scannedDocsDir)
    {
        if (patient.IsGPPatient)
        {
            DataTable dt = DBBase.GetGenericDataTable(null, "MedicalServiceType", "medical_service_type_id", "descr");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int    medicalServiceTypeID = Convert.ToInt32(dt.Rows[i]["medical_service_type_id"]);
                string descr                = Convert.ToString(dt.Rows[i]["descr"]);

                string folderName = "__" + medicalServiceTypeID + "__";
                string folderPath = scannedDocsDir + "\\" + folderName;

                if (!Directory.Exists(folderPath))
                    CreateDirectory(folderPath);
            }
        }
    }

    #endregion

    #region GetUrlParams

    private enum FormParamType { Patient, Booking, None };
    private FormParamType GetFormParamType()
    {
        if (Request.QueryString["patient"] != null)
            return FormParamType.Patient;
        else if (Request.QueryString["booking"] != null)
            return FormParamType.Booking;
        else
            return FormParamType.None;
    }

    private Patient GetFormPatient()
    {
        try
        {
            string patientID = Request.QueryString["patient"];
            if (patientID == null || !Regex.IsMatch(patientID, @"^\d+$"))
                throw new CustomMessageException("Invalid patient id");
            return PatientDB.GetByID(Convert.ToInt32(Request.QueryString["patient"]));
        }
        catch (CustomMessageException ex)
        {
            HideTableAndSetErrorMessage(Utilities.IsDev() ? ex.Message : "");
            return null;
        }
    }

    private Booking GetFormBooking()
    {
        try
        {
            string bookingID = Request.QueryString["booking"];
            if (bookingID == null || !Regex.IsMatch(bookingID, @"^\d+$"))
                throw new CustomMessageException("Invalid booking id");
            return BookingDB.GetByID(Convert.ToInt32(Request.QueryString["booking"]));
        }
        catch (CustomMessageException ex)
        {
            HideTableAndSetErrorMessage(Utilities.IsDev() ? ex.Message : "");
            return null;
        }
    }

    private IDandDescr GetFormMedicalSserviceType()
    {
        string mst = Request.QueryString["mst"];
        if (mst == null)
            return null;
        if (!Regex.IsMatch(mst, @"^\d+$"))
            throw new CustomMessageException("Invalid mst");

        DataTable dt = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "MedicalServiceType", "medical_service_type_id=" + mst, "", "medical_service_type_id", "descr");
        if (dt.Rows.Count == 0)
            throw new CustomMessageException("Invalid mst");

        return IDandDescrDB.Load(dt.Rows[0], "medical_service_type_id", "descr");
    }

    #endregion


    // http://www.4guysfromrolla.com/articles/090110-1.aspx

    #region GrdScannedDoc

    protected void FillScannedDocGrid()
    {
        try
        {
            // Get the list of files & folders in the CurrentFolder
            DirectoryInfo   currentDirInfo = new DirectoryInfo(GetFullyQualifiedFolderPath(this.CurrentFolder));
            bool            currentDirIsHomeFolder = TwoFoldersAreEquivalent(currentDirInfo.FullName, GetFullyQualifiedFolderPath(this.HomeFolder));

            if (!currentDirInfo.Exists) // in case the folder has been deleted in another page
                throw new CustomMessageException("Could not find directory: " + currentDirInfo.Name);

            DirectoryInfo[] folders        = currentDirInfo.GetDirectories();
            FileInfo[]      files          = currentDirInfo.GetFiles();


            List<FileSystemItemCS> fsItems = new List<FileSystemItemCS>(folders.Length + files.Length);

            // Add the ".." option, if needed
            if (!currentDirIsHomeFolder)
            {
                FileSystemItemCS parentFolder = new FileSystemItemCS(currentDirInfo.Parent);
                parentFolder.Name = "..";
                fsItems.Add(parentFolder);
            }

            Hashtable medicalServiceTypeHash = MedicalServiceTypeDB.GetMedicareServiceTypeHash();
            foreach (DirectoryInfo folder in folders)
            {
                FileSystemItemCS fileSystemItemCS = new FileSystemItemCS(folder);
                if (currentDirIsHomeFolder)
                    fileSystemItemCS.Name = MedicalServiceType.ReplaceMedicareServiceTypeFolders(fileSystemItemCS.Name, medicalServiceTypeHash, true);
                fsItems.Add(fileSystemItemCS);
            }

            foreach (FileInfo file in files)
                fsItems.Add(new FileSystemItemCS(file));


            //GrdScannedDoc.DataSource = fsItems;
            //GrdScannedDoc.DataBind();

            if (fsItems.Count > 0)
            {
                GrdScannedDoc.DataSource = fsItems;
                GrdScannedDoc.DataBind();
            }
            else
            {
                fsItems.Add(new FileSystemItemCS());
                GrdScannedDoc.DataSource = fsItems;
                GrdScannedDoc.DataBind();

                int TotalColumns = GrdScannedDoc.Rows[0].Cells.Count;
                GrdScannedDoc.Rows[0].Cells.Clear();
                GrdScannedDoc.Rows[0].Cells.Add(new TableCell());
                GrdScannedDoc.Rows[0].Cells[0].ColumnSpan = TotalColumns;
                GrdScannedDoc.Rows[0].Cells[0].Text = "No Files Or Folders Found";
            }



            string currentFolderDisplay = this.CurrentFolder;
            if (currentFolderDisplay.StartsWith("~/") || currentFolderDisplay.StartsWith("~\\"))
                currentFolderDisplay = currentFolderDisplay.Substring(2);
            if (currentFolderDisplay.StartsWith("~/") || currentFolderDisplay.StartsWith("~\\"))
                currentFolderDisplay = currentFolderDisplay.Substring(2);

            currentFolderDisplay = currentFolderDisplay.Substring(this.HomeFolder.Length);
            if (currentFolderDisplay.StartsWith("\\")) currentFolderDisplay = currentFolderDisplay.Substring(1);
            if (currentFolderDisplay == "") currentFolderDisplay = "[Root Directory]";

            currentFolderDisplay = MedicalServiceType.ReplaceMedicareServiceTypeFolders(currentFolderDisplay, medicalServiceTypeHash, true);

            lblCurrentPath.Text = "Viewing Folder: &nbsp;&nbsp; <b><font color=\"blue\">" + currentFolderDisplay + "</font></b>";
        }
        catch(CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch(Exception ex)
        {
            SetErrorMessage("", ex.ToString());
        }
    }
    protected void GrdScannedDoc_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {
        }

        UserView userview = UserView.GetInstance();
        if (!userview.IsMasterAdmin)
            e.Row.Cells[4].CssClass = "hiddencol";

    }
    protected void GrdScannedDoc_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FileSystemItemCS item = e.Row.DataItem as FileSystemItemCS;

            if (item.IsFolder)
            {
                LinkButton lbFolderItem = e.Row.FindControl("lbFolderItem") as LinkButton;
                lbFolderItem.Text = string.Format(@"<img src=""{0}"" alt="""" />&nbsp;{1}", Page.ResolveClientUrl("~/images/folder.png"), item.Name);
            }
            else
            {
                Literal ltlFileItem = e.Row.FindControl("ltlFileItem") as Literal;
                if (this.CurrentFolder.StartsWith("~"))
                    ltlFileItem.Text = string.Format(@"<a href=""{0}"" target=""_blank"">{1}</a>",
                            Page.ResolveClientUrl(string.Concat(this.CurrentFolder, "/", item.Name).Replace("//", "/")),
                            item.Name);
                else
                    ltlFileItem.Text = item.Name;
            }


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                btnDelete.OnClientClick =
                    item.IsFolder ?
                    "javascript:if (!confirm('Are you sure you want to PERMANENTLY delete this folder and all of it\\'s contents?')) return false;" :
                    "javascript:if (!confirm('Are you sure you want to PERMANENTLY delete this file?')) return false;";
            }

        }
    }
    protected void GrdScannedDoc_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdScannedDoc.EditIndex = -1;
        FillScannedDocGrid();
    }
    protected void GrdScannedDoc_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        /*
        Label   lblId     = (Label)GrdScannedDoc.Rows[e.RowIndex].FindControl("lblId");
        TextBox txtFolder = (TextBox)GrdScannedDoc.Rows[e.RowIndex].FindControl("txtFolder");


        DataTable dt = Session["scanned_doc_data"] as DataTable;
        DataRow[] foundRows = dt.Select("scanned_doc_id=" + lblId.Text);
        ScannedDoc scannedDoc = ScannedDocDB.LoadAll(foundRows[0]);

        ScannedDocDB.Update(
            scannedDoc.ScannedDocID, 
            scannedDoc.Patient.PatientID, 
            scannedDoc.MedicalServiceType == null ? -1 : scannedDoc.MedicalServiceType.ID, 
            txtFolder.Text.ToUpper(),
            scannedDoc.Filename.Trim());

        GrdScannedDoc.EditIndex = -1;
        FillScannedDocGrid();
        */
    }
    protected void GrdScannedDoc_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdScannedDoc_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "OpenFolder")
        {
            if (string.CompareOrdinal(e.CommandArgument.ToString(), "..") == 0)
            {
                string currentFullPath = this.CurrentFolder;
                if (currentFullPath.EndsWith("\\") || currentFullPath.EndsWith("/"))
                    currentFullPath = currentFullPath.Substring(0, currentFullPath.Length - 1);

                currentFullPath = currentFullPath.Replace("/", "\\");

                string[] folders = currentFullPath.Split("\\".ToCharArray());

                this.CurrentFolder = string.Join("\\", folders, 0, folders.Length - 1);
            }
            else
                this.CurrentFolder = (string)e.CommandArgument;

            FillScannedDocGrid();
        }
    }
    protected void GrdScannedDoc_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdScannedDoc.EditIndex = e.NewEditIndex;
        FillScannedDocGrid();
    }
    protected void GrdScannedDoc_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdScannedDoc.EditIndex >= 0)
            return;

        DataTable dataTable = Session["scanned_doc_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["scanned_doc_sortexpression"] == null)
                Session["scanned_doc_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["scanned_doc_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["scanned_doc_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdScannedDoc.DataSource = dataView;
            GrdScannedDoc.DataBind();
        }
    }

    #endregion

    #region Properties

    public string HomeFolder
    {
        get
        {
            return ViewState["HomeFolder"] as string;
        }
        set
        {
            ViewState["HomeFolder"] = value;
        }
    }

    public string CurrentFolder
    {
        get { return ViewState["CurrentFolder"] as string; }
        set { ViewState["CurrentFolder"] = value; }
    }

   
    #endregion

    #region DisplaySize, GetFullyQualifiedFolderPath, TwoFoldersAreEquivalent

    protected string DisplaySize(long? size)
    {
        if (size == null)
            return string.Empty;
        else
        {
            if (size < 1024)
                return string.Format("{0:N0} bytes", size.Value);
            else if (size < 1048576)
                return String.Format("{0:N0} KB", size.Value / 1024);
            else
                return String.Format("{0:0.#} MB", (double)size.Value / 1048576);
        }
    }    

    private string GetFullyQualifiedFolderPath(string folderPath)
    {
        if (folderPath.StartsWith("~"))
            return Server.MapPath(folderPath);
        else
            return folderPath;
    }

    private bool TwoFoldersAreEquivalent(string folderPath1, string folderPath2)
    {
        // Chop off any trailing slashes...
        if (folderPath1.EndsWith("\\") || folderPath1.EndsWith("/"))
            folderPath1 = folderPath1.Substring(0, folderPath1.Length - 1);

        if (folderPath2.EndsWith("\\") || folderPath2.EndsWith("/"))
            folderPath2 = folderPath1.Substring(0, folderPath2.Length - 1);

        return string.CompareOrdinal(folderPath1, folderPath2) == 0;
    }

    #endregion



    #region btnUpload_Click, btnDownload_Click, btnDeleteFie_Click

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            SaveFiles(this.CurrentFolder, chkAllowOverwrite.Checked);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            SetErrorMessage("Connection to network files currently unavailable.");
            return;
        }
    }

    protected void btnDownload_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton lnkbtn = (LinkButton)sender;

            string fileToDownload = lnkbtn.CommandArgument;
            FileInfo fi = new FileInfo(fileToDownload);

            string fileNameToDownload = fi.Name.Substring(Utilities.IndexOfNth(fi.Name, '_', 2) + 1);
            DownloadDocument(fileToDownload, fileNameToDownload, false);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
    }

    protected void btnDeleteFie_Click(object sender, EventArgs e)
    {
        try
        {
            ImageButton btnDelete = (ImageButton)sender;
            string itemToDelete = btnDelete.CommandArgument;

            if (Directory.Exists(itemToDelete) || File.Exists(itemToDelete)) // don't throw an error if it was deleted in another page
            {
                // delete the actual file/folder
                FileAttributes attr = File.GetAttributes(itemToDelete);
                if (attr.HasFlag(FileAttributes.Directory))
                    System.IO.Directory.Delete(itemToDelete, true);
                else
                    System.IO.File.Delete(itemToDelete);
            }

            FillScannedDocGrid();
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            SetErrorMessage("Connection to network files currently unavailable.");
            return;
        }

    }

    protected void btnAddDirectory_Click(object sender, EventArgs e)
    {
        txtNewDirectory.Text = txtNewDirectory.Text.Trim();

        string curFolder = this.CurrentFolder;
        if (!curFolder.EndsWith("\\"))
            curFolder += "\\";

        try
        {
            if (Regex.IsMatch(txtNewDirectory.Text, @"__\d+__"))
                throw new CustomMessageException("Folder name can not be double underscore, followed by digits, followed by a double underscore");

            CreateDirectory(curFolder + txtNewDirectory.Text);
            txtNewDirectory.Text = string.Empty;
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException && ex.Message == "Illegal characters in path.")
                SetErrorMessage("Contains invalid folder characters");
            else
                SetErrorMessage(ex.Message);
        }
        finally
        {
            FillScannedDocGrid();
        }
    }


    #region SaveFiles()

    private bool SaveFiles(string dir, bool allowOverwrite)
    {
        string allowedFileTypes = "bmp|dcs|doc|docm|docx|dot|dotm|dotx|gz|gif|htm|html|ipg|jpe|jpeg|jpg|mp3|mp4|wav|pdf|png|ppa|ppam|pps|ppsm|ppt|pptm|tar|tgz|tif|tiff|txt|xla|xlam|xlc|xld|xlk|xll|xlm|xls|xlsx|xlt|xltm|xltx|xlw|xml|z|zip|7z";

        dir = dir.EndsWith(@"\") ? dir : dir + @"\";

        System.Text.StringBuilder _messageToUser = new System.Text.StringBuilder("Files Uploaded:<br>");

        try
        {

            HttpFileCollection _files = Request.Files;

            if (_files.Count == 0 || (_files.Count == 1 && System.IO.Path.GetFileName(_files[0].FileName) == string.Empty))
            {
                lblUploadMessage.Text = " <font color=\"red\">No Files Selected</font> <BR>";
                return true;
            }


            for (int i = 0; i < _files.Count; i++)
            {
                HttpPostedFile _postedFile = _files[i];
                string _fileName = System.IO.Path.GetFileName(_postedFile.FileName);
                if (_fileName.Length == 0)
                    continue;

                if (_postedFile.ContentLength > 20971520)
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! Over allowable file size limit!</font> <BR>");
                if (!ExtIn(System.IO.Path.GetExtension(_fileName), allowedFileTypes))
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! Only " + ExtToDisplay(allowedFileTypes) + " files allowed!</font> <BR>");

                if (!allowOverwrite && File.Exists(dir + "\\" + _fileName))
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! File already exists. To allow overwrite, check the \"Allowed File Overwrite\" box</font> <BR>");
            }

            int countZeroFileLength = 0;
            for (int i = 0; i < _files.Count; i++)
            {
                HttpPostedFile _postedFile = _files[i];
                string _fileName = System.IO.Path.GetFileName(_postedFile.FileName);
                if (_fileName.Length == 0)
                    continue;

                if (_postedFile.ContentLength > 0)
                {
                    string dbName = System.Web.HttpContext.Current.Session["DB"].ToString().Replace("_TestDB", "");
                    bool useLocalFolder = Convert.ToBoolean(
                        System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] != null ?
                        System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] :
                        System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal"]
                        );

                    if (useLocalFolder)
                    {
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        _postedFile.SaveAs(dir + _fileName);
                        _messageToUser.Append(_fileName + "<BR>");
                    }
                    else  // use network folder
                    {
                        string username    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderUsername"];
                        string password    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderPassword"];

                        string networkName =
                            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] != null ?
                            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] :
                            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder"];

                        if (networkName.EndsWith(@"\"))
                            networkName = networkName.Substring(0, networkName.Length - 1);

                        using (new NetworkConnection(networkName, new System.Net.NetworkCredential(username, password)))
                        {
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);

                            _postedFile.SaveAs(dir + _fileName);
                            _messageToUser.Append(_fileName + "<BR>");
                        }
                    }


                    /*
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    //_postedFile.SaveAs(Server.MapPath("MyFiles") + "\\" + System.IO.Path.GetFileName(_postedFile.FileName));
                    _postedFile.SaveAs(dir + _fileName);
                    _messageToUser.Append(_fileName + "<BR>");
                    */

                }
                else
                {
                    countZeroFileLength++;
                }
            }

            if (_files.Count > 0 && countZeroFileLength == _files.Count)
                throw new Exception("<font color=\"red\">File" + (_files.Count > 1 ? "s are" : " is") + " 0 kb.</font>");
            else if (_files.Count > 0 && countZeroFileLength > 0)
                throw new Exception("<font color=\"red\">File(s) of 0 kb were not uploaded.</font>");

            lblUploadMessage.Text = _messageToUser.ToString();
            return true;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            lblUploadMessage.Text = "Connection to network files currently unavailable.";
            return false;
        }
        catch (System.Exception ex)
        {
            lblUploadMessage.Text = ex.Message;
            return false;
        }
        finally
        {
            try
            {
                FillScannedDocGrid();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                SetErrorMessage("Connection to network files currently unavailable.");
            }

        }
    }

    protected bool ExtIn(string ext, string allowedExtns)
    {
        ext = ext.ToUpper();
        string[] allowedExtnsList = allowedExtns.Split('|');
        foreach (string curExt in allowedExtnsList)
            if (ext == "." + curExt.ToUpper())
                return true;

        return false;
    }

    protected string ExtToDisplay(string allowedExtns)
    {
        string ret = string.Empty;
        foreach (string curExt in allowedExtns.Split('|'))
        {
            if (ret.Length > 0)
                ret += ",";

            ret += ("." + curExt);
        }

        return ret;
    }

    #endregion

    #region DownloadFile

    protected void DownloadDocument(string filepath, string downloadFileName, bool deleteDocAfterDownload)
    {
        System.IO.FileInfo file = new System.IO.FileInfo(filepath);
        if (!file.Exists)
            throw new CustomMessageException("File doesn't exist: " + file.Name);

        try
        {
            string contentType = "application/octet-stream";
            try { contentType = Utilities.GetMimeType(System.IO.Path.GetExtension(downloadFileName)); }
            catch (Exception) { }

            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFileName + "\"" );
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.WriteFile(filepath, true);
            if (deleteDocAfterDownload)
                File.Delete(filepath);
            Response.End();
        }
        catch (System.Web.HttpException ex) 
        {
            // ignore exception where user closed the download box
            if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                throw;
        }
    }

    #endregion

    #endregion

    #region CreateDirectory(dir)

    protected void CreateDirectory(string dir)
    {
        string dbName = System.Web.HttpContext.Current.Session["DB"].ToString().Replace("_TestDB", "");
        bool useLocalFolder = Convert.ToBoolean(
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] != null ?
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] :
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal"]
            );

        if (useLocalFolder)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        else  // use network folder
        {
            string username    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderUsername"];
            string password    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderPassword"];

            string networkName =
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] != null ?
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] :
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder"];

            if (networkName.EndsWith(@"\"))
                networkName = networkName.Substring(0, networkName.Length - 1);

            using (new NetworkConnection(networkName, new System.Net.NetworkCredential(username, password)))
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
        }
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        spn_manage_files.Visible = false;
        SetErrorMessage(errMsg, details);
    }
    private void SetErrorMessage(string errMsg = "", string details = "")
    {
        if (errMsg.Contains(Environment.NewLine))
            errMsg = errMsg.Replace(Environment.NewLine, "<br />");

        // double escape so shows up literally on webpage for 'alert' message
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"#\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

        lblErrorMessage.Visible = true;
        if (errMsg != null && errMsg.Length > 0)
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}