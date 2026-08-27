using AppLaunchMenu.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEditor;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScriptEditor : Page
    {
        ScriptViewModel m_objScriptViewModel;

        public ScriptEditor(ModalDialog p_objModalDialog, ScriptViewModel p_objScriptViewModel)
        {
            m_objScriptViewModel = p_objScriptViewModel;

            this.InitializeComponent();
            DataContext = p_objScriptViewModel;

            Editor.Editor.SetText(p_objScriptViewModel.Code);
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            // Needs to set focus explicitly due to WinUI 3 regression https://github.com/microsoft/microsoft-ui-xaml/issues/8816 
            ((Control)sender).Focus(FocusState.Programmatic);
        }
    }
}