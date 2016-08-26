<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebClimbing.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 282px;
        }
    </style>
</head>
<body>
<asp:Panel ID="pnl" runat="server" BackColor="Orange">
<form id="frm1" runat="server">
    <asp:scriptmanager ID="Scriptmanager1" runat="server" EnablePageMethods="true" EnablePartialRendering="true"></asp:scriptmanager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <table width="100%">
        <tr align="center">
            <td colspan="2">
                <asp:Label ID="lblTitle" runat="server" Text="Добро пожаловать в программу ClimbingCompetition"
                     Font-Bold="true" Font-Size="Larger" ForeColor="Blue" Font-Names="Comic Sans Ms" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblSelComp" runat="server" Text="Выберите соревнование" />
            </td>
            <td align="left">
                <asp:DropDownList ID="cbSelectComp" runat="server" TabIndex="1" 
                    AutoPostBack="true" 
                    onselectedindexchanged="cbSelectComp_SelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                        <asp:GridView ID="competitionParamsGrid" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:Button ID="btnProceed" runat="server" Text="Продолжить" 
                    onclick="btnProceed_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
            </td>
        </tr>
        <!--
        <tr>
            <td align="right" colspan="2">
                <asp:HyperLink ID="hlLoginAdmin" runat="server" Text="войти в систему" Font-Size="Smaller" ForeColor="Blue"
                    NavigateUrl="~/login.aspx" Target="_self" />
            </td>
        </tr>
        -->
    </table>
    </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="cbSelectComp" />
        </Triggers>
    </asp:UpdatePanel>
    
    </form>
</asp:Panel>
</body>
</html>
