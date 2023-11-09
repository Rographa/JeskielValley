using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace PlayerSaveData
{
    public abstract class SaveLoad
    {
        public static string SaveEndpoint => Application.persistentDataPath + "/jeskiel.save";
        public static SaveData CurrentSaveData;

        public static void Save()
        {
            CurrentSaveData ??= new SaveData();
            var binaryFormatter = new BinaryFormatter();
            var file = File.Create(SaveEndpoint);
            binaryFormatter.Serialize(file, CurrentSaveData);
            file.Close();
        }

        public static void Save(SaveData saveData)
        {
            CurrentSaveData = saveData;
            Save();
        }

        public static void Load()
        {
            if (File.Exists(SaveEndpoint))
            {
                var binaryFormatter = new BinaryFormatter();
                var file = File.Open(SaveEndpoint, FileMode.Open);
                CurrentSaveData = binaryFormatter.Deserialize(file) as SaveData;
                file.Close();
            }
            else
            {
                CurrentSaveData = new();
            }
        }
    }
}