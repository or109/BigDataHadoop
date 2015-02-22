using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BigDataFinalProject
{
    public partial class ReportForm : Form
    {
        private List<Stock> lstStocks;

        public ReportForm()
        {
            InitializeComponent();
        }

        public ReportForm(List<Stock> p_lstStocks)
        {
            InitializeComponent();

            this.lstStocks = p_lstStocks;
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            this.rptReport.RefreshReport();

            ReportDataSource rds = new ReportDataSource("StocksReport", this.lstStocks);

            this.rptReport.LocalReport.DataSources.Clear();
            this.rptReport.LocalReport.DataSources.Add(rds);
            this.rptReport.LocalReport.Refresh();

            this.rptReport.RefreshReport();
        }
    }
}
