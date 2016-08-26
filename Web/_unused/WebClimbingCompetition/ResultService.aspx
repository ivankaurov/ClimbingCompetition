<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResultService.aspx.cs" 
    Inherits="WebClimbing._PResultService" EnableEventValidation="false" Theme="appSkin" MasterPageFile="~/WebAppl.Master"%>
<%@ Register TagPrefix="clm" TagName="TimeRefresh" Src="~/TimeRefreshingControl.ascx"  %>




<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        
    </script>
      
      <asp:UpdatePanel ID="UpdatePanel2" runat="server">
      <ContentTemplate>
      <table>
        <tr>
          <td>
            <asp:DropDownList ID="styles" runat="server" Width="200px"
               onselectedindexchanged="styles_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList><br />
            <asp:DropDownList ID="groups" runat="server" AutoPostBack="True" Width="200px"
               onselectedindexchanged="groups_SelectedIndexChanged">
            </asp:DropDownList><br />
            <asp:DropDownList ID="rounds" runat="server" AutoPostBack="True" Width="200px"
               onselectedindexchanged="rounds_SelectedIndexChanged">
            </asp:DropDownList>
          </td>      
          <td>
            <asp:UpdateProgress ID="UpdateProgress1" runat="server">
            <ProgressTemplate>
               <img src="files/partnerscfr/ajax-loader.gif" alt="Loading..."/>
            </ProgressTemplate>
            </asp:UpdateProgress>
          </td>
        </tr>
      </table>
                <asp:Label ID="lblTitle" runat="server" Font-Size="Larger" Font-Bold="True"></asp:Label>
                <br />
                
                <asp:Timer ID="Timer1" runat="server" Interval="60000" OnTick="timerEv">
                </asp:Timer>
                <asp:Label ID="lblTMP" runat="server" Text="До обновления осталось 60 сек."></asp:Label>
                <asp:GridView ID="GridView1" runat="server" Font-Underline="False">
                </asp:GridView>
                <asp:Label ID="lblList" runat="server"></asp:Label>
                </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        //setInterval("OnTimerN()", 1000);
        var timeLeft = 60;
        function OnTimerN() {
            if (timeLeft >= 0) {
                timeLeft = timeLeft - 1;
                b = true;
                if (timeLeft < 0) 
                    b = false;
                }
                var lbl = $get("ContentPlaceHolder1_lblTMP");
                if (lbl != null) {
                    if (b)
                        lbl.innerHTML = "до обновления осталось " + timeLeft + " сек.";
                    else
                        lbl.innerHTML = "Загрузка...";
                }
            }
        

        function set2() {
            lbl = $get("ContentPlaceHolder1_lblTitle");
            if (lbl != null) {
                s = lbl.innerHTML;
                i = s.indexOf("Live");
                if (i > 0) {
                    setInterval("OnTimerN()", 1000);
                } else {
                    lbl = $get("ContentPlaceHolder1_lblTMP");
                    if (lbl != null)
                        lbl.innerHTML = "";
                }
            }
        }

        set2();

        
    </script>
</asp:Content>
