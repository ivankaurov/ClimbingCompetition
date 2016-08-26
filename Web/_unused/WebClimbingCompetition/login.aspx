<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="WebClimbing._Plogin"
 MasterPageFile="~/WebAppl.Master" Theme="appSkin" EnableEventValidation="false"%>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="LoginContent" runat="server">
    

    <asp:Label ID="lblHeader" runat="server" Font-Bold="true" Text="Аутентификация" />
    <br />
    <asp:Label ID="lblUser" runat="server" Text="Пользователь: " />
    <asp:DropDownList ID="cbUsers" runat="server">
    </asp:DropDownList>
    
    <br />
    <asp:Label ID="lblPassword" runat="server" Text="Пароль: " />
    <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" />
    
    <br />
    <table width="100%">
    <tr>
    <td>
        <asp:Button ID="btnLogin" runat="server" Text="Войти в систему" 
            onclick="btnLogin_Click"/>
    </td>
    <td />
    <td>
        <asp:Button ID="getPassword" runat="server" Text="Забыли пароль?" onclick="getPassword_Click" />
    </td>
     </tr>
    
    </table>
    <br />
    <asp:Label ID="lblStatus" runat="server" Text="" EnableViewState="false" />   
    
</asp:Content>