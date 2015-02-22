using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BigDataFinalProject
{
    public class DataTransferManager
    {
        private const int T1 = 20;
        private const int T2 = 13;

        // Consts
        private string JAVA_WIN_PATH = BigDataFinalProject.Properties.Settings.Default.JAVA_WIN_PATH;
        private string WIN_OUTPUT_PATH = BigDataFinalProject.Properties.Settings.Default.WIN_OUTPUT_PATH;
        private string JAVA_MAIN_CLASS_NAME = BigDataFinalProject.Properties.Settings.Default.JAVA_MAIN_CLASS_NAME;
        private string INPUT_WIN_PATH = BigDataFinalProject.Properties.Settings.Default.INPUT_WIN_PATH;
        private string CLOUDERA_PATH = BigDataFinalProject.Properties.Settings.Default.CLOUDERA_PATH;
        private string INPUT_CLOUDERA_FOLDER = BigDataFinalProject.Properties.Settings.Default.INPUT_CLOUDERA_FOLDER;
        private string JAVA_CLOUDERA_FOLDER = BigDataFinalProject.Properties.Settings.Default.JAVA_CLOUDERA_FOLDER;
        private string CLOUDERA_OUTPUT_FOLDER = BigDataFinalProject.Properties.Settings.Default.CLOUDERA_OUTPUT_FOLDER;
        private string CLASSES_FOLDER = BigDataFinalProject.Properties.Settings.Default.CLASSES_FOLDER;
        private string JAR_NAME = BigDataFinalProject.Properties.Settings.Default.JAR_NAME;
        private string MAP_REDUCE_FOLDER = BigDataFinalProject.Properties.Settings.Default.MAP_REDUCE_FOLDER;
        private string PACKAGE_NAME = BigDataFinalProject.Properties.Settings.Default.PACKAGE_NAME;
        private string CLOUDERA_COMPILED_CLASS_PATH = BigDataFinalProject.Properties.Settings.Default.CLOUDERA_COMPILED_CLASS_PATH;

        // Variables
        private SshClient sshClient;
        private ScpClient scpClient;
        private string hostname;
        private string userName;
        private string password;

        private void OpenConnecion()
        {
            // Try to SSH the server
            sshClient = new SshClient(hostname, userName, password);
            sshClient.Connect();

            // Try to SCP the server
            scpClient = new ScpClient(hostname, userName, password);
            scpClient.Connect();
        }

        private void CloseConnection()
        {
            sshClient.Disconnect();

            scpClient.Disconnect();
        }

        private void RunCommand(string strCommand)
        {
            sshClient.CreateCommand(strCommand).Execute();
        }

        public void TransferFilesAndAnalyze(string strHost, string strUserName, string strPassword, int K, int numOfStocks)
        {
            this.hostname = strHost;
            this.userName = strUserName;
            this.password = strPassword;

            string hadoopInputFolder = MAP_REDUCE_FOLDER + "/input";
            string hadoopOutputFolder = MAP_REDUCE_FOLDER + "/output";
            string classLinuxPath = CLOUDERA_PATH + "/" + CLASSES_FOLDER;

            try
            {
                // Open SSH and SCP connections
                OpenConnecion();

                // Create the linux base folder
                RunCommand("mkdir " + CLOUDERA_PATH);

                // Coping the logs to the linux
                scpClient.Upload(new DirectoryInfo(INPUT_WIN_PATH), CLOUDERA_PATH + "/" + INPUT_CLOUDERA_FOLDER);

                // Cretae input dir in the hadoop fs
                RunCommand("hadoop fs -mkdir " + hadoopInputFolder);

                // Coping thr logs tothe hadop fs
                RunCommand("hadoop fs -put " + CLOUDERA_PATH + "/" + INPUT_CLOUDERA_FOLDER + "/* " + MAP_REDUCE_FOLDER + "/input");

                scpClient.Upload(new DirectoryInfo(JAVA_WIN_PATH), CLOUDERA_PATH + "/" + JAVA_CLOUDERA_FOLDER);

                // Create linux class folder
                RunCommand("mkdir " + classLinuxPath);

                // Compile the MapReduce code in linux
                RunCommand("javac -cp " + CLOUDERA_COMPILED_CLASS_PATH + " -d " +
                            classLinuxPath + " " + CLOUDERA_PATH + "/" + JAVA_CLOUDERA_FOLDER + "/*");

                // Create jar file in linux
                RunCommand("jar -cvf " + CLOUDERA_PATH + "/" + JAR_NAME + " -C " + classLinuxPath + "/ .");

                // Run the MapReduce code in linxu
                RunCommand("hadoop jar " + CLOUDERA_PATH + "/" + JAR_NAME + " " + PACKAGE_NAME + "." + JAVA_MAIN_CLASS_NAME
                                         + " " + MAP_REDUCE_FOLDER + " " + K + " " + T1 + " " + T2 + " " + numOfStocks);

                // Create output folder i linux (not in the hdfs)
                RunCommand("mkdir " + CLOUDERA_PATH + "/" + CLOUDERA_OUTPUT_FOLDER);

                // Copy the output file from the hdfs to the linux
                RunCommand("hadoop fs -get " + hadoopOutputFolder + "/part-r-00000 " + CLOUDERA_PATH + "/" + CLOUDERA_OUTPUT_FOLDER + "/");

                // Copy the output file from linux to the windows
                scpClient.Download(CLOUDERA_PATH + "/" + CLOUDERA_OUTPUT_FOLDER + "/part-r-00000", new FileInfo(WIN_OUTPUT_PATH + "\\part-r-00000.txt"));

                // Cloes the conection
                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void CleanFilesInHost(string p_strHost, string p_strUserName, string p_strPassword)
        {
            try
            {
                this.hostname = p_strHost;
                this.userName = p_strUserName;
                this.password = p_strPassword;

                OpenConnecion();

                RunCommand("rm -rf " + CLOUDERA_PATH + "/");
                RunCommand("hadoop fs -rm -r " + MAP_REDUCE_FOLDER + "/");

                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool CheckConnection(string p_strHost, string p_strUserName, string p_strPassword)
        {
            try
            {
                this.hostname = p_strHost;
                this.userName = p_strUserName;
                this.password = p_strPassword;

                OpenConnecion();

                CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
