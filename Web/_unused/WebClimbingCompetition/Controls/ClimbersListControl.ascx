<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClimbersListControl.ascx.cs" Inherits="WebClimbing.Controls.ClimbersListControl" %>

<asp:Label ID="lblMessageTop" runat="server" Text="" Font-Bold = "true" ForeColor="Red" Font-Size="Larger"/>
<br />
<asp:Label ID="lblHeaderSENT" runat="server" Text="Не отправленные на подтверждение старшему тренеру заяки:"
     ForeColor="Red" />
     <asp:GridView ID="gvUnsent" AutoGenerateColumns="false" runat="server" AlternatingRowStyle-BackColor="LightBlue"
      HeaderStyle-BackColor="LightGray" DataKeyNames="Iid" 
    onrowdeleting="gvUnsent_RowDeleting">
        <Columns>
            <asp:BoundField DataField="Iid" HeaderText="Iid" Visible="false" />
            <asp:BoundField DataField="Name" HeaderText="Фамилия, Имя" />
            <asp:BoundField DataField="Age" HeaderText="Г.р." />
            <asp:BoundField DataField="Qf" HeaderText="Разр." />
            <asp:BoundField DataField="Team" HeaderText="Команда" />
            <asp:BoundField DataField="Grp" HeaderText="Группа" />
            <asp:BoundField DataField="Lead" HeaderText="Трудность" />
            <asp:BoundField DataField="Speed" HeaderText="Скорость" />
            <asp:BoundField DataField="Boulder" HeaderText="Боулдеринг" />
            <asp:BoundField DataField="State" HeaderText="Состояние" />
            <asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Отмена" />
        </Columns>
    </asp:GridView>
      <table width="100%">
    <tr>
    <td><asp:Button ID="btnConfirmEmail" runat="server" 
            Text="Подтвердить все заявки через e-mail" onclick="btnConfirmEmail_Click" /></td>
    <td></td>
    <td><asp:Button ID="btnAddApp" runat="server" Text="Ещё заявки" 
            onclick="btnAddApp_Click" /></td>
    <td></td>
    <td><asp:Button ID="btnCancelAll" runat="server" 
            Text="Отменить все неотправленные заявки" 
            onclick="btnCancelAll_Click" /></td>
    </tr>
    </table>
      <br />
    <asp:Label ID="lblHeaderUnconfirmed" runat="server" Text="Не подтверждённые старшим тренером заявки:"
     ForeColor="Red" />
     <asp:GridView ID="gvUnconfirmed" AutoGenerateColumns="false" runat="server" AlternatingRowStyle-BackColor="LightBlue"
      HeaderStyle-BackColor="LightGray" DataKeyNames="Iid" 
    onrowdeleting="gvUnconfirmed_RowDeleting">
        <Columns>
            <asp:BoundField DataField="Iid" HeaderText="Iid" Visible="false" />
            <asp:BoundField DataField="Name" HeaderText="Фамилия, Имя" />
            <asp:BoundField DataField="Age" HeaderText="Г.р." />
            <asp:BoundField DataField="Qf" HeaderText="Разр." />
            <asp:BoundField DataField="Team" HeaderText="Команда" />
            <asp:BoundField DataField="Grp" HeaderText="Группа" />
            <asp:BoundField DataField="Lead" HeaderText="Трудность" />
            <asp:BoundField DataField="Speed" HeaderText="Скорость" />
            <asp:BoundField DataField="Boulder" HeaderText="Боулдеринг" />
            <asp:BoundField DataField="State" HeaderText="Состояние" />
            <asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Отмена" />
        </Columns>
    </asp:GridView>
      <br />
      <asp:Button ID="btnConfirmAgain" runat="server" Text="Отправить повторный запрос"
            onclick="btnConfirmAgain_Click" />
