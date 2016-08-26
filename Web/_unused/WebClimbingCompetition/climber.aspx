<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="climber.aspx.cs" Inherits="WebClimbing._Pclimber"
 MasterPageFile="~/WebAppl.Master" Theme="appSkin" EnableEventValidation="false"%>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="ClimberContent" runat="server">

    <script type="text/javascript">
        setInterval("OnTimer()", 1000);
        function loadS() {
            var lbl = $get("ctl00_ContentPlaceHolder1_Label2");
            if (lbl != null)
                lbl.innerHTML = "<img src=\"files/partnerscfr/ajax-loader.gif\" alt=\"Loading...\"/>";
        }
    </script>
    <table>
        <tr>
            <td>
                <div>
                    <asp:Label ID="lblName" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
                    <br />
                    №: <asp:Label ID="lblIid" runat="server" Font-Bold="true"></asp:Label><br />
                    группа : <asp:Label ID="lblGroup" runat="server" Font-Bold="true"></asp:Label><br />
                    команда: <asp:Label ID="lblTeam" runat="server" Font-Bold="true"></asp:Label><br />
                    Г.р.: <asp:Label ID="lblAge" runat="server" Font-Bold="true"></asp:Label><br />
                    Разряд: <asp:Label ID="lblQf" runat="server" Font-Bold="true"></asp:Label>
                    <br />
                    <asp:Label ID="Label3" runat="server"></asp:Label>
                    <br />
                </div>
                <div>
                    <asp:Label ID="Label1" runat="server" Font-Bold="true" Font-Size="Medium" Text="Рейтинг:"></asp:Label>
                    <asp:Label ID="lblRLead" runat="server" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lblRSpeed" runat="server" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lblRBoulder" runat="server" Font-Bold="true"></asp:Label>
                </div>
            </td>
                
            <td>
            <asp:Label ID="Label2" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Image ID="photoBox" runat="server" Height="300"/>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblRes" runat="server" Text="Результаты:" Font-Bold="true"></asp:Label><br />
                <asp:Label ID="lblLead" runat="server" Text="Трудность:" Visible="false"></asp:Label>
                <asp:BulletedList ID="listLead" runat="server" DisplayMode="HyperLink" Visible="false" ForeColor="White"></asp:BulletedList>
                <asp:Label ID="lblSpeed" runat="server" Text="Скорость:" Visible="false"></asp:Label>
                <asp:BulletedList ID="listSpeed" runat="server" DisplayMode="HyperLink" Visible="false" ForeColor="White"></asp:BulletedList>
                <asp:Label ID="lblBoulder" runat="server" Text="Боулдеринг:" Visible="false"></asp:Label>
                <asp:BulletedList ID="listBoulder" runat="server" DisplayMode="HyperLink" Visible="false" ForeColor="White"></asp:BulletedList>
            </td>
        </tr>
    </table>
       
</asp:Content>
