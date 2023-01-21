// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;
using WinUIFileSearchEngine.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIFileSearchEngine;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : WinUIEx.WindowEx
{
    public MainViewModel ViewModel { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        this.SetIcon("Assets/search.ico");
        Closed += MainWindow_Closed;
        ViewModel = new();
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        ViewModel.CancelBackgroundTasks();
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.GetSearchResults();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.GetFolderPath(this.GetWindowHandle());
    }
}
