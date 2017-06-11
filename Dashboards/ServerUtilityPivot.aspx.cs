using DevExpress.XtraPivotGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dashboards
{
    public partial class ServerUtility : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void gvDashboard_CustomCellStyle(object sender, DevExpress.Web.ASPxPivotGrid.PivotCustomCellStyleEventArgs e)
        {
            if (e.ColumnValueType != PivotGridValueType.Value ||
                e.RowValueType != PivotGridValueType.Value) return;

            switch (e.DataField.FieldName)
            {
                case "CPU":
                    if (Convert.ToInt32(e.Value) > 80 && Convert.ToInt32(e.Value) <= 90)
                    {
                        e.CellStyle.BackColor = Color.FromName("#FFFFA3");
                        e.CellStyle.Font.Bold = true;
                    }
                    else if (Convert.ToInt32(e.Value) > 90)
                    {
                        e.CellStyle.BackColor = Color.FromName("#FFBCBC");
                        e.CellStyle.Font.Bold = true;
                    }
                    break;
                case "HardDisk":
                    if (Convert.ToInt32(e.Value) > 80 && Convert.ToInt32(e.Value) <= 90)
                    {
                        e.CellStyle.BackColor = Color.FromName("#FFFFA3");
                        e.CellStyle.Font.Bold = true;
                    }
                    else if (Convert.ToInt32(e.Value) > 90)
                    {
                        e.CellStyle.BackColor = Color.FromName("#FFBCBC");
                        e.CellStyle.Font.Bold = true;
                    }
                    break;
                case "Memory":
                    if (Convert.ToInt32(e.Value) > 80 && Convert.ToInt32(e.Value) <= 90)
                    {
                        e.CellStyle.BackColor = Color.FromName("#FFFFA3");
                        e.CellStyle.Font.Bold = true;
                    }
                    else if (Convert.ToInt32(e.Value) > 90)
                    {
                        e.CellStyle.BackColor = Color.FromName("#FFBCBC");
                        e.CellStyle.Font.Bold = true;
                    }
                    break;
            }

            /*
            if (Convert.ToInt32(e.Value) > 90 &&
                (e.DataField.FieldName == "CPU") ||
                (e.DataField.FieldName == "HardDisk") ||
                (e.DataField.FieldName == "Memory")
                )
            {
                e.CellStyle.BackColor = Color.FromName("#FFBCBC");
                e.CellStyle.Font.Bold = true;
            }
            else if (Convert.ToInt32(e.Value) > 80 && Convert.ToInt32(e.Value) <= 90 &&
                (e.DataField.FieldName == "CPU") ||
                (e.DataField.FieldName == "HardDisk") ||
                (e.DataField.FieldName == "Memory")
                )
            {
                e.CellStyle.BackColor = Color.FromName("#FFFFA3");
                e.CellStyle.Font.Bold = true;
            }
            */
        }

        void Export(bool saveAs) 
        {
            //foreach(PivotGridField field in gvDashboard.Fields) {
            //    if(field.ValueFormat != null && !string.IsNullOrEmpty(field.ValueFormat.FormatString))
            //        field.UseNativeFormat = checkCustomFormattedValuesAsText.Checked ? DefaultBoolean.False : DefaultBoolean.True;
            //}
            //ASPxPivotGridExporter1.OptionsPrint.PrintHeadersOnEveryPage = checkPrintHeadersOnEveryPage.Checked;
            //ASPxPivotGridExporter1.OptionsPrint.MergeColumnFieldValues = checkMergeColumnFieldValues.Checked;
            //ASPxPivotGridExporter1.OptionsPrint.MergeRowFieldValues = checkMergeRowFieldValues.Checked;

            //ASPxPivotGridExporter1.OptionsPrint.PrintFilterHeaders = checkPrintFilterFieldHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
            //ASPxPivotGridExporter1.OptionsPrint.PrintColumnHeaders = checkPrintColumnFieldHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
            //ASPxPivotGridExporter1.OptionsPrint.PrintRowHeaders = checkPrintRowFieldHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;
            //ASPxPivotGridExporter1.OptionsPrint.PrintDataHeaders = checkPrintDataFieldHeaders.Checked ? DefaultBoolean.True : DefaultBoolean.False;

            string fileName = "PivotGrid";
            switch (listExportFormat.SelectedIndex)
            {
                case 0:
                    ASPxPivotGridExporter1.ExportPdfToResponse(fileName, saveAs);
                    break;
                case 1:
                    ASPxPivotGridExporter1.ExportXlsToResponse(fileName, saveAs);
                    break;
                case 2:
                    ASPxPivotGridExporter1.ExportMhtToResponse(fileName, "utf-8", "ASPxPivotGrid Printing Sample", true, saveAs);
                    break;
                case 3:
                    ASPxPivotGridExporter1.ExportRtfToResponse(fileName, saveAs);
                    break;
                case 4:
                    ASPxPivotGridExporter1.ExportTextToResponse(fileName, saveAs);
                    break;
                case 5:
                    ASPxPivotGridExporter1.ExportHtmlToResponse(fileName, "utf-8", "ASPxPivotGrid Printing Sample", true, saveAs);
                    break;
            }
        }

        protected void buttonOpen_Click(object sender, EventArgs e)
        {
            Export(false);
        }
        protected void buttonSaveAs_Click(object sender, EventArgs e)
        {
            Export(true);
        }
    }
}