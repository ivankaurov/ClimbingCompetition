<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs"
Inherits="WebClimbing.Apps.UserAccountManagement._PUserManagement"
 MasterPageFile="~/WebAppl.Master" Theme="appSkin" EnableEventValidation="false"%>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="UMContent" runat="server">

    <!--
    <script type="text/javascript">
        setInterval("OnTimer()", 1000);
    </script>
    -->
    <asp:Label ID="lblT" runat="server" Text="Управление пользоваетелями" />
    <br />
    <asp:HyperLink ID="hlNewUser" NavigateUrl="~/Apps/UserAccountManagement/NewUser.aspx"
                    Text="Новый пользователь" runat="server" />
    <br />
    <div>
    <asp:GridView ID="gvUsers" runat="server" DataKeyNames="Iid"
                  AutoGenerateColumns="false" AllowSorting="true"
                  AutoGenerateSelectButton="true" 
        onselectedindexchanged="gvUsers_SelectedIndexChanged">
        <AlternatingRowStyle BackColor="LightGray" />
        <HeaderStyle BackColor="LightBlue" Font-Bold="true" HorizontalAlign="Center" />
        <Columns>
            <asp:BoundField DataField="Iid" HeaderText="Iid" />
            <asp:BoundField DataField="Region" HeaderText="Команда" />
            <asp:BoundField DataField="Email" HeaderText="E-mail" />
            <asp:BoundField DataField="LastLogin" HeaderText="Последний вход" />
            <asp:CheckBoxField DataField="Admin" HeaderText="Администратор БД" />
            <asp:CheckBoxField DataField="AdminComp" HeaderText="Администратор текущих соревнований" />
            <asp:CheckBoxField DataField="UserComp" HeaderText="Пользователь текущих соревнований" />
        </Columns>
     </asp:GridView>
     </div>
     <br />
     <asp:Panel ID="panelSendMessage" runat="server" Visible="true">
        <table>
            <tr>
                <td colspan="2" align="center"><asp:Label ID="lblMessageMail" runat="server" Text="Отправка сообщений" /></td>
            </tr>
            <tr>
                <td align="right">Тема:</td>
                <td align="left"><asp:TextBox ID="tbSubj" runat="server" Text="" /></td>
            </tr>
            <tr>
                <td align="right">Сообщение:</td>
                <td align="left"><asp:TextBox ID="tbMessage" runat="server" Rows="10" Columns="40" 
                        Text="" TextMode="MultiLine" /></td>
            </tr>
            <tr>
                <td align="center"><asp:Button ID="btnSend" runat="server" Text="Отправить" 
                        onclick="btnSend_Click" /></td>
                <td align="center"><asp:Button ID="btnClear" runat="server" Text="Очистить" 
                        onclick="btnClear_Click" /></td>
            </tr>
        </table>
        
     </asp:Panel>
     <div>
     <asp:Label ID="lbluTitle" runat="server" Text="Выбранный пользователь" />
     <table width="100%" border="0">
        <tr>
            <td align="right">Iid:</td>
            <td>
                <asp:Label ID="lblIid" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr> <td align="right">Регион:</td>
            <td>
                <asp:TextBox ID="tbName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr> <td align="right">E-mail:</td>
            <td>
                <asp:TextBox ID="tbEmail" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr> <td align="right">Команда:</td>
            <td>
                <asp:DropDownList ID="cbTeams" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right"><asp:Label ID="lblSetAdminRoles" runat="server" Text="Настройки ролей администртора" /></td>
            <td align="left">
                <asp:RadioButtonList ID="rbListAdminProperties" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right"><asp:Label ID="lblSetCompRoles" runat="server" Text="Настройки ролей соревнований" /></td>
            <td align="left">
                <asp:RadioButtonList ID="rbListCompProperties" runat="server" />
            </td>
        </tr>
     </table>
     <table width="100%">
     <tr><td>
     <asp:Button ID="btnChange" runat="server" Text="Изменить данные" 
             onclick="btnChange_Click" />
     </td> <td></td> <td>
     <asp:Button ID="btnDel" runat="server" Text="Удалить пользователя" 
             onclick="btnDel_Click" />
     </td></tr>
     </table>
     <asp:Label ID="lblMessage" runat="server" Text="" />
     </div>
</asp:Content>