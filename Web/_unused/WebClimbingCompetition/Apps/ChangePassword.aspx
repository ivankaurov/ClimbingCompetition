<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs"
Inherits="WebClimbing.Apps._PChangePassword"
 MasterPageFile="~/WebAppl.Master" Theme="appSkin" EnableEventValidation="false"%>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="AppsContent" runat="server">
    <script type="text/javascript">
        setInterval("OnTimer()", 1000);
    </script>
    <table>
    <tr><td align="right">Старый пароль:</td>
    <td><asp:TextBox ID="oldPwd" runat="server" TextMode="Password" />
    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="oldPwd"
                      ErrorMessage="Требуется старый пароль." ToolTip="Требуется старый пароль." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
    </td>
    </tr>
    <tr><td align="right">Новый пароль:</td>
    <td><asp:TextBox ID="newPwd" runat="server" TextMode="Password" />
    <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="newPwd"
                      ErrorMessage="Требуется пароль." ToolTip="Требуется пароль." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
    </td>
    </tr>
    <tr><td align="right">Подтверждение пароля:</td>
    <td><asp:TextBox ID="confPwd" runat="server" TextMode="Password" />
    <asp:RequiredFieldValidator ID="ConfPasswordRequired" runat="server" ControlToValidate="confPwd"
                      ErrorMessage="Требуется подтверждение пароля." ToolTip="Требуется подтверждение пароля." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
    <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="newPwd"
                  ControlToValidate="confPwd" Display="Dynamic" ErrorMessage="Пароли не совпадают."
                 ValidationGroup="CreateUserWizard1"></asp:CompareValidator>
    </td>
    </tr>
    </table>
    <br />
    <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
    <br />
    <asp:Button ID="btnChange" runat="server" Text="Сменить пароль" 
        onclick="btnChange_Click" />
</asp:Content>