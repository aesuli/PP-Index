namespace Esuli.MiPai.Image.Test
{
    partial class MiPaiImageTestForm
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.createIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.listView = new System.Windows.Forms.ListView();
            this.openImageFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openIndexFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.numericUpDownParallelIndexingThreads = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownResultsCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownReferencePoints = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTopPoints = new System.Windows.Forms.NumericUpDown();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.radioButtonFeatureThumbnail = new System.Windows.Forms.RadioButton();
            this.radioButtonBIC = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownAdditionalQueries = new System.Windows.Forms.NumericUpDown();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownSeed = new System.Windows.Forms.NumericUpDown();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownParallelIndexingThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResultsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReferencePoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTopPoints)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAdditionalQueries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSeed)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createIndexToolStripMenuItem,
            this.loadIndexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1017, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // createIndexToolStripMenuItem
            // 
            this.createIndexToolStripMenuItem.Name = "createIndexToolStripMenuItem";
            this.createIndexToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.createIndexToolStripMenuItem.Text = "&Create Index";
            this.createIndexToolStripMenuItem.Click += new System.EventHandler(this.createIndexToolStripMenuItem_Click);
            // 
            // loadIndexToolStripMenuItem
            // 
            this.loadIndexToolStripMenuItem.Name = "loadIndexToolStripMenuItem";
            this.loadIndexToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.loadIndexToolStripMenuItem.Text = "&Load Index";
            this.loadIndexToolStripMenuItem.Click += new System.EventHandler(this.loadIndexToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.searchToolStripMenuItem.Text = "&Search";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 594);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1017, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(739, 570);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            // 
            // openImageFileDialog
            // 
            this.openImageFileDialog.Filter = "jpeg files|*.jpg|bmp files|*.bmp|All files|*.*";
            // 
            // openIndexFileDialog
            // 
            this.openIndexFileDialog.Filter = "ppr files|*.ppr|All files|*.*";
            // 
            // numericUpDownParallelIndexingThreads
            // 
            this.numericUpDownParallelIndexingThreads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.numericUpDownParallelIndexingThreads, true);
            this.numericUpDownParallelIndexingThreads.Location = new System.Drawing.Point(197, 101);
            this.numericUpDownParallelIndexingThreads.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDownParallelIndexingThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownParallelIndexingThreads.Name = "numericUpDownParallelIndexingThreads";
            this.numericUpDownParallelIndexingThreads.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownParallelIndexingThreads.TabIndex = 3;
            this.numericUpDownParallelIndexingThreads.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownParallelIndexingThreads.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numericUpDownResultsCount
            // 
            this.numericUpDownResultsCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.numericUpDownResultsCount, true);
            this.numericUpDownResultsCount.Location = new System.Drawing.Point(136, 205);
            this.numericUpDownResultsCount.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDownResultsCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownResultsCount.Name = "numericUpDownResultsCount";
            this.numericUpDownResultsCount.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownResultsCount.TabIndex = 2;
            this.numericUpDownResultsCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownResultsCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numericUpDownReferencePoints
            // 
            this.numericUpDownReferencePoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.numericUpDownReferencePoints, true);
            this.numericUpDownReferencePoints.Location = new System.Drawing.Point(153, 153);
            this.numericUpDownReferencePoints.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericUpDownReferencePoints.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownReferencePoints.Name = "numericUpDownReferencePoints";
            this.numericUpDownReferencePoints.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownReferencePoints.TabIndex = 1;
            this.numericUpDownReferencePoints.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownReferencePoints.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // numericUpDownTopPoints
            // 
            this.numericUpDownTopPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.numericUpDownTopPoints, true);
            this.numericUpDownTopPoints.Location = new System.Drawing.Point(135, 127);
            this.numericUpDownTopPoints.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownTopPoints.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownTopPoints.Name = "numericUpDownTopPoints";
            this.numericUpDownTopPoints.Size = new System.Drawing.Size(56, 20);
            this.numericUpDownTopPoints.TabIndex = 0;
            this.numericUpDownTopPoints.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownTopPoints.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label6);
            this.flowLayoutPanel1.Controls.Add(this.label8);
            this.flowLayoutPanel1.Controls.Add(this.radioButtonFeatureThumbnail);
            this.flowLayoutPanel1.Controls.Add(this.radioButtonBIC);
            this.flowLayoutPanel1.Controls.Add(this.label9);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownSeed);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownParallelIndexingThreads);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownTopPoints);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownReferencePoints);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownResultsCount);
            this.flowLayoutPanel1.Controls.Add(this.label7);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownAdditionalQueries);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(274, 570);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label6, true);
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 10);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Indexing options:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label8, true);
            this.label8.Location = new System.Drawing.Point(3, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 23);
            this.label8.TabIndex = 14;
            this.label8.Text = "Type of features:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // radioButtonFeatureThumbnail
            // 
            this.radioButtonFeatureThumbnail.AutoSize = true;
            this.radioButtonFeatureThumbnail.Checked = true;
            this.radioButtonFeatureThumbnail.Location = new System.Drawing.Point(3, 52);
            this.radioButtonFeatureThumbnail.Name = "radioButtonFeatureThumbnail";
            this.radioButtonFeatureThumbnail.Size = new System.Drawing.Size(102, 17);
            this.radioButtonFeatureThumbnail.TabIndex = 13;
            this.radioButtonFeatureThumbnail.TabStop = true;
            this.radioButtonFeatureThumbnail.Text = "16x16 thumbnail";
            this.radioButtonFeatureThumbnail.UseVisualStyleBackColor = true;
            // 
            // radioButtonBIC
            // 
            this.radioButtonBIC.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.radioButtonBIC, true);
            this.radioButtonBIC.Location = new System.Drawing.Point(111, 52);
            this.radioButtonBIC.Name = "radioButtonBIC";
            this.radioButtonBIC.Size = new System.Drawing.Size(42, 17);
            this.radioButtonBIC.TabIndex = 15;
            this.radioButtonBIC.Text = "BIC";
            this.radioButtonBIC.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 26);
            this.label1.TabIndex = 4;
            this.label1.Text = "Number of parallel indexing processes:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 26);
            this.label2.TabIndex = 5;
            this.label2.Text = "Permutation prefix length:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 26);
            this.label3.TabIndex = 6;
            this.label3.Text = "Number of reference objects:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label5, true);
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 186);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Search options:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 202);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 26);
            this.label4.TabIndex = 7;
            this.label4.Text = "Number of search results:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 228);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(200, 26);
            this.label7.TabIndex = 11;
            this.label7.Text = "Number of additional permutated queries:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownAdditionalQueries
            // 
            this.numericUpDownAdditionalQueries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.numericUpDownAdditionalQueries, true);
            this.numericUpDownAdditionalQueries.Location = new System.Drawing.Point(209, 231);
            this.numericUpDownAdditionalQueries.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownAdditionalQueries.Name = "numericUpDownAdditionalQueries";
            this.numericUpDownAdditionalQueries.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownAdditionalQueries.TabIndex = 10;
            this.numericUpDownAdditionalQueries.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownAdditionalQueries.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView);
            this.splitContainer1.Size = new System.Drawing.Size(1017, 570);
            this.splitContainer1.SplitterDistance = 274;
            this.splitContainer1.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 72);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 26);
            this.label9.TabIndex = 16;
            this.label9.Text = "Random seed:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownSeed
            // 
            this.numericUpDownSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.numericUpDownSeed, true);
            this.numericUpDownSeed.Location = new System.Drawing.Point(85, 75);
            this.numericUpDownSeed.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDownSeed.Name = "numericUpDownSeed";
            this.numericUpDownSeed.Size = new System.Drawing.Size(88, 20);
            this.numericUpDownSeed.TabIndex = 17;
            this.numericUpDownSeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MiPaiImageTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 616);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MiPaiImageTestForm";
            this.Text = "MiPai Image Test";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownParallelIndexingThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResultsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReferencePoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTopPoints)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAdditionalQueries)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem loadIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createIndexToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.OpenFileDialog openImageFileDialog;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.OpenFileDialog openIndexFileDialog;
        private System.Windows.Forms.NumericUpDown numericUpDownTopPoints;
        private System.Windows.Forms.NumericUpDown numericUpDownReferencePoints;
        private System.Windows.Forms.NumericUpDown numericUpDownResultsCount;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown numericUpDownParallelIndexingThreads;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDownAdditionalQueries;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton radioButtonFeatureThumbnail;
        private System.Windows.Forms.RadioButton radioButtonBIC;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDownSeed;
    }
}

