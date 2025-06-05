using System.Windows;
using FFXIVIMDicGenerator.UI;

namespace FFXIVIMDicGenerator
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序主入口点
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var application = new App();
            var mainWindow = new MainWindow();
            application.Run(mainWindow);
        }
    }
}