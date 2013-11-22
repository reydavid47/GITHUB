function starRating(iData) {
    /*
    iData Structure
    iData.action
    iData.callback
    */

    var _getRatingText = function (iData) {
        /*
        iData Structure
        iData.position
        */
        var ratingText = {
            0: 'Please rate',
            1: 'Very Dissatisfied',
            2: 'Dissatisfied',
            3: 'Satisfied',
            4: 'Very Satisfied',
            5: 'Extremely Satisfied'
        };
        return ratingText[iData.position];
    };

    var _getPosition = function (iData) {
        /*
        iData Structure
        iData.position
        iData.starClass
        */
        /* -- position setup : [default, mini] -- */
        var positions = {
            0: ['-212px', '-212px'],
            1: ['-173px', '-122px'],
            2: ['-129px', '-91px'],
            3: ['-86px', '-61px'],
            4: ['-43px', '-30px'],
            5: ['0px', '0px']
        };
        function positionByClass() {
            if (iData.starClass != undefined) {
                /* -- checks for miniStar -- */
                if (iData.starClass.indexOf('miniStar') > -1) {
                    /* -- returns mini position -- */
                    return positions[iData.position][1];
                }
                else {
                    /* -- returns standard position -- */
                    return positions[iData.position][0];
                }
            }
            else {
                /* -- returns standard position -- */
                return positions[iData.position][0];
            }
        };
        return positionByClass();

    };

    var _showHoverRating = function (iData) {
        /*
        iData Structure
        iData.element
        iData.ratingValue
        */
        /* -- positions hover effect -- */
        $(iData.element).closest('.starWrapper').children(':first-child').css('left', _getPosition({ position: iData.ratingValue, starClass:$(iData.element).closest('.starWrapper').parent().attr('class') }));
        /* -- changes rating text -- */
        $(iData.element).closest('.starWrapper').find('.starsText').html(_getRatingText({ position: iData.ratingValue }));
    };

    var _showClickRating = function (iData) {
        /*
        iData Structure
        iData.element
        iData.ratingValue
        */
        var userSelectedClass = 'userSelectedStar';
        /* -- restores no selected state -- */
        $(iData.element).parent().children().each(function () {
            $(this).removeClass(userSelectedClass);
        });
        /* -- adds selected class for tracking -- */
        $(iData.element).addClass(userSelectedClass);
        /* -- positions click effect -- */
        $(iData.element).closest('.starWrapper').children(':nth-child(2)').css('left', _getPosition({ position: iData.ratingValue, starClass: $(iData.element).closest('.starWrapper').parent().attr('class') }));
        /* -- changes rating text -- */
        $(iData.element).closest('.starWrapper').find('.starsText').attr('selectedText', _getRatingText({ position: iData.ratingValue }));
        function resetValidation() {

        };
        if ($(iData.element).closest('.starWrapper').hasClass('noStarRating')) {
            $(iData.element).closest('.starWrapper').removeClass('noStarRating');
        }
    };

    var _resetRating = function (iData) {
        /*
        iData Structure
        iData.element
        iData.ratingValue
        */
        var userSelectedClass = 'userSelectedStar';
        /* -- restores no selected state -- */
        $(iData.element).parent().children().each(function () {
            $(this).removeClass(userSelectedClass);
        });
        /* -- restores hover effect : default -- */
        $(iData.element).closest('.starWrapper').children(':first-child').css('left', _getPosition({ position: iData.ratingValue, starClass: $(iData.element).closest('.starWrapper').parent().attr('class') }));
        /* -- restores click effect : default -- */
        $(iData.element).closest('.starWrapper').children(':nth-child(2)').css('left', _getPosition({ position: iData.ratingValue, starClass: $(iData.element).closest('.starWrapper').parent().attr('class') }));
        /* -- restores rating text : default -- */
        $(iData.element).closest('.starWrapper').find('.starsText').attr('selectedText', _getRatingText({ position: iData.ratingValue }));
        /* -- restores text : default */
        _showHoverRating({
            element: $(iData.element),
            ratingValue: 0
        });
    };

    var _showOriginalRating = function () {
        $('.starWrapper').each(function () {
            var _t, existingRating;
            _t = this;
            existingRating = '';
            existingRating = $(this).parent().attr('existingrating');
            if (existingRating != '' && existingRating != undefined) {
                _showHoverRating({
                    element: $(_t).find('ul li:nth-child(' + existingRating + ')'),
                    ratingValue: parseInt(existingRating)
                });
                _showClickRating({
                    element: $(_t).find('ul li:nth-child(' + existingRating + ')'),
                    ratingValue: parseInt(existingRating)
                });                
            }
        });
    };

    var _getCurrentRating = function (iData) {
        /*
        iData Structure
        iData.element
        */
        return ($(iData.element).parent().children('.userSelectedStar').index() + 1);
    };

    switch (iData.action) {
        case 'init':
            var star, ratingSelected, starsContainer, isInactiveStar;
            star = '.starWrapper li';
            starsContainer = '.starWrapper ul';
            $(star).each(function () {

                /* -- check for active/inactive state -- */
                isInactiveStar = $(this).closest('.userRatings').attr('class');
                if (isInactiveStar.indexOf('inactiveStar') > 0) {
                    return;
                }

                $(this).unbind('hover');
                $(this).hover(
                    function () {
                        ratingSelected = $(this).index() + 1;
                        _showHoverRating({
                            element: $(this),
                            ratingValue: ratingSelected
                        });
                    },
                    function () {
                        ratingSelected = $(this).index() + 1;
                        _showHoverRating({
                            element: $(this),
                            ratingValue: ratingSelected
                        });
                    }
                );

                $(this).unbind('click');
                $(this).on('click', function () {
                    ratingSelected = $(this).index() + 1;
                    _showClickRating({
                        element: $(this),
                        ratingValue: ratingSelected
                    });
                });
            });

            $(starsContainer).each(function () {
                $(this).unbind('hover');
                $(this).hover(
                    function () { },
                    function () {
                        if ($(this).children('.userSelectedStar').length > 0) {
                            _showHoverRating({
                                element: $(this).children('.userSelectedStar'),
                                ratingValue: $(this).children('.userSelectedStar').index() + 1
                            });
                        } else {
                            _showHoverRating({
                                element: $(this).children(':first-child'),
                                ratingValue: 0
                            });
                        }
                    }
                );
            });
            if (iData.callback != undefined) {
                iData.callback();
            }
            break;

        case 'inactive':
            var inactiveStar = '.inactiveStar';
            $(inactiveStar).each(function () {
                $(this).find('.starsText').remove();
            });
            _showOriginalRating();
            break;

        case 'restore':
            _resetRating({
                element: $(iData.element),
                ratingValue: 0
            });
            break;
        case 'showOriginalRating':
            _showOriginalRating();
            break;
        case 'getCurrentRating':
            return _getCurrentRating(iData);
            break;
    }
};