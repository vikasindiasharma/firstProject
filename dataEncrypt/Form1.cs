using dataEncrypt.core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dataEncrypt
{
    public partial class lblProgress : Form
    {
        public lblProgress()
        {
            InitializeComponent();
        }

        string recentfiles = "data\\recentfiles.txt";
        string logs = @"data\logs.txt";
        StringBuilder logsBuilder = new StringBuilder();
        StringBuilder recentFileBuilders = new StringBuilder();

        private void Form1_Load(object sender, EventArgs e)
        {
          
            
            /*  AutoCompleteStringCollection source = new AutoCompleteStringCollection();
            // Add each item to the collection
            

            var lines = File.ReadAllLines(recentfiles);
            for (var i = 0; i < lines.Length; i += 1)
            {
                source.Add(lines[i]);
                recentFileBuilders.AppendLine(lines[i]);
                // Process line
            }

            txtDestination.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtDestination.AutoCompleteCustomSource = source;

            txtSourceFolder.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtSourceFolder.AutoCompleteCustomSource = source;*/

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Please enter password");
                return;
            }
            StartJob();
            DataArgument arg = new DataArgument();
            arg.Encrypt = true;
            arg.SourceFolder = txtSourceFolder.Text;
            arg.DestinationFolder = txtDestination.Text;
            arg.Password = txtPassword.Text;
            backgroundWorker1.RunWorkerAsync(arg);
 
        }

        private void opnSourceFolder_Click(object sender, EventArgs e)
        {


        }

        EncryptionFile enc = new EncryptionFile();
        DecryptionFile dec = new DecryptionFile();

        public void StarDecrypt(DataArgument argument, BackgroundWorker bwWorker, string sourceDirectory, string DestinationDir)
        {

            string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);


            string password = argument.Password;

            for (int i = 0; i < files.Length; i++)
            {
                argument.Message = String.Format("Processing DeEncrypt {0} of {1}, File {2}", (i + 1), files.Length, files[i]);
                bwWorker.ReportProgress(0,argument);
                string destinationFile = Path.Combine(DestinationDir, Path.GetFileName(files[i]));
                if (File.Exists(destinationFile))
                {
                    string copiedFile = destinationFile + (".org_" +  getCurrentDateTime());

                    argument.Message = "Renaming  Existing file " + destinationFile + " to " + copiedFile;
                    bwWorker.ReportProgress(0, argument);
                    File.Copy(destinationFile, copiedFile);
                }
                //    Console.WriteLine(files[i]);

                try
                {
                    dec.DecryptFile(files[i], password, destinationFile);
                }
                catch (Exception exp)
                {

                    argument.Message = string.Format("Error :File [{0}] can not be decrypted . Check if password is corectt , error {1}", files[i], exp.Message);
                    bwWorker.ReportProgress(0, argument);
        } 
         

            }
        }
    

        
        private void StartDirectoryLevelProcess (string sourceDirectory ,  string DestinationDir, BackgroundWorker bwWorker, DataArgument argument)
        {
            string[] directories = Directory.GetDirectories(sourceDirectory);

            
            
            try
            {
                foreach(string dir in directories)
                {
                    string dirName = Path.GetFileName(dir);
                    string destination = Path.Combine(DestinationDir, dirName);

                    if (! Directory.Exists(destination))
                    {
                        Directory.CreateDirectory(destination);
                    }
                    StartDirectoryLevelProcess(dir, destination, bwWorker, argument);

                }

                if (argument.Encrypt)
                {
                    StartEncrypt(argument, bwWorker, sourceDirectory, DestinationDir);
                }
                else
                {
                    StarDecrypt(argument, bwWorker, sourceDirectory, DestinationDir);
                }


            }
            catch( Exception ex)
            {
                argument.Message = "Error " + ex.Message;
                bwWorker.ReportProgress(0,argument);
            }

            




        }
    public void StartEncrypt(DataArgument argument,BackgroundWorker bwWorker, string sourceDirectory, string DestinationDir)
        {

            string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.TopDirectoryOnly);


            string password = argument.Password;

            for (int i = 0; i < files.Length; i++)
            {
                argument.Message= String.Format("Processing Encrypt {0} of {1}, File {2}", (i+1), files.Length,files[i]);

                bwWorker.ReportProgress(0, argument);
                string destinationFile = Path.Combine(DestinationDir, Path.GetFileName( files[i]));
                if ( File.Exists(destinationFile))
                {
                    argument.Message= "Deleting Existing file " + destinationFile;
                    bwWorker.ReportProgress(0, argument);
                    File.Delete(destinationFile);
                }
            //    Console.WriteLine(files[i]);
                enc.EncryptFile(files[i], password,destinationFile);
                //dec.DecryptFile(files[i], password);

            if ( bwWorker.CancellationPending )
                {
                    return;

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ( txtPassword.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Please enter password");
                return;
            }
            StartJob();
            DataArgument arg = new DataArgument();
            arg.Encrypt = false;
            arg.SourceFolder = txtSourceFolder.Text;
            arg.DestinationFolder = txtDestination.Text;
            arg.Password = txtPassword.Text;
            backgroundWorker1.RunWorkerAsync(arg);
           
        }

        private void StartJob()
        {
            recentFileBuilders.AppendLine(txtSourceFolder.Text);
            recentFileBuilders.AppendLine(txtDestination.Text);

            File.Delete(recentfiles);
            System.IO.File.WriteAllText(recentfiles, recentFileBuilders.ToString());

            txtPassword.Enabled = false;
            txtDestination.Enabled = false;
            txtSourceFolder.Enabled = false;
            btnStart.Enabled = false;
            button1.Enabled = false;
            opnDestinationFolder.Enabled = false;
            opnSourceFolder.Enabled = false;
            lblProgressLabel.Text = "";
            logsBuilder.Clear();

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var argument= e.Argument as DataArgument;
            BackgroundWorker helperBW = sender as BackgroundWorker;

            string path = Path.Combine(argument.DestinationFolder, Path.GetFileName(argument.SourceFolder));

            if (! Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            StartDirectoryLevelProcess(argument.SourceFolder, path, helperBW, argument);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            JobFinished();

        }

        private void JobFinished()
        {
            txtPassword.Enabled = true;
            txtDestination.Enabled = true;
            txtSourceFolder.Enabled = true;
            btnStart.Enabled = true;
            button1.Enabled = true;
            opnDestinationFolder.Enabled = true;
            opnSourceFolder.Enabled = true;
            String fileName = @"data\log_" + getCurrentDateTime() + ".txt";
            File.WriteAllText(fileName, logsBuilder.ToString());
            MessageBox.Show("Done!!!");
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var data  = e.UserState as DataArgument;

            lblProgressLabel.Text = data.Message;
            logsBuilder.AppendLine(String.Format( "{0} :{1}", DateTime.Now, data.Message));
        }

        public static string getCurrentDateTime()
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        }
    }
}
