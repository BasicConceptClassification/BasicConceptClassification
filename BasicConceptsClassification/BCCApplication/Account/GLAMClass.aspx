﻿<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GLAMClass.aspx.cs" Inherits="BCCApplication.Account.GLAMClass" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        
            <br />
            <table style="width:100%;">
                <tr>
                    <td class="auto-style1">
                        <h3>BCC Term Updates:</h3>
                        <select id="RecAddedTerms" name="D1" size="8" runat="server"></select><br /><br />
                        <h3>Your Recently Classified:</h3>
                        <select id="RecClassObj" name="D1" size="8" runat="server"></select>
                        <div>
                            <asp:Label ID="LabelRecClassObj" runat="server" Visible="false"></asp:Label>
                        </div>
                    </td>
                    <td><center><h1>Welcome, Classifier!</h1>
                        <p>Search for items below, add new or classify items waiting to be classified! (And other instructions.)</p>
                        <p>Search: <asp:TextBox ID="TextBox2" runat="server" placeholder="(term1)(term2)"></asp:TextBox>
                            <asp:Button ID="Button1" runat="server" Text="Search" OnClick="Button1_Click"/>
                        </p>
                        <form action="SearchResults.aspx" method="get" target="_self">
                        </form>
                        </center>
                        
                         <table style="width:100%;" id ="NewsBoxClass" runat="server">
                            <tr>
                                <td>
                                    <asp:Label ID="Label2" runat="server" Text="News:" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                    <br />
                                    <center><asp:TextBox ID="NewsBox" runat="server" Height="222px" Width="648px"></asp:TextBox></center>
                                    <br />

                                </td>
                            </tr>
                        </table>
                        <div>
                            <asp:Label ID="Label1" runat="server" Text="Manage Classifiables" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        </div>
                        <table style="width:100%;">
                            <tr>
                                <td>
                                    <h3>Not Classified</h3>
                                    <div id="NotClassifiedExists">
                                        <center>
                                            <select id="UnClassList" name="D1" size="5" runat="server"></select><br />
                                            <asp:Button ID="ButtGLAMClassClassNow" runat="server" Text="Classify Now" OnClick="ClassNow_Click" Font-Size="Small" />
                                        </center>
                                    </div>
                                    <div id="NotClassifiedNoneExist">
                                        <asp:Label ID="LabelNotClassified" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <h3>Not Classified - Special</h3>
                                    <div id="NotClassifiedSpecialExists">
                                    <center>
                                    <select id="UnClassAdminCause" name="D1" size="5" runat="server"></select><br />
                                    <asp:Button ID="ButtGLAMClassReClassNow" runat="server" Text="Reclassify Now" OnClick="ReClassNow_Click" Font-Size="Small" />

                                    </center>
                                        </div>
                                    <div id="NotClassifiedSpecialNoneExist">
                                        <asp:Label ID="LabelNotClassifiedSpecial" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </td>
                                <td><center><p>OR</p></center></td>
                                <td><center><asp:Button ID="ButtGLAMClassAddNew" runat="server" Text="Add New" OnClick="AddNew_Click" Font-Size="Small"/><br /><br />
                                    <asp:Button ID="RemoveClass" runat="server" Text="Remove" OnClick="RemoveClassPage_Click" Font-Size="Small"/>
                                    </center></td>
                            </tr>
                        </table>
                        <br />                       
                    </td>
                </tr>
            </table>
            &nbsp;
        
    </div>
</asp:Content>

<asp:Content ID="Content1" runat="server" contentplaceholderid="HeadContent">
    <style type="text/css">
        #UnClassList {
            width: 187px;
        }
        #UnClassAdminCause {
             width: 187px;
        }
        .auto-style1 {
            width: 307px;
        }
        #RecAddedTerms {
            width: 187px;
        }
        #RecClassObj {
            width: 187px;
        }
    </style>
</asp:Content>


