using CefSharp.DevTools.Overlay;
using LittleWarGameClient.Properties;
using Loyc.Syntax;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LittleWarGameClient
{
    internal static class Program
    {
        internal static FontFamily? LWG_FONT;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            string font_filename = "lwgFont.ttf";
            if (!File.Exists(font_filename))
                File.WriteAllBytes(font_filename, Resources.LcdSolidFont);
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(font_filename);
            LWG_FONT = pfc.Families[0];
            Application.Run(OverlayForm.Instance);
        }
    }
}