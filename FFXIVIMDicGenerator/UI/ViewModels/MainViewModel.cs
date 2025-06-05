using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using FFXIVIMDicGenerator.Configuration;
using FFXIVIMDicGenerator.Models;
using FFXIVIMDicGenerator.Services;
using FFXIVIMDicGenerator.UI.Infrastructure;
using FFXIVIMDicGenerator.Utils;

namespace FFXIVIMDicGenerator.UI.ViewModels
{
    /// <summary>
    /// 主窗口ViewModel - 实现MVVM模式的UI状态管理和业务逻辑
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly ICsvProcessingService csvProcessingService;


        private readonly IFileOperationService fileOperationService;
        private readonly IConfigurationService configurationService;

        private string folderPath = "请选择包含 .csv 数据文件的目录 (仅本地生成用)";
        private string onlineLinksFilePath = string.Empty;
        private string windowTitle = $"FFXIV IMDic Generator {ApplicationConstants.LocalVersion}";
        private int localFileCount;
        private int onlineLinkCount;
        private int progressValue;
        private int progressMaximum = 100;
        private string statusText = "生成";
        private bool isProcessing;


        private List<string> onlineLinksFromFile = new();
        private List<string> linksName = new();
        private bool chineseMirrorReplace;

        public MainViewModel()
        {
            // 服务依赖注入 (简化版本)
            fileOperationService = new FileOperationService();
            configurationService = new ConfigurationService(fileOperationService);
            csvProcessingService = new CsvProcessingService();



            InitializeCommands();
            InitializeData();
        }

        #region Properties

        /// <summary>
        /// 窗口标题
        /// </summary>
        public string WindowTitle
        {
            get => windowTitle;
            set => SetProperty(ref windowTitle, value);
        }

        /// <summary>
        /// 本地文件夹路径
        /// </summary>
        public string FolderPath
        {
            get => folderPath;
            set => SetProperty(ref folderPath, value);
        }

        /// <summary>
        /// 在线链接文件路径
        /// </summary>
        public string OnlineLinksFilePath
        {
            get => onlineLinksFilePath;
            set => SetProperty(ref onlineLinksFilePath, value);
        }

        /// <summary>
        /// 本地文件数量
        /// </summary>
        public int LocalFileCount
        {
            get => localFileCount;
            set
            {
                if (SetProperty(ref localFileCount, value))
                {
                    OnPropertyChanged(nameof(LocalFileCountText));
                }
            }
        }

        /// <summary>
        /// 本地文件数量显示文本
        /// </summary>
        public string LocalFileCountText => $"当前文件数: {LocalFileCount}";

        /// <summary>
        /// 在线链接数量
        /// </summary>
        public int OnlineLinkCount
        {
            get => onlineLinkCount;
            set
            {
                if (SetProperty(ref onlineLinkCount, value))
                {
                    OnPropertyChanged(nameof(OnlineLinkCountText));
                }
            }
        }

        /// <summary>
        /// 在线链接数量显示文本
        /// </summary>
        public string OnlineLinkCountText => $"当前链接数: {OnlineLinkCount}";

        /// <summary>
        /// 进度条当前值
        /// </summary>
        public int ProgressValue
        {
            get => progressValue;
            set => SetProperty(ref progressValue, value);
        }

        /// <summary>
        /// 进度条最大值
        /// </summary>
        public int ProgressMaximum
        {
            get => progressMaximum;
            set => SetProperty(ref progressMaximum, value);
        }

        /// <summary>
        /// 状态文本
        /// </summary>
        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        /// <summary>
        /// 是否正在处理
        /// </summary>
        public bool IsProcessing
        {
            get => isProcessing;
            set
            {
                if (SetProperty(ref isProcessing, value))
                {
                    OnPropertyChanged(nameof(IsNotProcessing));
                }
            }
        }

        /// <summary>
        /// 是否未在处理（用于控件启用状态绑定）
        /// </summary>
        public bool IsNotProcessing => !IsProcessing;



        /// <summary>
        /// 在线文件列表
        /// </summary>
        public ObservableCollection<OnlineFileItem> OnlineFileItems { get; } = new();

        #endregion

        #region Commands

        public ICommand BrowseFolderCommand { get; private set; } = null!;
        public ICommand ConvertLocalFilesCommand { get; private set; } = null!;
        public ICommand ConvertOnlineFilesCommand { get; private set; } = null!;
        public ICommand ReloadOnlineLinksCommand { get; private set; } = null!;
        public ICommand EditOnlineLinksCommand { get; private set; } = null!;
        public ICommand OpenFfxivDataminingCommand { get; private set; } = null!;
        public ICommand OpenDeepBlueConverterCommand { get; private set; } = null!;
        public ICommand OpenCsvReferenceCommand { get; private set; } = null!;
        public ICommand ToggleMirrorCommand { get; private set; } = null!;
        public ICommand OpenGitHubCommand { get; private set; } = null!;
        public ICommand SelectAllCommand { get; private set; } = null!;
        public ICommand SelectNoneCommand { get; private set; } = null!;
        public ICommand InvertSelectionCommand { get; private set; } = null!;

        #endregion

        #region Initialization

        private void InitializeCommands()
        {
            BrowseFolderCommand = new RelayCommand(OnBrowseFolder, () => IsNotProcessing);
            ConvertLocalFilesCommand = new RelayCommand(async () => await OnConvertLocalFiles(), () => IsNotProcessing);
            ConvertOnlineFilesCommand = new RelayCommand(async () => await OnConvertOnlineFiles(), () => IsNotProcessing);
            ReloadOnlineLinksCommand = new RelayCommand(OnReloadOnlineLinks, () => IsNotProcessing);
            EditOnlineLinksCommand = new RelayCommand(OnEditOnlineLinks, () => IsNotProcessing);
            OpenFfxivDataminingCommand = new RelayCommand(OnOpenFfxivDatamining);
            OpenDeepBlueConverterCommand = new RelayCommand(OnOpenDeepBlueConverter);
            OpenCsvReferenceCommand = new RelayCommand(OnOpenCsvReference);
            ToggleMirrorCommand = new RelayCommand(OnToggleMirror);
            OpenGitHubCommand = new RelayCommand(OnOpenGitHub);
            SelectAllCommand = new RelayCommand(OnSelectAll);
            SelectNoneCommand = new RelayCommand(OnSelectNone);
            InvertSelectionCommand = new RelayCommand(OnInvertSelection);
        }

        private void InitializeData()
        {
            // 初始化在线文件列表
            InitializeOnlineFileList();
            AnalyzeDomains();
        }

        private void InitializeOnlineFileList()
        {
            OnlineFileItems.Clear();

            foreach (var fileType in FfxivDataSources.FileTypeNames)
            {
                var item = new OnlineFileItem
                {
                    DisplayName = fileType.Key.StartsWith("Seperator") ? fileType.Value : $"{fileType.Key} - {fileType.Value}",
                    Key = fileType.Key,
                    IsChecked = false
                };

                item.PropertyChanged += OnlineFileItem_PropertyChanged;
                OnlineFileItems.Add(item);
            }

            RefreshOnlineComponents();
        }

        private void OnlineFileItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OnlineFileItem.IsChecked))
            {
                RefreshOnlineComponents();
            }
        }

        #endregion

        #region Command Handlers

        private void OnBrowseFolder()
        {
            // 这里需要在View中处理文件夹选择对话框
            // 通过事件或回调来设置FolderPath
        }

        private async Task OnConvertLocalFiles()
        {
            if (!IsValidFolderPath(FolderPath))
            {
                StatusText = "请先选择有效的文件夹";
                return;
            }

            try
            {
                IsProcessing = true;
                StatusText = "正在处理本地文件...";
                await ProcessLocalFiles(FolderPath);
                StatusText = "生成完成！";
                
                // 打开输出文件夹
                fileOperationService.OpenFolder(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                StatusText = $"处理失败: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"本地文件处理异常: {ex}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task OnConvertOnlineFiles()
        {
            if (onlineLinksFromFile.Count == 0)
            {
                StatusText = "请先选择要下载的在线文件";
                return;
            }

            try
            {
                IsProcessing = true;
                ProgressMaximum = onlineLinksFromFile.Count;
                ProgressValue = 0;
                StatusText = "正在处理在线文件...";

                await ProcessOnlineFiles();
                StatusText = "生成完成！";
            }
            catch (Exception ex)
            {
                StatusText = $"处理失败: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"在线文件处理异常: {ex}");
            }
            finally
            {
                IsProcessing = false;
                // 打开输出文件夹
                fileOperationService.OpenFolder(Environment.CurrentDirectory);
            }
        }

        private void OnReloadOnlineLinks()
        {
            AnalyzeDomains();
        }

        private void OnEditOnlineLinks()
        {
            // 在View中处理文件编辑
        }

        private void OnOpenFfxivDatamining()
        {
            ProcessUtils.OpenUrl("https://github.com/thewakingsands/ffxiv-datamining-cn");
        }

        private void OnOpenDeepBlueConverter()
        {
            ProcessUtils.OpenUrl("https://github.com/studyzy/imewlconverter");
        }

        private void OnOpenCsvReference()
        {
            ProcessUtils.OpenUrl("https://github.com/thewakingsands/ffxiv-datamining-cn/tree/master/PlaceName");
        }

        private void OnToggleMirror()
        {
            chineseMirrorReplace = !chineseMirrorReplace;
            configurationService.UpdateMirrorSetting(chineseMirrorReplace);
            AnalyzeDomains();
        }

        private void OnOpenGitHub()
        {
            ProcessUtils.OpenUrl("https://github.com/Kurris/FFXIV-IMDic-Generator-CN");
        }

        private void OnSelectAll()
        {
            foreach (var item in OnlineFileItems)
            {
                if (!item.Key.StartsWith("Seperator"))
                {
                    item.IsChecked = true;
                }
            }
        }

        private void OnSelectNone()
        {
            foreach (var item in OnlineFileItems)
            {
                item.IsChecked = false;
            }
        }

        private void OnInvertSelection()
        {
            foreach (var item in OnlineFileItems)
            {
                if (!item.Key.StartsWith("Seperator"))
                {
                    item.IsChecked = !item.IsChecked;
                }
            }
        }

        #endregion

        #region Business Logic

        private async Task ProcessLocalFiles(string folderPath)
        {
            var csvFiles = Directory.GetFiles(folderPath, "*.csv");
            if (csvFiles.Length == 0)
            {
                throw new InvalidOperationException("文件夹中没有找到任何 CSV 文件");
            }

            var options = new CsvProcessingOptions
            {
                Keywords = ApplicationConstants.CsvKeywords,
                IncludeChineseOnly = true,
                MinimumWordLength = 1,
                RemoveSpecialCharacters = true
            };

            var result = await csvProcessingService.ProcessCsvFilesAsync(csvFiles, options);
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.Message);
            }

            // 直接输出纯汉字格式，一行一词
            var chineseWords = result.ProcessedData.Select(word => word.Trim()).Where(word => !string.IsNullOrEmpty(word)).Distinct();

            var outputFilePath = Path.Combine(Environment.CurrentDirectory, ApplicationConstants.OutputFileName);
            await fileOperationService.WriteAllLinesAsync(outputFilePath, chineseWords);
        }

        private async Task ProcessOnlineFiles()
        {
            var options = new CsvProcessingOptions
            {
                Keywords = ApplicationConstants.CsvKeywords,
                IncludeChineseOnly = true,
                MinimumWordLength = 1,
                RemoveSpecialCharacters = true
            };

            var result = await csvProcessingService.ProcessCsvFilesAsync(onlineLinksFromFile, options);
            ProgressValue = onlineLinksFromFile.Count;

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.Message);
            }

            // 直接输出纯汉字格式，一行一词
            var chineseWords = result.ProcessedData.Select(word => word.Trim()).Where(word => !string.IsNullOrEmpty(word)).Distinct();

            var outputFilePath = Path.Combine(Environment.CurrentDirectory, ApplicationConstants.OutputFileName);
            await fileOperationService.WriteAllLinesAsync(outputFilePath, chineseWords);
        }



        private bool IsValidFolderPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && 
                   path != "请选择包含 .csv 数据文件的目录 (仅本地生成用)" && 
                   Directory.Exists(path);
        }



        private void RefreshOnlineComponents(int param = -1)
        {
            UpdateOnlineFileListSelection();

            var linksConfig = configurationService.LoadOnlineLinksFromFile();
            OnlineLinksFilePath = linksConfig.FilePath;
            OnlineLinkCount = linksConfig.LinkCount;

            onlineLinksFromFile = linksConfig.Links;
            linksName = linksConfig.Names;
        }

        private void UpdateOnlineFileListSelection()
        {
            onlineLinksFromFile.Clear();
            linksName.Clear();

            foreach (var item in OnlineFileItems.Where(x => x.IsChecked))
            {
                if (item.Key.StartsWith("Seperator"))
                    continue;

                if (FfxivDataSources.OnlineLinks.TryGetValue(item.Key, out var link))
                {
                    var processedLink = chineseMirrorReplace ? 
                        configurationService.GetMirrorLink(link) : link;
                    
                    onlineLinksFromFile.Add(processedLink);
                    linksName.Add(item.Key);
                }
            }

            OnlineLinkCount = onlineLinksFromFile.Count;
        }

        private void AnalyzeDomains()
        {
            var linksConfig = configurationService.LoadOnlineLinksFromFile();
            onlineLinksFromFile = linksConfig.Links;
            linksName = linksConfig.Names;
            chineseMirrorReplace = configurationService.GetMirrorSetting();

            RefreshOnlineComponents();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 设置文件夹路径（由View调用）
        /// </summary>
        public void SetFolderPath(string path)
        {
            FolderPath = path;
            if (IsValidFolderPath(path))
            {
                LocalFileCount = fileOperationService.CountCsvFiles(path);
            }
        }

        /// <summary>
        /// 更新进度（由业务服务调用）
        /// </summary>
        public void UpdateProgress(int current, int maximum, string status)
        {
            ProgressValue = current;
            ProgressMaximum = maximum;
            StatusText = status;
        }

        #endregion
    }

    /// <summary>
    /// 在线文件项数据模型
    /// </summary>
    public class OnlineFileItem : BaseViewModel
    {
        private bool isChecked;
        private string displayName = string.Empty;
        private string key = string.Empty;

        public bool IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }

        public string DisplayName
        {
            get => displayName;
            set => SetProperty(ref displayName, value);
        }

        public string Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }
    }
} 
