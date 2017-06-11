using DevExpress.Web.ASPxGridView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dashboards
{
    public partial class ServerUtility1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected virtual void PreparePersen1FilterItems(ASPxGridViewHeaderFilterEventArgs e)
        {
            e.Values.Clear();
            if (e.Column.Settings.HeaderFilterMode == HeaderFilterMode.List)
                e.AddShowAll();

            e.AddValue(string.Format("< 60%"), "", string.Format("[Persen1] < 60"));
            e.AddValue(string.Format(">= 60%"), "", string.Format("[Persen1] >= 60"));
            e.AddValue(string.Format(">= 70%"), "", string.Format("[Persen1] >= 70"));
            e.AddValue(string.Format(">= 80%"), "", string.Format("[Persen1] >= 80"));
            e.AddValue(string.Format(">= 90%"), "", string.Format("[Persen1] >= 90"));
        }

        protected virtual void PreparePersen2FilterItems(ASPxGridViewHeaderFilterEventArgs e)
        {
            e.Values.Clear();
            if (e.Column.Settings.HeaderFilterMode == HeaderFilterMode.List)
                e.AddShowAll();

            e.AddValue(string.Format("< 60%"), "", string.Format("[Persen2] < 60"));
            e.AddValue(string.Format(">= 60%"), "", string.Format("[Persen2] >= 60"));
            e.AddValue(string.Format(">= 70%"), "", string.Format("[Persen2] >= 70"));
            e.AddValue(string.Format(">= 80%"), "", string.Format("[Persen2] >= 80"));
            e.AddValue(string.Format(">= 90%"), "", string.Format("[Persen2] >= 90"));
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            dsNetworkUtilization.DataBind();
            gvNetworkUtil.ExpandAll();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            gridExport.WriteXlsToResponse();
        }

        protected void gvNetworkUtil_HeaderFilterFillItems(object sender, ASPxGridViewHeaderFilterEventArgs e)
        {
            if (e.Column.FieldName == "Persen1")
                PreparePersen1FilterItems(e);
            if (e.Column.FieldName == "Persen2")
                PreparePersen2FilterItems(e);
        }

        protected void gvNetworkUtil_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            //if (e.ColumnValueType != PivotGridValueType.Value ||
            //    e.RowValueType != PivotGridValueType.Value) return;

            if (e.CellValue.Equals(DBNull.Value))
                return;

            switch (e.DataColumn.FieldName)
            {
                case "Persen1":
                    if (Convert.ToInt32(e.CellValue) > 80 && Convert.ToInt32(e.CellValue) <= 90)
                    {
                        e.Cell.BackColor = Color.FromName("#FFFFA3");
                        e.Cell.Font.Bold = true;
                    }
                    else if (Convert.ToInt32(e.CellValue) > 90)
                    {
                        e.Cell.BackColor = Color.FromName("#FFBCBC");
                        e.Cell.Font.Bold = true;
                    }
                    break;
                case "Persen2":
                    if (Convert.ToInt32(e.CellValue) > 80 && Convert.ToInt32(e.CellValue) <= 90)
                    {
                        e.Cell.BackColor = Color.FromName("#FFFFA3");
                        e.Cell.Font.Bold = true;
                    }
                    else if (Convert.ToInt32(e.CellValue) > 90)
                    {
                        e.Cell.BackColor = Color.FromName("#FFBCBC");
                        e.Cell.Font.Bold = true;
                    }
                    break;
                //case "Memory":
                //    if (Convert.ToInt32(e.Value) > 80 && Convert.ToInt32(e.Value) <= 90)
                //    {
                //        e.CellStyle.BackColor = Color.FromName("#FFFFA3");
                //        e.CellStyle.Font.Bold = true;
                //    }
                //    else if (Convert.ToInt32(e.Value) > 90)
                //    {
                //        e.CellStyle.BackColor = Color.FromName("#FFBCBC");
                //        e.CellStyle.Font.Bold = true;
                //    }
                //    break;
            }

        }

    }
}