using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BulkLetterSendingQueueBatch
{

    public BulkLetterSendingQueueBatch(int bulk_letter_sending_queue_batch_id, string email_address_to_send_printed_letters_to, bool ready_to_process)
    {
        this.bulk_letter_sending_queue_batch_id       = bulk_letter_sending_queue_batch_id;
        this.email_address_to_send_printed_letters_to = email_address_to_send_printed_letters_to;
        this.ready_to_process                         = ready_to_process;
    }

    private int bulk_letter_sending_queue_batch_id;
    public int BulkLetterSendingQueueBatchID
    {
        get { return this.bulk_letter_sending_queue_batch_id; }
        set { this.bulk_letter_sending_queue_batch_id = value; }
    }
    private string email_address_to_send_printed_letters_to;
    public string EmailAddressToSendPrintedLettersTo
    {
        get { return this.email_address_to_send_printed_letters_to; }
        set { this.email_address_to_send_printed_letters_to = value; }
    }
    private bool ready_to_process;
    public bool ReadyToProcess
    {
        get { return this.ready_to_process; }
        set { this.ready_to_process = value; }
    }
    public override string ToString()
    {
        return bulk_letter_sending_queue_batch_id.ToString() + " " + email_address_to_send_printed_letters_to.ToString() + " " + ready_to_process.ToString();
    }

}