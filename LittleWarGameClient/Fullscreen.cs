using Microsoft.Web.WebView2.WinForms;

namespace LittleWarGameClient
{
    internal class Fullscreen
    {
        readonly Form TargetForm;
        readonly Settings settings;

        FormWindowState PreviousWindowState;

        internal Fullscreen(Form targetForm, Settings s)
        {
            TargetForm = targetForm;
            PreviousWindowState = TargetForm.WindowState;
            settings = s;
            if (settings.GetFullScreen())
                Enter();
            else
                Leave();
        }
        internal void Toggle()
        {
            bool state;
            if (TargetForm.WindowState == FormWindowState.Maximized && TargetForm.FormBorderStyle == FormBorderStyle.None)
            {
                state = false;
                Leave();
            }
            else
            {
                state = true;
                Enter();
            }
            settings.SetFullScreen(state);
        }

        private void Enter()
        {
            if (TargetForm.WindowState != FormWindowState.Maximized || TargetForm.FormBorderStyle != FormBorderStyle.None)
            {
                PreviousWindowState = TargetForm.WindowState;
                TargetForm.WindowState = FormWindowState.Normal;
                TargetForm.FormBorderStyle = FormBorderStyle.None;
                TargetForm.WindowState = FormWindowState.Maximized;
            }
        }

        private void Leave()
        {
            TargetForm.FormBorderStyle = FormBorderStyle.Sizable;
            TargetForm.WindowState = PreviousWindowState;
        }
    }
}
