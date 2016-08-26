<%@ Page Title="" Language="C#" MasterPageFile="~/WebAppl.Master" AutoEventWireup="true" CodeBehind="CompetitionManagement.aspx.cs" Inherits="WebClimbing.Apps.UserAccountManagement.DBManagement.CompetitionManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td>
            <asp:Panel ID="panelComps" runat="server" Visible="true">
                <asp:GridView ID="gvComps" runat="server" DataKeyNames="iid"
                  AutoGenerateColumns="false" AllowSorting="true"
                  AutoGenerateSelectButton="true" 
                  onselectedindexchanged="gvComps_SelectedIndexChanged">
                    <AlternatingRowStyle BackColor="LightGray" />
                    <HeaderStyle BackColor="LightBlue" Font-Bold="true" HorizontalAlign="Center" />
                    <Columns>
                        <asp:BoundField DataField="iid" HeaderText="iid" />
                        <asp:BoundField DataField="short_name" HeaderText="Название" />
                        <asp:BoundField DataField="place" HeaderText="Место проведения" />
                        <asp:BoundField DataField="date" HeaderText="Сроки проведения" />
                    </Columns>
                </asp:GridView>
                <br />
                <br />
                <asp:Button ID="btnNewComp" runat="server" Text="Новые соревнования" 
                    onclick="btnNewComp_Click" />
            </asp:Panel>
            </td>
        </tr>
        <tr><td><br /></td></tr>
        <tr><td>
            <asp:Panel ID="panelSelectedComp" runat="server" Visible="false">
                <table width="100%">
                    <tr>
                    <td align="center" colspan="2">
                    <asp:Label ID="lblErrMsg" runat="server" ForeColor="Red" Text = ""/>
                    </td>
                    </tr>
                    <tr>
                    <td align="right">Iid:</td>
                    <td align="left"><asp:Label ID="lblCompIid" runat="server" /></td>
                    </tr>
                    <tr>
                    <td align="right">Название: </td>
                    <td align="left"><asp:TextBox ID="tbCompTitle" runat="server" /></td>
                    </tr>
                    <tr>
                    <td align="right">Краткое наименование: </td>
                    <td align="left"><asp:TextBox ID="tbCompShort" runat="server" /></td>
                    </tr>
                    <tr>
                    <td align="right">Место проведения: </td>
                    <td align="left"><asp:TextBox ID="tbCompPlace" runat="server" /></td>
                    </tr>
                    <tr>
                    <td align="right">Сроки с: </td>
                    <td align="left"><asp:Calendar ID="clndFrom" runat="server" SelectedDayStyle-BackColor="LightGray" /></td>
                    </tr>
                    <tr>
                    <td align="right">Сроки по: </td>
                    <td align="left"><asp:Calendar ID="clndTo" runat="server" SelectedDayStyle-BackColor="LightGray" /></td>
                    </tr>
                    <tr>
                    <td align="right">Заявки до: </td>
                    <td align="left"><asp:Calendar ID="clndDeadLine" runat="server" SelectedDayStyle-BackColor="LightGray" /></td>
                    </tr>
                    <tr>
                    <td align="right">Коррекция заявок до: </td>
                    <td align="left"><asp:Calendar ID="clndCorrectionDeadLine" runat="server" SelectedDayStyle-BackColor="LightGray" /></td>
                    </tr>
                    <tr>
                    <td align="right">Разрешить дозаявки до окончания корректировок: </td>
                    <td align="left"><asp:CheckBox ID="cbAllowAfterDeadline" runat="server" /></td>
                    </tr>
                    <tr>
                    <td align="right">Виды: </td>
                    <td align="left">
                        <asp:CheckBoxList ID="cbListStyles" runat="server">
                            <asp:ListItem Text="Трудность" Value="L" />
                            <asp:ListItem Text="Скорость" Value="S" />
                            <asp:ListItem Text="Боулдеринг" Value="B" />
                        </asp:CheckBoxList>
                    </td>
                    </tr>
                    <tr>
                    <td align="left"><asp:Button ID="btnSubmit" runat="server" Text="Подтвердить" 
                            onclick="btnSubmit_Click" /></td>
                    <td align="right"><asp:Button ID="btnCancel" runat="server" Text="Отмена" 
                            onclick="btnCancel_Click" /><br /><br />
                    <asp:Button ID="btnDel" runat="server" Text="Удалить соревнования" 
                            onclick="btnDel_Click" />
                    </td>
                    </tr>
                </table>
            </asp:Panel>
        </td></tr>
    </table>
</asp:Content>
