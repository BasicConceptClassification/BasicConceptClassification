﻿<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="HomePage.aspx.cs" Inherits="HomePage" %>


<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Basic Concepts Classification</h1>
        <p>Welcome to the Basic Concepts Classification. Here you can search for items found in the 
            galleries, archieves, and museums. Each Item can be searched for by terms in the classification.</p>
            <form action="SearchResults.aspx" method="get" target="_self">
                Search:
                <input type="search" name="BCCSearch" placeholder ="[term1] [term2]">
                <input type="button" onclick="location.href = 'SearchResults.aspx'" value='Search' />
            </form>
        </center>
    </div>
</asp:Content>
