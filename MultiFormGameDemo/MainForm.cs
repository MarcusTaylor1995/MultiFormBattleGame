using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiFormGameDemo
{
    public partial class MainForm : Form
    {
        // This is the variable that controls which form to load
        // By making it public and static, it is available from
        // any other form. Otherwise, it would be difficult for
        // one form to pass information back to this form.
        public static string NextForm = "";

        // Initial state

        public MainForm()
        {
            InitializeComponent();

        }



        private void buttonStartGame_Click(object sender, EventArgs e)
        {
            NextForm = "FirstForm";

            this.Visible = false;

            while (NextForm != "GameOver")
            {
                Form theForm = null;
                switch (NextForm)
                {
                    case "FirstForm":
                        theForm = new FirstForm();
                        break;
                    case "SecondForm":
                        theForm = new SecondForm();
                        break;
                    case "ThirdForm":
                        theForm = new ThirdForm();
                        break;
                }
                // If you get a NullReferenceException here, it means
                // you didn't set NextForm to a valid value.
                theForm.StartPosition = FormStartPosition.CenterScreen;
                theForm.ControlBox = false;
                theForm.MaximizeBox = false;
                theForm.MinimizeBox = false;
                theForm.ShowIcon = false;
                theForm.ShowInTaskbar = false;
                theForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                theForm.ShowDialog();
            }

            this.Visible = true;
        }

        
    }
}
