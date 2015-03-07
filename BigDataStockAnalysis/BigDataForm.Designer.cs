namespace BigDataStockAnalysis
{
    partial class BigDataForm
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
            this.lblStockNum = new System.Windows.Forms.Label();
            this.lblDays = new System.Windows.Forms.Label();
            this.txtStockNum = new System.Windows.Forms.TextBox();
            this.txtDays = new System.Windows.Forms.TextBox();
            this.lblDaysBefore = new System.Windows.Forms.Label();
            this.lblFeatures = new System.Windows.Forms.Label();
            this.cbOpen = new System.Windows.Forms.CheckBox();
            this.cbHigh = new System.Windows.Forms.CheckBox();
            this.cbLow = new System.Windows.Forms.CheckBox();
            this.cbClose = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtClustersNum = new System.Windows.Forms.TextBox();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStockNum
            // 
            this.lblStockNum.AutoSize = true;
            this.lblStockNum.Location = new System.Drawing.Point(15, 21);
            this.lblStockNum.Name = "lblStockNum";
            this.lblStockNum.Size = new System.Drawing.Size(95, 13);
            this.lblStockNum.TabIndex = 0;
            this.lblStockNum.Text = "Number of Stocks:";
            // 
            // lblDays
            // 
            this.lblDays.AutoSize = true;
            this.lblDays.Location = new System.Drawing.Point(15, 52);
            this.lblDays.Name = "lblDays";
            this.lblDays.Size = new System.Drawing.Size(44, 13);
            this.lblDays.TabIndex = 1;
            this.lblDays.Text = "Analyze";
            // 
            // txtStockNum
            // 
            this.txtStockNum.Location = new System.Drawing.Point(122, 18);
            this.txtStockNum.Name = "txtStockNum";
            this.txtStockNum.Size = new System.Drawing.Size(100, 20);
            this.txtStockNum.TabIndex = 2;
            this.txtStockNum.Text = "10";
            // 
            // txtDays
            // 
            this.txtDays.Location = new System.Drawing.Point(67, 49);
            this.txtDays.Name = "txtDays";
            this.txtDays.Size = new System.Drawing.Size(46, 20);
            this.txtDays.TabIndex = 3;
            this.txtDays.Text = "3";
            // 
            // lblDaysBefore
            // 
            this.lblDaysBefore.AutoSize = true;
            this.lblDaysBefore.Location = new System.Drawing.Point(119, 52);
            this.lblDaysBefore.Name = "lblDaysBefore";
            this.lblDaysBefore.Size = new System.Drawing.Size(138, 13);
            this.lblDaysBefore.TabIndex = 4;
            this.lblDaysBefore.Text = "Days from today backwards";
            // 
            // lblFeatures
            // 
            this.lblFeatures.AutoSize = true;
            this.lblFeatures.Location = new System.Drawing.Point(15, 79);
            this.lblFeatures.Name = "lblFeatures";
            this.lblFeatures.Size = new System.Drawing.Size(106, 13);
            this.lblFeatures.TabIndex = 5;
            this.lblFeatures.Text = "Features for analysis:";
            // 
            // cbOpen
            // 
            this.cbOpen.AutoSize = true;
            this.cbOpen.Checked = true;
            this.cbOpen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOpen.Location = new System.Drawing.Point(129, 79);
            this.cbOpen.Name = "cbOpen";
            this.cbOpen.Size = new System.Drawing.Size(52, 17);
            this.cbOpen.TabIndex = 6;
            this.cbOpen.Text = "Open";
            this.cbOpen.UseVisualStyleBackColor = true;
            // 
            // cbHigh
            // 
            this.cbHigh.AutoSize = true;
            this.cbHigh.Checked = true;
            this.cbHigh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHigh.Location = new System.Drawing.Point(129, 102);
            this.cbHigh.Name = "cbHigh";
            this.cbHigh.Size = new System.Drawing.Size(48, 17);
            this.cbHigh.TabIndex = 7;
            this.cbHigh.Text = "High";
            this.cbHigh.UseVisualStyleBackColor = true;
            // 
            // cbLow
            // 
            this.cbLow.AutoSize = true;
            this.cbLow.Location = new System.Drawing.Point(129, 125);
            this.cbLow.Name = "cbLow";
            this.cbLow.Size = new System.Drawing.Size(46, 17);
            this.cbLow.TabIndex = 8;
            this.cbLow.Text = "Low";
            this.cbLow.UseVisualStyleBackColor = true;
            // 
            // cbClose
            // 
            this.cbClose.AutoSize = true;
            this.cbClose.Location = new System.Drawing.Point(129, 148);
            this.cbClose.Name = "cbClose";
            this.cbClose.Size = new System.Drawing.Size(52, 17);
            this.cbClose.TabIndex = 9;
            this.cbClose.Text = "Close";
            this.cbClose.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 187);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Clusters:";
            // 
            // txtClustersNum
            // 
            this.txtClustersNum.Location = new System.Drawing.Point(67, 184);
            this.txtClustersNum.Name = "txtClustersNum";
            this.txtClustersNum.Size = new System.Drawing.Size(48, 20);
            this.txtClustersNum.TabIndex = 11;
            this.txtClustersNum.Text = "5";
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnAnalyze.Location = new System.Drawing.Point(18, 245);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(103, 28);
            this.btnAnalyze.TabIndex = 12;
            this.btnAnalyze.Text = "Go Analyze!";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::BigDataStockAnalysis.Properties.Resources.hadoop;
            this.pictureBox2.Location = new System.Drawing.Point(203, 79);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(221, 194);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // BigDataAnalyze
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 296);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.txtClustersNum);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbClose);
            this.Controls.Add(this.cbLow);
            this.Controls.Add(this.cbHigh);
            this.Controls.Add(this.cbOpen);
            this.Controls.Add(this.lblFeatures);
            this.Controls.Add(this.lblDaysBefore);
            this.Controls.Add(this.txtDays);
            this.Controls.Add(this.txtStockNum);
            this.Controls.Add(this.lblDays);
            this.Controls.Add(this.lblStockNum);
            this.Name = "BigDataAnalyze";
            this.Text = "BigData - Mantz&Johnny - Stock Analysis using Hadoop";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStockNum;
        private System.Windows.Forms.Label lblDays;
        private System.Windows.Forms.TextBox txtStockNum;
        private System.Windows.Forms.TextBox txtDays;
        private System.Windows.Forms.Label lblDaysBefore;
        private System.Windows.Forms.Label lblFeatures;
        private System.Windows.Forms.CheckBox cbOpen;
        private System.Windows.Forms.CheckBox cbHigh;
        private System.Windows.Forms.CheckBox cbLow;
        private System.Windows.Forms.CheckBox cbClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtClustersNum;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}

