<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpages/Default.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RedButtonMarketingWeb.Pages.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="View1" runat="server">
            <asp:Button ID="Button1" runat="server" Text="Get buzzword search results" OnClientClick="ReplaceButtonWithText" OnClick="Button1_Click" />
            <asp:Button ID="Button2" runat="server" Text="Transfer buzzwords" OnClick="Button2_Click" />
            <div id="mystatuspanel" style="color:red;"></div>
        </asp:View>
        <asp:View ID="View2" runat="server">
            <span style="color:red;">
                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
            </span>
        </asp:View>
    </asp:MultiView>
</asp:Content>
