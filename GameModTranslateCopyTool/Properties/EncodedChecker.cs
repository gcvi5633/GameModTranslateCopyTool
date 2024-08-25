using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GameModTranslateCopyTool
{

    public class EncodedChecker
    {
        public void StartCheck(string filePath, string rootPath)
        {
            if (File.Exists(filePath) == false)
            {
                ShowRedLine($"{filePath} is not found.");
                return;
            }

            if (filePath.Contains(".txt") == false)
            {
                ShowRedLine($"Skip, {filePath} not txt.");
                return;
            }

            bool show = false;

            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
            {
                var original = sr.ReadToEnd();
                var b = original.ToCharArray();
                if (b.Any(x => ((int)x).Equals(65533)))
                {
                    show = true;
                }

                sr.Close();
            }

            if (show)
            {
                Encoding encoding = Encoding.Default;
                using (StreamReader sr = new StreamReader(filePath, Encoding.Default, true))
                {
                    encoding = sr.CurrentEncoding;
                    var txt = sr.ReadToEnd();
                    byte[] b = Encoding.Default.GetBytes(txt); //將字串轉為byte[]
                    // Console.WriteLine(Encoding.Default.GetString(b)); //驗證轉碼後的字串
                    byte[] c =
                        Encoding.Convert(Encoding.Default, Encoding.UTF8,
                            b); //進行轉碼,參數1,來源編碼,參數二,目標編碼,參數三,欲編碼變數
                    var ddd = Encoding.UTF8.GetString(c);
                    var eee = ddd.Substring(0, ddd.Length > 300 ? 300 : ddd.Length);
                    filePath = filePath.Replace(rootPath, "");
                    Console.WriteLine($"Path: {filePath}, Encode: {encoding.BodyName}");
                    Console.WriteLine("瀏覽： " + eee); //顯示轉為UTF8後,顯示字串
                    sr.Close();

                    Console.WriteLine("是否要儲存轉換後的檔案？ y/other");
                    var input = Console.ReadLine();
                    switch (input)
                    {
                        case "y":
                            File.WriteAllText(filePath, ddd);
                            Console.WriteLine("已轉換");
                            break;
                        default:
                            Console.WriteLine("略過");
                            break;
                    }
                }
            }
        }

        void ShowRedLine(string content)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(content);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

}