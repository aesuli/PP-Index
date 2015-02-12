// Scheggia - A .NET Information Retrieval system
// Copyright (C) 2007 Andrea Esuli
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Andrea Esuli (andrea@esuli.it)

using System;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Esuli.MiPai.Index;
using Esuli.MiPai.IO;

namespace Esuli.MiPai.Image.Test
{
    public partial class MiPaiImageTestForm : Form
    {
        private MiPaiImageIndex index;

        public MiPaiImageTestForm()
        {
            index = null;
            InitializeComponent();
        }

        private void createIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.Description = "Select folder containing images to be indexed";
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }
            string startingPath = folderBrowserDialog.SelectedPath;

            folderBrowserDialog1.ShowNewFolderButton = true;
            folderBrowserDialog1.Description = "Select folder where index have to be saved";
            dialogResult = folderBrowserDialog1.ShowDialog();
            if (dialogResult != DialogResult.OK)
                return;
            string indexLocation = folderBrowserDialog1.SelectedPath;

            if (index != null)
            {
                index.Dispose();
                index = null;
            }

            var featureType = FeatureType.Thumbnail;
            if (radioButtonBIC.Checked)
            {
                featureType = FeatureType.BIC;
            }

            IndexingThread indexingThread = new IndexingThread((int)numericUpDownSeed.Value, featureType, (int)numericUpDownParallelIndexingThreads.Value, indexLocation, startingPath, statusStrip, (int)numericUpDownTopPoints.Value, (int)(Math.Max(numericUpDownTopPoints.Value, numericUpDownReferencePoints.Value)));

            Thread thread = new Thread(new ThreadStart(indexingThread.Run));

            thread.Start();
        }

        private void loadIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openIndexFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            FileInfo fileInfo = new FileInfo(openIndexFileDialog.FileName);
            string indexLocation = fileInfo.Directory.FullName;
            string indexName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);

            openIndexFileDialog.InitialDirectory = indexLocation;

            if (index != null)
            {
                index.Dispose();
                index = null;
            }
            index = new MiPaiImageIndex(indexLocation, indexName);
            toolStripStatusLabel.Text = "Index loaded (" + PermutationPrefixGeneratorInfo.GetFeatureType(indexName,indexLocation)+ " features, "+ index.PPIndex.Count + " images).";
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (index == null)
            {
                MessageBox.Show("You have to load an index first.");
                return;
            }

            DialogResult dialogResult = openImageFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            openImageFileDialog.InitialDirectory = new FileInfo(openImageFileDialog.FileName).DirectoryName;

            listView.Clear();

            Bitmap image = null;
            try
            {
                image = new Bitmap(openImageFileDialog.FileName);
            }
            catch
            {
                MessageBox.Show("Unable to load query image.");
                return;
            }
            int counter = 0;

            listView.LargeImageList = new ImageList();
            listView.LargeImageList.ImageSize = new Size(64, 64);

            listView.LargeImageList.Images.Add(image);
            listView.Items.Add("Query: " + openImageFileDialog.FileName, counter);
            ++counter;

            long startl = 0;
            long endl = 0;

            startl = DateTime.Now.Ticks;

            IEnumerator<string> results = index.Search(image, (int)numericUpDownResultsCount.Value, (int)numericUpDownAdditionalQueries.Value);

            while (results.MoveNext())
            {
                string imageFilename = results.Current;

                listView.LargeImageList.Images.Add(new Bitmap(imageFilename));
                listView.Items.Add(imageFilename, counter);
                ++counter;
                if (counter == numericUpDownResultsCount.Value)
                    break;
            }

            endl = DateTime.Now.Ticks;

            long ts = endl - startl;

            --counter;
            toolStripStatusLabel.Text = counter + " results in " + (((double)ts) / IndexingThread.ticksPerSecond).ToString("0.000000")
            + " seconds, " + index.LastAccessed + " candidates checked.";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("(c) 2013 Andrea Esuli\nhttp://www.esuli.it", "MiPai image demo application", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }
    }
}
