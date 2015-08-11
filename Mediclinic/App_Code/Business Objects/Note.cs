using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Note
{

    public Note(int note_id, int entity_id, int note_type_id, int body_part_id, string text,
                DateTime date_added, DateTime date_modified, DateTime date_deleted,
                int added_by, int modified_by, int deleted_by, int site_id)
    {
        this.note_id       = note_id;
        this.entity_id     = entity_id;
        this.note_type     = new IDandDescr(note_type_id);
        this.body_part     = new IDandDescr(body_part_id);
        this.text          = text;
        this.date_added    = date_added;
        this.date_modified = date_modified;
        this.date_deleted  = date_deleted;
        this.added_by      = added_by    == -1 ? null : new Staff(added_by);
        this.modified_by   = modified_by == -1 ? null : new Staff(modified_by);
        this.deleted_by    = deleted_by  == -1 ? null : new Staff(deleted_by);
        this.site          = new Site(site_id);
    }
    public Note(int note_id)
    {
        this.note_id = note_id;
    }

    private int note_id;
    public int NoteID
    {
        get { return this.note_id; }
        set { this.note_id = value; }
    }
    private int entity_id;
    public int EntityID
    {
        get { return this.entity_id; }
        set { this.entity_id = value; }
    }
    private IDandDescr note_type;
    public IDandDescr NoteType
    {
        get { return this.note_type; }
        set { this.note_type = value; }
    }
    private IDandDescr body_part;
    public IDandDescr BodyPart
    {
        get { return this.body_part; }
        set { this.body_part = value; }
    }
    private string text;
    public string Text
    {
        get { return this.text; }
        set { this.text = value; }
    }
    private DateTime date_added;
    public DateTime DateAdded
    {
        get { return this.date_added; }
        set { this.date_added = value; }
    }
    private DateTime date_modified;
    public DateTime DateModified
    {
        get { return this.date_modified; }
        set { this.date_modified = value; }
    }
    private DateTime date_deleted;
    public DateTime DateDeleted
    {
        get { return this.date_deleted; }
        set { this.date_deleted = value; }
    }
    private Staff added_by;
    public Staff AddedBy
    {
        get { return this.added_by; }
        set { this.added_by = value; }
    }
    private Staff modified_by;
    public Staff ModifiedBy
    {
        get { return this.modified_by; }
        set { this.modified_by = value; }
    }
    private Staff deleted_by;
    public Staff DeletedBy
    {
        get { return this.deleted_by; }
        set { this.deleted_by = value; }
    }
    private Site site;
    public Site Site
    {
        get { return this.site; }
        set { this.site = value; }
    }
    public override string ToString()
    {
        return note_id.ToString() + " " + entity_id.ToString() + " " + note_type.ID.ToString() + " " + text.ToString() + " " + date_added.ToString() + " " +
                date_modified.ToString() + " " + site.SiteID.ToString();
    }

    public bool IsDeleted
    {
        get { return deleted_by != null || date_deleted != DateTime.MinValue; }
    }

    public static string GetPopupLinkText(int screenID, int entityID, bool hasNotes, bool updateAfterPopupClosed, int width, int height, string noNotesImage, string hasNotesImage, string functionsToCallAfter = null)
    {
        string screen_id = screenID.ToString();
        string allFeatures = "dialogWidth:"+width+"px;dialogHeight:"+height+"px;center:yes;resizable:no; scroll:no";
        string js = "javascript:window.showModalDialog('" + "NoteInfo.aspx?id=" + entityID.ToString() + "&screen=" + screen_id + "', '', '" + allFeatures + "');" + (functionsToCallAfter != null ? functionsToCallAfter+";" : "") + (updateAfterPopupClosed ? "" : "return false;");
        string img = hasNotes ? hasNotesImage : noNotesImage;
        string notesText = "<input type=\"image\" title=\"Notes\" src=\"" + img + "\" alt=\"Notes\" onclick=\"" + js + "\" />";
        return notesText;
    }
    public static string GetPopupLinkTextV2(int screenID, int entityID, bool hasNotes, bool updateAfterPopupClosed, int width, int height, string noNotesImage, string hasNotesImage, string functionsToCallAfter = null, bool usePopup = true)
    {
        string screen_id = screenID.ToString();
        string allFeatures = "dialogWidth:"+width+"px;dialogHeight:"+height+"px;center:yes;resizable:no; scroll:no";

        string js =
            usePopup ?
            "javascript:window.showModalDialog('" + "NoteListV2.aspx?id=" + entityID.ToString() + "&screen=" + screen_id + "', '', '" + allFeatures + "');" + (functionsToCallAfter != null ? functionsToCallAfter+";" : "") + (updateAfterPopupClosed ? "" : "return false;") :
            "javascript: open_new_tab('" + "NoteListV2.aspx?id=" + entityID.ToString() + "&screen=" + screen_id + "&refresh_on_close=1', '_blank');return false;";
        
        string img = hasNotes ? hasNotesImage : noNotesImage;
        string notesText = "<input type=\"image\" title=\"Notes\" src=\"" + img + "\" alt=\"Notes\" onclick=\"" + js + "\" />";
        return notesText;
    }


    public static string GetBookingPopupLinkTextV2(int bkEntityID, bool hasNotes, bool updateAfterPopupClosed, int width, int height, string noNotesImage, string hasNotesImage, string functionsToCallAfter = null, bool usePopup = true)
    {
        string allFeatures = "dialogWidth:" + width + "px;dialogHeight:" + height + "px;center:yes;resizable:no; scroll:no";

        string js =
            usePopup ?
            "javascript:window.showModalDialog('" + "NotesV2.aspx?bk_id=" + bkEntityID.ToString() + "', '', '" + allFeatures + "');" + (functionsToCallAfter != null ? functionsToCallAfter + ";" : "") + (updateAfterPopupClosed ? "" : "return false;") :
            "javascript: open_new_tab('" + "NotesV2.aspx?bk_id=" + bkEntityID.ToString() + "&refresh_on_close=1', '_blank');return false;";

        string img = hasNotes ? hasNotesImage : noNotesImage;
        string notesText = "<input type=\"image\" title=\"Notes\" src=\"" + img + "\" alt=\"Notes\" onclick=\"" + js + "\" />";
        return notesText;
    }


}