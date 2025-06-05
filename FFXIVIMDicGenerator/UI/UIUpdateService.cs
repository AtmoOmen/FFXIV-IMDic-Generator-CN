using System.Windows.Threading;
using FFXIVIMDicGenerator.Models;
using FFXIVIMDicGenerator.UI.ViewModels;

namespace FFXIVIMDicGenerator.UI;

/// <summary>
/// UI更新服务 - 处理WPF UI状态更新逻辑
/// </summary>
public class UIUpdateService
{
    private readonly MainViewModel viewModel;
    private readonly Dispatcher dispatcher;

    public UIUpdateService(MainViewModel viewModel, Dispatcher dispatcher)
    {
        this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public void UpdateProgress(int currentValue, int maxValue = 100)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => UpdateProgress(currentValue, maxValue));
            return;
        }

        viewModel.ProgressMaximum = maxValue;
        viewModel.ProgressValue = Math.Min(currentValue, maxValue);
    }

    public void UpdateProgressWithText(int currentValue, int maxValue, string statusText)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => UpdateProgressWithText(currentValue, maxValue, statusText));
            return;
        }

        UpdateProgress(currentValue, maxValue);
        viewModel.StatusText = statusText;
    }

    public void UpdateLocalFileCount(int fileCount)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => UpdateLocalFileCount(fileCount));
            return;
        }

        viewModel.LocalFileCount = fileCount;
    }

    public void UpdateOnlineLinkCount(int linkCount)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => UpdateOnlineLinkCount(linkCount));
            return;
        }

        viewModel.OnlineLinkCount = linkCount;
    }

    public void EnableControls(bool enabled)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => EnableControls(enabled));
            return;
        }

        viewModel.IsProcessing = !enabled;
    }

    public void SetFolderPath(string folderPath)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => SetFolderPath(folderPath));
            return;
        }

        viewModel.FolderPath = folderPath;
    }

    public void SetLinksFilePath(string filePath)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => SetLinksFilePath(filePath));
            return;
        }

        viewModel.OnlineLinksFilePath = filePath;
    }

    public void ResetProgressBar()
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(ResetProgressBar);
            return;
        }

        viewModel.ProgressValue = 0;
        viewModel.StatusText = "生成";
    }

    public void ShowMessage(string message, string title = "信息", System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Information)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => ShowMessage(message, title, icon));
            return;
        }

        System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.OK, icon);
    }

    public void ShowError(string errorMessage)
    {
        ShowMessage(errorMessage, "错误", System.Windows.MessageBoxImage.Error);
    }

    public void ShowSuccess(string successMessage)
    {
        ShowMessage(successMessage, "成功", System.Windows.MessageBoxImage.Information);
    }

    public void UpdateProcessingStatus(ProcessingResult result)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => UpdateProcessingStatus(result));
            return;
        }

        if (result.IsSuccess)
        {
            UpdateProgressWithText(100, 100, $"处理完成 - 共处理 {result.ProcessedCount} 项");
        }
        else
        {
            UpdateProgressWithText(0, 100, "处理失败");
            ShowError($"处理时发生错误: {result.Message}");
        }
    }

    public void SetControlsEnabled(bool enabled)
    {
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => SetControlsEnabled(enabled));
            return;
        }

        viewModel.IsProcessing = !enabled;
    }

    public async Task ShowProgressiveMessage(string initialMessage, Func<IProgress<string>, Task> operation)
    {
        var progress = new Progress<string>(message =>
        {
            UpdateProgressWithText(viewModel.ProgressValue + 1, viewModel.ProgressMaximum, message);
        });

        UpdateProgressWithText(0, 100, initialMessage);
        
        try
        {
            await operation(progress);
        }
        catch (Exception ex)
        {
            ShowError($"操作失败: {ex.Message}");
        }
    }
} 