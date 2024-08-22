// See https://aka.ms/new-console-template for more information
using System;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace GameModTranslateCopyTool
{
    internal class Program
    {
        // 要使用 System.Windows.Forms 或是 Microsoft.WindowsAPICodePack.Dialogs 來開啟檔案瀏覽視窗，就要下 [STAThread] 這個標籤
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("!HelloWorld!");
        }
    }
}