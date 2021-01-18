using System;
using System.Windows.Forms;

namespace ResponseAnalyzer
{
    static class Program
    {
        public static bool DISTRIBUTE = false;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ResponseAnalyzer());
        }
    }
}
