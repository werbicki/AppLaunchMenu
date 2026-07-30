using AppLaunchMenu.ViewModels;
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
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewModelDialogContent : Page
    {
        private ViewModelNotifyBase m_objViewModel;

        public ViewModelDialogContent(ViewModelNotifyBase p_objViewModel)
        {
            m_objViewModel = p_objViewModel;

            this.InitializeComponent();
            DataContext = p_objViewModel;

            StackPanel objStackPanel = BuildDynamicForm(p_objViewModel);

            Content = new ScrollViewer
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Content = objStackPanel
            };

            InvalidateMeasure();
        }

        private StackPanel BuildDynamicForm(ViewModelNotifyBase p_objViewModel)
        {
            var objStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Padding = new Thickness(10)
            };

            var objType = p_objViewModel.GetType();
            foreach (var objProperty in objType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = objProperty.GetCustomAttribute<DialogContentAttribute>();
                if (attr != null)
                {
                    // Label
                    objStackPanel.Children.Add(new TextBlock
                    {
                        Text = attr.Label,
                        FontWeight = Microsoft.UI.Text.FontWeights.Bold
                    });

                    // Input control based on property type
                    FrameworkElement objInputControl = CreateControlForProperty(objProperty, p_objViewModel);
                    objStackPanel.Children.Add(objInputControl);
                }
            }

            objStackPanel.InvalidateMeasure();

            return objStackPanel;
        }

        /// <summary>
        /// Creates an appropriate input control for a property type.
        /// </summary>
        private FrameworkElement CreateControlForProperty(PropertyInfo objPropertyInfo, object objObject)
        {
            var objValue = objPropertyInfo.GetValue(objObject);

            if (objPropertyInfo.PropertyType == typeof(string) || objPropertyInfo.PropertyType.IsPrimitive)
            {
                return new TextBox
                {
                    Text = objValue?.ToString() ?? string.Empty
                };
            }
            else if (objPropertyInfo.PropertyType == typeof(bool))
            {
                return new CheckBox
                {
                    IsChecked = (bool?)objValue ?? false
                };
            }
            else if (objPropertyInfo.PropertyType == typeof(DateTime))
            {
                return new CalendarDatePicker
                {
                    Date = (DateTimeOffset?)(objValue != null ? new DateTimeOffset((DateTime)objValue) : null)
                };
            }
            else
            {
                // Fallback for unsupported types
                return new TextBlock
                {
                    Text = objValue?.ToString() ?? "(null)",
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray)
                };
            }
        }
    }
}
