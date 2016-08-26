<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Applications.aspx.cs" Inherits="WebClimbing.Apps._PApplications"
 MasterPageFile="~/WebAppl.Master" Theme="appSkin" EnableEventValidation="false"%>
<%@ Register src="../Controls/ClimberControl.ascx" tagname="ClimberControl" tagprefix="uc1" %>
<%@ Register src="../Controls/ClimbersListControl.ascx" tagname="ClimbersListControl" tagprefix="uc3" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="AppsContent" runat="server">
    <script type="text/javascript">
        function ValidateLabel(tbNameID, lblErrID) {
            var tbName = $get(tbNameID);
            var lblErr = $get(lblErrID);
            if (lblErr == null || tbName == null)
                return 0;
            if (tbName.value == '')
                return -1;
            if(lblErr.innerText != 'OK' && lblErr.innerText != '')
                return 1;
            return 0;
        }
    </script>
    <asp:Label ID="Label1" runat="server" Text="Новая заявка" Font-Bold="true" Font-Size="Large"></asp:Label>
    <br />
    <asp:Label ID="lblMessageTop" runat="server" ForeColor="Red" Text="" />
    <br />
    <asp:Panel ID="panelClimbers" runat="server">
    <div>
         <asp:Label ID="lblNotif" runat="server" Font-Bold="true"> 
            Пожалуйста, вводите имена и фамилии участников так, как они написаны в РЕЙТИНГЕ
            <br />
            Иначе мы не можем гарантировать правильность указания рейтинга для введённых участников
          </asp:Label>
         <br />
         
         <asp:Label ID="lblNotifAdd" runat="server" Font-Bold="true" Font-Size="Large" ForeColor="Red">
         </asp:Label>
         <asp:Label ID="lblNotifDeadline" runat="server" Font-Bold="true" Font-Size="Large">
         </asp:Label>
         <br />
         <asp:DropDownList ID="cbTeam" runat="server" />
         <br />
        <div>
        <uc1:ClimberControl ID="ClimberControl1" runat="server"  />
        </div>
        <div>
        <uc1:ClimberControl ID="ClimberControl2" runat="server"  />
        </div>
        <br />
        <div>
        <uc1:ClimberControl ID="ClimberControl3" runat="server"  />
        </div>
        <br />
        <div>
        <uc1:ClimberControl ID="ClimberControl4" runat="server"  />
        </div>
    </div>
    <br />
    <asp:Button ID="btnSubmitClimbers" runat="server" Text="Подать заявку" 
            onclick="btnSubmitClimbers_Click" />
    </asp:Panel>
    <asp:Panel ID="panelConfirm" runat="server">
        <asp:Label ID="lblConfMessage" runat="server" Text="" />
        <table width="100%">
            <tr>
            <td><asp:Button ID="btnConfirm" runat="server" Text="Подтвердить" 
                    onclick="btnConfirm_Click" /></td>
            <td></td>
            <td><asp:Button ID="btnCancel" runat="server" Text="Назад" 
                    onclick="btnCancel_Click" /></td>
            </tr>
        </table>
    </asp:Panel>
    
    
    <br />
    <asp:Panel ID="AppsData" runat="server">
        <uc3:ClimbersListControl ID="uncClm" runat="server" OnbtnAddAppClick="btnAddApp_Click" OnConfirmationSent="uncClm_ConfSent" />
        
    </asp:Panel>
    
</asp:Content>