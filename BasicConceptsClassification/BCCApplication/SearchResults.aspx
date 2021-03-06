﻿<%@ Page Title="Search Results" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True" Inherits="SearchResults" CodeBehind="SearchResults.aspx.cs" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <h2><%: Title %></h2>
        <div>
            <asp:Label ID="LabelDescription" runat="server"></asp:Label>
        </div>
        <center>
            <asp:Label ID="SearchPrompt" runat="server" CssClass="control-label">Search for:</asp:Label>
            <asp:TextBox ID="searching_textbox" runat="server" CssClass="input-xxlarge search-query" placeholder="(Search)(by)(Terms)"/>
            <asp:Button ID="btnclick" onclick="btnclick_Click" Text="Search" runat="server" />
            <br />
            <asp:Button ID="name_sort" runat="server" Text="Sort by name" OnClick="name_sort_Click" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
            <asp:Button ID="relev_sort" runat="server" OnClick="relev_sort_Click" Text="Sort by relevancy" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;<asp:Button ID="order_sort" runat="server" OnClick="order_sort_Click" Text="Sort by order of terms" />
            &nbsp;&nbsp;&nbsp;
            <br />
        </center>
        <div id="SearchReCon" runat="server"></div>
    </div>
</asp:Content>

