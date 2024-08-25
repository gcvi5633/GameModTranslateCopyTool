using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace GameModTranslateCopyTool
{
    class KeyAndValue
    {
        public string Key;
        public string Value;

        public KeyAndValue(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public class CNandCTSetting
    {
        public string ChineseSimplifiedKey;
        public string ChineseTraditionalKey;
        public string CNReplaceTargetKey;
        public string CNReplaceTargetValue;
        
        /// <summary>
        /// 設定簡體中文與繁體中文的路徑關鍵字
        /// </summary>
        /// <param name="CNkey">簡體中文關鍵字</param>
        /// <param name="CTKey">繁體中文關鍵字</param>
        /// <param name="replaceTargetKey">複製簡中時要替換的內容的關鍵字，會連檔名一起換掉，不填就不會替換</param>
        /// <param name="replaceTargetValue">複製簡中時要替換的新內容</param>
        public CNandCTSetting(string CNkey, string CTKey, string replaceTargetKey = "", string replaceTargetValue = "")
        {
            ChineseSimplifiedKey = CNkey;
            ChineseTraditionalKey = CTKey;
            CNReplaceTargetKey = replaceTargetKey;
            CNReplaceTargetValue = replaceTargetValue;
        } 
    }

    public class TranslateTool
    {
        private List<KeyAndValue> _keyAndValues = new List<KeyAndValue>();
        
        public CNandCTSetting translateSetting = null;
        public void SetJson(string jsonPath)
        {
            // 使用 Assembly.GetManifestResourceStream 方法來讀取嵌入資源中的檔案
            // 獲取當前執行的程序
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(jsonPath))
            {
                if (stream == null)
                {
                    Console.WriteLine("資源未找到！");
                    return;
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    string jsonStr = reader.ReadToEnd();
                    Dictionary<string, string> json = JsonSerializer.Deserialize<Dictionary<string,string>>(jsonStr);
                    _keyAndValues.Clear();
                    json = json.OrderByDescending(x => x.Key.Length).ToDictionary(x=>x.Key, x=>x.Value);
                    foreach (KeyValuePair<string, string> item in json)
                    {
                        string key = item.Key;
                        string value = item.Value;
                        _keyAndValues.Add(new KeyAndValue(key, value));
                        Console.WriteLine($"Json key: {key} value: {value}");
                    }
                    Console.WriteLine("Set json end.");
                    Console.WriteLine();
                }
            }

        }

        public void CopySomeExtensionFile(string extension)
        {
            string path = Program.SelectDirectory("選取要複製的簡體中文資料夾根目錄");
            string newPath = Program.SelectDirectory("選取要輸出的資料夾根目錄");
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(newPath))
            {
                Console.WriteLine($"位置不可以為空，path: {path}, newPath: {newPath}");
                return;
            }
            DirectoryInfo info = new DirectoryInfo(path);
            CheckExtensionFile(info, newPath, extension);
        }

        void CheckExtensionFile(DirectoryInfo info, string newPath, string extension)
        {
            DirectoryInfo[] directories = info.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                string dirPath = $"{newPath}\\{directories[i].Name}";
                CheckExtensionFile(directories[i],dirPath,extension);
            } 
            
            if (Directory.Exists(newPath) == false)
            {
                Directory.CreateDirectory(newPath);
            }
            for (int i = 0; i < info.GetFiles().Length; i++)
            {
        
                FileInfo p = info.GetFiles()[i];
                if (p.Extension.Equals(extension) == false)
                {
                    continue;
                }
                using (StreamReader file = p.OpenText())
                {
                    string str = file.ReadToEnd();
                    string newFilePath = newPath + "\\" + p.Name;
                    file.Close();
                    File.WriteAllText(newFilePath,str);
                    Console.WriteLine($"Replace file : {newFilePath}");
                }
            }
        }
        
        [STAThread]
        public void CopyOnlyChineseSimplified()
        {
            string path = Program.SelectDirectory("選取要複製的簡體中文資料夾根目錄");
            string newPath = Program.SelectDirectory("選取要輸出的資料夾根目錄");
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(newPath))
            {
                Console.WriteLine($"位置不可以為空，path: {path}, newPath: {newPath}");
                return;
            }
            DirectoryInfo info = new DirectoryInfo(path);
            CheckDirCHS(info,newPath);
        }
        
        void CheckDirCHS(DirectoryInfo info, string newPath,bool onlyCHS = false)
        {
            DirectoryInfo[] directories = info.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                string dirPath = $"{newPath}\\{directories[i].Name}";
                bool chs = directories.ToList().FindIndex(x => x.FullName.Contains(translateSetting.ChineseSimplifiedKey)) > -1;
                bool cht = directories.ToList().FindIndex(x => x.FullName.Contains(translateSetting.ChineseTraditionalKey)) > -1;
                bool checkCHS = onlyCHS;
                dirPath = dirPath.Replace(translateSetting.ChineseSimplifiedKey, translateSetting.ChineseTraditionalKey);
                if (onlyCHS == false)
                {
                    checkCHS = (chs && cht == false);
                }
                CheckDirCHS(directories[i],dirPath,checkCHS);
            }

            if (info.FullName.Contains(translateSetting.ChineseSimplifiedKey) && onlyCHS)
            {
                if (Directory.Exists(newPath) == false)
                {
                    Directory.CreateDirectory(newPath);
                }
                for (int i = 0; i < info.GetFiles().Length; i++)
                {
                    FileInfo p = info.GetFiles()[i];
                    using (StreamReader file = p.OpenText())
                    {
                        string str = file.ReadToEnd();
                        string fileName = p.Name;
                        if (string.IsNullOrEmpty(translateSetting.CNReplaceTargetKey) == false)
                        {
                            fileName = fileName.Replace(translateSetting.CNReplaceTargetKey, translateSetting.CNReplaceTargetValue);
                            str = str.Replace(translateSetting.CNReplaceTargetKey, translateSetting.CNReplaceTargetValue);
                        }
                        string newFilePath = newPath + "\\" + fileName;
                        File.WriteAllText(newFilePath,str);
                        file.Close(); 
                        Console.WriteLine($"Copy file : {newFilePath}");
                    }
                }
            }
        }

        public void CopyCHT()
        {
            string path = Program.SelectDirectory("選取要複製的正體中文資料夾根目錄");
            string newPath = Program.SelectDirectory("選取要輸出的資料夾根目錄");
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(newPath))
            {
                Console.WriteLine($"位置不可以為空，path: {path}, newPath: {newPath}");
                return;
            } 
            DirectoryInfo info = new DirectoryInfo(path);
            CheckDirCHT(info, newPath);
        }
        
        void CheckDirCHT(DirectoryInfo info, string newPath)
        {
            DirectoryInfo[] directories = info.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                string dirPath = $"{newPath}\\{directories[i].Name}";
                CheckDirCHT(directories[i], dirPath);
            }

            if (info.FullName.Contains(translateSetting.ChineseTraditionalKey))
            {
                if (Directory.Exists(newPath) == false)
                {
                    Directory.CreateDirectory(newPath);
                }
                for (int i = 0; i < info.GetFiles().Length; i++)
                {
            
                    FileInfo p = info.GetFiles()[i];
                    using (StreamReader file = p.OpenText())
                    {
                        string str = file.ReadToEnd();
                        for (int j = 0; j < _keyAndValues.Count; j++)
                        {
                            KeyAndValue keyAndValue = _keyAndValues[j];
                            str = str.Replace(keyAndValue.Key, keyAndValue.Value);
                        }
                        string newFilePath = newPath + "\\" + p.Name;
                        file.Close();
                        File.WriteAllText(newFilePath,str);
                        Console.WriteLine($"Replace file : {newFilePath}");
                    }
                }
            }
        }

        public void OnlyReplaceDictionary()
        {
            string path = Program.SelectDirectory("選取要修改字典的資料夾根目錄");
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine($"位置不可以為空，path: {path}");
                return;
            }
            DirectoryInfo info = new DirectoryInfo(path);
            CheckReplaceDir(info, path);
        }
        
        void CheckReplaceDir(DirectoryInfo info, string newPath)
        {
            DirectoryInfo[] directories = info.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                string dirPath = $"{newPath}\\{directories[i].Name}";
                CheckReplaceDir(directories[i], dirPath);
            }

            if (Directory.Exists(newPath) == false)
            {
                Directory.CreateDirectory(newPath);
            }
            var files = info.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
        
                FileInfo p = files[i];
                using (StreamReader file = p.OpenText())
                {
                    string str = file.ReadToEnd();
                    for (int j = 0; j < _keyAndValues.Count; j++)
                    {
                        KeyAndValue keyAndValue = _keyAndValues[j];
                        str = str.Replace(keyAndValue.Key, keyAndValue.Value);
                    }
                    string newFilePath = newPath + "\\" + p.Name;
                    file.Close();
                    File.WriteAllText(newFilePath,str);
                    Console.WriteLine($"Replace file : {newFilePath}");
                }
            }
        }
    }
}