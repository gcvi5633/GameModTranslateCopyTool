using System;

// 在 Dependencies 右鍵 Reference 來加入
using System.Windows.Forms;

// 在 NuGet 加入
using System.Text.Json;

// 在 NuGet 加入
using Microsoft.WindowsAPICodePack.Dialogs;

namespace GameModTranslateCopyTool
{
    internal class Program
    {
        // 要使用 System.Windows.Forms 或是 Microsoft.WindowsAPICodePack.Dialogs 來開啟檔案瀏覽視窗，就要下 [STAThread] 這個標籤
        [STAThread]
        public static void Main(string[] args)
        {
            string startupPath = Application.StartupPath;
            Console.WriteLine($"此檔案位置: {startupPath}");
            
            // 設置檔案為嵌入資源：
            // 在「Solution Explorer」中，右鍵點擊 JSON 檔案，選擇「Properties」。
            // 將「Build Action」設置為「Embedded Resource」。
            // 嵌入資源的名稱通常為 "<Namespace>.<Folder>.<FileName>"
            string jsonPath = "GameModTranslateCopyTool.Properties.TransWords.json";
            TranslateTool translateTool = new TranslateTool();
            translateTool.SetJson(jsonPath);
            while (true)
            {
                bool exit = false;
                var needConvert = WaitReadLine("選擇進行的動作：\n1. 轉換 ANSI 成 UTF8\n2. 單純轉換字典\n3. 只複製某個副檔名的檔案\n其他：下一步");
                switch (needConvert.ToLower())
                {
                    case "1":
                        var path = SelectDirectory("要轉碼的目錄");
                        if (string.IsNullOrEmpty(path))
                        {
                            Console.WriteLine($"位置不可以為空，path: {path}");
                            break;
                        }
                        var textFinder = new CHTextFinder();
                        textFinder.Find(path);
                        break;
                    case "2":
                        translateTool.OnlyReplaceDictionary();
                        break;
                    case "3":
                        var extension = WaitReadLine("請輸入副檔名，例如，'.txt'：");
                        translateTool.CopySomeExtensionFile(extension);
                        break;
                    default:
                        exit = true;
                        break;
                }
                if (exit)
                {
                    break;
                }
            }

            while (true)
            {
                var line = WaitReadLine("選擇目標遊戲 Mod 根目錄？\n0. 離開\n1. Rim World\n2. Project Zomboid");
                bool selected = false;
                switch (line.ToLower())
                {
                    case "0":
                        Console.WriteLine("Exit.");
                        return;
                    case "1":
                        translateTool.translateSetting = new CNandCTSetting("ChineseSimplified", "ChineseTraditional");
                        selected = true;
                        break;
                    case "2":
                        translateTool.translateSetting = new CNandCTSetting(@"Translate\CN", @"Translate\CH", "_CN", "_CH");
                        selected = true;
                        break;
                }

                if (selected == false)
                {
                    Console.WriteLine("重新選擇");
                }
                else
                {
                    break;
                }
            }
            

            string input = string.Empty;
            while (input.ToLower().Equals("exit") == false)
            {;
                input = WaitReadLine("選擇要做什麼？\n0. 離開\n1. 複製只有簡體的檔案\n2. 複製所有正體中文檔案");
                switch (input.ToLower())
                {
                    case "0":
                        input = "exit";
                        break;
                    case "1":
                        translateTool.CopyOnlyChineseSimplified();
                        break;
                    case "2":
                        translateTool.CopyCHT();
                        break;
                }
            }

            Console.WriteLine("Exit.");
            Console.ReadLine();
        }

        public static string WaitReadLine(string input)
        {
            Console.WriteLine(input);
            var line = Console.ReadLine();
            Console.WriteLine();
            return line;
        }
        
        public static string SelectDirectory(string title)
        {
            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = title;
                folderDialog.IsFolderPicker = true;
                CommonFileDialogResult dialog = folderDialog.ShowDialog();
                if (dialog == CommonFileDialogResult.Ok)
                {
                    var path = folderDialog.FileName;
                    Console.WriteLine($"資料夾路徑: {path}");
                    return path;
                }

                Console.WriteLine($"取消");
                return string.Empty;
            }
        }

        static void OpenFileTest()
        {
            using (var fileDialog = new CommonOpenFileDialog())
            {
                fileDialog.Title = "檔案開啟測試--選取檔案";
                var show = fileDialog.ShowDialog();
                if (show == CommonFileDialogResult.Ok)
                {
                    var path = fileDialog.FileName;
                    Console.WriteLine($"檔案路徑: {path}");
                }
                else
                {
                    Console.WriteLine($"取消");
                } 
            }
        }

        static void OpenFolderTest()
        {
            using (var folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = "資料夾開啟測試--選取資料夾";
                
                // 是不是選擇資料夾模式，true 只能選擇資料夾，false 只能選擇檔案
                folderDialog.IsFolderPicker = true;
                var dialog = folderDialog.ShowDialog();
                if (dialog == CommonFileDialogResult.Ok)
                {
                    var path = folderDialog.FileName;
                    Console.WriteLine($"資料夾路徑: {path}");
                }
                else
                {
                    Console.WriteLine($"取消");  
                }
            }
        }
    }
}