﻿<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True" Inherits="SearchResults" CodeBehind="SearchResults.aspx.cs" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search Results</h1></center>
        <br />
        <center>
            Search Results:
            <asp:TextBox ID="testForYu" runat="server" placeholder ="Enter text to print here"/>
            <asp:Button ID="btnclick" onclick="btnclick_Click" Text="Test Button!" runat="server" />
            <br />
            <asp:Label ID="printResults" runat="server"></asp:Label>
            <br />
            <br />
        </center>
        <div id="SearchReCon" runat="server"></div>
    </div>
</asp:Content>

