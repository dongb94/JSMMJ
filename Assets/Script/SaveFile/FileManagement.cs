
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;


public class FileManagement
{
    public static readonly string BasePath = string.Format($"{Application.persistentDataPath}/" + "MenuSaveData/");

    public static void SaveFile(List<MenuListItem> menu, string name = "AutoSave")
    {
        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
        }

        var saveFile = new List<string>();
        
        foreach (var menuItem in menu)
        {
            saveFile.Add(menuItem.Text);
        }
        
        Write(saveFile.ToArray(), name);
    }

    public static string[] LoadFile(string name = "AutoSave")
    {
        return (string[]) Read(name);

        //return (List<MenuListItem>) Read(name);
    }

    public static void DeleteFile(string name = "AutoSave")
    {
        Delete(name);
    }

    public static string[] LoadSaveDataList()
    {
        var list = Directory.GetFiles(BasePath).ToList();
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = list[i].Substring(list[i].LastIndexOf('/') + 1);
            if (list[i] == "AutoSave")
            {
                list.Remove(list[i]);
                i--;
            }
        }
        
        return list.ToArray();
    }

    private static void Write(object menuFile, string fileName)
    {
        var filePath = BasePath + fileName;
        byte[] buffer;
        
        if(menuFile == null)
            return;
        
        var bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, menuFile);
            buffer = ms.ToArray();
        }
        
        if (File.Exists(filePath))
        {
            if (fileName != "AutoSave")
            {
                var count = 1;
                while (File.Exists(filePath + $"({count})")) count++;
                filePath += $"({count})";    
            }
        }
        var file = File.Create(filePath);
        file.Write(buffer, 0, buffer.Length);
        file.Flush();
        file.Close();
        
    }

    private static object Read(string fileName)
    {
        var filePath = BasePath + fileName;
        if (!File.Exists(filePath))
            return null;
        
        using (var file = File.OpenRead(filePath))
        {
            var bf = new BinaryFormatter();
            return bf.Deserialize(file);
        }
    }

    private static void Delete(string fileName)
    {
        var filePath = BasePath + fileName;
        if (!File.Exists(filePath)) return;
        File.Delete(filePath);
    }
}
