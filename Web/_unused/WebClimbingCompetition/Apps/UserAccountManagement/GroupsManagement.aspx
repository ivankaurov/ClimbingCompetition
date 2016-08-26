<%@ Page Title="" Language="C#" MasterPageFile="~/WebAppl.Master" AutoEventWireup="true" CodeBehind="GroupsManagement.aspx.cs" Inherits="WebClimbing.Apps.UserAccountManagement.GroupsManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="panelSelect" runat="server" Enabled = "true" Visible="true">
    <asp:GridView ID="gvGroups" runat="server" DataKeyNames="Iid"
                  AutoGenerateColumns="false" AllowSorting="true"
                  AutoGenerateSelectButton="true" 
        onselectedindexchanged="gvGroups_SelectedIndexChanged">
        <AlternatingRowStyle BackColor="LightGray" />
        <HeaderStyle BackColor="LightBlue" Font-Bold="true" HorizontalAlign="Center" />
        <Columns>
            <asp:BoundField DataField="Iid" HeaderText="Iid" />
            <asp:BoundField DataField="Name" HeaderText="Название" />
            <asp:BoundField DataField="Ages" HeaderText="Возраста" />
            <asp:BoundField DataField="Gender" HeaderText="Пол" />
            <asp:CheckBoxField DataField="UsingNow" HeaderText="Используется в текущем соревновании" />
        </Columns>
     </asp:GridView>
     <br />
     <asp:Button ID="btnNewGroup" runat="server" Text="Новая группа" 
        onclick="btnNewGroup_Click" />
     </asp:Panel>
     <asp:Label ID="lblError" runat="server" Text="" ForeColor="Red" />
     <asp:Panel ID="panelEdit" runat="server" Visible="false">
        <table width="100%">
            <tr>
                <td align="right">iid:</td>
                <td align="left"><asp:Label ID="lblIid" runat="server" /></td>
            </tr>
            <tr>
                <td align="right">Наименование:</td>
                <td align="left"><asp:TextBox ID="tbName" runat="server" /></td>
            </tr>
            <tr>
                <td align="right">Возраст от (лет на 31 декабря):</td>
                <td align="left"><asp:TextBox ID="tbYoung" runat="server" /></td>
            </tr>
            <tr>
                <td align="right">Возраст до:</td>
                <td align="left"><asp:TextBox ID="tbOld" runat="server" /></td>
            </tr>
            <tr>
                <td align="right">Пол:</td>
                <td align="left"><asp:DropDownList ID="cbGender" runat="server">
                                    <asp:ListItem Text="М" Value="M" Selected="True" />
                                    <asp:ListItem Text="Ж" Value="F" />
                                 </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right"><asp:Label ID="lblCompUsage" runat="server" /></td>
                <td align="left"><asp:CheckBox ID="cbUsedNow" runat="server" Text="" /></td>
            </tr>
            <tr>
                <td align="right"><asp:Label ID="lblGroupQF" runat="server" Text="Ограничеие по разрядам (на тек. соревнование):"/></td>
                <td align="left"><asp:DropDownList ID="qfList" runat="server" /></td>
            </tr>
            <tr>
                <td align="left"><asp:Button ID="btnSubmit" runat="server" Text="Подтвердить" 
                        onclick="btnSubmit_Click" /></td>
                <td align="right"><asp:Button ID="btnDelete" runat="server" Text="Удалить" 
                        onclick="btnDelete_Click" /></td>
            </tr>
            <tr>
                <td colspan="2" align="right"><asp:Button ID="btnCancel" runat="server" Text="Отмена" 
                        onclick="btnCanel_Click" /></td>
            </tr>
        </table>
     </asp:Panel>
</asp:Content>
