using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Repository
{
    static Repository instance;
    Database _database;

    public static Repository GetInstance()
    {
        if (instance == null)
        {
            instance = new Repository();
        }
        return instance;
    }


    public Database GetData()
    {
        if (_database != null)
        {
            return _database;
        }

        string path = Application.persistentDataPath + "/data.save";

        if (!File.Exists(path) || new FileInfo(path).Length == 0)
        {
            _database = new Database();
            return _database;
        }

        using (FileStream file = File.OpenRead(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            _database = (Database)bf.Deserialize(file);
        }

        return _database;

    }

    public void SaveData()
    {

        string path = Application.persistentDataPath + "/data.save";
        FileStream file = null;
        if (File.Exists(path))
        {
            file = File.OpenWrite(path);
        }
        else
        {
            file = File.Create(path);
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, _database);
        file.Close();
    }
}
