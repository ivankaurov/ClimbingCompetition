<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClimberControl.ascx.cs" Inherits="WebClimbing.Controls.ClimberControl" %>
<table width="100%">
<asp:HiddenField ID="hfIid" runat="server" Value="" />
<asp:HiddenField ID="hfTeamID" runat="server" Value="" />

            <tr><td align="right">Фамилия, Имя:</td>
            <td><asp:TextBox ID="tbName1" runat="server" AutoCompleteType="Disabled" /></td>
            </tr>
            <tr><td align="right">Г.р.:</td>
            <td><asp:TextBox ID="tbAge1" runat="server" AutoCompleteType="Disabled" 
                    Wrap="False" /></td>
            </tr>
            <tr><td align="right">Пол:</td>
            <td><asp:DropDownList ID="cbGender1" runat="server">
                    <asp:ListItem Text="М" Value="0" />
                    <asp:ListItem Text="Ж" Value="1" />
                </asp:DropDownList>
            </td>
            </tr>
            <tr>
                <td align="right">Группа:</td>
                <td>
                    <asp:Label ID="lblGroup" runat="server" Text="" />
                </td>
            </tr>
            <tr><td align="right"><!--<label style="color:Red">-->Разряд:<!--</label>--></td>
            <td><asp:DropDownList ID="cbQf1" runat="server">
                    <asp:ListItem Text="" Value="" />
                    <asp:ListItem Text="ЗМС" Value="ЗМС"/>
                    <asp:ListItem Text="МСМК" Value = "МСМК"/>
                    <asp:ListItem Text="МС" Value="МС"/>
                    <asp:ListItem Text="КМС" Value="КМС"/>
                    <asp:ListItem Text="1" Value="1" />
                    <asp:ListItem Text="2" Value="2" />
                    <asp:ListItem Text="3" Value="3" />
                    <asp:ListItem Text="1ю" Value="1ю" />
                    <asp:ListItem Text="2ю" Value="2ю" />
                    <asp:ListItem Text="3ю" Value="3ю" />
                    <asp:ListItem Text="б/р" Value="б/р" />
                </asp:DropDownList>
            </td>
            </tr>
            <tr>
            <td align="right">Участие в видах:</td>
            <td>
              <asp:Panel runat="server" ID="pnlStyles" Enabled = "true">
                <table>
                    <tr>
                    <td align="right">Трудность:</td>
                    <td align="left">
                    <asp:DropDownList runat="server" ID="cbSelectLead">
                        <asp:ListItem Text="-" Value="0" Enabled="true" Selected="True" />
                        <asp:ListItem Text="+" Value="1" Enabled="true" Selected="False" />
                        <asp:ListItem Text="Лично" Value="2" Enabled="true" Selected="False" />
                    </asp:DropDownList>
                    </td>
                    </tr>
                    <tr>
                    <td align="right">Скорость:</td>
                    <td align="left">
                    <asp:DropDownList runat="server" ID="cbSelectSpeed">
                        <asp:ListItem Text="-" Value="0" Enabled="true" Selected="True" />
                        <asp:ListItem Text="+" Value="1" Enabled="true" Selected="False" />
                        <asp:ListItem Text="Лично" Value="2" Enabled="true" Selected="False" />
                    </asp:DropDownList>
                    </td>
                    </tr>
                    <tr>
                    <td align="right">Боулдеринг:</td>
                    <td align="left">
                    <asp:DropDownList runat="server" ID="cbSelectBoulder">
                        <asp:ListItem Text="-" Value="0" Enabled="true" Selected="True" />
                        <asp:ListItem Text="+" Value="1" Enabled="true" Selected="False" />
                        <asp:ListItem Text="Лично" Value="2" Enabled="true" Selected="False" />
                    </asp:DropDownList>
                    </td>
                    </tr>
                </table>
              </asp:Panel>
                <!--<asp:CheckBoxList ID="cbl12" runat="server" Visible="false" >
                    <asp:ListItem Text="Трудность" Selected="False" />
                    <asp:ListItem Text="Скорость" Selected="False" />
                    <asp:ListItem Text="Боулдеринг" Selected="False" />
                </asp:CheckBoxList>-->
            </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:Label ID="lblMessage1" runat="server" Text="" ForeColor="Red" />
                </td>
            </tr>
        </table>
