<%@ Page Title="" Language="C#" MasterPageFile="~/WebAppl.Master" AutoEventWireup="true" CodeBehind="TeamsManagement.aspx.cs" Inherits="WebClimbing.Apps.UserAccountManagement.TeamsManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <asp:Panel ID="panelTeams" runat="server" Visible="true" Enabled="true">
    <asp:Panel ID="panelSelect" runat="server" Enabled = "true" Visible="true">
    <asp:GridView ID="gvTeams" runat="server" DataKeyNames="Iid"
                  AutoGenerateColumns="false" AllowSorting="true"
                  AutoGenerateSelectButton="true" 
        onselectedindexchanged="gvTeams_SelectedIndexChanged">
        <AlternatingRowStyle BackColor="LightGray" />
        <HeaderStyle BackColor="LightBlue" Font-Bold="true" HorizontalAlign="Center" />
        <Columns>
            <asp:BoundField DataField="Iid" HeaderText="Iid" />
            <asp:BoundField DataField="Region" HeaderText="Команда" />
            <asp:BoundField DataField="GroupName" HeaderText="Группа команд" />
            <asp:CheckBoxField DataField="UsingNow" HeaderText="Используется в текущем соревновании" />
        </Columns>
     </asp:GridView>
     <br />
     <asp:Button ID="btnNewTeam" runat="server" Text="Новая команда" 
        onclick="btnNewTeam_Click" />
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
                <td align="right">Группа команд:</td>
                <td align="left"><asp:DropDownList ID="cbGroupToSet" runat="server" /></td>
            </tr>
            <tr>
                <td align="right"><asp:Label ID="lblCompUsage" runat="server" /></td>
                <td align="left"><asp:CheckBox ID="cbUsedNow" runat="server" Text="" /></td>
            </tr>
            <tr>
                <td align="right"><asp:Label ID="lblCompRanking" runat="server" Text="Позиция в рейтинге (на тек. соревнование):"/></td>
                <td align="left"><asp:TextBox ID="tbRanking" runat="server" /></td>
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
  </asp:Panel>
  <br />
  <asp:Panel ID="panelTeamGroups" runat="server" Visible="true" Enabled="true">
    <table width="100%">
    <tr><td align="center"><asp:Label ID="lblGrTtl" runat="server" Font-Size="Large" Font-Bold="true" ForeColor="Blue"
                                 Text="Группы команд"/></td></tr>
    </table>
    <asp:Panel ID="panelGroupsSelect" runat="server" Visible="true" Enabled="true">
     <asp:GridView ID="gvTeamGroups" runat="server" DataKeyNames="Iid"
                  AutoGenerateColumns="false" AllowSorting="true" 
            AutoGenerateSelectButton="true" 
            onselectedindexchanged="gvTeamGroups_SelectedIndexChanged" 
            onrowcommand="gvTeamGroups_RowCommand">
        <AlternatingRowStyle BackColor="LightGray" />
        <HeaderStyle BackColor="LightBlue" Font-Bold="true" HorizontalAlign="Center" />
        <Columns>
            <asp:BoundField DataField="Iid" HeaderText="Iid" Visible="false"/>
            <asp:BoundField DataField="GroupName" HeaderText="Группа команд" />
        </Columns>
     </asp:GridView>
     <br />
     <asp:Button ID="btnAddGroup" runat="server" Text="Новая группа команд" 
            onclick="btnAddGroup_Click" />
    </asp:Panel>
    <asp:Panel ID="panelGroupsEdit" runat="server" Visible="false">
      <asp:Label ID="lblTGEditError" runat="server" ForeColor="Red" Visible="true" Text="" />
      <table width="100%">
        <tr>
          <td align="right">Iid:</td>
          <td colspan="2" align="left"><asp:Label ID="lblGroupIid" runat="server" /></td>
        </tr>
        <tr>
          <td align="right">Название группы команд:</td>
          <td align="left" colspan="2"><asp:TextBox ID="tbGroupName" runat="server" /></td>
        </tr>
        <tr>
          <td align="right">Используется в текущем соревновании:</td>
          <td align="left" colspan="2"><asp:CheckBox ID="cbUseCompGroup" runat="server" /></td>
        </tr>
        <tr>
          <td align="right">Квота для группы команд на данное соревнование:</td>
          <td align="left" colspan="2"><asp:TextBox ID="tbGroupQuota" runat="server" /></td>
        </tr>
        <tr>
          <td align="left">
            <asp:Button ID="btnGroupSubmit" runat="server" Text="Подтвердить" 
                  onclick="btnGroupSubmit_Click" />
          </td>
          <td align="center">
            <asp:Button ID="btnGroupCancel" runat="server" Text="Отмена" 
                  onclick="btnGroupCancel_Click" />
          </td>
          <td align="right">
            <asp:Button ID="btnGroupDel" runat="server" Text="Удалить" 
                  onclick="btnGroupDel_Click" />
          </td>
        </tr>
      </table>
    </asp:Panel>
  </asp:Panel>
</asp:Content>
