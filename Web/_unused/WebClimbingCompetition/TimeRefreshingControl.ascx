<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeRefreshingControl.ascx.cs" Inherits="WebClimbing._CTimeRefreshingControl" %>
<asp:Timer ID="Timer1" runat="server" Interval="10000" ontick="Timer1_Tick">
</asp:Timer>
<asp:Label ID="Label1" runat="server" 
    Text="Автоматическое обновление через 180 секунд"></asp:Label>

