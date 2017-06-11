<%@ Page Title="" Language="C#" MasterPageFile="~/Dashboards.Master" AutoEventWireup="true" CodeBehind="ServerVariables.aspx.cs" Inherits="Dashboards.SysVar.ServerVariables" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<%@ Register assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpContentTop" runat="server">
    <div class="row">
        <div class="col-lg-12">
                <h1 class="page-header right">Server Variables </h1>
        </div>
        <!-- /.col-lg-12 -->
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading"> Settings </div>
                <div class="panel-body">
                    <div class="table-responsive">
                        <dx:ASPxGridView ID="gvServerVariables" runat="server" AutoGenerateColumns="False" DataSourceID="dsServerVariables" EnableTheming="True" KeyFieldName="ServerId" Theme="MetropolisBlue" Width="100%">
                            <Columns>
                                <dx:GridViewCommandColumn ShowEditButton="True" VisibleIndex="0">
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataTextColumn FieldName="ServerId" ReadOnly="True" Visible="False" VisibleIndex="1">
                                    <EditFormSettings Visible="False" />
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ServerHostName" VisibleIndex="2">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ServerIPAddress" VisibleIndex="3">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ServerPortNo" VisibleIndex="4">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ServerAPIKey" VisibleIndex="5">
                                </dx:GridViewDataTextColumn>
                            </Columns>
                            <SettingsPager Visible="False">
                            </SettingsPager>
                            <SettingsEditing EditFormColumnCount="1" Mode="PopupEditForm" />
                            <SettingsPopup>
                                <EditForm Modal="True" />
                            </SettingsPopup>
                            <SettingsDataSecurity AllowDelete="False" AllowInsert="False" />
                        </dx:ASPxGridView>
                        <asp:ObjectDataSource ID="dsServerVariables" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" TypeName="Dashboards.Models.dsETLManageEngineTableAdapters.ServerVariablesTableAdapter" UpdateMethod="UpdateQuery">
                            <UpdateParameters>
                                <asp:Parameter Name="ServerHostName" Type="String" />
                                <asp:Parameter Name="ServerIPAddress" Type="String" />
                                <asp:Parameter Name="ServerPortNo" Type="Int32" />
                                <asp:Parameter Name="ServerAPIKey" Type="String" />
                                <asp:Parameter Name="original_ServerId" Type="Int32" />
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
