using System;
using System.Windows.Forms;

namespace WinFormsApp4_registration_
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StudentsForm());
        }
    }
}
