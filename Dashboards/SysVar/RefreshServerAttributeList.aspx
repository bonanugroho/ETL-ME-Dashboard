<%@ Page Title="" Language="C#" MasterPageFile="~/Dashboards.Master" AutoEventWireup="true" CodeBehind="RefreshServerAttributeList.aspx.cs" Inherits="Dashboards.SysVar.RefreshServerAttributeList" %>
<%@ Register assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxEditors" tagprefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpContentTop" runat="server">

    <table>
        <tr>
            <td>
                Current
            </td>
            <td>
                :
            </td>
            <td>
                <dx:ASPxLabel ID="lbCurrent" runat="server" Text="-"></dx:ASPxLabel>
            </td>
        </tr>
        <tr>
            <td>
                Uploaded
            </td>
            <td>
                :
            </td>
            <td>
                <dx:ASPxLabel ID="lblUploaded" runat="server" Text="-"></dx:ASPxLabel>
            </td>
        </tr>
    </table>
    <br />

    <dx:ASPxButton ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" Text="Refresh">
    </dx:ASPxButton>
   

</asp:Content>
