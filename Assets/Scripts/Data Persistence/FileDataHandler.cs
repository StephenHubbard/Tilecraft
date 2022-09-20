using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

public class FileDataHandler 
{
    private string dataDirPath = "";

    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName) {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load() {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadedData = null;
        if (File.Exists(fullPath)) {
            try {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e) {
                Debug.Log("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;

    }

    public void Save(GameData data) {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        // Delete old file
        // try 
        // {
        //     if (File.Exists(fullPath)) {
        //         Debug.Log("previous save file deleted");
        //         Directory.Delete(Path.GetDirectoryName(fullPath), true);
        //     } else {
        //         Debug.LogWarning("Tried to delete data, but the data didn't exist");
        //     }
        // }
        // catch (Exception e) {
        //     Debug.LogError("Failed to delte profile data for profile ID : at path: " + fullPath + "\n" + e);
        // }

        // New file
        Debug.Log(fullPath);
        try {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e) {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}