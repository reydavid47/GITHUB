<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="facility_doctor_detail.aspx.cs" Inherits="ClearCostWeb.SearchInfo.facility_doctor_detail" %>

<asp:Content ID="facility_doctor_detail_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <h1>
    Search Result Details</h1>
    <%--<div id="result_buttons">
        <div class="button">
            <a href="#" onclick="javascript:window.print(); return false;">Print Results</a>
        </div>
        <!--
        <div class="button">
            <a href="#">Save this Search</a>
        </div>
        -->
    </div>--%>
    <!-- end result buttons -->
    <p>
        <a class="back" href="facility_detail.aspx">Return to search results</a>
    </p>
    <div id="header_search_info">
        <h3 class="displayinline">
            Search Information</h3>
        &nbsp;&nbsp;
        <p class="displayinline smaller">
            [ <a href="search.aspx">Edit</a> ] &nbsp; <span style="display:inline-block;">[ <b><a href="search.aspx">New Search</a></b> ]</span></p>
        <br class="clearboth" />
    </div>
    <b class="smaller">Service: <span class="upper">Pediatrician - Initial Office Visit</span></b>
    <div class="learnmore">
        <a title="Learn more">
            <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
        <div class="moreinfo">
            <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                style="cursor: pointer;" />
            <p>
                <b class="upper">Pediatrician, New Patient Office Visit</b><br />
                Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                Etiam aliquet massa et lorem.</p>
        </div>
        <!-- end moreinfo -->
    </div>
    <!-- end learnmore -->
    <div class="h2bg healthgrades">
        Dr. Samuel Abe (Rockman Pediatrics) <span class="floatright" style="display: inline-block;
            line-height: 2.8em">Healthgrades Recognized Physician
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Healthgrades&trade;</b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </span>
    </div>
    <!-- start collapsible table -->
    <table width="100%" cellspacing="0" cellpadding="0" border="0" id="doctorfacilitytable">
        <tr class="h3 rowclosed category" id="overview">
            <td>
                Overview
            </td>
        </tr>
        <tr class="careinstance">
            <td>
                <div id="searchdetails">
                    <div id="detailinfo">
                        <table id="detailtable" cellspacing="0" cellpadding="4" border="0">
                            <tr>
                                <td>
                                    Total Estimated Cost:
                                </td>
                                <td>
                                    $115 - $125
                                    <div class="learnmore">
                                        <a title="Learn more">
                                            <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                                        <div class="moreinfo">
                                            <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                                                style="cursor: pointer;" />
                                            <p>
                                                <b class="upper">Estimated Cost</b><br />
                                                Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                                                arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                                                Etiam aliquet massa et lorem.</p>
                                        </div>
                                        <!-- end moreinfo -->
                                    </div>
                                    <!-- end learnmore -->
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Primary Office Address:
                                </td>
                                <td>
                                    4355 Montgomery Rd.<br />
                                    Naperville, IL 60564
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Alternate Office Address:
                                </td>
                                <td>
                                    224 Sansome St.<br />
                                    Naperville, IL 60564
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Phone:
                                </td>
                                <td>
                                    (630) 236-8300
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Fax:
                                </td>
                                <td>
                                    (630) 236-9860
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Email:
                                </td>
                                <td>
                                    info@pteach.com
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Website:
                                </td>
                                <td>
                                    www.pteach.com
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Primary Specialty:
                                </td>
                                <td>
                                    Pediatrics - Board Certified
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="detailmap">
                        <iframe width="425" height="350" frameborder="0" scrolling="no" marginheight="0"
                            marginwidth="0" src="http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=4355+Montgomery+Rd.+Naperville,+IL+60564&amp;aq=&amp;sll=41.732121,-88.208354&amp;sspn=0.010761,0.015621&amp;ie=UTF8&amp;hq=&amp;hnear=4355+Montgomery+Rd,+Naperville,+Illinois+60564&amp;z=14&amp;ll=41.732121,-88.208354&amp;output=embed">
                        </iframe>
                        <br />
                        <small><a href="http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=4355+Montgomery+Rd.+Naperville,+IL+60564&amp;aq=&amp;sll=41.732121,-88.208354&amp;sspn=0.010761,0.015621&amp;ie=UTF8&amp;hq=&amp;hnear=4355+Montgomery+Rd,+Naperville,+Illinois+60564&amp;z=14&amp;ll=41.732121,-88.208354"
                            style="color: #0000FF; text-align: left">View Larger Map</a></small>
                        <p>
                            8.3 miles from your home
                        </p>
                        <p>
                            <a href="#">Get directions</a></p>
                    </div>
                    <div class="clearboth">
                    </div>
                </div>
            </td>
        </tr>
        <tr class="h3 rowopen category" id="education">
            <td>
                Education
            </td>
        </tr>
        <tr class="careinstance">
            <td>
                <p>
                    <b>Medical School:</b><br />
                    Osmania Med College Hyderabad India<br />
                    Graduated: 1974
                </p>
                <p>
                    <b>Residency Hospital:</b><br />
                    Cook County Hospital Chicago
                </p>
                <p>
                    <b>Internship Hospital:</b><br />
                    Cook County Hospital Chicago
                </p>
                <p>
                    <b>Fellowship Hospital:</b><br />
                    Sinai Grace Hospital<br />
                    Detroit, MI, USA<br />
                    Year completed: 1981
                </p>
            </td>
        </tr>
        <tr class="h3 rowopen category" id="rating">
            <td>
                Patient Satisfaction
            </td>
        </tr>
        <tr class="careinstance">
            <td>
                <table cellspacing="0" cellpadding="4" border="0" width="100%">
                    <tr class="roweven">
                        <td>
                            <b>OVERALL RATING</b><br />
                            (based on 7 reviews)
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_half">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Recommend to a friend:</b><br />
                            Would you recommend your physician to family/friends?
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_half">
                                *</div>
                            <div class="star_none">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Level of Trust:</b><br />
                            Do you trust your physician to make decisions /recommendations that are in your
                            best interests?
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_half">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Helps Patients Understand Their Condition:</b><br />
                            Does the physician help you to understand your medical condition(s)?
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_half">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Listens and Answers Questions:</b><br />
                            Does the physician listen to you and answer your questions?
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_half">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Time Spent with Patient</b><br />
                            Do you feel the physician spends an appropriate amount of time with you?
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_none">
                                *</div>
                            <div class="star_none">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Scheduling Appointments:</b><br />
                            Ease of scheduling urgent appointments when you feel ill
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_half">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Office Environment:</b><br />
                            Cleanliness, comfort, lighting, temperature, location
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_half">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Office Friendliness:</b><br />
                            Friendliness and courtesy of office staff
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_half">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Average Wait Time:</b>
                        </td>
                        <td align="center">
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_full">
                                *</div>
                            <div class="star_half">
                                *</div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <!-- end doctorfacilitytable -->
    <p>
        <asp:Label ID="lblAllResult1DisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
    </p>
</asp:Content>

