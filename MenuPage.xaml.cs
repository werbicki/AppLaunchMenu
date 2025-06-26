using AppLaunchMenu.Dialogs;
using AppLaunchMenu.ViewModels;
using CommunityToolkit.WinUI.Controls;
using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuPage : Page, INotifyPropertyChanged
    {
        private LaunchMenu m_objLaunchMenu;
        private readonly MenuViewModel m_objMenuViewModel;
        private bool m_blnDragDropEnabled = true;
        private bool m_blnLoaded = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        public MenuPage(LaunchMenu p_objLaunchMenu, MenuViewModel p_objMenuViewModel)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objMenuViewModel = p_objMenuViewModel;

            this.InitializeComponent();

            this.DataContext = m_objMenuViewModel;

            m_objLaunchMenu.PropertyChanged += LaunchMenu_PropertyChanged;
        }

        private void ParentPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_blnLoaded)
            {
                m_blnLoaded = true;
                TreeViewItemWidth = new GridLength(m_objTreeView.ActualWidth);
            }
        }

        private void LaunchMenu_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditMode))
            {
                OnPropertyChanged(nameof(EditMode));
                OnPropertyChanged(nameof(DragDropEnabled));
            }
        }

        private MenuViewModel Menu
        {
            get { return m_objMenuViewModel; }
        }

        private bool EditMode
        {
            get { return m_objLaunchMenu.EditMode; }
        }

        private bool DragDropEnabled
        {
            get
            {
                if (!EditMode)
                    return false;
                return m_blnDragDropEnabled;
            }
            set
            {
                m_blnDragDropEnabled = value;
                OnPropertyChanged(nameof(DragDropEnabled));
            }
        }

        private GridLength TreeViewItemWidth
        {
            get { return m_objMenuViewModel.TreeViewItemWidth; }
            set
            {
                m_objMenuViewModel.TreeViewItemWidth = value;
                OnPropertyChanged(nameof(TreeViewItemWidth));
            }
        }

        private void Apps_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            object? objItem = args.InvokedItem;

            if ((objItem != null) && (objItem.GetType() == typeof(ApplicationViewModel)))
            {
                ApplicationViewModel objApplicationViewModel = (ApplicationViewModel)objItem;
                m_objLaunchMenu.SelectedApplication = objApplicationViewModel.Application;
            }
        }

        private void Apps_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            object? objItem = null;
            
            if (sender.GetType() == typeof(TreeView))
                objItem = ((TreeView)sender).SelectedItem;

            if ((objItem != null) && (objItem.GetType() == typeof(ApplicationViewModel)))
            {
                ApplicationViewModel objApplicationViewModel = (ApplicationViewModel)objItem;
                m_objLaunchMenu.SelectedApplication = objApplicationViewModel.Application;

                m_objLaunchMenu.Execute();
            }
        }

        private void HideConextMenus()
        {
            m_objNewFolderContextMenu.Hide();
            m_objFolderContextMenu.Hide();
            m_objNewApplicationContextMenu.Hide();
            m_objApplicationContextMenu.Hide();
            m_objNewEnvironmentContextMenu.Hide();
            m_objEnvironmentContextMenu.Hide();
            m_objVariableContextMenu.Hide();
        }

        private void TreeViewItem_FolderContextMenu(UIElement sender, ContextRequestedEventArgs args)
        {
            if (sender is TreeViewItem objItem)
            {
                FlyoutShowOptions objFlyoutShowOptions = new FlyoutShowOptions();
                objFlyoutShowOptions.ShowMode = FlyoutShowMode.Standard;
                m_objFolderContextMenuEdit.Tag = objItem.DataContext;
                m_objFolderContextMenuDelete.Tag = objItem.DataContext;
                m_objFolderContextMenu.ShowAt(sender, objFlyoutShowOptions);
            }
        }

        private void TreeViewItem_ApplicationContextMenu(UIElement sender, ContextRequestedEventArgs args)
        {
            if (sender is TreeViewItem objItem)
            {
                FlyoutShowOptions objFlyoutShowOptions = new FlyoutShowOptions();
                objFlyoutShowOptions.ShowMode = FlyoutShowMode.Standard;
                m_objApplicationContextMenuEdit.Tag = objItem.DataContext;
                m_objApplicationContextMenuDelete.Tag = objItem.DataContext;
                m_objApplicationContextMenu.ShowAt(sender, objFlyoutShowOptions);
            }
        }

        private void TreeViewItem_EnvironmentContextMenu(UIElement sender, ContextRequestedEventArgs args)
        {
            if (sender is TreeViewItem objItem)
            {
                FlyoutShowOptions objFlyoutShowOptions = new FlyoutShowOptions();
                objFlyoutShowOptions.ShowMode = FlyoutShowMode.Standard;
                m_objEnvironmentContextMenuDelete.Tag = objItem.DataContext;
                m_objEnvironmentContextMenu.ShowAt(sender, objFlyoutShowOptions);
            }
        }

        private void TreeViewItem_VariableContextMenu(UIElement sender, ContextRequestedEventArgs args)
        {
            if (sender is TreeViewItem objItem)
            {
                FlyoutShowOptions objFlyoutShowOptions = new FlyoutShowOptions();
                objFlyoutShowOptions.ShowMode = FlyoutShowMode.Standard;
                m_objVariableContextMenuEdit.Tag = objItem.DataContext;
                m_objVariableContextMenuDelete.Tag = objItem.DataContext;
                m_objVariableContextMenu.ShowAt(sender, objFlyoutShowOptions);
            }
        }

        static int m_intCount = 0;

        private void OnNewFolderContextMenu(object sender, RoutedEventArgs e)
        {
            FlyoutShowOptions objFlyoutShowOptions = new FlyoutShowOptions();
            objFlyoutShowOptions.ShowMode = FlyoutShowMode.Standard;
            m_objNewFolderContextMenu.ShowAt((DependencyObject)sender, objFlyoutShowOptions);
        }

        private void OnNewApplicationContextMenu(object sender, RoutedEventArgs e)
        {
            FlyoutShowOptions objFlyoutShowOptions = new FlyoutShowOptions();
            objFlyoutShowOptions.ShowMode = FlyoutShowMode.Standard;
            m_objNewApplicationContextMenu.ShowAt((DependencyObject)sender, objFlyoutShowOptions);
        }

        private void OnNewEnvironmentContextMenu(object sender, RoutedEventArgs e)
        {
            FlyoutShowOptions objFlyoutShowOptions = new FlyoutShowOptions();
            objFlyoutShowOptions.ShowMode = FlyoutShowMode.Standard;
            m_objNewEnvironmentContextMenu.ShowAt((DependencyObject)sender, objFlyoutShowOptions);
        }

        private async void OnNewFolderClick(object sender, RoutedEventArgs e)
        {
            HideConextMenus();

            if (e.OriginalSource is AppBarButton)
            {
                AppBarButton objButton = (AppBarButton)e.OriginalSource;
                if (objButton.DataContext != null)
                {
                    TreeViewItemViewModel objItem = (TreeViewItemViewModel)objButton.DataContext;
                    if (objItem != null)
                    {
                        FolderViewModel? objFolderViewModel = m_objMenuViewModel?.CreateFolder("New Folder " + m_intCount++);
                        if (objFolderViewModel != null)
                        {
                            PromptDialog objDialog = new()
                            {
                                XamlRoot = this.Content.XamlRoot,
                                Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                                //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                                Title = "New",
                                Content = new FolderDialogContent(objFolderViewModel),
                                CloseButtonText = "OK",
                                PrimaryButtonText = "Cancel",
                                DefaultButton = ContentDialogButton.Primary,
                            };

                            ContentDialogResult objContentDialogResult = await objDialog.ShowAsync();
                            if (objContentDialogResult == ContentDialogResult.None)
                                objItem.Children.Add(objFolderViewModel);
                        }
                    }
                }
            }
        }

        private async void OnNewApplicationClick(object sender, RoutedEventArgs e)
        {
            HideConextMenus();

            if (e.OriginalSource is AppBarButton)
            {
                AppBarButton objButton = (AppBarButton)e.OriginalSource;
                if (objButton.DataContext != null)
                {
                    TreeViewItemViewModel objItem = (TreeViewItemViewModel)objButton.DataContext;
                    if (objItem != null)
                    {
                        ApplicationViewModel? objApplicationViewModel = m_objMenuViewModel?.CreateApplication("New Application " + m_intCount++);
                        if (objApplicationViewModel != null)
                        {
                            PromptDialog objDialog = new()
                            {
                                XamlRoot = this.Content.XamlRoot,
                                Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                                //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                                Title = "New",
                                Content = new ApplicationDialogContent(objApplicationViewModel),
                                CloseButtonText = "OK",
                                PrimaryButtonText = "Cancel",
                                DefaultButton = ContentDialogButton.Primary,
                            };

                            ContentDialogResult objContentDialogResult = await objDialog.ShowAsync();
                            if (objContentDialogResult == ContentDialogResult.None)
                                objItem.Children.Add(objApplicationViewModel);
                        }
                    }
                }
            }
        }

        private async void OnNewVariableClick(object sender, RoutedEventArgs e)
        {
            HideConextMenus();

            if (e.OriginalSource is AppBarButton)
            {
                AppBarButton objButton = (AppBarButton)e.OriginalSource;
                if (objButton.DataContext != null)
                {
                    TreeViewItemViewModel objItem = (TreeViewItemViewModel)objButton.DataContext;
                    if (objItem != null)
                    {
                        VariableViewModel? objVariableViewModel = null;

                        {
                            if (objItem is FolderViewModel objFolderViewModel)
                                objVariableViewModel = objFolderViewModel.CreateVariable("New Variable " + m_intCount++);
                            else if (objItem is ApplicationViewModel objApplicationViewModel)
                                objVariableViewModel = objApplicationViewModel.CreateVariable("New Variable " + m_intCount++);
                        }

                        if (objVariableViewModel != null)
                        {
                            PromptDialog objDialog = new()
                            {
                                XamlRoot = this.Content.XamlRoot,
                                Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                                //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                                Title = "New",
                                Content = new VariableDialogContent(objVariableViewModel),
                                CloseButtonText = "OK",
                                PrimaryButtonText = "Cancel",
                                DefaultButton = ContentDialogButton.Primary,
                            };

                            ContentDialogResult objContentDialogResult = await objDialog.ShowAsync();
                            if (objContentDialogResult == ContentDialogResult.None)
                            {
                                if (objItem is FolderViewModel objFolderViewModel)
                                    objFolderViewModel.Environment.Variables.Add(objVariableViewModel);
                                else if (objItem is ApplicationViewModel objApplicationViewModel)
                                    objApplicationViewModel.Environment.Variables.Add(objVariableViewModel);
                            }
                        }
                    }
                }
            }
        }

        private async void OnEditClick(object sender, RoutedEventArgs e)
        {
            HideConextMenus();

            if (e.OriginalSource is AppBarButton)
            {
                AppBarButton objButton = (AppBarButton)e.OriginalSource;
                if (objButton.DataContext != null)
                {
                    TreeViewItemViewModel objItem = (TreeViewItemViewModel)objButton.Tag;
                    if (objItem != null)
                    {
                        PromptDialog objRenameDialog = new()
                        {
                            XamlRoot = this.Content.XamlRoot,
                            Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                            //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                            Title = "Edit",
                            DefaultButton = ContentDialogButton.Close,
                            //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                        };

                        if (objItem is FolderViewModel)
                            objRenameDialog.Content = new FolderDialogContent((FolderViewModel)objItem);
                        else if (objItem is ApplicationViewModel)
                            objRenameDialog.Content = new ApplicationDialogContent((ApplicationViewModel)objItem);
                        else if (objItem is VariableViewModel)
                            objRenameDialog.Content = new VariableDialogContent((VariableViewModel)objItem);
                        else
                            throw new ArgumentException();

                        ContentDialogResult objContentDialogResult = await objRenameDialog.ShowAsync();
                        if (objContentDialogResult == ContentDialogResult.None)
                            objButton.DataContext = objItem;
                    }
                }
            }
        }

        private async void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            HideConextMenus();

            if (e.OriginalSource is AppBarButton)
            {
                AppBarButton objButton = (AppBarButton)e.OriginalSource;
                if (objButton.DataContext != null)
                {
                    TreeViewItemViewModel? objItem = (TreeViewItemViewModel)objButton.Tag;
                    if (objItem != null)
                    {
                        ContentDialog objExitDialog = new ContentDialog
                        {
                            XamlRoot = this.Content.XamlRoot,
                            Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                            //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                            Title = "Delete",
                            Content = "Would you like to delete '" + objItem.Name + "'",
                            CloseButtonText = "OK",
                            PrimaryButtonText = "Cancel",
                            DefaultButton = ContentDialogButton.Primary
                        };

                        ContentDialogResult objResult = await objExitDialog.ShowAsync();
                        if (objResult != ContentDialogResult.Primary)
                            objItem?.Parent?.Children.Remove(objItem);
                    }
                }
            }
            else if (e.OriginalSource is Button)
            {
                Button objButton = (Button)e.OriginalSource;
                if (objButton.Parent is RelativePanel)
                {
                    RelativePanel objRelativePanel = (RelativePanel)objButton.Parent;
                    if (objRelativePanel.Parent is TreeViewItem)
                    {
                        TreeViewItem objTreeViewItem = (TreeViewItem)objRelativePanel.Parent;
                        if (objTreeViewItem.Tag != null)
                        {
                            TreeViewItemViewModel? objItem = (TreeViewItemViewModel)objTreeViewItem.Tag;
                            if (objItem != null)
                            {
                                ContentDialog objExitDialog = new ContentDialog
                                {
                                    XamlRoot = this.Content.XamlRoot,
                                    Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                                    //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                                    Title = "Delete",
                                    Content = "Would you like to delete '" + objItem.Name + "'",
                                    CloseButtonText = "OK",
                                    PrimaryButtonText = "Cancel",
                                    DefaultButton = ContentDialogButton.Primary
                                };

                                ContentDialogResult objResult = await objExitDialog.ShowAsync();
                                if (objResult != ContentDialogResult.Primary)
                                    objItem?.Parent?.Children.Remove(objItem);
                            }
                        }
                    }
                }
            }
        }

        private void m_objTreeListView_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            DragDropEnabled = false;
        }

        private void m_objTreeListView_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.OriginalSource is GridSplitter)
            {
                GridSplitter objGridSplitter = (GridSplitter)e.OriginalSource;

                if (objGridSplitter.Parent is Grid)
                {
                    Grid objGrid = (Grid)objGridSplitter.Parent;
                    TreeViewItemWidth = objGrid.ColumnDefinitions[0].Width;
                }
            }
        }

        private void m_objTreeListView_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.OriginalSource is GridSplitter)
            {
                GridSplitter objGridSplitter = (GridSplitter)e.OriginalSource;

                if (objGridSplitter.Parent is Grid)
                {
                    Grid objGrid = (Grid)objGridSplitter.Parent;
                    TreeViewItemWidth = objGrid.ColumnDefinitions[0].Width;
                }
            }

            DragDropEnabled = true;
        }
    }

    class TreeViewItemTemplateSelector : DataTemplateSelector
    {
        // Template to use for folder items in the TreeView.
        public DataTemplate? FolderTreeViewItemTemplate { get; set; }

        // Template to use for file items in the TreeView.
        public DataTemplate? ApplicationTreeViewItemTemplate { get; set; }

        // Template to use for file items in the TreeView.
        public DataTemplate? EnvironmentTreeViewItemTemplate { get; set; }

        // Template to use for file items in the TreeView.
        public DataTemplate? VariableTreeViewItemTemplate { get; set; }

        // Determines which template to use for each item in the TreeView based on its type.
        protected override DataTemplate? SelectTemplateCore(object item)
        {
            TreeViewItemViewModel objTreeViewItemViewModel = (TreeViewItemViewModel)item;

            if (objTreeViewItemViewModel is VariableViewModel)
                return VariableTreeViewItemTemplate;
            else if (objTreeViewItemViewModel is EnvironmentViewModel)
                return EnvironmentTreeViewItemTemplate;
            else if (objTreeViewItemViewModel is ApplicationViewModel)
                return ApplicationTreeViewItemTemplate;
            else
                return FolderTreeViewItemTemplate;
        }
    }
}
