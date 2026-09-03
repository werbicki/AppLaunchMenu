using CommunityToolkit.WinUI.Converters;
using Windows.UI.Text;

namespace AppLaunchMenu.Helpers
{
    /// <summary>
    /// This class converts a boolean value into a Visibility enumeration.
    /// </summary>
    public class BoolToTextDecorationsConverter : BoolToObjectConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToTextDecorationConverter"/> class.
        /// </summary>
        public BoolToTextDecorationsConverter()
        {
            TrueValue = TextDecorations.None;
            FalseValue = TextDecorations.Strikethrough;
        }
    }
}
