<%@ Page Title="" Language="C#" MasterPageFile="~/Dashboards.Master" AutoEventWireup="true" CodeBehind="NetworkUtility.aspx.cs" Inherits="Dashboards.NetworkUtility" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView.Export" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPivotGrid" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.ASPxPivotGrid.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPivotGrid.Export" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpContentTop" runat="server">
    <div class="row">
        <div class="col-lg-12">
                <h3 class="page-header right">Network Utilization </h3>
        </div>
        <!-- /.col-lg-12 -->
    </div>

    <div class="row">

            <div class="col-lg-12">
                                
                <dx:ASPxDateEdit ID="dtStart" runat="server"></dx:ASPxDateEdit>
                <dx:ASPxDateEdit ID="dtEnd" runat="server"></dx:ASPxDateEdit>
        <br />


                            <dx:ASPxGridView ID="gvNetworkUtil" ClientInstanceName="grid" runat="server" AutoGenerateColumns="False" DataSourceID="dsNetworkUtilization" Width="100%" Theme="Moderno" EnableTheming="True" OnHeaderFilterFillItems="gvNetworkUtil_HeaderFilterFillItems" OnHtmlDataCellPrepared="gvNetworkUtil_HtmlDataCellPrepared">
                                <Columns>
                                    <dx:GridViewCommandColumn VisibleIndex="0">
                                    </dx:GridViewCommandColumn>
                                    <dx:GridViewDataTextColumn Caption="Regional" FieldName="regional" GroupIndex="1" VisibleIndex="1">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="Cabang" FieldName="router_displayname" VisibleIndex="2">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataDateColumn Caption="Tanggal" FieldName="sDate" GroupIndex="0" VisibleIndex="3">
                                        <PropertiesDateEdit DisplayFormatString="MMM-yyyy">
                                        </PropertiesDateEdit>
                                        <Settings GroupInterval="DateMonth" />
                                    </dx:GridViewDataDateColumn>
                                    <dx:GridViewBandColumn Caption="Link 1" VisibleIndex="4">
                                        <Columns>
                                            <dx:GridViewDataTextColumn Caption="Type" FieldName="LinkType1" ReadOnly="True" >
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn Caption="Provider" FieldName="Provider1" ReadOnly="True" >
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn Caption="(Kbps)" FieldName="Speed1" ReadOnly="True">
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn Caption="Util %" FieldName="Persen1" ReadOnly="True">
                                            </dx:GridViewDataTextColumn>
                                        </Columns>
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </dx:GridViewBandColumn>
                                    <dx:GridViewBandColumn Caption="Link 2" VisibleIndex="5">
                                        <Columns>
                                            <dx:GridViewDataTextColumn Caption="Type" FieldName="LinkType2" ReadOnly="True" >
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn Caption="Provider" FieldName="Provider2" ReadOnly="True">
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn Caption="(Kbps)" FieldName="Speed2" ReadOnly="True">
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn Caption="Util %" FieldName="Persen2" ReadOnly="True">
                                            </dx:GridViewDataTextColumn>
                                        </Columns>
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </dx:GridViewBandColumn>
                                </Columns>
                                <SettingsBehavior AutoExpandAllGroups="True" />
                                <SettingsPager Mode="ShowAllRecords">
                                </SettingsPager>
                                <Settings ShowGroupPanel="true" ShowHeaderFilterButton="True" />
                                <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />
                        </dx:ASPxGridView>
                    
                    <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="gvNetworkUtil"></dx:ASPxGridViewExporter>

                        <asp:ObjectDataSource ID="dsNetworkUtilization" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" TypeName="Dashboards.Models.dsETLManageEngineTableAdapters.usp_GetPivotResultTableAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="dtStart" Name="startDate" PropertyName="Value" Type="DateTime" />
                                <asp:ControlParameter ControlID="dtEnd" Name="endDate" PropertyName="Value" Type="DateTime" />
                            </SelectParameters>
                         </asp:ObjectDataSource>
            </div>


    </div>

</asp:Content>
