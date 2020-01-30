namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using Microsoft.CSharp.RuntimeBinder;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class AuthorizeDialog : LoadMaterialForm
    {
        private IContainer components;
        private PictureBox pictureBox1;
        private MaterialSingleLineTextField txtUsername;
        private MaterialSingleLineTextField txtPassword;
        private MaterialRaisedButton btnAuthorize;
        private PictureBox pictureBox2;
        private LinkLabel lblForgotPassword;
        private MaterialMessage lblPrompt;
        private BackgroundWorker authWorker;
        private BackgroundWorker passwordlessWorker;

        public AuthorizeDialog()
        {
            this.InitializeComponent();
            MaterialSkinManager instance = MaterialSkinManager.Instance;
            instance.Theme = MaterialSkinManager.Themes.LIGHT;
            instance.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey800, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            instance.AddFormToManage(this);
        }

        private void AuthorizeDialog_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Select();
            base.SetSpinning(true);
            this.passwordlessWorker.RunWorkerAsync();
        }

        private void authWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = API.authenticateClassic(this.txtUsername.Text, this.txtPassword.Text);
        }

        private void authWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            object result = e.Result;
            if (<>o__4.<>p__2 == null)
            {
                CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                <>o__4.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(AuthorizeDialog), argumentInfo));
            }
            if (<>o__4.<>p__1 == null)
            {
                CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null) };
                <>o__4.<>p__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof(AuthorizeDialog), argumentInfo));
            }
            if (<>o__4.<>p__0 == null)
            {
                CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                <>o__4.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "error", typeof(AuthorizeDialog), argumentInfo));
            }
            if (<>o__4.<>p__2.Target(<>o__4.<>p__2, <>o__4.<>p__1.Target(<>o__4.<>p__1, <>o__4.<>p__0.Target(<>o__4.<>p__0, result), null)))
            {
                this.lblPrompt.Error = true;
                if (<>o__4.<>p__4 == null)
                {
                    <>o__4.<>p__4 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(AuthorizeDialog)));
                }
                if (<>o__4.<>p__3 == null)
                {
                    CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                    <>o__4.<>p__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "error", typeof(AuthorizeDialog), argumentInfo));
                }
                this.lblPrompt.Text = <>o__4.<>p__4.Target(<>o__4.<>p__4, <>o__4.<>p__3.Target(<>o__4.<>p__3, result));
            }
            else
            {
                if (<>o__4.<>p__6 == null)
                {
                    <>o__4.<>p__6 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(string), typeof(AuthorizeDialog)));
                }
                if (<>o__4.<>p__5 == null)
                {
                    CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                    <>o__4.<>p__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "privkey", typeof(AuthorizeDialog), argumentInfo));
                }
                KeyManager.setEncodedPrivateKey(<>o__4.<>p__6.Target(<>o__4.<>p__6, <>o__4.<>p__5.Target(<>o__4.<>p__5, result)));
                if (<>o__4.<>p__8 == null)
                {
                    <>o__4.<>p__8 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(bool), typeof(AuthorizeDialog)));
                }
                if (<>o__4.<>p__7 == null)
                {
                    CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                    <>o__4.<>p__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "guard", typeof(AuthorizeDialog), argumentInfo));
                }
                if (<>o__4.<>p__8.Target(<>o__4.<>p__8, <>o__4.<>p__7.Target(<>o__4.<>p__7, result)))
                {
                    new AccountGuard().ShowDialog();
                }
                base.Hide();
                new MainForm().ShowDialog();
            }
            base.SetSpinning(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblPrompt = new MaterialMessage();
            this.txtUsername = new MaterialSingleLineTextField();
            this.txtPassword = new MaterialSingleLineTextField();
            this.btnAuthorize = new MaterialRaisedButton();
            this.lblForgotPassword = new LinkLabel();
            this.pictureBox2 = new PictureBox();
            this.pictureBox1 = new PictureBox();
            this.authWorker = new BackgroundWorker();
            this.passwordlessWorker = new BackgroundWorker();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.lblPrompt.BackColor = SystemColors.ControlLight;
            this.lblPrompt.Depth = 0;
            this.lblPrompt.Error = false;
            this.lblPrompt.Font = new Font("Roboto", 11f);
            this.lblPrompt.ForeColor = Color.FromArgb(0xde, 0, 0, 0);
            this.lblPrompt.Location = new Point(13, 0x4c);
            this.lblPrompt.MouseState = MouseState.HOVER;
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Padding = new Padding(10);
            this.lblPrompt.Size = new Size(0x199, 80);
            this.lblPrompt.TabIndex = 1;
            this.lblPrompt.Text = "Looks like this computer has not been authorized yet to run Platinum Cheats. Simply sign in with your Platinum Cheats credentials to get started.";
            this.txtUsername.Depth = 0;
            this.txtUsername.Hint = "Username";
            this.txtUsername.Location = new Point(12, 0xac);
            this.txtUsername.MouseState = MouseState.HOVER;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.PasswordChar = '\0';
            this.txtUsername.SelectedText = "";
            this.txtUsername.SelectionLength = 0;
            this.txtUsername.SelectionStart = 0;
            this.txtUsername.Size = new Size(0x199, 0x17);
            this.txtUsername.TabIndex = 10;
            this.txtUsername.TabStop = false;
            this.txtUsername.UseSystemPasswordChar = false;
            this.txtPassword.Depth = 0;
            this.txtPassword.Hint = "Password";
            this.txtPassword.Location = new Point(12, 0xc9);
            this.txtPassword.MouseState = MouseState.HOVER;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '\0';
            this.txtPassword.SelectedText = "";
            this.txtPassword.SelectionLength = 0;
            this.txtPassword.SelectionStart = 0;
            this.txtPassword.Size = new Size(0x199, 0x17);
            this.txtPassword.TabIndex = 12;
            this.txtPassword.TabStop = false;
            this.txtPassword.UseSystemPasswordChar = true;
            this.btnAuthorize.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.btnAuthorize.Depth = 0;
            this.btnAuthorize.Location = new Point(12, 230);
            this.btnAuthorize.MouseState = MouseState.HOVER;
            this.btnAuthorize.Name = "btnAuthorize";
            this.btnAuthorize.Primary = true;
            this.btnAuthorize.Size = new Size(0x219, 0x20);
            this.btnAuthorize.TabIndex = 13;
            this.btnAuthorize.Text = "AUTHORIZE";
            this.btnAuthorize.UseVisualStyleBackColor = true;
            this.btnAuthorize.Click += new EventHandler(this.materialRaisedButton1_Click);
            this.lblForgotPassword.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.lblForgotPassword.AutoSize = true;
            this.lblForgotPassword.Location = new Point(9, 0x10b);
            this.lblForgotPassword.Name = "lblForgotPassword";
            this.lblForgotPassword.Size = new Size(0x5b, 13);
            this.lblForgotPassword.TabIndex = 15;
            this.lblForgotPassword.TabStop = true;
            this.lblForgotPassword.Text = "Forgot password?";
            this.pictureBox2.BackColor = Color.Transparent;
            this.pictureBox2.Image = Resources.desktop_computer_locked_screen;
            this.pictureBox2.Location = new Point(440, 0x4c);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(0x6d, 0x94);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 14;
            this.pictureBox2.TabStop = false;
            this.pictureBox1.BackColor = Color.Transparent;
            this.pictureBox1.Image = Resources.platinumcheats_wide_compressed_white;
            this.pictureBox1.Location = new Point(0x17d, 0x1d);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0xa8, 0x20);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.authWorker.DoWork += new DoWorkEventHandler(this.authWorker_DoWork);
            this.authWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.authWorker_RunWorkerCompleted);
            this.passwordlessWorker.DoWork += new DoWorkEventHandler(this.passwordlessWorker_DoWork);
            this.passwordlessWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.passwordlessWorker_RunWorkerCompleted);
            base.AcceptButton = this.btnAuthorize;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x233, 0x121);
            base.Controls.Add(this.lblForgotPassword);
            base.Controls.Add(this.pictureBox2);
            base.Controls.Add(this.btnAuthorize);
            base.Controls.Add(this.txtPassword);
            base.Controls.Add(this.txtUsername);
            base.Controls.Add(this.lblPrompt);
            base.Controls.Add(this.pictureBox1);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "AuthorizeDialog";
            base.Sizable = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Authorize this computer";
            base.TopMost = true;
            base.TransparencyKey = Color.FromArgb(0xc0, 0, 0xc0);
            base.Load += new EventHandler(this.AuthorizeDialog_Load);
            base.Controls.SetChildIndex(this.pictureBox1, 0);
            base.Controls.SetChildIndex(this.lblPrompt, 0);
            base.Controls.SetChildIndex(this.txtUsername, 0);
            base.Controls.SetChildIndex(this.txtPassword, 0);
            base.Controls.SetChildIndex(this.btnAuthorize, 0);
            base.Controls.SetChildIndex(this.pictureBox2, 0);
            base.Controls.SetChildIndex(this.lblForgotPassword, 0);
            ((ISupportInitialize) this.pictureBox2).EndInit();
            ((ISupportInitialize) this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            base.SetSpinning(true);
            this.authWorker.RunWorkerAsync();
        }

        private void passwordlessWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = API.authenticatePasswordless();
        }

        private void passwordlessWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PasswordlessResult result = (PasswordlessResult) e.Result;
            if (result.authSuccess)
            {
                if (result.guard)
                {
                    new AccountGuard().ShowDialog();
                }
                base.Hide();
                new MainForm().ShowDialog();
                base.Close();
            }
            else
            {
                base.SetSpinning(false);
            }
        }

        [CompilerGenerated]
        private static class <>o__4
        {
            public static CallSite<Func<CallSite, object, object>> <>p__0;
            public static CallSite<Func<CallSite, object, object, object>> <>p__1;
            public static CallSite<Func<CallSite, object, bool>> <>p__2;
            public static CallSite<Func<CallSite, object, object>> <>p__3;
            public static CallSite<Func<CallSite, object, string>> <>p__4;
            public static CallSite<Func<CallSite, object, object>> <>p__5;
            public static CallSite<Func<CallSite, object, string>> <>p__6;
            public static CallSite<Func<CallSite, object, object>> <>p__7;
            public static CallSite<Func<CallSite, object, bool>> <>p__8;
        }
    }
}

