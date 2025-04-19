namespace ZenthosClient
{
    partial class MonitorUI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorUI));
            WView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            NotifyIcon = new NotifyIcon(components);
            MenuStrip = new ContextMenuStrip(components);
            exitToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)WView2).BeginInit();
            MenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // WView2
            // 
            WView2.AllowExternalDrop = true;
            WView2.CreationProperties = null;
            WView2.DefaultBackgroundColor = Color.White;
            WView2.Dock = DockStyle.Fill;
            WView2.Location = new Point(0, 0);
            WView2.Name = "WView2";
            WView2.Size = new Size(554, 171);
            WView2.TabIndex = 0;
            WView2.ZoomFactor = 1D;
            WView2.WebMessageReceived += WView2_WebMessageReceived;
            // 
            // NotifyIcon
            // 
            NotifyIcon.Icon = (Icon)resources.GetObject("NotifyIcon.Icon");
            NotifyIcon.Text = "ZenthosClient";
            NotifyIcon.Visible = true;
            // 
            // MenuStrip
            // 
            MenuStrip.Items.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            MenuStrip.Name = "MenuStrip";
            MenuStrip.Size = new Size(94, 26);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(93, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // MonitorUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(554, 171);
            Controls.Add(WView2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MonitorUI";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ZenthosClient";
            ((System.ComponentModel.ISupportInitialize)WView2).EndInit();
            MenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 WView2;
        private NotifyIcon NotifyIcon;
        private ContextMenuStrip MenuStrip;
        private ToolStripMenuItem exitToolStripMenuItem;
    }
}
