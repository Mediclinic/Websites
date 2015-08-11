using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;


public class LetterBest
{

    public LetterBest(int letter_id, int letter_type_id, string code, string docname, bool is_send_to_medico, bool is_allowed_reclaim, bool is_manual_override, int num_copies_to_print)
    {
        this.letter_id = letter_id;
        this.letter_type = new IDandDescr(letter_type_id);
        this.code = code;
        this.docname = docname;
        this.is_send_to_medico = is_send_to_medico;
        this.is_allowed_reclaim = is_allowed_reclaim;
        this.is_manual_override = is_manual_override;
        this.num_copies_to_print = num_copies_to_print;
    }
    public LetterBest(int letter_id)
    {
        this.letter_id = letter_id;
    }

    private int letter_id;
    public int LetterID
    {
        get { return this.letter_id; }
        set { this.letter_id = value; }
    }
    private IDandDescr letter_type;
    public IDandDescr LetterType
    {
        get { return this.letter_type; }
        set { this.letter_type = value; }
    }
    private string code;
    public string Code
    {
        get { return this.code; }
        set { this.code = value; }
    }
    private string docname;
    public string Docname
    {
        get { return this.docname; }
        set { this.docname = value; }
    }
    private bool is_send_to_medico;
    public bool IsSendToMedico
    {
        get { return this.is_send_to_medico; }
        set { this.is_send_to_medico = value; }
    }
    private bool is_allowed_reclaim;
    public bool IsAllowedReclaim
    {
        get { return this.is_allowed_reclaim; }
        set { this.is_allowed_reclaim = value; }
    }
    private bool is_manual_override;
    public bool IsManualOverride
    {
        get { return this.is_manual_override; }
        set { this.is_manual_override = value; }
    }
    private int num_copies_to_print;
    public int NumCopiesToPrint
    {
        get { return this.num_copies_to_print; }
        set { this.num_copies_to_print = value; }
    }
    public override string ToString()
    {
        return letter_id.ToString() + " " + letter_type.ID.ToString() + " " + code.ToString() + " " + docname.ToString() + " " + is_send_to_medico.ToString() + " " +
                is_allowed_reclaim.ToString() + " " + is_manual_override.ToString() + " " + num_copies_to_print.ToString();
    }

}