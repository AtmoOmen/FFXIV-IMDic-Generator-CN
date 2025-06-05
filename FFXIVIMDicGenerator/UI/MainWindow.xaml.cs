using System.Windows;
using Microsoft.Win32;
using FFXIVIMDicGenerator.UI.ViewModels;
using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FFXIVIMDicGenerator.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            viewModel = new MainViewModel();
            DataContext = viewModel;
            
            // 在ViewModel初始化后，我们需要重新设置这些需要UI交互的命令
        }

        /// <summary>
        /// 处理文件夹浏览
        /// </summary>
        private void OnBrowseFolder()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "选择包含 .csv 数据文件的目录";
            
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                viewModel.SetFolderPath(dialog.FileName);
            }
        }

        /// <summary>
        /// 处理在线链接文件编辑
        /// </summary>
        private void OnEditOnlineLinks()
        {
            var filePath = viewModel.OnlineLinksFilePath;
            if (File.Exists(filePath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"无法打开文件: {ex.Message}", "错误", 
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("链接文件不存在", "错误", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
} 