<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="no_access.aspx.cs" 
    Inherits="WebClimbing._PNoAccess" EnableEventValidation="false" Theme="appSkin" MasterPageFile="~/WebAppl.Master"%>
<%@ Register TagPrefix="clm" TagName="TimeRefresh" Src="~/TimeRefreshingControl.ascx"  %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">   
        setInterval("OnTimer()", 1000);
        
    </script>
      
      
                <asp:Label ID="Label2" runat="server" 
        Text="Ошибка 403. Доступ запрещён."></asp:Label>
    <br />
                
                     
                        <br />
                    
                </asp:Content>
