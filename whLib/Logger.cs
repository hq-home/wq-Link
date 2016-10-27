using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wowhead
{
    public class Logger
    {
        private bool IsConsole
        {
            get { return string.IsNullOrEmpty(_path); }
        }

        private string _path;
        public Logger(string path)
        {
            _path = path;
        }

        public void Debug(string s)
        {
            string fs = string.Format("{0} [DEBUG] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), s);
            if (IsConsole)
                Console.WriteLine(fs);
            else
                File.AppendAllText(_path, fs + Environment.NewLine, Encoding.UTF8);
        }
    }
}
