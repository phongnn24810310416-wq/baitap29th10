using System;
using System.Windows.Forms;
namespace Bai54Hotel
{
    internal static class Program
    {
        [STAThread] static void Main() { Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); Application.Run(new GUI.MainForm()); }
    }
}
