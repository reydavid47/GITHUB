<%@ Page Title="" Language="C#" MasterPageFile="~/AdminPortal/MasterAdmin.master" AutoEventWireup="true" CodeFile="Child.aspx.cs" Inherits="AdminPortal_Child" %>

        
         
        <asp:Content ContentPlaceHolderID="AdminContent" runat="server">
                        
            
            <input id="button1" type="button" value="get data" />
            <div id="table_div" style="width:400; height:300"></div>


            <%-- 
            <table id="SearchResultsTbl">
                
                <tr>
                    <asp:Repeater ID="TableHeaders" runat="server"> 
                        <ItemTemplate>  
                            <th><%# Container.DataItem %></th>
                        </ItemTemplate>  
                    </asp:Repeater>
                </tr> 
               
                <tr> 
                <asp:Repeater ID="TableRows" runat="server"> 
                    <ItemTemplate>
                        
                            <td><%# Container.DataItem %></td>
                        
                    </ItemTemplate>  
                </asp:Repeater>
                </tr>

            </table> 
            --%>
        </asp:Content>
        