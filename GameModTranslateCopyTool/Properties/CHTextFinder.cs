using System.IO;

namespace GameModTranslateCopyTool
{
    public class CHTextFinder
    {
        EncodedChecker EncodedChecker
        {
            get
            {
                if (_encodedChecker == null)
                {
                    _encodedChecker = new EncodedChecker();
                }
                return _encodedChecker;
            }
        }
        private EncodedChecker _encodedChecker = null;

        private string _rootPath = string.Empty;
        public void Find(string rootPath)
        {
            DirectoryInfo info = new DirectoryInfo(rootPath);
            _rootPath = rootPath;
            CheckDirCHS(info, rootPath);
        }
        
        void CheckDirCHS(DirectoryInfo info, string newPath)
        {
            DirectoryInfo[] directories = info.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                string dirPath = $"{newPath}\\{directories[i].Name}";
                CheckDirCHS(directories[i],dirPath);
            }

            if (info.FullName.Contains("Translate\\CH"))
            {
                var files = info.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    EncodedChecker.StartCheck(files[i].FullName, _rootPath);
                } 
            }
        } 
    } 
}