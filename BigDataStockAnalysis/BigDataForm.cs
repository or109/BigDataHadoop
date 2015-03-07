using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace BigDataStockAnalysis
{
    enum FeaturesColumnNumbers
    {
        OPEN = 1,
        HIGH,
        LOW,
        CLOSE
    }

    public partial class BigDataForm : Form
    {
        // Consts
        string USER_NAME_FOR_HOST = BigDataStockAnalysis.Properties.Settings.Default.USER_NAME_FOR_HOST;
        string PASSWORD_FOR_HOST = BigDataStockAnalysis.Properties.Settings.Default.PASSWORD_FOR_HOST;
        const string STOCK_FOLDER_NAME = "Stocks";
        const string OUTPUT_FOLDER_NAME = "output";
        const string INPUT_FOLDER_NAME = "Input";
        const string TXT_FILE_ENDING = ".txt";
        const string COMMA = ",";
        private int MAX_ROW_NUMBER_INPUT_FILE = int.Parse(BigDataStockAnalysis.Properties.Settings.Default.MAX_ROW_NUMBER_INPUT_FILE);

        // Variables
        private int stkNum;
        private int daysNum;
        private int clusterNum;
        private string clouderaIP = BigDataStockAnalysis.Properties.Settings.Default.IP_ADDRESS;

        private Dictionary<string, string> dctStocks;
        private List<Stock> lstStocks;
        private DataControlManager dtmDataTransfer;
        private List<FeaturesColumnNumbers> lstFeatures;
        private Dictionary<string, int> dctClustering;

        public BigDataForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Disable the Analyze button
            this.btnAnalyze.Enabled = false;

            this.stkNum = int.Parse(this.txtStockNum.Text);
            this.daysNum = int.Parse(this.txtDays.Text);
            this.clusterNum = int.Parse(this.txtClustersNum.Text);

            this.DownloadStockFiles();
            this.CreateInputFiles();
            this.Analyze();
            this.CreateReportData();
            this.CreateReport();

            // Enable the Analyze button
            this.btnAnalyze.Enabled = true;
        }

        private void Analyze()
        {
            this.dtmDataTransfer = new DataControlManager();

            if (this.dtmDataTransfer.CheckConnection(clouderaIP, USER_NAME_FOR_HOST, PASSWORD_FOR_HOST))
            {
                this.dtmDataTransfer.CleanFilesInHost(clouderaIP, USER_NAME_FOR_HOST, PASSWORD_FOR_HOST);
                this.dtmDataTransfer.TransferFilesAndAnalyze(clouderaIP, USER_NAME_FOR_HOST, PASSWORD_FOR_HOST, this.clusterNum, this.dctStocks.Count);
            }
        }

        private void DownloadStockFiles()
        {
            string strRemoteUri = "ftp://ftp.nasdaqtrader.com/symboldirectory/";
            string strFileName = "nasdaqlisted.txt";
            string myStringWebResource = null;
            this.dctStocks = new Dictionary<string, string>();

            this.CreateDirectoryInBin(STOCK_FOLDER_NAME);
            this.CreateDirectoryInBin(OUTPUT_FOLDER_NAME);

            // Create a new WebClient instance.
            using (WebClient myWebClient = new WebClient())
            {
                myStringWebResource = strRemoteUri + strFileName;
                myWebClient.DownloadFile(myStringWebResource, strFileName);
                string[] lines = File.ReadAllLines(strFileName);

                // Getting all the stockes we need
                for (int nIndex = 1; dctStocks.Count < this.stkNum; ++nIndex)
                {
                    string strStockRow = lines[nIndex];
                    string[] strSplitedLine = strStockRow.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    string strStockID = strSplitedLine[0];
                    string strStockName = strSplitedLine[1];

                    // Getting for each stock the data from the web service            
                    DateTime dtStartDate = DateTime.Now.Subtract(TimeSpan.FromDays(this.daysNum)); ;

                    int nStartMonth = dtStartDate.Month - 1;
                    int nStartDay = dtStartDate.Day;
                    int nStartYear = dtStartDate.Year;
                    int nEndMonth = DateTime.Now.Month - 1;
                    int nEndDay = DateTime.Now.Day;
                    int nEndYear = DateTime.Now.Year;

                    myStringWebResource = "http://ichart.yahoo.com/table.csv?s=";
                    myStringWebResource += strStockID;

                    // month 
                    myStringWebResource += "&a=" + nStartMonth;

                    // day
                    myStringWebResource += "&b=" + nStartDay;

                    // year
                    myStringWebResource += "&c=" + nStartYear;

                    // month 
                    myStringWebResource += "&d=" + nEndMonth;

                    // day
                    myStringWebResource += "&e=" + nEndDay;

                    // year
                    myStringWebResource += "&f=" + nEndYear;

                    // static part
                    myStringWebResource += "&ignore=.csv";

                    // Download the Web resource and save it into the current filesystem folder.
                    try
                    {
                        myWebClient.DownloadFile(myStringWebResource, STOCK_FOLDER_NAME + "\\" + strStockID + TXT_FILE_ENDING);
                        dctStocks.Add(strStockID, strStockName);
                    }
                    catch (WebException e)
                    {
                        Console.WriteLine("Error getting data for stock" + strStockID + ": " + e.Message.ToString());
                    }
                }
            }
        }

        private void CreateInputFiles()
        {
            // Variables            
            int nRowIndex = 1;
            int nFileIndex = 0;
            string strCurrInputFileName;
            List<string> lstInputFileRows = new List<string>();

            // Check what the features we need
            List<FeaturesColumnNumbers> lstFeatures = this.GetFeatures();

            // Create the input directory
            this.CreateDirectoryInBin(INPUT_FOLDER_NAME);

            // Going over each stock file and creating row for each
            foreach (string strCurrStock in this.dctStocks.Keys)
            {
                string strLineToInputFile = strCurrStock;
                string strStockFilePath = STOCK_FOLDER_NAME + "\\" + strCurrStock + TXT_FILE_ENDING;
                string[] lines = File.ReadAllLines(strStockFilePath);

                lines = lines.Skip(1).ToArray();

                // Get the features we need from the file
                foreach (string strCurrLine in lines)
                {
                    string[] strSplitedLine = strCurrLine.Split(new string[] { "," }, StringSplitOptions.None);

                    foreach (FeaturesColumnNumbers currFeature in lstFeatures)
                    {
                        // Add the feature of the day to the line and before him a comma to separate
                        strLineToInputFile += COMMA + strSplitedLine[(int)currFeature];
                    }
                }

                // Add the Line to the List of lines to write
                lstInputFileRows.Add(strLineToInputFile);

                // Check if we have the max rows to input file and need to create a new input file
                if (nRowIndex == MAX_ROW_NUMBER_INPUT_FILE)
                {
                    // Create new input file
                    strCurrInputFileName = INPUT_FOLDER_NAME + "_" + nFileIndex + TXT_FILE_ENDING;

                    // Get the path of the assembly
                    Assembly assAssembly = Assembly.GetExecutingAssembly();
                    string strAssemblyPath = System.IO.Path.GetDirectoryName(assAssembly.Location);
                    string strPathString = System.IO.Path.Combine(strAssemblyPath, INPUT_FOLDER_NAME);
                    strPathString = Path.Combine(strPathString, strCurrInputFileName);

                    // Create and Write to file all lines
                    File.WriteAllLines(strPathString, lstInputFileRows);

                    // Initialaize the row index
                    nRowIndex = 1;

                    // initialize the Row list
                    lstInputFileRows.Clear();

                    // Increase the file index
                    ++nFileIndex;
                }
                else
                {
                    ++nRowIndex;
                }
            }

            // Write the last file if needed
            if (lstInputFileRows.Count() != 0)
            {
                // Create new input file
                strCurrInputFileName = INPUT_FOLDER_NAME + "_" + nFileIndex + TXT_FILE_ENDING;

                // Get the path of the assembly
                Assembly assAssembly = Assembly.GetExecutingAssembly();
                string strAssemblyPath = System.IO.Path.GetDirectoryName(assAssembly.Location);
                string strPathString = System.IO.Path.Combine(strAssemblyPath, INPUT_FOLDER_NAME);
                strPathString = Path.Combine(strPathString, strCurrInputFileName);

                // Create and Write to file all lines
                File.WriteAllLines(strPathString, lstInputFileRows);

                // initialize the Row list
                lstInputFileRows.Clear();
            }
        }

        private List<FeaturesColumnNumbers> GetFeatures()
        {
            // Create thelist
            lstFeatures = new List<FeaturesColumnNumbers>();

            // Checking for each check box if need to add the feature
            if (this.cbOpen.Checked)
            {
                lstFeatures.Add(FeaturesColumnNumbers.OPEN);
            }

            if (this.cbHigh.Checked)
            {
                lstFeatures.Add(FeaturesColumnNumbers.HIGH);
            }

            if (this.cbLow.Checked)
            {
                lstFeatures.Add(FeaturesColumnNumbers.LOW);
            }

            if (this.cbClose.Checked)
            {
                lstFeatures.Add(FeaturesColumnNumbers.CLOSE);
            }

            return lstFeatures;
        }

        private void CreateDirectoryInBin(string strFolderName)
        {
            // Get the path of the assembly
            Assembly assAssembly = Assembly.GetExecutingAssembly();
            string strAssemblyPath = System.IO.Path.GetDirectoryName(assAssembly.Location);
            string strPathString = System.IO.Path.Combine(strAssemblyPath, strFolderName);

            // Check if the directory already exist
            if (Directory.Exists(strPathString))
            {
                Directory.Delete(strPathString, true);
            }

            // Create the Directory
            System.IO.Directory.CreateDirectory(strPathString);
        }

        private void CreateFileInBin(string strFolderName, string strFileName)
        {
            // Get the path of the assembly
            Assembly assAssembly = Assembly.GetExecutingAssembly();
            string strAssemblyPath = System.IO.Path.GetDirectoryName(assAssembly.Location);
            string strPathString = System.IO.Path.Combine(strAssemblyPath, strFolderName);
            strPathString = Path.Combine(strPathString, strFileName);

            // Create The File
            File.Create(strPathString);
        }

        private void LoadClustering()
        {
            this.dctClustering = new Dictionary<string, int>();

            // Get the output file path
            string strOutputFilePath = OUTPUT_FOLDER_NAME + "\\part-r-00000" + TXT_FILE_ENDING;

            // Read the file to memory           
            List<string> lstOutput = File.ReadAllLines(strOutputFilePath).ToList<string>();

            foreach (string currOutput in lstOutput)
            {
                List<string> strSplitedLine = currOutput.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

                if (strSplitedLine.Count == 2)
                {
                    int nClsterNumber = int.Parse(strSplitedLine[0]);
                    string strStockID = strSplitedLine[1];

                    this.dctClustering.Add(strStockID, nClsterNumber);
                }
            }
        }

        private void CreateReportData()
        {
            this.lstStocks = new List<Stock>();

            this.LoadClustering();

            foreach (string strCurrStockID in this.dctStocks.Keys)
            {
                // Get the file path
                string strStockFilePath = STOCK_FOLDER_NAME + "\\" + strCurrStockID + TXT_FILE_ENDING;

                // Read the file to memory           
                List<string> lines = File.ReadAllLines(strStockFilePath).Skip(1).ToList<string>();

                int nNumOfDays = lines.Count;

                // Get the features we need from the file
                foreach (string strCurrLine in lines)
                {
                    Stock currStock = new Stock();
                    int nCurrDay = nNumOfDays;
                    string[] strSplitedLine = strCurrLine.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (FeaturesColumnNumbers currFeature in lstFeatures)
                    {
                        switch (currFeature)
                        {
                            case FeaturesColumnNumbers.OPEN:
                                {
                                    currStock.OpenValue = double.Parse(strSplitedLine[(int)currFeature]);
                                    break;
                                }
                            case FeaturesColumnNumbers.HIGH:
                                {
                                    currStock.HighValue = double.Parse(strSplitedLine[(int)currFeature]);
                                    break;
                                }
                            case FeaturesColumnNumbers.LOW:
                                {
                                    currStock.LowValue = double.Parse(strSplitedLine[(int)currFeature]);
                                    break;
                                }
                            case FeaturesColumnNumbers.CLOSE:
                                {
                                    currStock.CloseValue = double.Parse(strSplitedLine[(int)currFeature]);
                                    break;
                                }
                            default:
                                break;
                        }

                        currStock.DateNumber = nCurrDay;
                        nCurrDay--;
                        currStock.StockName = this.dctStocks[strCurrStockID];
                        currStock.Cluster = this.dctClustering[strCurrStockID] + 1;
                        this.lstStocks.Add(currStock);
                    }
                }
            }
        }

        public void CreateReport()
        {
            StockReportForm frmReport = new StockReportForm(this.lstStocks);

            frmReport.ShowDialog();
        }
    }
}
