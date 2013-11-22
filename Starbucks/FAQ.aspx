<%@ Page Title="" Language="C#" MasterPageFile="~/Starbucks/Starbucks.master" AutoEventWireup="true" CodeFile="FAQ.aspx.cs" Inherits="ClearCostWeb.Starbucks.FAQ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("p.q").click(function () {
                var displayIs = $(this).nextUntil("hr").css('display');
                if (displayIs.match(/block/)) {
                    //alert(displayIs);
                    $(this).removeClass("q-open");
                    $(this).addClass("q-closed");
                } else {
                    $(this).removeClass("q-closed");
                    $(this).addClass("q-open");
                }
                $(this).nextUntil("hr").slideToggle('slow');

            });
        });
    </script>
    <h1>
        Frequently Asked Questions
    </h1>
    <% if (Membership.GetUser() != null && Membership.GetUser().IsOnline)
       { %><a class="back" href="javascript:history.back();" style="background-position-y:center;background-image: url('../Images/sm_arrow_square_back.png');">Return</a><% } %>
    <div id="faq">
        <p class="q q-closed">
            Why can't I find the service I am looking for?
        </p>
        <p class="answer">
            We provide pricing data for approximately 300 common services performed in an outpatient
            setting. These include common lab tests, imaging studies, office visits, and routine
            outpatient procedures. The service you searched for may not be included in our list
            of services. In addition, sometimes medical terms can be difficult to spell. If you're 
            not sure exactly how the service is spelled, try the pull-down menus on our Find a Service page. 
        </p>
        <hr />
        <p class="q q-closed">
            What exactly does "Estimated Total Cost" mean?
        </p>
        <p class="answer">
            Estimated Total Cost refers to the expected total price paid by you and your employer
            for this service, based on recent payments made to this provider from your health
            plan. How much of this total price is paid by you versus your employer will vary
            based on your health plan and the other medical expenses you have had this year.
            Because prices can change and the exact services you receive can vary, the actual
            total price may fall outside of the range presented here. To find out estimated
            out-of-pocket price, select the "See out-of-pocket costs" option when you see pricing
            results from a search.
        </p>
        <hr />
        <p class="q q-closed">
            How can I see the cost to me personally?
        </p>
        <p class="answer">
            When you search for a service, in the Search Results page you will see an option
            to view your out-of-pocket cost. Selecting this option will display a range for
            your likely out-of-pocket costs, based on your health care spending since the start
            of the plan year. Because of the timing involved in claims submission and processing,
            please be aware that we may not have all of your most recent health care services
            in our system. It is possible that the price you pay out-of-pocket may fall outside
            of the range presented here.
        </p>
        <hr />
        <p class="q q-closed">
            What is a ClearCost Health Shopper?
        </p>
        <p class="answer">
            If you are eligible, a ClearCost Health Shopper will reach out to you with personalized
            information on how you and your family members can save money on your health care.
            The ClearCost Health Shopper will walk you through your past health care expenditures
            and explain how you can save money in the future. For instance, our Shopper might
            show you how to switch your prescription drugs to a different pharmacy to save money,
            or explain how to talk to your doctor about using a different facility for your
            lab tests.
        </p>
        <hr />
        <p class="q q-closed">
            What are Savings Alerts?
        </p>
        <p class="answer">
            Savings Alerts are personalized email messages that you will receive from ClearCost
            Health when we notice an opportunity for you to save money on your health care without
            sacrificing quality. For example, if you could save money by getting your lab tests
            done at a different facility, you will receive a Savings Alert that will let you
            know how much money you could save and then will direct you to the search page with
            more cost-effective options for future care.
        </p>
        <hr />
        <p class="q q-closed">
            What is the green Fair Price checkmark?
        </p>
        <p class="answer">
            When you are searching for a specific service, the Fair Price checkmark takes into
            account the amount that a facility gets paid for a given service in comparison to
            other facilities in the area. When you are searching for a doctor, the Fair Price
            designation takes into consideration not just the price of an office visit or procedure,
            but also where that doctor will refer you for services like lab tests, and imaging
            studies, as all of these downstream costs will determine your overall expenses with
            this doctor.
        </p>
        <hr />
        <p class="q q-closed">
            What does a Healthgrades Recognized Physician mean?
        </p>
        <p class="answer" style="padding-bottom: 0; margin-bottom: 5px;">
            Healthgrades Recognized ratings are assigned to physicians who:
        </p>
        <ol>
            <li>are board-certified in their specialty of practice</li>
            <li>have never had their medical license revoked</li>
            <li>are free of state/federal disciplinary sanctions in the last five years</li>
            <li>are free of any malpractice claims</li>
        </ol>
        <hr />
        <p class="q q-closed">
            Can ClearCost Health help me to select a doctor or medical services?
        </p>
        <p class="answer">
            Our service can be helpful for understanding cost and basic physician quality measures.
            However, we cannot recommend that you obtain any particular medical service or prescription
            drug &mdash; only a physician can make those recommendations.
        </p>
        <hr />
        <p class="q q-closed">
            Why do I see a range for prices, instead of an exact price?
        </p>
        <p class="answer">
            We provide a range to cover the most likely prices for a given type of service.
            The price of a service is often impossible to know in advance, until the doctor
            has decided the exact type of service required, often at the time the service is
            provided. Additionally, we group services together so you do not need to know the
            exact type of service required to search for a price. For example, a chest X-ray
            can be done using from one to four views. Since a user may not know how many views
            were ordered, we group all of these together under the general heading of "Chest
            X-ray."
        </p>
        <hr />
        <p class="q q-closed">
            There's a note at the bottom of the search results page that says that actual current
            prices may vary. Why would the actual price be different?
        </p>
        <p class="answer">
            There are several reasons why the actual price you pay may differ from the estimated
            price we show you. Our estimated prices are based on the most routine procedures
            that a physician orders or performs under normal circumstances. Sometimes, the actual
            procedure you need ends up more or less complicated than the most typical procedure,
            and therefore could cost more or less than our estimate. Another reason why your
            actual price might differ from our estimate is that our prices are based on what
            your health plan has paid a health care provider in the recent past. Occasionally,
            a provider and a health plan can change their contracted rates before that price
            is reflected in our database. As a result, we cannot guarantee that the estimated
            price we show you will be the price you will actually pay.
        </p>
        <hr />
        <p class="q q-closed">
            What if I want to see my spouse's or dependents' ClearCost Health information?
        </p>
        <p class="answer">
            For family members, partners, and dependents over age 18, you will not be able to
            see their information and they will not be able to see yours without explicit consent.
            While in general, parents have the right to see information on dependents who are
            younger than 18, all family members over the age of 18 have the option to withhold
            their medical history. To change your sharing settings, visit Other People on My
            Plan in My Account in the upper right-hand corner of the screen.
        </p>
        <hr />
        <p class="q q-closed">
            Does ClearCost Health have access to my claims data?
        </p>
        <p class="answer">
            Yes, we have access to your claims data in order to provide the personalized service
            that you will use . All data is guarded in a highly secure fashion, in compliance
            with government regulations.
        </p>
        <hr />
        <p class="q q-closed">
            How secure is my information?
        </p>
        <p class="answer">
            All information that we receive is guarded in a manner that is compliant with the
            Health Insurance Portability and Accountability Act of 1996 (HIPAA), which means
            that the information is stored as safely as it is with your health plan. We handle
            your Protected Health Information (PHI) in compliance with all government privacy
            regulations.
        </p>
        <hr />
        <p class="q q-closed">
            What if I don't want a phone call from ClearCost Health?
        </p>
        <p class="answer">
            If you do not want to receive phone calls from us, you can opt out of receiving
            them. In Notification Settings within My Account, you will see an option in the
            upper right-hand corner that allows you to opt out of phone calls. Keep in mind
            that doing so will limit the ways that we can help you save.
        </p>
        <hr />
        <p class="q q-closed">
            How can I contact ClearCost Health if I have any questions?
        </p>
        <p class="answer">
            Our phone number is 800-390-6855, and we are open from 6am-5pm PST on Monday through
            Friday.
            <br />
            Our email address is <a href="mailto:starbucks@clearcosthealth.com">starbucks@clearcosthealth.com</a>.
        </p>
    </div>
</asp:Content>
