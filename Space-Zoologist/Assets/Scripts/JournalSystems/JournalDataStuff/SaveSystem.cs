using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {

    public static void SaveJournal(JournalData journal)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "journal.bin");
        using (FileStream stream = new FileStream(path, FileMode.Append))
        {
            JournalData data = new JournalData(journal);
            formatter.Serialize(stream, data);
        } 
    }

    public static JournalData LoadJournal()
    {
        string path = Path.Combine(Application.persistentDataPath, "journal.bin");
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                JournalData data = formatter.Deserialize(stream) as JournalData;
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
