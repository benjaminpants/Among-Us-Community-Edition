
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
            this.PasswordBox = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.InstallLocationBox = new System.Windows.Forms.MaskedTextBox();
            this.InstallLocationButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.InstallButton = new System.Windows.Forms.Button();
            this.LogBox = new System.Windows.Forms.RichTextBox();
            this.InstallOption = new System.Windows.Forms.RadioButton();
            this.UpgradeOption = new System.Windows.Forms.RadioButton();
            this.InstallMethodGroup = new System.Windows.Forms.GroupBox();
            this.RememberPasswordCheckbox = new System.Windows.Forms.CheckBox();
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.InstallMethodGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // PasswordBox
            // 
            this.PasswordBox.Location = new System.Drawing.Point(12, 64);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.PasswordChar = '●';
            this.PasswordBox.Size = new System.Drawing.Size(467, 20);
            this.PasswordBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Steam Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Steam Password:";
            // 
            // InstallLocationBox
            // 
            this.InstallLocationBox.Location = new System.Drawing.Point(12, 169);
            this.InstallLocationBox.Name = "InstallLocationBox";
            this.InstallLocationBox.Size = new System.Drawing.Size(431, 20);
            this.InstallLocationBox.TabIndex = 4;
            // 
            // InstallLocationButton
            // 
            this.InstallLocationButton.Location = new System.Drawing.Point(449, 169);
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
            this.label3.Location = new System.Drawing.Point(9, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Install Location:";
            // 
            // InstallButton
            // 
            this.InstallButton.Location = new System.Drawing.Point(12, 415);
            this.InstallButton.Name = "InstallButton";
            this.InstallButton.Size = new System.Drawing.Size(467, 23);
            this.InstallButton.TabIndex = 7;
            this.InstallButton.Text = "Install";
            this.InstallButton.UseVisualStyleBackColor = true;
            this.InstallButton.Click += new System.EventHandler(this.InstallButton_Click);
            // 
            // LogBox
            // 
            this.LogBox.BackColor = System.Drawing.Color.Black;
            this.LogBox.ForeColor = System.Drawing.Color.White;
            this.LogBox.Location = new System.Drawing.Point(12, 196);
            this.LogBox.Name = "LogBox";
            this.LogBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.LogBox.Size = new System.Drawing.Size(467, 213);
            this.LogBox.TabIndex = 9;
            this.LogBox.Text = "";
            // 
            // InstallOption
            // 
            this.InstallOption.AutoSize = true;
            this.InstallOption.Checked = true;
            this.InstallOption.Location = new System.Drawing.Point(6, 14);
            this.InstallOption.Name = "InstallOption";
            this.InstallOption.Size = new System.Drawing.Size(52, 17);
            this.InstallOption.TabIndex = 11;
            this.InstallOption.TabStop = true;
            this.InstallOption.Text = "Install";
            this.InstallOption.UseVisualStyleBackColor = true;
            // 
            // UpgradeOption
            // 
            this.UpgradeOption.AutoSize = true;
            this.UpgradeOption.Checked = global::AmongUs_CE_Installer.Properties.Settings.Default.UpgradeMode;
            this.UpgradeOption.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AmongUs_CE_Installer.Properties.Settings.Default, "UpgradeMode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.UpgradeOption.Location = new System.Drawing.Point(64, 14);
            this.UpgradeOption.Name = "UpgradeOption";
            this.UpgradeOption.Size = new System.Drawing.Size(66, 17);
            this.UpgradeOption.TabIndex = 12;
            this.UpgradeOption.Text = "Upgrade";
            this.UpgradeOption.UseVisualStyleBackColor = true;
            // 
            // InstallMethodGroup
            // 
            this.InstallMethodGroup.Controls.Add(this.InstallOption);
            this.InstallMethodGroup.Controls.Add(this.UpgradeOption);
            this.InstallMethodGroup.Location = new System.Drawing.Point(12, 113);
            this.InstallMethodGroup.Name = "InstallMethodGroup";
            this.InstallMethodGroup.Size = new System.Drawing.Size(142, 37);
            this.InstallMethodGroup.TabIndex = 13;
            this.InstallMethodGroup.TabStop = false;
            this.InstallMethodGroup.Text = "Install Method:";
            // 
            // RememberPasswordCheckbox
            // 
            this.RememberPasswordCheckbox.AutoSize = true;
            this.RememberPasswordCheckbox.Checked = global::AmongUs_CE_Installer.Properties.Settings.Default.RememberPassword;
            this.RememberPasswordCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AmongUs_CE_Installer.Properties.Settings.Default, "RememberPassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RememberPasswordCheckbox.Location = new System.Drawing.Point(12, 90);
            this.RememberPasswordCheckbox.Name = "RememberPasswordCheckbox";
            this.RememberPasswordCheckbox.Size = new System.Drawing.Size(126, 17);
            this.RememberPasswordCheckbox.TabIndex = 10;
            this.RememberPasswordCheckbox.Text = "Remember Password";
            this.RememberPasswordCheckbox.UseVisualStyleBackColor = true;
            // 
            // UsernameBox
            // 
            this.UsernameBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::AmongUs_CE_Installer.Properties.Settings.Default, "LastUsername", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.UsernameBox.Location = new System.Drawing.Point(12, 25);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(467, 20);
            this.UsernameBox.TabIndex = 0;
            this.UsernameBox.Text = global::AmongUs_CE_Installer.Properties.Settings.Default.LastUsername;
            this.UsernameBox.TextChanged += new System.EventHandler(this.UsernameBox_TextChanged);
            // 
            // Installer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 450);
            this.Controls.Add(this.InstallMethodGroup);
            this.Controls.Add(this.RememberPasswordCheckbox);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.InstallButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.InstallLocationButton);
            this.Controls.Add(this.InstallLocationBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.UsernameBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Installer";
            this.Text = "Among Us C.E. Installer";
            this.InstallMethodGroup.ResumeLayout(false);
            this.InstallMethodGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UsernameBox;
        private System.Windows.Forms.MaskedTextBox PasswordBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox InstallLocationBox;
        private System.Windows.Forms.Button InstallLocationButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button InstallButton;
        private System.Windows.Forms.RichTextBox LogBox;
        private System.Windows.Forms.CheckBox RememberPasswordCheckbox;
        private System.Windows.Forms.RadioButton InstallOption;
        private System.Windows.Forms.RadioButton UpgradeOption;
        private System.Windows.Forms.GroupBox InstallMethodGroup;
    }
}

