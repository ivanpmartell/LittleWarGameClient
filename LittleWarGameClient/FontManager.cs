using LittleWarGameClient.Properties;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static nud2dlib.Windows.Forms.Win32;

namespace LittleWarGameClient
{
    internal class FontManager
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
