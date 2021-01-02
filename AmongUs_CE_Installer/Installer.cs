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
        public const string ArgumentsString = "-app 945360 -depot 945361 -manifest 2146956919302566155 -user {0} -password {1} -dir \"{2}\"";
        public Installer()
        {
            InitializeComponent();
            GetDefaultValues();
            Console.SetOut(new ControlWriter(this.LogBox));
        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            UpdateSavedPrefrences();
            if (!AlreadyRun) Task.Run(Install);
            else Close();
        }

        private void UpdateSavedPrefrences()
        {
            bool SavePassword = Properties.Settings.Default.RememberPassword;
            Properties.Settings.Default.LastPassword = (SavePassword ? PasswordBox.Text : string.Empty);
            Properties.Settings.Default.Save();
        }

        private void GetDefaultValues()
        {
            InstallLocationBox.Text = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "Among Us CE");
        }

        private async void Install()
        {
            string Username = UsernameBox.Text;
            string Password = PasswordBox.Text;
            string InstallLocation = InstallLocationBox.Text;
            var ResultingString = string.Format(ArgumentsString, Username, Password, InstallLocation);  
            var Arguments = Extensions.CommandLineToArgs(ResultingString);
            bool UpgradeOnly = Properties.Settings.Default.UpgradeMode;

            InstallButton.Invoke((MethodInvoker)(() =>
            {
                InstallButton.Enabled = false;
            }));

            UsernameBox.Invoke((MethodInvoker)(() =>
            {
                UsernameBox.Enabled = false;
            }));

            PasswordBox.Invoke((MethodInvoker)(() =>
            {
                PasswordBox.Enabled = false;
            }));

            RememberPasswordCheckbox.Invoke((MethodInvoker)(() =>
            {
                RememberPasswordCheckbox.Enabled = false;
            }));

            InstallLocationBox.Invoke((MethodInvoker)(() =>
            {
                InstallLocationBox.Enabled = false;
            }));

            InstallLocationButton.Invoke((MethodInvoker)(() =>
            {
                InstallLocationButton.Enabled = false;
            }));

            InstallMethodGroup.Invoke((MethodInvoker)(() =>
            {
                InstallMethodGroup.Enabled = false;
            }));

            if (!UpgradeOnly) await DepotDownloader.Program.MainAsync(Arguments);
            await MoveModFilesAsync(InstallLocation);

            InstallButton.Invoke((MethodInvoker)(() =>
            {
                InstallButton.Enabled = true;
                InstallButton.Text = "Close";
            }));

            AlreadyRun = true;


        }

        private void UsernameBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSavedPrefrences();
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

            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine(System.IO.Path.GetFileName(newPath));
                await Task.Run(() => File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true));
            }

            return 1;
        }
    }
}
