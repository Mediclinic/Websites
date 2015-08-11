using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class LetterTreatmentTemplate
{

    public LetterTreatmentTemplate(int letter_treatment_template_id, int field_id, int first_letter_id, int last_letter_id, int last_letter_pt_id, int last_letter_when_replacing_epc_id, int treatment_notes_letter_id, int site_id)
    {
        this.letter_treatment_template_id   = letter_treatment_template_id;
        this.field                          = new IDandDescr(field_id);
        this.first_letter                   = new Letter(first_letter_id);
        this.last_letter                    = new Letter(last_letter_id);
        this.last_letter_pt                 = new Letter(last_letter_pt_id);
        this.last_letter_when_replacing_epc = new Letter(last_letter_when_replacing_epc_id);
        this.treatment_notes_letter         = new Letter(treatment_notes_letter_id);
        this.site                           = new Site(site_id);
    }

    private int letter_treatment_template_id;
    public int LetterTreatmentTemplateID
    {
        get { return this.letter_treatment_template_id; }
        set { this.letter_treatment_template_id = value; }
    }
    private IDandDescr field;
    public IDandDescr Field
    {
        get { return this.field; }
        set { this.field = value; }
    }
    private Letter first_letter;
    public Letter FirstLetter
    {
        get { return this.first_letter; }
        set { this.first_letter = value; }
    }
    private Letter last_letter;
    public Letter LastLetter
    {
        get { return this.last_letter; }
        set { this.last_letter = value; }
    }
    private Letter last_letter_pt;
    public Letter LastLetterPT
    {
        get { return this.last_letter_pt; }
        set { this.last_letter_pt = value; }
    }
    private Letter last_letter_when_replacing_epc;
    public Letter LastLetterWhenReplacingEPC
    {
        get { return this.last_letter_when_replacing_epc; }
        set { this.last_letter_when_replacing_epc = value; }
    }
    private Letter treatment_notes_letter;
    public Letter TreatmentNotesLetter
    {
        get { return this.treatment_notes_letter; }
        set { this.treatment_notes_letter = value; }
    }
    private Site site;
    public Site Site
    {
        get { return this.site; }
        set { this.site = value; }
    }
    public override string ToString()
    {
        return letter_treatment_template_id.ToString() + " " + field.ID.ToString() + " " + first_letter.LetterID.ToString() + " " + treatment_notes_letter.LetterID.ToString() + " " + last_letter.LetterID.ToString() + " " + site.SiteID;
    }

}