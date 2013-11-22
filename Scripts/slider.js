function hexFromRGB(r, g, b) {
    var hex = [
      r.toString( 16 ),
      g.toString( 16 ),
      b.toString( 16 )
    ];
    $.each( hex, function( nr, val ) {
      if ( val.length === 1 ) {
        hex[ nr ] = "0" + val;
      }
    });
    return hex.join( "" ).toUpperCase();
  }
function refreshSwatch() {
    var red = $( "#medication_slider1" ).slider( "value" ),
      green = $( "#medication_slider2" ).slider( "value" ),
      blue = $( "#medication_slider3" ).slider( "value" ),
      hex = hexFromRGB( red, green, blue );
    $( "#swatch" ).css( "background-color", "#" + hex );
  }
  function updateSlider() {
    $(this).parent().next().children().first().val($(this).slider("value"));
  }
  function changeSliderHandlers() {
      var sliderOptions = $(this).slider("option");
      $(this).on("slidechange", updateSlider);
      $(this).on("slide", updateSlider);
      $(this).off("slidestart", changeSliderHandlers);
  }
