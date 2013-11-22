<%@ Page Language="C#" AutoEventWireup="true" CodeFile="landing.aspx.cs" Inherits="ClearCostWeb.ClientPages.landing" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="robots" content="noindex,nofollow" />
    <title>Shop for medical services based on cost and quality ratings</title>
    <link href="../Styles/landing.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        div.compatWarn 
        {
            margin-bottom: 30px;
            background-color: #fcb21f;
            font-size: 18px;
            font-weight: bold;
            color: #fff;
            display:inline-block;
            padding:5px 10px;
        }
        div#vidContainer_wrapper { border: 1px solid gray; }
        .centric a { font-size: 15px !important; line-height: 16px !important; }
    </style>
    <script src="../Scripts/jquery-1.5.1.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.bxSlider.min.js" type="text/javascript"></script>
    <%--<script src="../Scripts/jwplayer.js" type="text/javascript"></script>--%>
    <script type="text/javascript">
        var slider;
        $(document).ready(function () {
            slider = $('#mainframe').bxSlider({
                auto: false,
                infiniteLoop: false,
                controls: false,
                nextSelector: "div.bx-next"
            });
            $(".bx-next").click(function () { slider.goToNextSlide(); return false; });
        });
    </script>
    <script type="text/javascript">
        /*** Google Analytics ***/
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-34081191-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="main">
        <div id="topbar">
            <div id="logo">
                <asp:Literal ID="ltlCompatabilityWarning" runat="server" Text="" />
                <a href="landing.aspx" style="cursor:pointer;">
                    <img src="../Images/clearcosthealth_logo.gif" alt="ClearCost Health" width="469"
                        height="70" border="0" />
                </a>
            </div>
            <asp:Panel ID="loginregister" runat="server" ClientIDMode="Static" Visible="false">
                <asp:Panel ID="pnlSignIn" runat="server" ClientIDMode="Static" Visible="false" CssClass="button-signin" onclick="location.href = 'Sign_in.aspx';" style="cursor:pointer;">
                    <a>Sign In</a>
                </asp:Panel>
                <asp:Literal ID="ltlBreaker" runat="server" Text="&nbsp;|&nbsp;" Visible="false" />
                <asp:Panel ID="pnlRegister" runat="server" ClientIDMode="Static" Visible="false" CssClass="button-register" onclick="location.href = 'Welcome.aspx';" style="cursor:pointer;">
                    <a>Register</a>
                </asp:Panel>
            </asp:Panel>
            <!-- end floatright -->
            <div class="clearboth block">
            </div>
        </div>
        <div id="ctop">
        </div>
        <div id="cmid">
            <div id="cbotdemo">
                <div class="intwide">
                    <div id="mainframe">
                        <!-- SLIDE ONE -->
                        <div>
                            <h1>
                                Shop for medical services based on<br />
                                cost and quality ratings</h1>
                            <div id="landingmain">
                                <div id="computerbg">
                                    <div align="center" style="width: 257px; display: inline-block; float: left;">
                                        <asp:Image ID="imgLogo" CssClass="landingLogo" runat="server" AlternateText="logo" BorderWidth="0" Visible="false" Style="max-width: 182px; margin-left: auto;" />
                                    </div>
                                    <div style="float:right;margin-right:55px;margin-top:10px;">
                                        <%--<iframe width="575" height="323" src="https://www.youtube-nocookie.com/embed/E0G5dIpPH4I?rel=0" frameborder="0" allowfullscreen style="border:1px solid black;"></iframe>--%>
                                        <div id="movieframe">
                                            <script type="text/javascript" language="javascript" src="../Scripts/jwplayer.js"></script>
                                            <div id="mediaspace" style="width:450px;height:254px;"></div>
                                            <script type="text/javascript" language="javascript">
                                                jwplayer("mediaspace").setup({
                                                    flashplayer: '<%= ResolveUrl("~/Images/player.swf") %>',
                                                    file: "http://www.youtube.com/watch?v=eMRf6NAooXU",
                                                    image: '<%= ResolveUrl("~/Images/screen.jpg") %>',
                                                    skin: '<%= ResolveUrl("~/Images/glow.zip") %>',
                                                    width: 450,
                                                    height: 254,
                                                    "controlbar.idlehide": true
                                                });
                                            </script>
                                        </div>
                                    </div>                                   
                                    <br />
                                    
                                    <asp:Panel ID="pnlRegNow" runat="server" CssClass="button-orange-arrow-lg" Visible="false" 
                                        onclick="location.href = 'Welcome.aspx';" style="width: 235px; margin-bottom: 15px; cursor:pointer;">
                                        <a>Register Now</a>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlSignInNow" runat="server" CssClass="button-white-arrow-lg" Visible="false"
                                        onclick="location.href = 'Sign_In.aspx';" style="width: 235px; margin-bottom: 15px; cursor:pointer;">
                                        <a>Sign In Now</a>
                                    </asp:Panel>
                                    <br /><br />
                                    <div class="button-gray-white-arrow-lg bx-next" style="width: 235px;cursor:pointer;">
                                        <a>Tell me more</a>
                                    </div>
                                    <asp:Literal ID="ltlDisclamer" runat="server" Visible="false">
                                        <p class="disclaimer">
                                            PLEASE NOTE: Only {1} participants enrolled in a{0} medical plan can access ClearCost Health.
                                        </p>
                                    </asp:Literal>
                                    
                                    <%--<p class="disclaimer">
                                        PLEASE NOTE: Only partners enrolled in a <asp:Literal ID="ltlInsurer" runat="server" Text="" /> medical plan can access ClearCost
                                        Health.
                                    </p>--%>
                                </div>
                                <!-- end computerbg -->
                            </div>
                            <!-- end landingmain -->
                        </div>
                        <!-- end SLIDE ONE -->
                        <!-- SLIDE TWO -->
                        <div>
                            <h1>
                                Did you know...</h1>
                            <h2>
                                there are big differences in price for routine medical services depending on which
                                "in-network" provider you go to?</h2>
                            <br />
                            <br />
                            <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                <tr>
                                    <td width="50%" align="center">
                                        <img src="../Images/img_knee_mri.png" alt="" width="295" height="203" border="0" />
                                    </td>
                                    <td width="50%" align="center">
                                        <img src="../Images/img_office_visit.png" alt="" width="295" height="203" border="0" />
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            <br />
                            <div style="padding-left: 350px;">
                                <div class="button-gray-white-arrow-lg bx-next" style="cursor:pointer;">
                                    <a>Really? Why?</a>
                                </div>
                            </div>
                        </div>
                        <!-- end SLIDE TWO -->
                        <!-- SLIDE THREE -->
                        <div>
                            <h2>
                                Some providers (such as large hospitals and big groups of doctors) can demand higher
                                prices:</h2>
                            <table cellspacing="1" cellpadding="0" border="0" class="transparencygrid">
                                <tr>
                                    <th class="desc">
                                        Description
                                    </th>
                                    <th>
                                        Highest Cost In-Network
                                    </th>
                                    <th>
                                        Lowest Cost In-Network
                                    </th>
                                </tr>
                                <tr class="odd">
                                    <td class="desc">
                                        Cholesterol test
                                    </td>
                                    <td class="fig">
                                        $232
                                    </td>
                                    <td class="fig">
                                        $13
                                    </td>
                                </tr>
                                <tr class="even">
                                    <td class="desc">
                                        CT scan, pelvis
                                    </td>
                                    <td class="fig">
                                        $3,071
                                    </td>
                                    <td class="fig">
                                        $348
                                    </td>
                                </tr>
                                <tr class="odd">
                                    <td class="desc">
                                        Office visit, new patient
                                    </td>
                                    <td class="fig">
                                        $186
                                    </td>
                                    <td class="fig">
                                        $36
                                    </td>
                                </tr>
                                <tr class="even">
                                    <td class="desc">
                                        Generic Prozac (one year)
                                    </td>
                                    <td class="fig">
                                        $336
                                    </td>
                                    <td class="fig">
                                        $48
                                    </td>
                                </tr>
                            </table>
                            <h2>
                                Studies show there is little correlation between cost and quality in healthcare,*
                                so higher cost does not mean better quality.</h2>
                            <br />
                            <br />
                            <br />
                            <div style="padding-left: 300px;">
                                <div class="button-gray-white-arrow-lg bx-next" style="cursor:pointer;">
                                    <a>What does this mean for me?</a>
                                </div>
                            </div>
                            <br />
                            <br />
                            <p>
                                * "<a href='http://www.mass.gov/ago/docs/healthcare/2011-hcctd.pdf'>Examination of Health
                                    Care Cost Trends and Cost Drivers</a>" Report for Annual Public Hearing issues
                                by Massachusetts Attorney General Marth Coakley, June 22, 2011
                            </p>
                        </div>
                        <!-- END SLIDE THREE -->
                        <!-- SLIDE FOUR -->
                        <div>
                            <h2>
                                With access to this information, you could save a lot of money on your health care
                                by shopping for medical services and prescription drugs, without sacrificing quality.</h2>
                            <br />
                            <div class="aligncenter">
                                <img src="../Images/img_search_results_knee.png" alt="" width="614" height="412" border="0" />
                            </div>
                            <br />
                            <br />
                            <div style="padding-left: 300px;">
                                <div class="button-gray-white-arrow-lg bx-next" style="cursor:pointer;">
                                    <a>How do I do that?</a>
                                </div>
                            </div>
                        </div>
                        <!-- END SLIDE FOUR -->
                        <!-- SLIDE FIVE -->
                        <div>
                            <h1>
                                With ClearCost Health!</h1>
                            <h2>
                                <br />
                                ClearCost Health allows you to search for doctors, lab tests, imaging studies, procedures
                                and prescription drugs based on cost, quality and convenience.
                                <br />
                                <br />
                                With our service, you'll be able to make more informed decisions about your health
                                care and save money.
                            </h2>
                            <br />
                            <br />
                            <asp:Panel ID="pnlRegNow2" runat="server" CssClass="button-orange-arrow-lg" Visible="false" 
                                onclick="location.href = 'Welcome.aspx';" style="width: 260px; margin-bottom: 15px; cursor:pointer;margin-left:300px;">
                                <a>Register Now</a>
                            </asp:Panel>
                            <%--<div style="padding-left: 300px;">
                                <div class="button-orange-arrow-lg ">
                                    <a href="Welcome.aspx">Register Now</a>
                                </div>
                                <br />
                            </div>--%>
                        </div>
                        <!-- END SLIDE FIVE -->
                    </div>
                    <!-- end mainframe -->
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
            <a href='AboutUs.aspx'>About Us</a><img src="../Images/nav_div.gif" alt="" width="1" height="17"
                border="0" /><a href='Contact_us.aspx'>Contact Us</a>
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
