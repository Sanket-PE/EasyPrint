using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Geometry;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System;
using Autodesk.AutoCAD.EditorInput;

namespace EasyPrint
{
    public class PrintHelper
    {
        //Static variable to store the selected block information.
        public static ObjectId? selectedBlockId = null;
        [CommandMethod("EP")]
        public void EP()
        {
            MainForm mf = new MainForm();
            mf.Show();
        }

        // Get List of Blocks matching with the user selected.
        public static List<BlockReference> GetBlocks(Database db, string blockName)
        {
            List<BlockReference> blocks = new List<BlockReference>();
            try
            {

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                    foreach (ObjectId objId in btr)
                    {
                        Entity ent = tr.GetObject(objId, OpenMode.ForRead) as Entity;
                        if (ent is BlockReference)
                        {
                            BlockReference blockRef = ent as BlockReference;
                            BlockTableRecord blockDef = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                            if (blockDef.Name == blockName)
                            {
                                blocks.Add(blockRef);
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                LogError(ex);
            }
            //MessageBox.Show($"Retrieved {blocks.Count} blocks with name {blockName}");
            return blocks;
        }

        public static List<(BlockReference, string)> GetNumbersFromDrawings(List<BlockReference> blocks, string attributeName)
        {
            List<(BlockReference, string)> blocksWithNumbers = new List<(BlockReference, string)>();
            try
            {

                using (Transaction tr = blocks.First().Database.TransactionManager.StartTransaction())
                {
                    //BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    //BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                    foreach (BlockReference blockRef in blocks)
                    {
                        foreach (ObjectId attId in blockRef.AttributeCollection)
                        {
                            AttributeReference attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                            if (attRef != null && attRef.Tag == attributeName)
                            {
                                blocksWithNumbers.Add((blockRef, attRef.TextString));
                                break;
                            }
                        }

                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                LogError(ex);
            }
            //MessageBox.Show($"Retrieved {blocks.Count} blocks with name {blockName}");
            return blocksWithNumbers;
        }

        //Get sorted list as per user selection, in list.
        //public static List<BlockReference> SortBlocks(List<BlockReference> blocks, string sortOrder)
        //{
        //    try
        //    {
        //        switch (sortOrder)
        //        {
        //            case "LeftRightTopBottom":
        //                return blocks.OrderBy(b => b.Position.X).ThenBy(b => b.Position.Y).ToList();
        //            case "RightLeftTopBottom":
        //                return blocks.OrderByDescending(b => b.Position.X).ThenBy(b => b.Position.Y).ToList();
        //            case "LeftRightBottomTop":
        //                return blocks.OrderBy(b => b.Position.X).ThenByDescending(b => b.Position.Y).ToList();
        //            case "RightLeftBottomTop":
        //                return blocks.OrderByDescending(b => b.Position.X).ThenByDescending(b => b.Position.Y).ToList();
        //            default:
        //                return blocks;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogError(ex);
        //        return blocks;
        //    }
        //}

        //Get local media name as per the canonical media name.
        public static (string[], string[]) GetPaperSizes(string printerName)
        {
            //Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            //Editor ed = doc.Editor;
            List<string> paperSizes = new List<string>();
            List<string> canoNames = new List<string>();

            try
            {

                PlotConfig plotConfig = PlotConfigManager.SetCurrentConfig(printerName);
                plotConfig.RefreshMediaNameList();

                foreach (string canonicalName in plotConfig.CanonicalMediaNames)
                {
                    string localMediaName = plotConfig.GetLocalMediaName(canonicalName);
                    paperSizes.Add(localMediaName);
                    canoNames.Add(canonicalName);
                    //ed.WriteMessage($"\nLocal Media Name: {localMediaName} ");
                    //ed.WriteMessage($"\nCanoName: {canonicalName}");
                }
            }
            catch (System.Exception ex)
            {
                LogError(ex);
            }

            return (paperSizes.ToArray(), canoNames.ToArray());

        }

        private static PlotRotation GetPlotRotation(string blockOrientation, double blockRotation)
        {
            switch (blockOrientation)
            {
                case "Portrait":
                    //if (blockRotation == 0) return PlotRotation.Degrees000;
                    if (blockRotation == 90) return PlotRotation.Degrees180;
                   // if (blockRotation == 180) return PlotRotation.Degrees180;
                    if (blockRotation == 270) return PlotRotation.Degrees000;
                    break;
                case "Landscape":
                    if (blockRotation == 0) return PlotRotation.Degrees090;
                    //if (blockRotation == 90) return PlotRotation.Degrees180;
                    if (blockRotation == 180) return PlotRotation.Degrees270;
                    //if (blockRotation == 270) return PlotRotation.Degrees000;
                    break;
            }
            return PlotRotation.Degrees000; // Default to 0 degrees if no match
        }

        public static void AddLayer(Database db, string layerName, Autodesk.AutoCAD.Colors.Color color)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                if (!lt.Has(layerName))
                {
                    lt.UpgradeOpen();
                    LayerTableRecord ltr = new LayerTableRecord
                    {
                        Name = layerName,
                        Color = color
                    };
                    lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);
                }
                tr.Commit();
            }
        }

        public static ObjectId AddRectangle(Database db, Point2d minPoint, Point2d maxPoint, string layerName)
        {
            ObjectId rectId = ObjectId.Null;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                Polyline rectangle = new Polyline();
                rectangle.AddVertexAt(0, minPoint, 0, 0, 0);
                rectangle.AddVertexAt(1, new Point2d(maxPoint.X, minPoint.Y), 0, 0, 0);
                rectangle.AddVertexAt(2, maxPoint, 0, 0, 0);
                rectangle.AddVertexAt(3, new Point2d(minPoint.X, maxPoint.Y), 0, 0, 0);
                rectangle.Closed = true;
                rectangle.Layer = layerName;

                rectId = btr.AppendEntity(rectangle);
                tr.AddNewlyCreatedDBObject(rectangle, true);

                tr.Commit();
            }

            return rectId;
        }
        //Print Function.
        public static void PrintBlocks(List<BlockReference> blocks, string printer, string paperSize, string plotStyle, int copies, string orientation, MainForm mainForm)
        {
            mainForm.WindowState = FormWindowState.Minimized;
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", 0);
            List<ObjectId> rectangleIds = new List<ObjectId>();
            //MessageBox.Show($"Printer: {printer}, PaperSize: {paperSize}, PlotStyle: {plotStyle}, Copies: {copies}, Orientation: {orientation}");
            try
            {
                using (DocumentLock documentLock = doc.LockDocument())
                {
                    // Delete existing rectangles
                    DeleteExistingRectangles(db, "_EP");

                    // Add the _EP layer
                    AddLayer(db, "_EP", Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red));

                    // Extract drawing numbers from the blocks
                    var blocksWithNumbers = GetNumbersFromDrawings(blocks, "DWGNO");

                    // Sort blocks based on drawing numbers
                    var sortedBlocks = blocksWithNumbers.OrderBy(b => b.Item2).Select(b => b.Item1).ToList();

                    for (int i = 0; i < copies; i++)
                    {

                        foreach (BlockReference blockRef in blocks)
                        {
                            Extents3d extents3d = blockRef.GeometricExtents;
                            Point2d minPoint = new Point2d(extents3d.MinPoint.X, extents3d.MinPoint.Y);
                            Point2d maxPoint = new Point2d(extents3d.MaxPoint.X, extents3d.MaxPoint.Y);

                            //MessageBox.Show($"\nPrinting block: {blockRef.Name},min: {minPoint},max: {maxPoint}");
                           
                            using (Transaction tr2 = db.TransactionManager.StartTransaction())
                            {
                                // We'll be plotting the current layout
                                BlockTableRecord btr = (BlockTableRecord)tr2.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                                Layout lo = (Layout)tr2.GetObject(btr.LayoutId, OpenMode.ForRead);

                                // Get the PlotInfo from the layout
                                PlotInfo pi = new PlotInfo();
                                pi.Layout = lo.ObjectId;

                                // We need a PlotSettings object based on the layout settings which we then customize
                                PlotSettings ps = new PlotSettings(lo.ModelType);
                                ps.CopyFrom(lo);

                                //The PlotSettingsValidator helps create a valid PlotSettings object
                                PlotSettingsValidator psv = PlotSettingsValidator.Current;

                                // We'll plot the extents, centered and scaled to fit
                                psv.SetPlotWindowArea(ps, new Extents2d(minPoint, maxPoint));
                                psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Window);
                                psv.SetUseStandardScale(ps, true);
                                psv.SetStdScaleType(ps, StdScaleType.ScaleToFit);
                                psv.SetPlotCentered(ps, true);
                                psv.SetPlotConfigurationName(ps, printer, paperSize);

                                //Determine plot rotation based on orientation and block rotation
                                string blockOrientation = orientation == "Auto" ? DetermineOrientation(minPoint, maxPoint) : orientation;
                                double blockRotation = blockRef.Rotation * (180.0 / Math.PI); // Convert radians to degrees
                                PlotRotation plotRotation = GetPlotRotation(blockOrientation, blockRotation);
                                psv.SetPlotRotation(ps, plotRotation);
                                //psv.SetPlotRotation(ps, blockOrientation == "Portrait" ? PlotRotation.Degrees000 : PlotRotation.Degrees090);                              

                                // Apply plot style
                                psv.SetCurrentStyleSheet(ps, plotStyle);
                                //MessageBox.Show($"Printer: {printer}, PaperSize: {paperSize}, PlotStyle: {plotStyle}, Orientation: {orientation}, PlotRotation: {plotRotation}");


                                // We need to link the PlotInfo to the PlotSettings and then validate it
                                pi.OverrideSettings = ps;
                                PlotInfoValidator piv = new PlotInfoValidator();
                                piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                                piv.Validate(pi);

                                // A PlotEngine does the actual plotting (can also create one for Preview)
                                if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
                                {
                                    using (PlotEngine pe = PlotFactory.CreatePublishEngine())
                                    {
                                        // Create a Progress Dialog to provide info and allow thej user to cancel                                       
                                        using (PlotProgressDialog ppd = new PlotProgressDialog(false, 1, true))
                                        {
                                            ppd.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Custom Plot Progress");
                                            ppd.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");
                                            ppd.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");
                                            ppd.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");
                                            ppd.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");
                                            ppd.LowerPlotProgressRange = 0;
                                            ppd.UpperPlotProgressRange = 100;
                                            ppd.PlotProgressPos = 0;

                                            // Let's start the plot, at last
                                            ppd.OnBeginPlot();
                                            ppd.IsVisible = true;
                                            pe.BeginPlot(ppd, null);

                                            // We'll be plotting a single document
                                            pe.BeginDocument(pi, doc.Name, null, copies, false, doc.Name);

                                            // Which contains a single sheet
                                            ppd.OnBeginSheet();
                                            ppd.LowerSheetProgressRange = 0;
                                            ppd.UpperSheetProgressRange = 100;
                                            ppd.SheetProgressPos = 0;

                                            PlotPageInfo ppi = new PlotPageInfo();
                                            pe.BeginPage(ppi, pi, true, null);
                                            pe.BeginGenerateGraphics(null);
                                            pe.EndGenerateGraphics(null);

                                            // Finish the sheet
                                            pe.EndPage(null);
                                            ppd.SheetProgressPos = 100;
                                            ppd.OnEndSheet();

                                            // Finish the document
                                            pe.EndDocument(null);

                                            // And finish the plot
                                            ppd.PlotProgressPos = 100;
                                            ppd.OnEndPlot();
                                            pe.EndPlot(null);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Another plot in progress.");
                                }
                                tr2.Commit();
                            }
                            // Add a rectangle around the block
                            ObjectId rectId = AddRectangle(db, minPoint, maxPoint, "_EP");
                            rectangleIds.Add(rectId);
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                LogError(ex);
            }
            finally
            {
                mainForm.WindowState = FormWindowState.Normal;
            }
            mainForm.Tag = rectangleIds;
        }
        // Preview Function.
        public static void PreviewBlocks(List<BlockReference> blocks, string printer, string paperSize, string plotStyle, string orientation, MainForm mainForm)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            //MessageBox.Show($"Printer: {printer}, PaperSize: {paperSize}, PlotStyle: {plotStyle}, Orientation: {orientation}");
            if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
            {
                mainForm.WindowState = FormWindowState.Minimized;
                int blockNum = 0;
                bool isFinished = false;
                bool isReadyForPlot = false;

                while (!isFinished)
                {
                    PreviewEngineFlags flags = PreviewEngineFlags.Plot;
                    if (blockNum > 0)
                        flags |= PreviewEngineFlags.PreviousSheet;
                    if (blockNum < blocks.Count - 1)
                        flags |= PreviewEngineFlags.NextSheet;


                    using (PlotEngine pre = PlotFactory.CreatePreviewEngine((int)flags))
                    {
                        PreviewEndPlotStatus stat = MultiplePreview(pre, true, blocks, blockNum, printer, paperSize, plotStyle, orientation);

                        if (stat == PreviewEndPlotStatus.Next)
                        {
                            blockNum++;
                        }
                        else if (stat == PreviewEndPlotStatus.Previous)
                        {
                            blockNum--;
                        }
                        else if (stat == PreviewEndPlotStatus.Normal || stat == PreviewEndPlotStatus.Cancel)
                        {
                            isFinished = true;
                        }
                        else if (stat == PreviewEndPlotStatus.Plot)
                        {
                            isFinished = true;
                            isReadyForPlot = true;
                        }
                    }
                }
                

                if (isReadyForPlot)
                {                   
                    using (PlotEngine ple = PlotFactory.CreatePublishEngine())
                    {
                        PreviewEndPlotStatus stat = MultiplePreview(ple, false, blocks, -1, printer, paperSize, plotStyle, orientation);
                    }
                }
            }
            else
            {
                doc.Editor.WriteMessage("\nAnother plot is in progress.");
            }
            mainForm.WindowState = FormWindowState.Normal;
        }
        static PreviewEndPlotStatus MultiplePreview(PlotEngine pe, bool isPreview, List<BlockReference> blocks, int blockNumIfPreview, string printer, string paperSize, string plotStyle, string orientation)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            PreviewEndPlotStatus ret = PreviewEndPlotStatus.Cancel;
            List<BlockReference> blocksToPlot;

            if (isPreview && blockNumIfPreview >= 0)
            {
                blocksToPlot = new List<BlockReference> { blocks[blockNumIfPreview] };
            }
            else
            {
                blocksToPlot = new List<BlockReference>(blocks);
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {             
                using (PlotProgressDialog ppd = new PlotProgressDialog(isPreview, blocksToPlot.Count, true))
                {
                    int numSheet = 1;
                    foreach (BlockReference blockRef in blocksToPlot)
                    {
                        Extents3d extents3d = blockRef.GeometricExtents;
                        Point2d minPoint = new Point2d(extents3d.MinPoint.X, extents3d.MinPoint.Y);
                        Point2d maxPoint = new Point2d(extents3d.MaxPoint.X, extents3d.MaxPoint.Y);

                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                        Layout lo = (Layout)tr.GetObject(btr.LayoutId, OpenMode.ForRead);

                        PlotInfo pi = new PlotInfo();
                        pi.Layout = lo.ObjectId;

                        PlotSettings ps = new PlotSettings(lo.ModelType);
                        ps.CopyFrom(lo);

                        PlotSettingsValidator psv = PlotSettingsValidator.Current;
                        psv.SetPlotWindowArea(ps, new Extents2d(minPoint, maxPoint));
                        psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Window);
                        psv.SetUseStandardScale(ps, true);
                        psv.SetStdScaleType(ps, StdScaleType.ScaleToFit);
                        psv.SetPlotCentered(ps, true);
                        psv.SetPlotConfigurationName(ps, printer, paperSize);

                        // Determine plot rotation based on orientation and block rotation
                        string blockOrientation = orientation == "Auto" ? DetermineOrientation(minPoint, maxPoint) : orientation;
                        double blockRotation = blockRef.Rotation * (180.0 / Math.PI); // Convert radians to degrees
                        PlotRotation plotRotation = GetPlotRotation(blockOrientation, blockRotation);
                        psv.SetPlotRotation(ps, plotRotation);
                        psv.SetCurrentStyleSheet(ps, plotStyle);

                        pi.OverrideSettings = ps;
                        PlotInfoValidator piv = new PlotInfoValidator();
                        piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                        piv.Validate(pi);

                        if (numSheet == 1)
                        {
                            ppd.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Custom Preview Progress");
                            ppd.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");
                            ppd.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");
                            ppd.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");
                            ppd.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");
                            ppd.LowerPlotProgressRange = 0;
                            ppd.UpperPlotProgressRange = 100;
                            ppd.PlotProgressPos = 0;

                            ppd.OnBeginPlot();
                            ppd.IsVisible = true;
                            pe.BeginPlot(ppd, null);
                            pe.BeginDocument(pi, doc.Name, null,1, false,doc.Name);
                        }

                        ppd.LowerSheetProgressRange = 0;
                        ppd.UpperSheetProgressRange = 100;
                        ppd.SheetProgressPos = 0;

                        PlotPageInfo ppi = new PlotPageInfo();
                        pe.BeginPage(ppi, pi, (numSheet == blocksToPlot.Count), null);
                        ppd.OnBeginSheet();
                        pe.BeginGenerateGraphics(null);
                        ppd.SheetProgressPos = 50;
                        pe.EndGenerateGraphics(null);

                        PreviewEndPlotInfo pepi = new PreviewEndPlotInfo();
                        pe.EndPage(pepi);
                        ret = pepi.Status;
                        ppd.SheetProgressPos = 100;
                        ppd.OnEndSheet();
                        numSheet++;

                        ppd.PlotProgressPos += (100 / blocksToPlot.Count);
                    }

                    pe.EndDocument(null);
                    ppd.PlotProgressPos = 100;
                    ppd.OnEndPlot();
                    pe.EndPlot(null);
                }
            }

            return ret;
        }

        private static string DetermineOrientation(Point2d minPoint, Point2d maxPoint)
        {
            double width = maxPoint.X - minPoint.X;
            double height = maxPoint.Y - minPoint.Y;
            return width > height ? "Landscape" : "Portrait";
        }

        private static void LogError(System.Exception ex)
        {
            string logFilePath = "error.log";
            string logMessage = $"{System.DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n";
            File.AppendAllText(logFilePath, logMessage);
            MessageBox.Show($"An error occurred: {ex.Message}\n\nPlease check the log file for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void DeleteExistingRectangles(Database db, string layerName)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            using (DocumentLock documentLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                    foreach (ObjectId objId in btr)
                    {
                        Entity ent = tr.GetObject(objId, OpenMode.ForWrite) as Entity;
                        if (ent != null && ent.Layer == layerName && ent is Polyline)
                        {
                            ent.Erase();
                        }
                    }
                    tr.Commit();
                }
            }
        }
        public static void DeleteRectangles(MainForm mainForm)
        {
            if (mainForm.Tag is List<ObjectId> rectangleIds)
            {
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;

                try
                {
                    using (DocumentLock documentLock = doc.LockDocument())
                    {

                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                            foreach (ObjectId rectId in rectangleIds)
                            {
                                Entity ent = (Entity)tr.GetObject(rectId, OpenMode.ForWrite);
                                ent.Erase();
                            }
                            tr.Commit();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogError(ex);
                }
            }
        }


        [CommandMethod("EXPAT")]
        public void EXPAT()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            //prompt the user to select a block
            PromptEntityOptions peo = new PromptEntityOptions("\nSelect a title block ");
            peo.SetRejectMessage("\nOnly blocks are allowed");
            peo.AddAllowedClass(typeof(BlockReference), true);
            PromptEntityResult per = ed.GetEntity(peo);

            if (per.Status == PromptStatus.OK)
            {
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    BlockReference blockRef = tr.GetObject(per.ObjectId, OpenMode.ForRead) as BlockReference;
                    BlockTableRecord blockDef = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                    string blockName = blockDef.Name;

                    //Get all matching blocks
                    List<BlockReference> blocks = GetBlocks(doc.Database, blockName);

                    //Get attributes from all blocks
                    List<Dictionary<string, string>> attributesList = new List<Dictionary<string, string>>();
                    foreach (BlockReference blkref in blocks)
                    { 
                        Dictionary<string, string> attributes = new Dictionary<string, string>();
                        foreach (ObjectId attId in blkref.AttributeCollection)
                        {
                            AttributeReference attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                            if (attRef != null)
                            {
                                attributes[attRef.Tag] = attRef.TextString;
                            }
                        }

                        attributesList.Add(attributes);
                    }
                    // Export to CSV
                    string dwgPath = doc.Name;
                    string csvPath = Path.ChangeExtension(dwgPath, ".csv");
                    ExportAttriutesToCSV(attributesList, csvPath);
                    ed.WriteMessage($"\nAttributes exported to {csvPath}");
                    tr.Commit();
                    
                }
            }
        }
        private void ExportAttriutesToCSV(List<Dictionary<string, string>> attributesList, string csvPath)
        {
            using (StreamWriter sw = new StreamWriter(csvPath))
            {
                if (attributesList.Count > 0)
                {
                    //Write CSV Header
                    var header = string.Join(",", attributesList[0].Keys);
                    sw.WriteLine(header);

                    //Write CSV Rows
                    foreach (var attributes in attributesList) 
                    {
                        var row = string.Join(",", attributes.Values);
                        sw.WriteLine(row);
                    }
                }

            }
        }
    }
}