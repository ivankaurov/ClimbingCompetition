<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="error_occured.aspx.cs" 
    Inherits="WebClimbing._PErrorOccured" EnableEventValidation="false" Theme="appSkin" MasterPageFile="~/WebAppl.Master"%>
<%@ Register TagPrefix="clm" TagName="TimeRefresh" Src="~/TimeRefreshingControl.ascx"  %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">   
        setInterval("OnTimer()", 1000);
        
    </script>
      
      
                <asp:Label ID="Label2" runat="server" 
        Text="Неизвестная ошибка."></asp:Label>
    <br />
                
                     
                        <br />
                    
                </asp:Content>
