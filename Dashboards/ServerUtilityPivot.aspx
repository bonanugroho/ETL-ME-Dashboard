<%@ Page Title="" Language="C#" MasterPageFile="~/Dashboards.Master" AutoEventWireup="true" CodeBehind="ServerUtilityPivot.aspx.cs" Inherits="Dashboards.ServerUtility" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPivotGrid" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.ASPxPivotGrid.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPivotGrid.Export" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpContentTop" runat="server">
    <div class="row">
        <div class="col-lg-12">
                <h1 class="page-header right">Dashboard </h1>
        </div>
        <!-- /.col-lg-12 -->
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading"> Custom Reports Server Utilization </div>

                <div class="panel-body">
                    <table>
                        <tr>
                            <td></td>
                            <td>

                            <table >
                                <tr>
                                    <td>
                                        <strong>Export to:</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <dx:ASPxComboBox ID="listExportFormat" runat="server" Style="vertical-align: middle"
                                            SelectedIndex="0" ValueType="System.String" Width="100px">
                                            <Items>
                                                <dx:ListEditItem Text="Pdf" Value="0" />
                                                <dx:ListEditItem Text="Excel" Value="1" />
                                                <dx:ListEditItem Text="Mht" Value="2" />
                                                <dx:ListEditItem Text="Rtf" Value="3" />
                                                <dx:ListEditItem Text="Text" Value="4" />
                                                <dx:ListEditItem Text="Html" Value="5" />
                                            </Items>

<%--                                            <ClientSideEvents
                                                Init="function(s, e) {
                                                   var buttonsWidth = buttonSaveAs.GetWidth() + buttonOpen.GetWidth() + 4;
                                                   s.SetWidth(buttonsWidth);
                                                   checkCustomFormattedValuesAsText.SetEnabled(false);
                                                }"
                                                SelectedIndexChanged="function(s, e) {
                                                    var isExportToExcel = s.GetSelectedIndex() == 1;
                                                    checkCustomFormattedValuesAsText.SetEnabled(isExportToExcel);
                                                }"
                                             />
    --%>
                                        </dx:ASPxComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <dx:ASPxButton ID="buttonSaveAs" ClientInstanceName="buttonSaveAs" runat="server" ToolTip="Export and save"
                                            OnClick="buttonSaveAs_Click" Text="Save" />
                                        <dx:ASPxButton ID="buttonOpen" ClientInstanceName="buttonOpen" runat="server" ToolTip="Export and open"
                                            OnClick="buttonOpen_Click" Text="Open" />
                                    </td>
                                </tr>
                            </table>
                            </td>
                        </tr>


                    </table>

                    <div class="table-responsive">
                        <dx:ASPxPivotGrid CssClass="table" ID="gvDashboard" runat="server" ClientIDMode="AutoID" DataSourceID="dsServerUtilization" EnableTheming="True" OnCustomCellStyle="gvDashboard_CustomCellStyle" Theme="Moderno" Width="100%">
                            <Fields>
                                <dx:PivotGridField ID="fieldCalendarYear" Area="ColumnArea" AreaIndex="0" FieldName="CalendarYear">
                                </dx:PivotGridField>
                                <dx:PivotGridField ID="fieldCalendarMonth" Area="ColumnArea" AreaIndex="1" FieldName="CalendarMonth">
                                </dx:PivotGridField>
                                <dx:PivotGridField ID="fieldResourceType" AreaIndex="0" FieldName="ResourceType" Area="RowArea">
                                </dx:PivotGridField>
                                <dx:PivotGridField ID="fieldResourceName" Area="RowArea" AreaIndex="1" FieldName="ResourceName">
                                </dx:PivotGridField>
                                <dx:PivotGridField ID="fieldCPU" Area="DataArea" AreaIndex="0" FieldName="CPU" SummaryType="Max" CellFormat-FormatString="0.00" CellFormat-FormatType="Numeric">
                                </dx:PivotGridField>
                                <dx:PivotGridField ID="fieldHardDisk" Area="DataArea" AreaIndex="1" FieldName="HardDisk" SummaryType="Max" CellFormat-FormatString="0.00" CellFormat-FormatType="Numeric">
                                </dx:PivotGridField>
                                <dx:PivotGridField ID="fieldMemory" Area="DataArea" AreaIndex="2" FieldName="Memory" SummaryType="Max" CellFormat-FormatString="0.00" CellFormat-FormatType="Numeric">
                                </dx:PivotGridField>
                            </Fields>
                            <%--<OptionsView ShowColumnGrandTotalHeader="False" ShowColumnGrandTotals="False" ShowColumnHeaders="False" ShowColumnTotals="False" ShowDataHeaders="False" ShowFilterHeaders="False" ShowFilterSeparatorBar="False" ShowRowGrandTotalHeader="False" ShowRowGrandTotals="False" ShowRowTotals="False" />--%>
                            <OptionsView ShowColumnGrandTotalHeader="False" ShowColumnGrandTotals="False" ShowColumnHeaders="True" ShowColumnTotals="False" ShowDataHeaders="True" ShowFilterHeaders="True" ShowFilterSeparatorBar="True" ShowRowGrandTotalHeader="False" ShowRowGrandTotals="False" ShowRowTotals="False" />
                            <%--<OptionsCustomization AllowDrag="False" />--%>

                            <OptionsPager Visible="false">
                            </OptionsPager>

                        </dx:ASPxPivotGrid>

                    </div>
                </div>
                <%--<div class="panel-footer"> Panel Footer </div>--%>
            </div>
        </div>
        <!-- /.col-lg-12 -->
        <dx:ASPxPivotGridExporter ID="ASPxPivotGridExporter1" runat="server" ASPxPivotGridID="gvDashboard">
        </dx:ASPxPivotGridExporter>
    </div>

        <asp:ObjectDataSource ID="dsServerUtilization" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" TypeName="Dashboards.Models.dsETLManageEngineTableAdapters.urViewGetPivotTableAdapter"></asp:ObjectDataSource>

</asp:Content>
