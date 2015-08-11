using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Text.RegularExpressions;

public class MedicalServiceType
{

    public static string ReplaceMedicareServiceTypeFolders(string rawName, Hashtable medicalServiceTypeHash, bool setNewColour)
    {
        string[] fileParts = rawName.Split(new char[] { '\\' });

        // only change for base folder:  fileParts[0]
        if (fileParts.Length >= 1 && Regex.IsMatch(fileParts[0], @"^__\d+__$"))
            foreach (DictionaryEntry pair in medicalServiceTypeHash)
                fileParts[0] = fileParts[0].Replace("__" + pair.Key.ToString() + "__", (setNewColour ? "<font color=\"red\">" : "") + pair.Value.ToString() + (setNewColour ? "</font>" : ""));
        return string.Join("\\", fileParts);
    }

}