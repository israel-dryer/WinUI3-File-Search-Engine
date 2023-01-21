using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System;
using Windows.Storage.Pickers;
using WinRT.Interop;
using WinUIFileSearchEngine.Enums;
using WinUIFileSearchEngine.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WinUIFileSearchEngine.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string searchFolderPath;
    [ObservableProperty]
    private string searchTerm;
    [ObservableProperty]
    private int searchType;
    [ObservableProperty]
    private bool searchIsEnabled;
    [ObservableProperty]
    private bool progressIsEnabled;
    [ObservableProperty]
    private ObservableCollection<SearchResult> searchResults;
    private readonly BackgroundWorker backgroundWorker;

    public MainViewModel()
    {
        // Initial UI properties
        SearchTerm = "txt";
        SearchIsEnabled = true;
        SearchType = (int)SearchTypes.EndsWith;
        SearchResults = new();
        var startFolder = Environment.SpecialFolder.MyDocuments;
        SearchFolderPath = Environment.GetFolderPath(startFolder);

        // Setup background worker
        backgroundWorker = new()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };
        backgroundWorker.DoWork += BackgroundWorker_DoWork;
        backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
    }

    private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        var result = e.UserState as SearchResult;
        if (!backgroundWorker.CancellationPending)
            SearchResults.Add(result);
    }

    private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        SearchIsEnabled = true;
        ProgressIsEnabled = false;
        CancelBackgroundTasks();
    }

    private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        // Identify the search pattern used to locate files
        string searchPattern() => (SearchTypes)SearchType switch
        {
            SearchTypes.Contains => $"*{SearchTerm}*",
            SearchTypes.StartsWith => $"{SearchTerm}*",
            _ => $"{SearchTerm}*"
        };

        // Create an enumerator and extract the relevant files
        var fileQuery = Directory.EnumerateFiles(
            SearchFolderPath, searchPattern(), SearchOption.AllDirectories);

        foreach (var file in fileQuery)
        {
            var fileInfo = new FileInfo(file);
            backgroundWorker.ReportProgress(0, new SearchResult()
            {
                Name = fileInfo.Name,
                ModifiedDate = fileInfo.LastWriteTime,
                Type = fileInfo.Extension,
                Size = fileInfo.Length,
                Path = fileInfo.DirectoryName
            });
            Thread.Sleep(10);
        }

    }

    public void GetSearchResults()
    {
        SearchResults.Clear();
        SearchIsEnabled = false;
        ProgressIsEnabled = true;
        backgroundWorker.RunWorkerAsync();
    }

    public async void GetFolderPath(IntPtr hwnd)
    {
        var picker = new FolderPicker()
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            ViewMode = PickerViewMode.List
        };
        InitializeWithWindow.Initialize(picker, hwnd);
        var storageFolder = await picker.PickSingleFolderAsync();
        if (storageFolder != null)
            SearchFolderPath = storageFolder.Path;
    }

    public void CancelBackgroundTasks()
    {
        backgroundWorker.CancelAsync();
    }
}
