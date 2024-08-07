using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LittleWarGameClient
{
    public partial class SplashScreen : Form
    {
        private static SplashScreen? formInstance;
        internal static SplashScreen Instance
        {
            get
            {
                if (formInstance == null || formInstance.IsDisposed)
                    formInstance = new SplashScreen();
                return formInstance;
            }
        }

        public SplashScreen()
        {
            InitializeComponent();
            if (Program.LWG_FONT != null)
            {
                splashText.Font = new Font(Program.LWG_FONT, 24F, FontStyle.Regular, GraphicsUnit.Point);
                splashText.ForeColor = Color.White;
            }
        }

        internal void InvokeUI(Action a)
        {
            if (formInstance != null && formInstance.InvokeRequired)
            {
                if (formInstance.IsHandleCreated)
                    formInstance.BeginInvoke(new MethodInvoker(a));
            }
            else
            {
                a.Invoke();
            }
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            if (Program.IsDoubleInstance)
            {
                Application.Exit();
            }
        }
    }
}
