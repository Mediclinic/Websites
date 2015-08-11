using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Configuration;

public class PatientsContactCacheDB
{

    public static Hashtable GetBullkPhoneNumbers(int[] entity_ids, int site_id, string contact_type_ids = "30,33,34")
    {
        return GetBullk(null, contact_type_ids, entity_ids, site_id);
    }
    public static Hashtable GetBullkEmail(int[] entity_ids, int site_id)
    {
        return GetBullk(null, "27", entity_ids, site_id);
    }
    public static Hashtable GetBullkAddress(int[] entity_ids, int site_id)
    {
        return GetBullk("1", null, entity_ids, site_id);
    }
    public static Hashtable GetBullkBedrooms(int[] entity_ids, int site_id)
    {
        return GetBullk(null, "166", entity_ids, site_id);
    }

    public static Hashtable GetBullk(string contact_type_group_ids, string contact_type_ids, int[] entity_ids, int site_id)
    {
        if (entity_ids.Length == 0)
            return new Hashtable();


        // remove duplicates
        ArrayList uniqueIDs = new ArrayList();
        for (int i = 0; i < entity_ids.Length; i++)
            if (!uniqueIDs.Contains(entity_ids[i]))
                uniqueIDs.Add(entity_ids[i]);
        entity_ids = (int[])uniqueIDs.ToArray(typeof(int));


        Hashtable phoneNumbers = new Hashtable();
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] phoneNbrs = ContactDB.GetByEntityIDs(contact_type_group_ids, entity_ids, contact_type_ids, site_id);
            for (int i = 0; i < phoneNbrs.Length; i++)
            {
                if (phoneNumbers[phoneNbrs[i].EntityID] == null)
                    phoneNumbers[phoneNbrs[i].EntityID] = new ArrayList();
                ((ArrayList)phoneNumbers[phoneNbrs[i].EntityID]).Add(phoneNbrs[i]);
            }
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] phoneNbrs = ContactAusDB.GetByEntityIDs(contact_type_group_ids, entity_ids, contact_type_ids, site_id);
            for (int i = 0; i < phoneNbrs.Length; i++)
            {
                if (phoneNumbers[phoneNbrs[i].EntityID] == null)
                    phoneNumbers[phoneNbrs[i].EntityID] = new ArrayList();
                ((ArrayList)phoneNumbers[phoneNbrs[i].EntityID]).Add(phoneNbrs[i]);
            }
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());


        // convert arraylists to arrays (and sort if necessary)
        ArrayList keys = new ArrayList();
        foreach (DictionaryEntry de in phoneNumbers)
            keys.Add(de.Key);
        foreach (int key in keys)
        {
            if (Utilities.GetAddressType().ToString() == "Contact")
                phoneNumbers[key] = (Contact[])((ArrayList)phoneNumbers[key]).ToArray(typeof(Contact));
            else if (Utilities.GetAddressType().ToString() == "ContactAus")
                phoneNumbers[key] = (ContactAus[])((ArrayList)phoneNumbers[key]).ToArray(typeof(ContactAus));
        }


        return phoneNumbers;
    }



    public static string GetBedroom(Hashtable contactHash, int entityID)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] contacts = (Contact[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            foreach (Contact c in contacts)
            {
                string text = c.AddrLine1.Trim();
                if (text.Length > 0)
                    return text;
            }
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] contacts = (ContactAus[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            foreach (ContactAus c in contacts)
            {
                string text = c.AddrLine1.Trim();
                if (text.Length > 0)
                    return text;
            }
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        return null;
    }


}