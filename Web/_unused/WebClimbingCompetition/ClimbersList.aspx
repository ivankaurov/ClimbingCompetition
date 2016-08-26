<%@ Page Title="" Language="C#" MasterPageFile="~/WebAppl.Master" AutoEventWireup="true" CodeBehind="ClimbersList.aspx.cs" Inherits="WebClimbing._PClimbersList" %>

<%@ Register src="Controls/ClimberControl.ascx" tagname="ClimberControl" tagprefix="uc1" %>

<%@ Register src="Controls/ClimbersListControl.ascx" tagname="ClimbersListControl" tagprefix="uc2" %>

<asp:Content ID="clListId" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<asp:Label ID="lbl1" runat="server"></asp:Label>
                
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
            <ProgressTemplate>
                <img src="files/partnerscfr/ajax-loader.gif" alt="Loading..."/>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:Label ID="lblAdmMessage" runat="server" Text="" ForeColor="Red" />
        <asp:Panel ID="panelLoginButtons" runat="server">
            <asp:Button ID="btnSendClimbersList" runat="server"
                              Text="Отправить на почту список участников" 
                                onclick="btnSendClimbersList_Click" />
            <asp:Panel ID="panelLoginButtonsAdmin" runat="server">
                 <asp:Button ID="btnRecalculateQueue" runat="server"
                             Text="Пересчитать очередь" OnClick="btnRecalculateQueue_Click"/>
                 <asp:Button ID="btnRecalculateRanking" runat="server"
                             Text="Пересчитать рейтинг" OnClick="btnRecalculateRanking_Click"/>
            </asp:Panel>
        </asp:Panel>
        
        <asp:Panel ID="panelEdit" runat="server" Visible="false">
                <uc1:ClimberControl ID="climberEditControl" runat="server" Visible="true" />
                <asp:HiddenField ID="hfEditingClimber" runat="server" />
                <asp:Panel ID="panelAdminEdit" runat="server"> 
                    <br />
                    <asp:CheckBox ID="cbQueue" runat="server" Text="В очереди на заявку" Checked="false" />
                    <br />
                    <asp:CheckBox ID="cbChangeble" runat="server" Text="Участника можно заменить" Checked="false" />
                    <br />
                    <table width="100%">
                        <tr>
                            <td align="right">Комментарий к заявке:</td>
                            <td align="left"><asp:TextBox ID="tbComment" runat="server" Text="" /></td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Label ID="lblRTTT" runat="server" Font-Bold="true" Text="Рейтинг (место)" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">Трудность:</td>
                            <td align="left"><asp:TextBox ID="tbRnkLead" runat="server" Text="" /></td>
                        </tr>
                        <tr>
                            <td align="right">Скорость:</td>
                            <td align="left"><asp:TextBox ID="tbRnkSpeed" runat="server" Text="" /></td>
                        </tr>
                        <tr>
                            <td align="right">Боулдеринг:</td>
                            <td align="left"><asp:TextBox ID="tbRnkBoulder" runat="server" Text="" /></td>
                        </tr>
                    </table>
                   
                    
                </asp:Panel>
                <table width="100%">
                    <tr>
                        <td align="left">
                            <asp:Button ID="btnConfirmEditingClimber" runat="server" Text="Подтвердить" 
                                onclick="btnConfirmEditingClimber_Click" />
                        </td><td></td>
                        <td align="center">
                            <asp:Button ID="btnDelEditingClimber" runat="server" Text="Удалить" 
                                onclick="btnDelEditingClimber_Click" />
                        </td><td></td>
                        <td align="right">
                            <asp:Button ID="btnCamcelEditingClimber" runat="server" Text="Отмена" 
                                onclick="btnCamcelEditingClimber_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <uc2:ClimbersListControl ID="uncList" runat="server" Visible="false" btnAddAppEnabled="false" OnrowDeleted="uncList_rowDeleted" />

        <asp:Panel ID="panelView" runat="server" Visible="true">
        <asp:Label ID="lblCLMlist" runat="server" Font-Bold="true" Font-Size="Large" Text="Список участников:" />
        <br />
        <asp:DropDownList ID="teamList" runat="server" AutoPostBack="True"
            onselectedindexchanged="teamList_SelectedIndexChanged"></asp:DropDownList>
            <asp:DropDownList ID="groupsList" runat="server" AutoPostBack="True"
            onselectedindexchanged="teamList_SelectedIndexChanged"></asp:DropDownList>
            <br />
            
            <asp:GridView ID="clList" runat="server" AutoGenerateColumns="false" 
                AllowSorting="true" ForeColor="Black" OnRowCommand="clList_RowCommand" 
                DataKeyNames="iid" AlternatingRowStyle-BackColor="LightGray" HeaderStyle-BackColor="LightGray">
                <Columns>
                    <asp:BoundField DataField="iid" Visible="false" HeaderText="iid" />
                    <asp:BoundField DataField="num" HeaderText="№ п/п" 
                        ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="secretary_id" HeaderText="№" 
                        ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:HyperLinkField DataTextField="name" HeaderText="Фамилия, Имя" 
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Font-Underline="false"
                    DataNavigateUrlFields="iid" 
                        DataNavigateUrlFormatString="~/climber.aspx?iid={0}" 
                        ItemStyle-ForeColor="Black" ControlStyle-ForeColor="Blue">
                    <ItemStyle Font-Underline="False" ForeColor="Black" HorizontalAlign="Left" />
                    </asp:HyperLinkField>
                    <asp:BoundField DataField="age" HeaderText="Г.р." 
                        ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="qf" HeaderText="Разряд" 
                        ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="team" HeaderText="Команда" 
                        ItemStyle-HorizontalAlign="Left">
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="grp" HeaderText="Группа" 
                        ItemStyle-HorizontalAlign="Left">
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="lead" HeaderText="Тр." ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="speed" HeaderText="Ск." ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="boulder" HeaderText="Боулд." ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="appl_type" HeaderText="Примечание" ItemStyle-HorizontalAlign="Left" />
                    <asp:CheckBoxField DataField="is_changeble" HeaderText="Можно заменить" ItemStyle-HorizontalAlign="Center" />
                    <asp:ButtonField CommandName="EditLine" Text="Правка" ButtonType="Button"/>
                    <asp:ButtonField CommandName="MoveDown" Text="В очередь" ButtonType="Button" />
                </Columns>
            </asp:GridView>
            <br />
            <asp:Panel ID="panelQueue" runat="server" Visible="false">
            <asp:Label ID="LblQueuTxt" runat="server" Text="Очередь:" Font-Bold="true" Font-Size="Larger" />
            <asp:GridView ID="gvQueue" runat="server" AutoGenerateColumns="false" 
                AllowSorting="true" ForeColor="Black" OnRowCommand="clList_RowCommand" 
                DataKeyNames="iid">
                <Columns>
                    <asp:BoundField DataField="iid" Visible="false" HeaderText="iid" />
                    <asp:BoundField DataField="num" HeaderText="№ п/п" 
                        ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="secretary_id" HeaderText="№" 
                        ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:HyperLinkField DataTextField="name" HeaderText="Фамилия, Имя" 
                        ItemStyle-HorizontalAlign="Left" ItemStyle-Font-Underline="false"
                    DataNavigateUrlFields="iid" 
                        DataNavigateUrlFormatString="~/climber.aspx?iid={0}" 
                        ItemStyle-ForeColor="Black" ControlStyle-ForeColor="Blue">
                    <ItemStyle Font-Underline="False" ForeColor="Black" HorizontalAlign="Left" />
                    </asp:HyperLinkField>
                    <asp:BoundField DataField="age" HeaderText="Г.р." 
                        ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="qf" HeaderText="Разряд" 
                        ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="team" HeaderText="Команда" 
                        ItemStyle-HorizontalAlign="Left">
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="grp" HeaderText="Группа" 
                        ItemStyle-HorizontalAlign="Left">
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="lead" HeaderText="Тр." ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="speed" HeaderText="Ск." ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="boulder" HeaderText="Боулд." ItemStyle-HorizontalAlign="Center" />
                    <asp:ButtonField CommandName="EditLine" Text="Правка" ButtonType="Button"/>
                    <asp:ButtonField CommandName="MoveUp" Text="Вверх" ButtonType="Button" />
                    <asp:ButtonField CommandName="MoveDown" Text="Вниз" ButtonType="Button" />
                </Columns>
            </asp:GridView>
            </asp:Panel>
            
          </asp:Panel>  

        </ContentTemplate>
    </asp:UpdatePanel>
     
     
    <asp:SqlDataSource ID="dsTeams" runat="server" 
        ConnectionString="<%$ ConnectionStrings:db %>" 
        
        SelectCommand="SELECT 0 iid, 'Все команды' name, 0 pos UNION ALL SELECT iid, name, 1 pos FROM ONLteams(NOLOCK) ORDER BY pos, name"></asp:SqlDataSource>
        
        <asp:SqlDataSource ID="dsGroups" runat="server" 
        ConnectionString="<%$ ConnectionStrings:db %>" 
        
        SelectCommand="SELECT 0 iid, 'Все группы' name, 0 pos UNION ALL SELECT iid, name, 1 pos FROM ONLgroups(NOLOCK) ORDER BY pos, iid, name"></asp:SqlDataSource>
    

</asp:Content>
