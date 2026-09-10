using AppLaunchMenu.ViewModels;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewModelDialogContent : Page
    {
        private ModalDialog m_objModalDialog;
        private ViewModelNotifyBase m_objViewModel;
        private StackPanel m_objStackPanel;
        private Collection<FrameworkElement> m_objInputControls = new Collection<FrameworkElement>();

        public ViewModelDialogContent(ModalDialog p_objModalDialog, ViewModelNotifyBase p_objViewModel)
        {
            m_objViewModel = p_objViewModel;

            this.InitializeComponent();
            DataContext = p_objViewModel;

            m_objStackPanel = BuildDynamicForm(p_objViewModel);

            Content = new ScrollViewer
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Content = m_objStackPanel
            };

            InvalidateMeasure();

            m_objModalDialog = p_objModalDialog;
            m_objModalDialog.Closed += ModalDialog_Closed;
        }

        private void ModalDialog_Closed(object sender, WindowEventArgs args)
        {
            ContentDialogResult objContentDialogResult = m_objModalDialog.DialogResult;

            if (objContentDialogResult == ContentDialogResult.None)
                UpdateProperties(m_objViewModel);
        }

        private StackPanel BuildDynamicForm(ViewModelNotifyBase p_objViewModel)
        {
            StackPanel objPropertiesStackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Padding = new Thickness(10)
            };

            Expander objInheritedExpander = new Expander
            {
                Header = "Advanced Settings",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                IsExpanded = false
            };

            StackPanel objInheritedStackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Padding = new Thickness(10)
            };

            objInheritedExpander.Content = objInheritedStackPanel;

            Type objType = p_objViewModel.GetType();
            bool blnAdvancedSettings = false;

            foreach (PropertyInfo objPropertyInfo in objType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                bool blnIsInherited = objPropertyInfo.DeclaringType != objPropertyInfo.ReflectedType;

                DialogContentAttribute? objDialogContentAttribute = objPropertyInfo.GetCustomAttribute<DialogContentAttribute>();
                if (objDialogContentAttribute != null)
                {
                    TextBlock objLabel = new TextBlock
                    {
                        Name = "MainPanel",
                        Text = objDialogContentAttribute.Label,
                        FontWeight = Microsoft.UI.Text.FontWeights.Bold
                    };
                    FrameworkElement objInputControl = CreateControlForProperty(objPropertyInfo, p_objViewModel);

                    if (blnIsInherited)
                    {
                        blnAdvancedSettings = true;

                        objInheritedStackPanel.Children.Add(objLabel);
                        objInheritedStackPanel.Children.Add(objInputControl);
                    }
                    else
                    {
                        objPropertiesStackPanel.Children.Add(objLabel);
                        objPropertiesStackPanel.Children.Add(objInputControl);
                    }

                    m_objInputControls.Add(objInputControl);
                }
            }

            if (blnAdvancedSettings)
                objPropertiesStackPanel.Children.Add(objInheritedExpander);

            objPropertiesStackPanel.InvalidateMeasure();

            return objPropertiesStackPanel;
        }

        /// <summary>
        /// Creates an appropriate input control for a property type.
        /// </summary>
        private FrameworkElement CreateControlForProperty(PropertyInfo objPropertyInfo, object objObject)
        {
            object? objValue = objPropertyInfo.GetValue(objObject);

            if (objPropertyInfo.PropertyType == typeof(bool))
            {
                return new CheckBox
                {
                    Name = objPropertyInfo.Name,
                    IsChecked = (bool?)objValue ?? false
                };
            }
            else if (objPropertyInfo.PropertyType == typeof(DateTime))
            {
                return new CalendarDatePicker
                {
                    Name = objPropertyInfo.Name,
                    Date = (DateTimeOffset?)(objValue != null ? new DateTimeOffset((DateTime)objValue) : null)
                };
            }
            else if (objPropertyInfo.PropertyType == typeof(DateTimeOffset))
            {
                return new CalendarDatePicker
                {
                    Name = objPropertyInfo.Name,
                    Date = (DateTimeOffset?)(objValue != null ? objValue : null)
                };
            }
            else
            {
                return new TextBox
                {
                    Name = objPropertyInfo.Name,
                    Text = objValue?.ToString() ?? string.Empty
                };
            }
        }

        private void UpdateProperties(ViewModelNotifyBase p_objViewModel)
        {
            Type objType = p_objViewModel.GetType();
            foreach (PropertyInfo objPropertyInfo in objType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                DialogContentAttribute? objDialogContentAttribute = objPropertyInfo.GetCustomAttribute<DialogContentAttribute>();
                if (objDialogContentAttribute != null)
                {
                    foreach (FrameworkElement objFrameworkElement in m_objInputControls)
                    {
                        if (objFrameworkElement.Name == objPropertyInfo.Name)
                        {
                            if ((objPropertyInfo.PropertyType == typeof(bool))
                                && (objFrameworkElement is CheckBox objCheckBox)
                                )
                            {
                                objPropertyInfo.SetValue(p_objViewModel, objCheckBox.IsChecked);
                            }
                            else if (((objPropertyInfo.PropertyType == typeof(DateTime)) || (objPropertyInfo.PropertyType == typeof(DateTimeOffset)))
                                && (objFrameworkElement is CalendarDatePicker objCalendarDatePicker)
                                )
                            {
                                objPropertyInfo.SetValue(p_objViewModel, objCalendarDatePicker.Date);
                            }
                            else if ((objPropertyInfo.PropertyType == typeof(string) || (objPropertyInfo.PropertyType.IsPrimitive))
                                && (objFrameworkElement is TextBox objTextBox)
                                )
                            {
                                objPropertyInfo.SetValue(p_objViewModel, objTextBox.Text);
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}