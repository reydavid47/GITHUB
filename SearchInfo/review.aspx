<%@ Page Language="C#" AutoEventWireup="true" CodeFile="review.aspx.cs" Inherits="ClearCostWeb.SearchInfo.review" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>ClearCost Health</title>
    
    <link href="../Styles/skin.css" rel="stylesheet" type="text/css" />
    <link href="Styles/old/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="main">
        <div id="topbar">
            <div id="logo">
                <img src="../Images/clearcosthealth_logo.gif" alt="ClearCost Health" width="469" height="70"
                    border="0" />
            </div>
            <div id="tag">
                <img src="../Images/clearcosthealth_tag.gif" alt="We bring price transparency to health"
                    width="343" height="21" border="0" />
            </div>
            <!-- end floatright -->
            <div class="clearboth">
            </div>
        </div>
        <div id="ctop">
        </div>
        <div id="cmid">
            <div id="cbotdemo">
                <div class="intwide">
                    <h1>
                        Review</h1>
                    <h2>
                        Success! Your account has been validated. Please review the information below; if
                        any of the information is incorrect, please contact your HR department:
                    </h2>
                    <br />
                    <h3>
                        Your Personal Information</h3>
                    <form name="personalform" action="search.aspx">
                    <table class="formtable" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td>
                                Name:
                            </td>
                            <td>
                                George Jones
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Date of Birth
                            </td>
                            <td>
                                April 22, 1957
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Health Plan:
                            </td>
                            <td>
                                Blue Cross Blue Shield of Illinois &nbsp; BCBS PPO
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Home Address
                            </td>
                            <td>
                                100 W Galena Blvd<br />
                                Aurora, IL 60506
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Email:
                            </td>
                            <td>
                                George.jones@largeCo.com
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <h3>
                                    Adult Dependent Information</h3>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Name:
                            </td>
                            <td>
                                Molly Jones
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Date of Birth
                            </td>
                            <td>
                                April 22, 1957
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                Because Molly Jones is over the age of 18, you are not able to see her medical information
                                unless she grants access to you. To send Molly a link to authenticate her identification
                                and allow you access to her medical records, we need Molly's email address.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Molly's email:
                            </td>
                            <td>
                                <input class="boxed" type="text" name="dependent" value="" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <h3>
                                    Create a personalized account password</h3>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Password:
                            </td>
                            <td>
                                <input class="boxed" type="text" name="dependent" value="" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <h3>
                                    Terms and Conditions</h3>
                                <div style="width: 415px; height: 150px; overflow: scroll; border: 1px #666 solid;">
                                    <div style="width: 360px; margin: 10px">
                                        <p>
                                            Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor
                                            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud
                                            exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute
                                            irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla
                                            pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia
                                            deserunt mollit anim id est laborum.
                                        </p>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <a class="submitlink" onclick="document.personalform.submit();return false;">Access
                                    Account</a>
                        </tr>
                    </table>
                    </form>
                </div>
                <!-- end intwide -->
            </div>
            <!-- end cbot -->
        </div>
        <!-- end cmid -->
        <div class="clearboth">
        </div>
    </div>
    <!-- end main -->
    <div id="botbar">
        <div style="float: left" id="demobotnav">
            <a href='../about_us.html'>About Us</a><img src="../Images/nav_div.gif" alt="" width="1" height="17"
                border="0" /><a href='../contact_us.aspx'>Contact Us</a>
        </div>
        <p class="copyright">
            &copy; ClearCost Health. All Rights Reserved.
        </p>
        <div class="clearboth">
        </div>
    </div>
    </form>
</body>
</html>
