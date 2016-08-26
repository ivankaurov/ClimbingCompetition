<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewUser.aspx.cs"
Inherits="WebClimbing.Apps.UserAccountManagement._PNewUser"
 MasterPageFile="~/WebAppl.Master" Theme="appSkin" EnableEventValidation="false"%>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="NUContent" runat="server">
    <script type="text/javascript">
        setInterval("OnTimer()", 1000);
    </script>
    <asp:HyperLink ID="hlNewUser" NavigateUrl="~/Apps/UserAccountManagement/UserManagement.aspx"
                    Text="Список пользователей" runat="server" />
     <br />
    <div>
          <table border="0" style="font-size: 100%; font-family: Verdana">
          <tr>
              <td align="center" colspan="2" style="font-weight: bold; color: white; background-color: #5d7b9d">
                  Новый пользователь</td></tr><tr>
              <td align="right">
                  <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Имя пользователя (login, 3 символа):</asp:Label></td><td>
                  <asp:TextBox ID="UserName" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                  ErrorMessage="Требуется имя пользователя." ToolTip="Требуется имя пользователя." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator></td></tr><tr>
              <td align="right">
                  <asp:Label ID="lblReg" runat="server" AssociatedControlID="UserName">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Имя (отображается):</asp:Label></td><td>
                  <asp:TextBox ID="Region" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="RegionRequired" runat="server" ControlToValidate="Region"
                  ErrorMessage="Требуется имя для отображения." ToolTip="Требуется имя для отображения." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator></td></tr>
                  <tr>
                  <td align="right">Регион</td>
                  <td><asp:DropDownList ID="cbRegSelect" runat="server" /></td>
                  </tr>
                  <tr>
              <td align="right">
                  <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Пароль:</asp:Label></td><td>
                  <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox><asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                      ErrorMessage="Требуется пароль." ToolTip="Требуется пароль." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator></td></tr><tr>
              <td align="right">
              <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Подтверждение пароля:</asp:Label></td><td>
                  <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox><asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
                      ErrorMessage="Требуется подтверждение пароля." ToolTip="Требуется подтверждение пароля."
                      ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator></td></tr><tr>
              <td align="right">
                  <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; E-mail:</asp:Label></td><td>
                  <asp:TextBox ID="Email" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                      ErrorMessage="Требуется e-mail." ToolTip="Требуется e-mail." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator></td></tr>
                      <tr>
                      <td align="right"><asp:Label ID="lblSetAdminRoles" runat="server" Text="Настройки ролей администратора" /></td>
                      <td align="left">
                        <asp:RadioButtonList ID="rbListAdminProperties" runat="server">
                        </asp:RadioButtonList>
                      </td>
                      </tr>
                      <tr>
                      <td align="right"><asp:Label ID="lblSetCompRoles" runat="server" Text="Настройки ролей соревнований" /></td>
                      <td align="left">
                        <asp:RadioButtonList ID="rbListCompProperties" runat="server">
                        </asp:RadioButtonList>
                      </td>
                      </tr>
                      <tr>
              <td align="center" colspan="2">
               <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                  ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="Пароли не совпадают."
                 ValidationGroup="CreateUserWizard1"></asp:CompareValidator></td></tr><tr>
              <td align="center" colspan="2" style="color: red">
              <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal></td></tr>
              <tr><td align="left">
           <asp:Button ID="btnAddUser" runat="server" Text="Добавить пользователя" 
              onclick="btnAddUser_Click"/>
              </td>
              <td align="right">
              <asp:Button ID="btnGeneratePassword" runat="server" Text="Создать пароль" 
                      onclick="btnGeneratePassword_Click" />
              </td>
              </tr>
              </table>
    </div>
</asp:Content>
