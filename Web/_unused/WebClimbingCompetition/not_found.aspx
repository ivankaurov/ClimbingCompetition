<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="not_found.aspx.cs" 
    Inherits="WebClimbing._PNotFound" EnableEventValidation="false" Theme="appSkin" MasterPageFile="~/WebAppl.Master"%>
<%@ Register TagPrefix="clm" TagName="TimeRefresh" Src="~/TimeRefreshingControl.ascx"  %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">   
        setInterval("OnTimer()", 1000);
        
    </script>
      
      
                <asp:Label ID="Label2" runat="server" 
        Text="Ошибка 404. Запрашиваемая Вами страница не найдена."></asp:Label>
    <br />
                
                     
                        <br />
                    
                </asp:Content>
