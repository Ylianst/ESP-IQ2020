using System;
using Gtk;

namespace DataViewer
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Init();
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Run();
        }
    }
}
