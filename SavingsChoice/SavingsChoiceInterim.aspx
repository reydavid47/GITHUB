<%@ Page Title="" Language="C#" MasterPageFile="~/SavingsChoice/SavingsChoice.master" AutoEventWireup="true" CodeFile="SavingsChoiceInterim.aspx.cs" Inherits="ClearCostWeb.SavingsChoice.SavingsChoiceInterim" %>

<asp:Content ID="cInterimHead" ContentPlaceHolderID="head" Runat="Server">
    <link rel="stylesheet" href="../Styles/jquery-ui-1.10.3.css" />
    <link href="../Styles/overlay.css" rel="Stylesheet" /> 
    <script type="text/javascript" language="javascript" src="../Scripts/jquery-1.8.3.min.js"></script>
    <style type="text/css">
    #body_moveOn
    {
    background-image: url("../Images/buttons/dashboard.png") !important;
    background-position: -12px -6px !important;
    background-repeat: repeat-x;
    font-size: 1.2em;
    line-height: 40px !important;
    margin: 0 !important;
    padding: 0 20px 0 32px !important;
    width: auto !important;
    	}
    </style>
</asp:Content>

<asp:Content ID="cInterimBody" ContentPlaceHolderID="body" Runat="Server">
    <h1>Thank You!</h1>
    <div class="based">
        <p><asp:Label ID="lblStartingText" runat="Server" Text="" /></p>
    </div>
    <div class="moveon">
		<div class="dashboard">
            <asp:LinkButton ID="moveOn" PostBackUrl="~/SearchInfo/Search.aspx" runat="server" Text="Move on to ClearCost Health" CssClass="btn" />
			<%--<a href="SearchInfo/Search.aspx">Move onto your Savings Choice Home Page</a>--%>
		</div>
	</div>
    <div class="question">
		<div id="first_q" class="add_box expander collapsed">
			What is ClearCost?
		</div>
		<div id="first_a" class="content" style="display: none;">
			ClearCost Health is a free service that Caesars offers which allows you to shop for in-network health care providers based on cost, quality and convenience. You can search for services like doctors’ office visits, lab tests, radiology procedures (like x-rays and MRIs), prescription drugs and basic services (like colonoscopies and sleep studies) to find the best priced provider so you can save on your out-of-pocket health care expenses.
		</div>
	</div>
    <div class="question">	
		<div id="second_q" class="add_box expander collapsed">
			How do I avoid the penalty for 2014?
		</div>
		<div id="second_a" class="content" style="display: none;">
			You just need to register for ClearCost, which you have successfully done. If you have a spouse or domestic partner that is on your Caesars health plan, they also need to register for ClearCost. 
		</div>
	</div>
    <div class="question">	
		<div id="third_q" class="add_box expander collapsed">
			What services can I search for with ClearCost?
		</div>
		<div id="third_a" class="content" style="display: none;">
			ClearCost provides cost and quality information for basic, non-urgent health care services. These include doctors’ office visits, lab tests, radiology procedures (like x-rays and MRIs), prescription drugs and basic procedures (like colonoscopies and sleep studies). Visit the “Past Care” tab on the website to see how you could have saved money on the medical services that you received in the last year. 	
		</div>
	</div>
    <div class="question">
		<div id="fourth_q" class="add_box expander collapsed">
			I already have health insurance, why would I need ClearCost?
		</div>
		<div id="fourth_a" class="content" style="display: none;">
			ClearCost allows you to search for health care providers within your health care network. Few people realize that there can be substantial cost differences even among in-network health care providers. As a result, many individuals overpay for basic health care services. ClearCost helps you identify areas in your medical spending where you can save money. For instance, you might be able to save simply by switching your pharmacy! Use ClearCost to learn how you can save.
		</div>
	</div>
    <div class="question">	
		<div id="fifth_q" class="add_box expander collapsed">
			Who can use ClearCost?
		</div>
		<div id="fifth_a" class="content" style="display: none;">
			Caesars offers ClearCost for free to all over-18 employees and dependents on a Caesars medical plan.
		</div>
	</div>
    <div class="question">	
		<div id="sixth_q" class="add_box expander collapsed">
			How can I contact ClearCost?
		</div>
		<div id="sixth_a" class="content" style="display: none;">
			You can call ClearCost Health at <asp:Literal ID="ltlPhoneLM" runat="server" Text="" /> M-F, 6am – 5pm Pacific or email us at <asp:Literal ID="ltlEmployerName" runat="server" Text="" />@clearcosthealth.com.
		</div>
	</div>
    <div class="moveon">
		Questions? Our Health Shoppers are here to help! Call us at <asp:Literal ID="ltlPhoneQ" runat="server" Text="" />.
	</div>
    <script language="javascript" type="text/javascript">
        function expand(iData) {
            switch (iData.action) {
                case 'init':
                    $('.expander.collapsed').toggle(function () {
                        $(this).next().slideDown();
                        $(this).css('background-image', 'url("../Images/btn_subtract2.png")');
                    }, function () {
                        $(this).next().slideUp();
                        $(this).css('background-image', 'url("../Images/btn_add2.png")');
                    });
                    break;
            };
        };
        expand({ action: 'init' });
	</script>
</asp:Content>

