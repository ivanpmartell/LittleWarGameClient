using Microsoft.Web.WebView2.WinForms;

namespace LittleWarGameClient
{
    internal class Fullscreen
    {
        Form TargetForm;

        FormWindowState PreviousWindowState;

        internal Fullscreen(Form targetForm)
        {
            TargetForm = targetForm;
        }
        internal void Toggle()
        {
            if (TargetForm.WindowState == FormWindowState.Maximized && TargetForm.FormBorderStyle == FormBorderStyle.None)
            {
                Leave();
            }
            else
            {
                Enter();
            }
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
