$(document).ready(function () {

            $('a#tabcare').click(function () { // bind click event to link
                $("#tabs").tabs('select', 0); // switch to first tab
                return false;
            });
            $('a#tabrx').click(function () { // bind click event to link
                $("#tabs").tabs('select', 1); // switch to second tab
                return false;
            });
            $('a#tabdoc').click(function () { // bind click event to link
                $("#tabs").tabs('select', 2); // switch to second tab
                return false;
            });
            $('a#tabpast').click(function () { // bind click event to link
                $("#tabs").tabs('select', 3); // switch to third tab
                return false;
            });
            
            $("#txtSearch").autocomplete({
                source: function (request, response) {
                    PageMethods.Autocomplete(request.term, function (result) {
                        var tempData = result.split('|');
                        response(tempData);
                    });
                },
                minLength: 3
            });

            /* for the letter-based prescription nav */
            //$(".letternav").bind('click', letternav);

            // see http://stackoverflow.com/questions/3199130/updatepanel-breaks-jquery-script
            // and http://api.jquery.com/live/ and http://api.jquery.com/delegates/
            // for more info on how to handle jquery bindings with partial postbacks. 
            // Review the deprecation section for live() implementation! 
            $(document).delegate(".letternav", "click", letternav);

        });

        function letternav() {
            $("div.letterbox").hide();
            $("a.letternav").removeClass("callout");
            var letter = $(this).attr("id");
            $("div#letter" + letter).show();
            $("a#" + letter).addClass("callout");
        }
}