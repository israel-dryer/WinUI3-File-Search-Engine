// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using WinUIEx;
using WinUIFileSearchEngine.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIFileSearchEngine;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : WindowEx
{
    public MainViewModel ViewModel { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        this.SetIcon("Assets/search.ico");
        Closed += (object sender, WindowEventArgs e) => ViewModel.CancelBackgroundTasks();
        ViewModel = new(this);
    }
}
