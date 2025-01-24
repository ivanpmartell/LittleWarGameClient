using LittleWarGameClient.Properties;
using System.Drawing.Text;

namespace LittleWarGameClient.Handlers
{
    internal class FontHandler
    {
        private static FontFamily? lwgFontFamily = null;
        internal static FontFamily gameFontFamily
        {
            get
            {
                if (lwgFontFamily == null)
                {
                    string font_filename = "lwgFont.ttf";
                    if (!File.Exists(font_filename))
                        File.WriteAllBytes(font_filename, Resources.LcdSolidFont);
                    PrivateFontCollection pfc = new();
                    pfc.AddFontFile(font_filename);
                    lwgFontFamily = pfc.Families[0];
                }
                return lwgFontFamily;
            }
        }

        internal static Font gameFont(float size)
        {
            return new Font(gameFontFamily, size, FontStyle.Regular, GraphicsUnit.Point);
        }
    }
}
