<%@ Page Title="" Language="C#" MasterPageFile="~/WebAppl.Master" AutoEventWireup="true" CodeBehind="AppConfirm.aspx.cs" Inherits="WebClimbing.Apps._PAppConfirm" %>
<%@ Register src="../Controls/ClimberControl.ascx" tagname="ClimberControl" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblTitle" runat="server" Text="Подтверждение заявок" Font-Bold="true" Font-Size="Larger" ForeColor="Chocolate"/>
    <br />
    <asp:Label ID="LblErrorMessage" runat="server" Text="" ForeColor="Red"  Font-Bold="true" Font-Size="Larger"/>
    <br />
    <asp:Panel ID="panelConfApps" runat="server" Visible="true">
    <asp:GridView ID="gvApps" AutoGenerateColumns="false" runat="server"
     AlternatingRowStyle-BackColor="LightBlue" HeaderStyle-BackColor="LightGray"
      HeaderStyle-Font-Bold="true" AllowSorting="true" DataKeyNames="iid"
        onrowcommand="gvApps_RowCommand"> 
        <Columns>
            <asp:BoundField DataField="iid" HeaderText="iid" Visible="false" />
            <asp:BoundField DataField="secretary_id" HeaderText="№" />
            <asp:BoundField DataField="name" HeaderText="Фамилия, Имя" />
            <asp:BoundField DataField="team" HeaderText="Команда" />
            <asp:BoundField DataField="age" HeaderText="Г.р." />
            <asp:BoundField DataField="qf" HeaderText="Разряд" />
            <asp:BoundField DataField="grp" HeaderText="Группа" />
            <asp:BoundField DataField="lead" HeaderText="Трудность" />
            <asp:BoundField DataField="speed" HeaderText="Скорость" />
            <asp:BoundField DataField="boulder" HeaderText="Боулдеринг" />
            <asp:BoundField DataField="todo" HeaderText="Действие" />
            <asp:BoundField DataField="op_state" HeaderText="Состояние" />
            <asp:ButtonField CommandName="ConfirmLine" Text="Подтвердить" HeaderText="" ButtonType="Button" />
            <asp:ButtonField CommandName="EditLine" Text="Правка" HeaderText="" ButtonType="Button" />
            <asp:ButtonField CommandName="DeleteLine" Text="Отмена" HeaderText="" ButtonType="Button" />
        </Columns>
        </asp:GridView>
    </asp:Panel>
    <asp:Panel ID="panelEditClimber" runat="server" Visible="false">
        <asp:HiddenField ID="hfIid" runat="server" />
        <uc1:ClimberControl ID="clmEdit" runat="server" />
        <table width="100%">
        <tr>
        <td align="left">
        <asp:Button ID="btnConfirm" runat="server" Text="Подтвердить" 
                onclick="btnConfirm_Click" />
        </td>
        <td></td>
        <td align="right">
        <asp:Button ID="btnCancel" runat="server" Text="Отмена" onclick="btnCancel_Click" />
        </td>
        </tr>
        </table>
        
    </asp:Panel>
</asp:Content>
