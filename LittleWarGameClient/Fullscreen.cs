namespace LittleWarGameClient
{
    internal class Fullscreen
    {
        readonly Form gameForm;
        readonly Settings settings;

        FormWindowState PreviousWindowState;

        internal Fullscreen(Form targetForm, Settings s)
        {
            gameForm = targetForm;
            PreviousWindowState = gameForm.WindowState;
            settings = s;
            if (settings.GetFullScreen())
                Enter();
            else
                Leave();
        }
        internal void Toggle()
        {
            bool state;
            if (gameForm.WindowState == FormWindowState.Maximized && gameForm.FormBorderStyle == FormBorderStyle.None)
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
            settings.SaveAsync();
        }

        private void Enter()
        {
            if (gameForm.WindowState != FormWindowState.Maximized || gameForm.FormBorderStyle != FormBorderStyle.None)
            {
                PreviousWindowState = gameForm.WindowState;
                gameForm.WindowState = FormWindowState.Normal;
                gameForm.FormBorderStyle = FormBorderStyle.None;
                gameForm.WindowState = FormWindowState.Maximized;
            }
        }

        private void Leave()
        {
            gameForm.FormBorderStyle = FormBorderStyle.Sizable;
            gameForm.WindowState = PreviousWindowState;
        }
    }
}
