using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class LetterBestPrintHistory
{

    public LetterBestPrintHistory(int letter_print_history_id, int letter_id, int patient_id, DateTime date)
    {
        this.letter_print_history_id = letter_print_history_id;
        this.letter                  = letter_id  == -1 ? null : new LetterBest(letter_id);
        this.patient                 = patient_id == -1 ? null : new Patient(patient_id);
        this.date                    = date;
    }

    private int letter_print_history_id;
    public int LetterPrintHistoryID
    {
        get { return this.letter_print_history_id; }
        set { this.letter_print_history_id = value; }
    }
    private LetterBest letter;
    public LetterBest Letter
    {
        get { return this.letter; }
        set { this.letter = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private DateTime date;
    public DateTime Date
    {
        get { return this.date; }
        set { this.date = value; }
    }
    public override string ToString()
    {
        return letter_print_history_id.ToString() + " " + letter.LetterID.ToString() + " " + patient.PatientID.ToString() + " " + date.ToString();
    }

}