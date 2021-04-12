using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {

    public static void SaveJournal(JournalData journal)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "journal.bin");
        using (FileStream stream = File.Open(path, FileMode.Create))
        {
            JournalData data = new JournalData(journal);
            formatter.Serialize(stream, data);
            Debug.Log("Data saved");
        } 
    }

    public static JournalData LoadJournal()
    {
        string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "journal.bin");
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                JournalData data = formatter.Deserialize(stream) as JournalData;
                Debug.Log("Data loaded");
                return data;
            }
        }
        else
        {
            Debug.Log("Save file not found: " + path);
            return null;
        }
    }
}
