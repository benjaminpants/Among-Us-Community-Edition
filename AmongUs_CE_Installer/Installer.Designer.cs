
namespace AmongUs_CE_Installer
{
    partial class Installer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Installer));
            this.PasswordBox = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.InstallLocationBox = new System.Windows.Forms.MaskedTextBox();
            this.InstallLocationButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.InstallButton = new System.Windows.Forms.Button();
            this.LogBox = new System.Windows.Forms.RichTextBox();
            this.InstallOption = new System.Windows.Forms.RadioButton();
            this.InstallMethodGroup = new System.Windows.Forms.GroupBox();
            this.UpgradeOption = new System.Windows.Forms.RadioButton();
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.SteamInputGroup = new System.Windows.Forms.GroupBox();
            this.WhyMySteamInfoLink = new System.Windows.Forms.LinkLabel();
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.SplitContainer2 = new System.Windows.Forms.SplitContainer();
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CleanFilesCheckbox = new System.Windows.Forms.CheckBox();
            this.InstallMethodGroup.SuspendLayout();
            this.SteamInputGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer2)).BeginInit();
            this.SplitContainer2.Panel1.SuspendLayout();
            this.SplitContainer2.Panel2.SuspendLayout();
            this.SplitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PasswordBox
            // 
            this.PasswordBox.Location = new System.Drawing.Point(6, 78);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.PasswordChar = '●';
            this.PasswordBox.Size = new System.Drawing.Size(455, 20);
            this.PasswordBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Steam Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Steam Password:";
            // 
            // InstallLocationBox
            // 
            this.InstallLocationBox.Location = new System.Drawing.Point(6, 42);
            this.InstallLocationBox.Name = "InstallLocationBox";
            this.InstallLocationBox.Size = new System.Drawing.Size(419, 20);
            this.InstallLocationBox.TabIndex = 4;
            // 
            // InstallLocationButton
            // 
            this.InstallLocationButton.Location = new System.Drawing.Point(431, 42);
            this.InstallLocationButton.Name = "InstallLocationButton";
            this.InstallLocationButton.Size = new System.Drawing.Size(30, 20);
            this.InstallLocationButton.TabIndex = 5;
            this.InstallLocationButton.Text = "...";
            this.InstallLocationButton.UseVisualStyleBackColor = true;
            this.InstallLocationButton.Click += new System.EventHandler(this.InstallLocationButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Destination:";
            // 
            // InstallButton
            // 
            this.InstallButton.Location = new System.Drawing.Point(12, 443);
            this.InstallButton.Name = "InstallButton";
            this.InstallButton.Size = new System.Drawing.Size(467, 44);
            this.InstallButton.TabIndex = 7;
            this.InstallButton.Text = "Install";
            this.InstallButton.UseVisualStyleBackColor = true;
            this.InstallButton.Click += new System.EventHandler(this.InstallButton_Click);
            // 
            // LogBox
            // 
            this.LogBox.BackColor = System.Drawing.Color.Black;
            this.LogBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogBox.ForeColor = System.Drawing.Color.White;
            this.LogBox.Location = new System.Drawing.Point(0, 0);
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.LogBox.Size = new System.Drawing.Size(467, 222);
            this.LogBox.TabIndex = 9;
            this.LogBox.Text = "";
            // 
            // InstallOption
            // 
            this.InstallOption.AutoSize = true;
            this.InstallOption.Location = new System.Drawing.Point(337, 19);
            this.InstallOption.Name = "InstallOption";
            this.InstallOption.Size = new System.Drawing.Size(52, 17);
            this.InstallOption.TabIndex = 11;
            this.InstallOption.Text = "Install";
            this.InstallOption.UseVisualStyleBackColor = true;
            this.InstallOption.CheckedChanged += new System.EventHandler(this.InstallOption_CheckedChanged);
            // 
            // InstallMethodGroup
            // 
            this.InstallMethodGroup.Controls.Add(this.CleanFilesCheckbox);
            this.InstallMethodGroup.Controls.Add(this.InstallOption);
            this.InstallMethodGroup.Controls.Add(this.UpgradeOption);
            this.InstallMethodGroup.Controls.Add(this.InstallLocationButton);
            this.InstallMethodGroup.Controls.Add(this.InstallLocationBox);
            this.InstallMethodGroup.Controls.Add(this.label3);
            this.InstallMethodGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InstallMethodGroup.ForeColor = System.Drawing.SystemColors.ControlText;
            this.InstallMethodGroup.Location = new System.Drawing.Point(0, 0);
            this.InstallMethodGroup.MaximumSize = new System.Drawing.Size(467, 100);
            this.InstallMethodGroup.MinimumSize = new System.Drawing.Size(467, 100);
            this.InstallMethodGroup.Name = "InstallMethodGroup";
            this.InstallMethodGroup.Size = new System.Drawing.Size(467, 100);
            this.InstallMethodGroup.TabIndex = 13;
            this.InstallMethodGroup.TabStop = false;
            this.InstallMethodGroup.Text = "Install Options:";
            // 
            // UpgradeOption
            // 
            this.UpgradeOption.AutoSize = true;
            this.UpgradeOption.Location = new System.Drawing.Point(395, 19);
            this.UpgradeOption.Name = "UpgradeOption";
            this.UpgradeOption.Size = new System.Drawing.Size(66, 17);
            this.UpgradeOption.TabIndex = 12;
            this.UpgradeOption.Text = "Upgrade";
            this.UpgradeOption.UseVisualStyleBackColor = true;
            this.UpgradeOption.CheckedChanged += new System.EventHandler(this.UpgradeOption_CheckedChanged);
            // 
            // UsernameBox
            // 
            this.UsernameBox.Location = new System.Drawing.Point(6, 39);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(455, 20);
            this.UsernameBox.TabIndex = 0;
            // 
            // SteamInputGroup
            // 
            this.SteamInputGroup.Controls.Add(this.WhyMySteamInfoLink);
            this.SteamInputGroup.Controls.Add(this.label1);
            this.SteamInputGroup.Controls.Add(this.UsernameBox);
            this.SteamInputGroup.Controls.Add(this.PasswordBox);
            this.SteamInputGroup.Controls.Add(this.label2);
            this.SteamInputGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SteamInputGroup.Location = new System.Drawing.Point(0, 0);
            this.SteamInputGroup.Name = "SteamInputGroup";
            this.SteamInputGroup.Size = new System.Drawing.Size(467, 100);
            this.SteamInputGroup.TabIndex = 14;
            this.SteamInputGroup.TabStop = false;
            this.SteamInputGroup.Text = "Steam Login:";
            // 
            // WhyMySteamInfoLink
            // 
            this.WhyMySteamInfoLink.AutoSize = true;
            this.WhyMySteamInfoLink.Location = new System.Drawing.Point(236, 23);
            this.WhyMySteamInfoLink.Name = "WhyMySteamInfoLink";
            this.WhyMySteamInfoLink.Size = new System.Drawing.Size(225, 13);
            this.WhyMySteamInfoLink.TabIndex = 13;
            this.WhyMySteamInfoLink.TabStop = true;
            this.WhyMySteamInfoLink.Text = "Why do I need to Input my Steam information?";
            this.WhyMySteamInfoLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WhyMySteamInfoLink_LinkClicked);
            // 
            // SplitContainer
            // 
            this.SplitContainer.Location = new System.Drawing.Point(12, 10);
            this.SplitContainer.Name = "SplitContainer";
            this.SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.InstallMethodGroup);
            this.SplitContainer.Panel1MinSize = 70;
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.SplitContainer2);
            this.SplitContainer.Panel2MinSize = 120;
            this.SplitContainer.Size = new System.Drawing.Size(467, 430);
            this.SplitContainer.SplitterDistance = 100;
            this.SplitContainer.TabIndex = 11;
            // 
            // SplitContainer2
            // 
            this.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer2.Location = new System.Drawing.Point(0, 0);
            this.SplitContainer2.Name = "SplitContainer2";
            this.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer2.Panel1
            // 
            this.SplitContainer2.Panel1.Controls.Add(this.SteamInputGroup);
            this.SplitContainer2.Panel1MinSize = 70;
            // 
            // SplitContainer2.Panel2
            // 
            this.SplitContainer2.Panel2.Controls.Add(this.LogBox);
            this.SplitContainer2.Size = new System.Drawing.Size(467, 326);
            this.SplitContainer2.SplitterDistance = 100;
            this.SplitContainer2.TabIndex = 4;
            // 
            // PictureBox
            // 
            this.PictureBox.BackgroundImage = global::AmongUs_CE_Installer.Properties.Resources.Banner;
            this.PictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PictureBox.Location = new System.Drawing.Point(485, 10);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(227, 430);
            this.PictureBox.TabIndex = 12;
            this.PictureBox.TabStop = false;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(485, 443);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(227, 44);
            this.label4.TabIndex = 13;
            this.label4.Text = "Installer By: \r\nCarJem Generations\r\nMissingTextureMan101 © 2021\r\n";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CleanFilesCheckbox
            // 
            this.CleanFilesCheckbox.AutoSize = true;
            this.CleanFilesCheckbox.Location = new System.Drawing.Point(6, 64);
            this.CleanFilesCheckbox.Name = "CleanFilesCheckbox";
            this.CleanFilesCheckbox.Size = new System.Drawing.Size(109, 30);
            this.CleanFilesCheckbox.TabIndex = 13;
            this.CleanFilesCheckbox.Text = "Remove Old Files\r\n(Recommended)\r\n";
            this.CleanFilesCheckbox.UseVisualStyleBackColor = true;
            this.CleanFilesCheckbox.CheckedChanged += new System.EventHandler(this.CleanFilesCheckbox_CheckedChanged);
            // 
            // Installer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 496);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PictureBox);
            this.Controls.Add(this.SplitContainer);
            this.Controls.Add(this.InstallButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Installer";
            this.Text = "Among Us C.E. Installer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Installer_FormClosing);
            this.Load += new System.EventHandler(this.Installer_Load);
            this.InstallMethodGroup.ResumeLayout(false);
            this.InstallMethodGroup.PerformLayout();
            this.SteamInputGroup.ResumeLayout(false);
            this.SteamInputGroup.PerformLayout();
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            this.SplitContainer2.Panel1.ResumeLayout(false);
            this.SplitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer2)).EndInit();
            this.SplitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox UsernameBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox InstallLocationBox;
        private System.Windows.Forms.Button InstallLocationButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button InstallButton;
        private System.Windows.Forms.RichTextBox LogBox;
        private System.Windows.Forms.RadioButton InstallOption;
        private System.Windows.Forms.RadioButton UpgradeOption;
        private System.Windows.Forms.GroupBox InstallMethodGroup;
        private System.Windows.Forms.GroupBox SteamInputGroup;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.PictureBox PictureBox;
        private System.Windows.Forms.SplitContainer SplitContainer2;
        private System.Windows.Forms.MaskedTextBox PasswordBox;
        private System.Windows.Forms.LinkLabel WhyMySteamInfoLink;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox CleanFilesCheckbox;
    }
}

