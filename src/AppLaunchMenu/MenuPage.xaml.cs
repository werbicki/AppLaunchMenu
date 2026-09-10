using AppLaunchMenu.DataModels;
using AppLaunchMenu.Dialogs;
using AppLaunchMenu.ViewModels;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Controls;
using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu
{
    public struct ViewModelTag
    {
        public Type Type { get; set; }
        public ITreeViewItem? Parent { get; set;  }
        public ITreeViewItem Item { get; set;  }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MenuPage : PageNotifyPropertyChanged
    {
        private LaunchMenu m_objLaunchMenu;
        private readonly MenuViewModel m_objMenuViewModel;
        private bool m_blnDragDropEnabled = true;
        private bool m_blnLoaded = false;

        public MenuPage(LaunchMenu p_objLaunchMenu, MenuViewModel p_objMenuViewModel)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objMenuViewModel = p_objMenuViewModel;

            this.InitializeComponent();

            this.DataContext = this;
            m_objTreeView.DataContext = m_objMenuViewModel;

            m_objLaunchMenu.PropertyChanged += LaunchMenu_PropertyChanged;
        }

        private double GetTreeViewItemWidth()
        {
            return GetTreeViewItemWidth(m_objMenuViewModel.Children);
        }

        private double GetTreeViewItemWidth(ObservableCollection<ITreeViewItem> p_objChildren, double p_dblWidth = 0)
        {
            double dblWidth = p_dblWidth;

            foreach (ITreeViewItem objTreeViewItemViewModel in p_objChildren)
            {
                TreeViewItem? objTreeViewItem = (TreeViewItem)m_objTreeView.ContainerFromItem(objTreeViewItemViewModel);
                if (objTreeViewItem != null)
                {
                    object objContent = objTreeViewItem.Content;
                    if ((objContent != null)
                        && ((objContent.GetType().Equals(typeof(RelativePanel)))
                            || (objContent.GetType().GetTypeInfo().IsSubclassOf(typeof(RelativePanel)))
                        ))
                    {
                        RelativePanel objRelativePanel = (RelativePanel)objContent;
                        double dblContentWidth = 0;

                        objRelativePanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                        // We measure the TreeItemView Image, Text and Edit commands to get the column width.

                        foreach (var objItem in objRelativePanel.FindChildren().OfType<Microsoft.UI.Xaml.Controls.Image>())
                            dblContentWidth += objItem.DesiredSize.Width;

                        foreach (var objItem in objRelativePanel.FindChildren().OfType<TextBlock>())
                            dblContentWidth += objItem.DesiredSize.Width;

                        foreach (var objItem in objRelativePanel.FindChildren().OfType<Button>())
                        {
                            Visibility enumVisibility = objItem.Visibility;

                            // Manually set the Visibility to measure in the correct state because the bindings may not
                            // have updated.

                            if (EditMode)
                                objItem.Visibility = Visibility.Visible;
                            else
                                objItem.Visibility = Visibility.Collapsed;

                            objItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            dblContentWidth += objItem.DesiredSize.Width;

                            objItem.Visibility = enumVisibility;
                        }

                        if (dblContentWidth > dblWidth)
                            dblWidth = dblContentWidth;
                    }
                }

                dblWidth = GetTreeViewItemWidth(objTreeViewItemViewModel.Children, dblWidth);
            }

            return dblWidth;
        }

        private void ParentPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_blnLoaded)
            {
                m_blnLoaded = true;

                MenuViewModel.TreeViewItemMinWidth = GetTreeViewItemWidth();
            }
        }

        private void LaunchMenu_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditMode))
            {
                OnPropertyChanged(nameof(EditMode));
                OnPropertyChanged(nameof(DragDropEnabled));

                MenuViewModel.TreeViewItemMinWidth = GetTreeViewItemWidth();
            }
        }

        private MenuViewModel MenuViewModel
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
                if (EditMode)
                    return m_blnDragDropEnabled;
                return false;
            }
            set
            {
                m_blnDragDropEnabled = value;
                OnPropertyChanged(nameof(DragDropEnabled));
            }
        }

        private void Apps_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            object? objItem = args.InvokedItem;

            if ((objItem != null) && (objItem.GetType() == typeof(ApplicationViewModel)))
            {
                ApplicationViewModel objApplicationViewModel = (ApplicationViewModel)objItem;
                m_objLaunchMenu.SelectedApplication = objApplicationViewModel;
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
                m_objLaunchMenu.SelectedApplication = objApplicationViewModel;

                m_objLaunchMenu.Execute();
            }
        }

        private static int m_intCount = 0;

        private void HideConextMenus()
        {
            m_objAddContextMenu.Hide();
            m_objAddEditDeleteContextMenu.Hide();
            m_objEditDeleteContextMenu.Hide();
        }

        private async void OnNewClick(object sender, RoutedEventArgs e)
        {
            HideConextMenus();

            if (EditMode)
            {
                ViewModelTag? objViewModelTag = null;

                if (e.OriginalSource is AppBarButton objAppBarButton)
                {
                    if (objAppBarButton.Tag is ViewModelTag)
                        objViewModelTag = (ViewModelTag)objAppBarButton.Tag;
                }
                else if (e.OriginalSource is MenuFlyoutItem objMenuFlyoutItem)
                {
                    if (objMenuFlyoutItem.Tag is ViewModelTag)
                        objViewModelTag = (ViewModelTag)objMenuFlyoutItem.Tag;
                }

                if (objViewModelTag != null)
                {
                    ITreeViewItem? objParentViewModel = objViewModelTag?.Parent;
                    Type? objNewType = objViewModelTag?.Type;

                    if ((objParentViewModel != null)
                        && (objNewType != null)
                        )
                    {
                        ViewModelNotifyBase? objViewModel = null;

                        objViewModel = objParentViewModel.NewChild(objNewType, "New Variable " + m_intCount++, objParentViewModel);

                        if (objViewModel != null)
                        {
                            ModalDialog objNewItemDialog = new ModalDialog()
                            {
                                //Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                                //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                                Title = "New",
                                CloseButtonText = "OK",
                                PrimaryButtonText = "Cancel",
                                DefaultButton = ContentDialogButton.Primary,
                            };

                            if (objNewType == typeof(ScriptViewModel))
                                objNewItemDialog.Page = new ScriptEditor(objNewItemDialog, (ScriptViewModel)objViewModel);
                            else
                                objNewItemDialog.Page = new ViewModelDialogContent(objNewItemDialog, objViewModel);

                            ContentDialogResult objContentDialogResult = await objNewItemDialog.ShowAsync();

                            if (objContentDialogResult == ContentDialogResult.None)
                            {
                                //objParentViewModel.AddChild(objViewModel);
                                objParentViewModel.Children.Add((ITreeViewItem)objViewModel);
                            }
                        }
                    }
                }
            }
        }

        private async void OnEditClick(object sender, RoutedEventArgs e)
        {
            HideConextMenus();

            if (EditMode)
            {
                ViewModelTag? objViewModelTag = null;

                if (e.OriginalSource is AppBarButton objAppBarButton)
                {
                    if (objAppBarButton.Tag is ViewModelTag)
                        objViewModelTag = (ViewModelTag)objAppBarButton.Tag;
                }

                if (objViewModelTag != null)
                {
                    ITreeViewItem? objItemViewModel = objViewModelTag?.Item;
                    Type? objEditType = objViewModelTag?.Type;

                    if ((objItemViewModel != null)
                        && (objEditType != null)
                        )
                    {
                        ModalDialog objEditItemDialog = new ModalDialog()
                        {
                            //Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                            //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                            Title = "Rename",
                            CloseButtonText = "OK",
                            PrimaryButtonText = "Cancel",
                            DefaultButton = ContentDialogButton.Primary,
                        };

                        if (objEditType == typeof(ScriptViewModel))
                            objEditItemDialog.Page = new ScriptEditor(objEditItemDialog, (ScriptViewModel)objItemViewModel);
                        else
                            objEditItemDialog.Page = new ViewModelDialogContent(objEditItemDialog, (ViewModelNotifyBase)objItemViewModel);

                        ContentDialogResult objContentDialogResult = await objEditItemDialog.ShowAsync();

                        //if (objContentDialogResult == ContentDialogResult.None)
                        //    objButton.DataContext = objTreeViewItemViewModel;
                    }
                }
            }
        }

        private async void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            HideConextMenus();

            if (EditMode)
            {
                ViewModelTag? objViewModelTag = null;

                if (e.OriginalSource is AppBarButton objAppBarButton)
                {
                    if (objAppBarButton.Tag is ViewModelTag)
                        objViewModelTag = (ViewModelTag)objAppBarButton.Tag;
                }
                else if (e.OriginalSource is Button objButton)
                {
                    ITreeViewItem objTreeViewItem = (ITreeViewItem)objButton.DataContext;

                    objViewModelTag = new ViewModelTag()
                    {
                        Parent = objTreeViewItem.Parent,
                        Item = objTreeViewItem,
                    };
                }

                if (objViewModelTag != null)
                {
                    ITreeViewItem? objParentViewModel = objViewModelTag?.Parent;
                    ITreeViewItem? objItemViewModel = objViewModelTag?.Item;

                    if ((objParentViewModel != null)
                        && (objItemViewModel != null)
                        )
                    {
                        ModalDialog objDeleteDialog = new ModalDialog
                        {
                            //Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                            //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                            Title = "Delete",
                            Message = "Would you like to delete '" + objItemViewModel.Name + "'",
                            CloseButtonText = "OK",
                            PrimaryButtonText = "Cancel",
                            DefaultButton = ContentDialogButton.Primary
                        };

                        ContentDialogResult objResult = await objDeleteDialog.ShowAsync();

                        if (objResult != ContentDialogResult.Primary)
                        {
                            //objTreeViewItemViewModel.RemoveChild(objViewModel);
                            objParentViewModel.Children.Remove((ITreeViewItem)objItemViewModel);
                        }
                    }
                }
            }
        }

        private void TreeViewItem_OnAddContextMenu(object sender, RoutedEventArgs e)
        {
            if (EditMode)
            {
                FlyoutShowOptions objFlyoutShowOptions = new FlyoutShowOptions();
                objFlyoutShowOptions.ShowMode = FlyoutShowMode.Standard;

                if ((sender is Button objButton)
                    && (objButton.Parent is RelativePanel objRelativePanel)
                    && (objRelativePanel.Parent is TreeViewItem)
                    )
                {
                    ITreeViewItem objParentViewModel = (ITreeViewItem)m_objTreeView.ItemFromContainer((TreeViewItem)objRelativePanel.Parent);

                    if (objParentViewModel.ChildNodeTypes.Length > 0)
                    {
                        m_objAddContextMenu.PrimaryCommands.Clear();

                        foreach (Type objType in objParentViewModel.ChildNodeTypes)
                        {
                            var objAppBarButton = new AppBarButton
                            {
                                Label = objType.Name,
                                Tag = new ViewModelTag()
                                {
                                    Type = typeof(Folder),
                                    Parent = objParentViewModel,
                                }
                            };
                            objAppBarButton.Click += OnNewClick;

                            m_objAddContextMenu.PrimaryCommands.Add(objAppBarButton);
                        }

                        m_objAddContextMenu.ShowAt((DependencyObject)sender, objFlyoutShowOptions);
                    }
                }
                else if ((sender is Button objFolderButton)
                    && (objFolderButton.Parent is Grid objGrid)
                    )
                {
                    m_objAddContextMenu.PrimaryCommands.Clear();

                    var objAppBarButton = new AppBarButton
                    {
                        Label = "Folder",
                        Tag = new ViewModelTag()
                        {
                            Type = typeof(Folder),
                            Parent = m_objMenuViewModel,
                        }
                    };
                    objAppBarButton.Click += OnNewClick;

                    m_objAddContextMenu.PrimaryCommands.Add(objAppBarButton);

                    m_objAddContextMenu.ShowAt((DependencyObject)sender, objFlyoutShowOptions);
                }
            }
        }

        private void TreeViewItem_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            if (EditMode)
            {
                if (sender is TreeViewItem objTreeViewItem)
                {
                    if (args.TryGetPosition(sender, out Point objPoint))
                    {
                        FlyoutShowOptions objFlyoutShowOptions = new FlyoutShowOptions()
                        {
                            Placement = FlyoutPlacementMode.Right,
                            Position = objPoint
                        };

                        ITreeViewItem objViewModel = (ITreeViewItem)m_objTreeView.ItemFromContainer(objTreeViewItem);

                        if (objViewModel.ChildNodeTypes.Length > 0)
                        {
                            m_objAddEditDeleteMenuFlyout.Items.Clear();

                            foreach (Type objType in objViewModel.ChildNodeTypes)
                            {
                                var objMenuFlyoutItem = new MenuFlyoutItem
                                {
                                    Text = objType.Name,
                                    Tag = new ViewModelTag()
                                    {
                                        Type = objType,
                                        Parent = objViewModel,
                                    }
                                };
                                objMenuFlyoutItem.Click += OnNewClick;
                                // Retrieve the Style from your Page or App resources
                                if (App.Current.Resources.TryGetValue("CustomMenuFlyoutItemStyle", out var resource) &&
                                    resource is Microsoft.UI.Xaml.Style customStyle)
                                {
                                    objMenuFlyoutItem.Style = customStyle;
                                }

                                m_objAddEditDeleteMenuFlyout.Items.Add(objMenuFlyoutItem);
                            }

                            m_objAddEditDeleteContextMenuEdit.Tag = new ViewModelTag()
                            {
                                Type = objViewModel.GetType(),
                                Item = objViewModel,
                            };

                            m_objAddEditDeleteContextMenuDelete.Tag = new ViewModelTag()
                            {
                                Type = objViewModel.GetType(),
                                Parent = objViewModel.Parent,
                                Item = objViewModel,
                            };

                            m_objAddEditDeleteContextMenu.ShowAt(sender, objFlyoutShowOptions);
                        }
                        else
                        {
                            m_objEditDeleteContextMenuEdit.Tag = new ViewModelTag()
                            {
                                Type = objViewModel.GetType(),
                                Item = objViewModel,
                            };

                            m_objEditDeleteContextMenuDelete.Tag = new ViewModelTag()
                            {
                                Type = objViewModel.GetType(),
                                Parent = objViewModel.Parent,
                                Item = objViewModel,
                            };

                            m_objEditDeleteContextMenu.ShowAt(sender, objFlyoutShowOptions);
                        }
                    }
                }
            }
            else
                args.Handled = true;
        }

        private void TreeViewItemSplitter_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            DragDropEnabled = false;
        }

        private void TreeViewItemSplitter_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.OriginalSource is GridSplitter)
            {
                GridSplitter objGridSplitter = (GridSplitter)e.OriginalSource;

                if (objGridSplitter.Parent is Grid)
                {
                    Grid objGrid = (Grid)objGridSplitter.Parent;
                    MenuViewModel.TreeViewItemWidth = objGrid.ColumnDefinitions[0].Width;
                }
            }
        }

        private void TreeViewItemSplitter_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.OriginalSource is GridSplitter)
            {
                GridSplitter objGridSplitter = (GridSplitter)e.OriginalSource;

                if (objGridSplitter.Parent is Grid)
                {
                    Grid objGrid = (Grid)objGridSplitter.Parent;
                    MenuViewModel.TreeViewItemWidth = objGrid.ColumnDefinitions[0].Width;
                }
            }

            DragDropEnabled = true;
        }
    }

    class TreeViewItemTemplateSelector : DataTemplateSelector
    {
        // Template to use for folder items in the TreeView.
        public DataTemplate? DefaultTreeViewItemTemplate { get; set; }

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
            ITreeViewItem objTreeViewItemViewModel = (ITreeViewItem)item;

            if (objTreeViewItemViewModel is VariableViewModel)
                return VariableTreeViewItemTemplate;
            else if (objTreeViewItemViewModel is EnvironmentViewModel)
                return EnvironmentTreeViewItemTemplate;
            else if (objTreeViewItemViewModel is ApplicationViewModel)
                return ApplicationTreeViewItemTemplate;
            else if (objTreeViewItemViewModel is FolderViewModel)
                return FolderTreeViewItemTemplate;
            else
                return DefaultTreeViewItemTemplate;
        }
    }
}
