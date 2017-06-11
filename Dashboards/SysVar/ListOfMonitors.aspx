<%@ Page Title="" Language="C#" MasterPageFile="~/Dashboards.Master" AutoEventWireup="true" CodeBehind="ListOfMonitors.aspx.cs" Inherits="Dashboards.SysVar.ListOfMonitors" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<%@ Register assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpContentTop" runat="server">
    <div class="row">
        <div class="col-lg-12">
                <h1 class="page-header right">List Of Server </h1>
        </div>
        <!-- /.col-lg-12 -->
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading"> Settings </div>
                <div class="panel-body">
                    <div class="table-responsive">
                        <dx:ASPxGridView ID="gvListOfMonitors" runat="server" AutoGenerateColumns="False" DataSourceID="dsListOfMonitors" EnableTheming="True" KeyFieldName="ResourceId" Theme="MetropolisBlue" Width="100%">
                            <Columns>
                                <dx:GridViewCommandColumn ShowEditButton="True" VisibleIndex="0">
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataTextColumn FieldName="ResourceId" ReadOnly="True" VisibleIndex="1" Visible="False">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ResourceName" VisibleIndex="2">
                                    <Settings AutoFilterCondition="Contains" />
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ResourceDisplayName" VisibleIndex="3">
                                    <Settings AutoFilterCondition="Contains" />
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ResourceType" VisibleIndex="4">
                                    <Settings AutoFilterCondition="Contains" />
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="TypeShortName" VisibleIndex="5">
                                    <Settings AutoFilterCondition="Contains" />
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ResourceParent" VisibleIndex="6" Visible="False">
                                </dx:GridViewDataTextColumn>
                            </Columns>
                            <SettingsBehavior FilterRowMode="OnClick" />
                            <Settings ShowFilterRow="True"  />
                            <SettingsEditing EditFormColumnCount="1" Mode="PopupEditForm" />
                            <SettingsPopup>
                                <EditForm Modal="True" />
                            </SettingsPopup>
                            <SettingsDataSecurity AllowDelete="False" AllowInsert="False" />

                        </dx:ASPxGridView>
                        <asp:ObjectDataSource ID="dsListOfMonitors" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" TypeName="Dashboards.Models.dsETLManageEngineTableAdapters.dimResourcesTableAdapter" UpdateMethod="UpdateQuery">
                            <UpdateParameters>
                                <asp:Parameter Name="ResourceId" Type="Int32" />
                                <asp:Parameter Name="ResourceName" Type="String" />
                                <asp:Parameter Name="ResourceDisplayName" Type="String" />
                                <asp:Parameter Name="ResourceType" Type="String" />
                                <asp:Parameter Name="TypeShortName" Type="String" />
                                <asp:Parameter Name="ResourceParent" Type="Int32" />
                                <asp:Parameter Name="original_ResourceId" Type="Int32" />
                            </UpdateParameters>
                        </asp:ObjectDataSource>
                    </div>
                </div>
                <%--<div class="panel-footer"> Panel Footer </div>--%>
            </div>
        </div>
        <!-- /.col-lg-12 -->
    </div>

</asp:Content>

