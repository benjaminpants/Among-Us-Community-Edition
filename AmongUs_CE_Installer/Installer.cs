using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AmongUs_CE_Installer
{
    public partial class Installer : Form
    {
        private bool AlreadyRun = false;

        private bool UpgradeMode = false;

        public const string ArgumentsString = "-app 945360 -depot 945361 -manifest 2146956919302566155 -user {0} -password \"{1}\" -dir \"{2}\"";
        public Installer()
        {
            InitializeComponent();
            GetDefaultValues();
            Console.SetOut(new ControlWriter(this.LogBox));
        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            SplitContainer.Panel1Collapsed = true;
            SplitContainer2.Panel1Collapsed = true;
            if (!AlreadyRun) Task.Run(Install);
            else Close();
        }

        private void UpdateSavedPrefrences()
        {
            UpdateCollapsePanel();
        }

        private void UpdateCollapsePanel(bool Startup = false)
        {
            if (UpgradeMode)
            {
                SplitContainer2.Panel1Collapsed = true;
                if (Startup) UpgradeOption.Checked = true;
            }
            else
            {
                SplitContainer2.Panel1Collapsed = false;
                if (Startup) InstallOption.Checked = true;
            }
        }

        private void GetDefaultValues()
        {
            InstallLocationBox.Text = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "Among Us CE");
            UpdateCollapsePanel(true);
        }

        private async void Install()
        {
            string Username = UsernameBox.Text;
            string Password = PasswordBox.Text;
            string InstallLocation = InstallLocationBox.Text;
            var ResultingString = string.Format(ArgumentsString, Username, Password, InstallLocation);  
            var Arguments = Extensions.CommandLineToArgs(ResultingString);
            bool UpgradeOnly = UpgradeMode;

            InstallButton.Invoke((MethodInvoker)(() =>
            {
                InstallButton.Enabled = false;
                InstallButton.Text = "Working...";
            }));

            InstallMethodGroup.Invoke((MethodInvoker)(() =>
            {
                InstallMethodGroup.Enabled = false;
            }));

            SteamInputGroup.Invoke((MethodInvoker)(() =>
            {
                SteamInputGroup.Enabled = false;
            }));

            if (!UpgradeOnly)
            {
                if (await DepotDownloader.Program.MainAsync(Arguments) != 0)
                {
                    Console.WriteLine("An error occured with the install! Cancelling...");
                    InstallButton.Invoke((MethodInvoker)(() =>
                    {
                        InstallButton.Enabled = true;
                        InstallButton.Text = "Close";
                    }));

                    AlreadyRun = true;
                    return;
                }
            }
            await MoveModFilesAsync(InstallLocation);

            Console.WriteLine("Finished Installing!");

            InstallButton.Invoke((MethodInvoker)(() =>
            {
                InstallButton.Enabled = true;
                InstallButton.Text = "Close";
            }));

            AlreadyRun = true;


        }

        private void InstallLocationButton_Click(object sender, EventArgs e)
        {
            FolderSelectDialog folderSelectDialog = new FolderSelectDialog();
            folderSelectDialog.InitialDirectory = InstallLocationBox.Text;
            folderSelectDialog.Title = "Choose Install Location...";
            if (folderSelectDialog.ShowDialog())
            {
                InstallLocationBox.Text = folderSelectDialog.FileName;
            }
        }

        private static async Task<int> MoveModFilesAsync(string InstallDirectory)
        {
            Console.WriteLine("Moving Mod Files...");

            string InstallerDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string ModFilesLocation = System.IO.Path.Combine(InstallerDirectory, "Mod");

            string SourcePath = ModFilesLocation;
            string DestinationPath = InstallDirectory;

            if (System.IO.Directory.Exists(SourcePath))
            {
                foreach (string srcPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                {
                    string destPath = srcPath.Replace(SourcePath, DestinationPath);
                    string destDir = System.IO.Path.GetDirectoryName(destPath);
                    Directory.CreateDirectory(destDir);
                    Console.WriteLine(System.IO.Path.GetFileName(srcPath));
                    await Task.Run(() => File.Copy(srcPath, destPath, true));
                }
            }

            return 1;
        }

        private void Installer_Load(object sender, EventArgs e)
        {

        }

        private void UpgradeOption_CheckedChanged(object sender, EventArgs e)
        {
            if (UpgradeOption.Checked) UpgradeMode = true;
            UpdateSavedPrefrences();
        }

        private void InstallOption_CheckedChanged(object sender, EventArgs e)
        {
            if (InstallOption.Checked) UpgradeMode = false;
            UpdateSavedPrefrences();
        }

        private void WhyMySteamInfoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string Message = "Among Us: CE runs on an older version of Among Us, which is only accessable on Steam." +
            "The only legal way to download this old version is through the official Steam API, which requires a username and password." +
            "Rest assured that CE and its installer does not store this information anywhere" +
            "and is immiedetly discarded as soon as the download is finished.";
            string Title = "Why do I need to Input my Steam information?";
            MessageBox.Show(Message, Title);
        }

        private void Installer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
