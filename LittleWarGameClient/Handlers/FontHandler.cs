using LittleWarGameClient.Properties;
using System.Drawing.Text;

namespace LittleWarGameClient.Handlers
{
    internal class FontHandler
    {
        private static FontFamily? gameFont;
        internal static FontFamily lwgFont
        {
            get
            {
                if (gameFont == null)
                {
                    string font_filename = "lwgFont.ttf";
                    if (!File.Exists(font_filename))
                        File.WriteAllBytes(font_filename, Resources.LcdSolidFont);
                    PrivateFontCollection pfc = new();
                    pfc.AddFontFile(font_filename);
                    gameFont = pfc.Families[0];
                }
                return gameFont;
            }
        }
    }
}
