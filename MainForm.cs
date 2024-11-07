﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using System.Windows.Media.Animation;

namespace EasyPrint
{
    public partial class MainForm : Form
    { 
        public MainForm()
        {
            InitializeComponent();

            // Attach event handlers
            comboboxPrinter.SelectedIndexChanged += new EventHandler(comboboxPrinter_SelectedIndexChanged);
            comboboxPaper.SelectedIndexChanged += new EventHandler(comboboxPaper_SelectedIndexChanged);
            buttonPickBlock.Click += new EventHandler(ButtonPickBlock_Click);         
            buttonPrint.Click += new EventHandler(ButtonPrint_Click);
            buttonPreview.Click += new EventHandler(ButtonPreview_Click);
            buttonClose.Click += new EventHandler(ButtonClose_Click);
            buttonHow.Click += new EventHandler(ButtonHow_Click);
            this.FormClosing += MainForm_FormClosing;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PrintHelper.DeleteRectangles(this);
        }
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ButtonHow_Click(object sender, EventArgs e) 
        {
            MessageBox.Show("(1) Pick a block from drawing \n(2) Select Sort Order \n(3) Click Print/Preview.\n(4) For any Issue or error, \nPlease send a snapshot or error code to: \nsanketpatel.ca@gmail.com");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            PopulateComboboxes();
        }

        private void PopulateComboboxes()
        {
            // Populate Printer ComboBox
            foreach (string plotDevice in PlotSettingsValidator.Current.GetPlotDeviceList())
            {
                comboboxPrinter.Items.Add(plotDevice);
            }
            comboboxPrinter.SelectedIndexChanged += comboboxPrinter_SelectedIndexChanged;

            
            // Populate Plotstyle ComboBox
            foreach (string plotStyle in PlotSettingsValidator.Current.GetPlotStyleSheetList())
            {
                comboboxPlotstyle.Items.Add(plotStyle);
            }

            //// Set default values if available
            SetDefaultComboBoxValue(comboboxPrinter, "PDFCreator");
            SetDefaultComboBoxValue(comboboxPaper, "ARCH D");
            SetDefaultComboBoxValue(comboboxPlotstyle, "EREBAR.ctb");
            SetDefaultComboBoxValue(comboBoxOrientation, "Auto");

            // Optionally set the first item if default is not found
            if (comboboxPrinter.SelectedIndex == -1 && comboboxPrinter.Items.Count > 0) comboboxPrinter.SelectedIndex = 0;
            if (comboboxPaper.SelectedIndex == -1 && comboboxPaper.Items.Count > 0) comboboxPaper.SelectedIndex = 0;
            if (comboboxPlotstyle.SelectedIndex == -1 && comboboxPlotstyle.Items.Count > 0) comboboxPlotstyle.SelectedIndex = 0;
            if (comboBoxOrientation.SelectedIndex == -1 && comboBoxOrientation.Items.Count > 0) comboBoxOrientation.SelectedIndex = 0;
        }

        private (string[],string[]) paperSizes,canoName;
        private string selectedCanonicalName;

        // Event handlers for ComboBox selection changes
        private void comboboxPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPrinter = comboboxPrinter.SelectedItem.ToString();
            paperSizes = PrintHelper.GetPaperSizes(selectedPrinter);

            // Update the paper sizes combobox
            comboboxPaper.Items.Clear();
            comboboxPaper.Items.AddRange(paperSizes.Item1);
        }
        // Event handler for paper size selection changes
        private void comboboxPaper_SelectedIndexChanged(Object sender, EventArgs e)
        {
            int selectedIndex = comboboxPaper.SelectedIndex;
            if (selectedIndex >=0 && selectedIndex < paperSizes.Item2.Length)
            {
                selectedCanonicalName = paperSizes.Item2[selectedIndex];
                //MessageBox.Show($"selected CanoName: {selectedCanonicalName}");
            }
        }


        //private void RadioButton_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (radioButtonBlock.Checked)
        //    {
        //        EnableControls(true, false);
        //    }
        //    else if (radioButtonRectangle.Checked)
        //    {
        //        EnableControls(false, true);
        //    }
        //}
        //private void EnableControls(bool blockEnabled, bool rectangleEnabled)
        //{
        //    radioButtonBlock.Enabled = blockEnabled;
        //    buttonPickBlock.Enabled = blockEnabled;
        //    radioButtonRectangle.Enabled= rectangleEnabled;
        //    buttonPickRectangle.Enabled = rectangleEnabled;
        //}
        private void ButtonPickBlock_Click(object sender, EventArgs e)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            if (PrintHelper.selectedBlockId.HasValue)
            {
                // Use the existing selection
               
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    BlockReference blockRef = tr.GetObject(PrintHelper.selectedBlockId.Value, OpenMode.ForRead) as BlockReference;
                    BlockTableRecord blockDef = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                    textBoxBlockName.Text = blockDef.Name;
                    tr.Commit();
                }
            }
            else
            {
                // Prompt the user to select a block
                this.WindowState = FormWindowState.Minimized;
                Editor ed = doc.Editor;

                PromptEntityOptions peo = new PromptEntityOptions("\nSelect a block: ");
                peo.SetRejectMessage("\nOnly blocks are allowed.");
                peo.AddAllowedClass(typeof(BlockReference), true);

                PromptEntityResult per = ed.GetEntity(peo);
                if (per.Status == PromptStatus.OK)
                {
                    using (Transaction tr = doc.TransactionManager.StartTransaction())
                    {
                        BlockReference blockRef = tr.GetObject(per.ObjectId, OpenMode.ForRead) as BlockReference;
                        BlockTableRecord blockDef = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                        textBoxBlockName.Text = blockDef.Name;
                        tr.Commit();
                    }
                }
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void ButtonPrint_Click(object sender, EventArgs e)
        {
            try

            {
                // Retrieve user selections
                this.WindowState = FormWindowState.Minimized;
                string printer = comboboxPrinter.SelectedItem.ToString();
                string paperSize = string.Empty;

                if (!string.IsNullOrEmpty(selectedCanonicalName))
                {
                    paperSize = selectedCanonicalName;
                }
                else
                {
                    MessageBox.Show("Please select a paper size first.");
                    return;
                }

                string plotStyle = comboboxPlotstyle.SelectedItem.ToString();
                string blockName = textBoxBlockName.Text;
                int copies = int.Parse(textBoxCopies.Text);
                string orientation = comboBoxOrientation.SelectedItem.ToString();
                //string sortOrder = GetSelectedSortOrder();

                // Perform the printing operation based on the user selections
                List<BlockReference> blocks = PrintHelper.GetBlocks(Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, blockName);
                //blocks = PrintHelper.SortBlocks(blocks, sortOrder);

                // Extract drawing numbers from the blocks
                var blocksWithNumbers = PrintHelper.GetNumbersFromDrawings(blocks, "DWGNO");

                // Sort blocks based on drawing numbers
                var sortedBlocks = blocksWithNumbers.OrderBy(b => b.Item2).Select(b => b.Item1).ToList();

                PrintHelper.PrintBlocks(blocks, printer, paperSize, plotStyle, copies, orientation, this);               

                this.WindowState = FormWindowState.Normal;
            }
            catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}");
                LogError(ex);               
            }

        }
        private void ButtonPreview_Click(object sender, EventArgs e)
        {
            try
            {
                this.WindowState = FormWindowState.Minimized;
                // Retrieve user selections
                string printer = comboboxPrinter.SelectedItem.ToString();
                string paperSize = string.Empty;

                if (!string.IsNullOrEmpty(selectedCanonicalName))
                {
                    paperSize = selectedCanonicalName;
                }
                else
                {
                    MessageBox.Show("Please select a paper size first.");
                    return;
                }

                string plotStyle = comboboxPlotstyle.SelectedItem.ToString();
                string blockName = textBoxBlockName.Text;
                int copies = int.Parse(textBoxCopies.Text);
                string orientation = comboBoxOrientation.SelectedItem.ToString();               
                string sortOrder = GetSelectedSortOrder();

                // Perform the printing operation based on the user selections
                List<BlockReference> blocks = PrintHelper.GetBlocks(Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, blockName);
                blocks = PrintHelper.SortBlocks(blocks, sortOrder);

                PrintHelper.PreviewBlocks(blocks, printer, paperSize, plotStyle, orientation, this);             

                this.WindowState = FormWindowState.Normal;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                LogError(ex);
            }
        }
     
        private string GetSelectedSortOrder()
        {
            if (radioButtonLRTB.Checked) return "LeftRightTopBottom";
            if (radioButtonRLTB.Checked) return "RightLeftTopBottom";
            if (radioButtonLRBT.Checked) return "LeftRightBottomTop";
            if (radioButtonRLBT.Checked) return "RightLeftBottomTop";
            return null;
             //MessageBox.Show("Select a sort order");
        }
        private void SetDefaultComboBoxValue(ComboBox comboBox, string defaultValue)
        {
            int index = comboBox.Items.IndexOf(defaultValue);
            if (index != -1)
            {
                comboBox.SelectedIndex = index;
            }
        }      

        private void LogError(Exception ex)
        {
            string logFilePath = "error.log";
            string logMessage = $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n";
            File.AppendAllText(logFilePath, logMessage);
        }      
    }
}