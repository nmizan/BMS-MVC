<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Shared/ReportSite.Master" CodeBehind="InBondExBondRptVw.aspx.cs" Inherits="BMSPhase2Demo.Report.CrystalViewer.InBondExBondRptVw" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<asp:content id="Content1" contentplaceholderid="MainContent" runat="server">
    <h2>In-Bond Ex-Bond Raw Material Uses Status</h2>

    <%--<asp:Button value="Preview" Text="Preview" runat="server" ID="Preview" ValidationGroup="view" type="submit" OnClick="Preview_Click" />--%>

    <CR:CrystalReportViewer ID="InBondExBondRptVwr" runat="server" 
        HasCrystalLogo="False" Height="50px" EnableParameterPrompt="false" 
        ToolPanelWidth="200px" Width="350px" ToolPanelView="None" AutoDataBind="true" />


</asp:content>