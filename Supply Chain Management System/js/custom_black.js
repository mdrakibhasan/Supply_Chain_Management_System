/*
	Global values
*/

var chartWidth = '650px';
var chartHeight = '240px';


/*
	Find element's Y axis position
*/

function findPosY(obj) 
{
	var curtop = 0;
	if (obj.offsetParent) 
	{
		while (1) 
		{
			curtop+=obj.offsetTop;
			if (!obj.offsetParent) 
			{
				break;
			}
			obj=obj.offsetParent;
		}
	} 
	else if (obj.y) 
	{
		curtop+=obj.y;
	}
		
	return curtop;
}

/*
	Find element's X axis position
*/

function findPosX(obj) 
{
	var curtop = 0;
	if (obj.offsetParent) 
	{
		while (1) 
		{
			curtop+=obj.offsetLeft;
			if (!obj.offsetParent) 
			{
				break;
			}
			obj=obj.offsetParent;
		}
	} 
	else if (obj.x) 
	{
		curtop+=obj.x;
	} 
	
	return curtop;
}

/*
	Setup chart from given table and type
*/

function setChart(tableId, type, wrapper)
{
	//clear existing chart before create new one
	$(wrapper).html('');

	$(tableId).visualize({
		type: type,
		width: chartWidth,
		height: chartHeight,
		colors: ['#B11623', '#292C37']
	}).appendTo(wrapper);
	
	//if IE then need to add refresh event
	if (navigator.appName == "Microsoft Internet Explorer")
	{
		$('.visualize').trigger('visualizeRefresh');
	}
}

/*
	Setup notification badges for shortcut
*/
function setNotifications()
{
	// Setup notification badges for shortcut
	$('#shortcut_notifications span').each(function() {
		if($(this).attr('rel') != '')
		{
			target = $(this).attr('rel');
			
			if($('#' +target).length > 0)
			{
				var Ypos = findPosY(document.getElementById(target));
				var Xpos = findPosX(document.getElementById(target));
				
				$(this).css('top', Ypos-24 +'px');
				$(this).css('left', Xpos+60 +'px');
			}
		}
	});
	$('#shortcut_notifications').css('display', 'block');
}

$(function () {

    // Preload images
    $.preloadCssImages();



    // Find all the input elements with title attributes and add hint to it
    $('input[title!=""]').hint();



    // Setup WYSIWYG editor
    $('#wysiwyg').wysiwyg({
        css: "css/wysiwyg.css"
    });


    // Setup show and hide left panel
    $('#hide_menu').click(function () {        
        $('#left_menu').hide();
        $('#show_menu').show();
        $('body').addClass('nobg');
        $('#content').css('marginLeft', 15);
        setNotifications();
    });

    $('#show_menu').click(function () {
        $('#left_menu').show();
        $(this).hide();
        $('body').removeClass('nobg');
        $('#content').css('marginLeft', 0);
        setNotifications();
    });

    $('.alert_warning').click(function () {
        $(this).fadeOut('fast');
    });

    $('.alert_info').click(function () {
        $(this).fadeOut('fast');
    });

    $('.alert_success').click(function () {
        $(this).fadeOut('fast');
    });

    $('.alert_error').click(function () {
        // $(this).fadeOut('fast');
    });

    // Setup modal window for all photos
    $('.media_photos li a[rel=slide]').fancybox({
        padding: 0,
        titlePosition: 'outside',
        overlayColor: '#333333',
        overlayOpacity: .2
    });




    // Setup charts example	

    // Chart bar type
    $('#chart_bar').click(function () {
        setChart('table#graph_data', 'bar', '#chart_wrapper');

        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');
    });


    // Chart area type
    $('#chart_area').click(function () {
        setChart('table#graph_data', 'area', '#chart_wrapper');

        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');
    });


    // Chart pie type
    $('#chart_pie').click(function () {
        setChart('table#graph_data', 'pie', '#chart_wrapper');

        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $
		(this).addClass('active');
    });


    // Chart line type
    $('#chart_line').click(function () {
        setChart('table#graph_data', 'line', '#chart_wrapper');

        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');
    });



    //make table editable, refresh charts on blur	
    $(function () {
        $('table#graph_data td')
			.click(function () {
			    if (!$(this).is('.input') && $(this).attr('class') != 'no_input hover') {
			        $(this).addClass('input')
						.html('<input type="text" value="' + $(this).text() + '" size="4" />')
						.find('input').focus()
						.blur(function () {
						    //remove td class, remove input
						    $(this).parent().removeClass('input').html($(this).val() || 0);
						    //update charts	
						    $('.visualize').trigger('visualizeRefresh');
						});
			    }
			})
			.hover(function () { $(this).addClass('hover'); }, function () { $(this).removeClass('hover'); });
    });



    // Setup left panel calendar
    $("#calendar").datepicker({
        nextText: '&raquo;',
        prevText: '&laquo;'
    });

    // Setup datepicker input
    $("#datepicker").datepicker({
        nextText: '&raquo;',
        prevText: '&laquo;',
        showAnim: 'slideDown'
    });



    // Setup minimize and maximize window
    $('.onecolumn .header span').click(function () {
        if ($(this).parent().parent().children('.content').css('display') == 'block') {
            $(this).css('cursor', 's-resize');
        }
        else {
            $(this).css('cursor', 'n-resize');
        }

        $(this).parent().parent().children('.content').slideToggle('fast');
    });

    $('.twocolumn .header span').click(function () {
        if ($(this).parent().parent().children('.content').css('display') == 'block') {
            $(this).css('cursor', 's-resize');
        }
        else {
            $(this).css('cursor', 'n-resize');
        }

        $(this).parent().parent().children('.content').slideToggle('fast');
    });

    $('.threecolumn .header span').click(function () {
        if ($(this).parent().parent().children('.content').css('display') == 'block') {
            $(this).css('cursor', 's-resize');
        }
        else {
            $(this).css('cursor', 'n-resize');
        }

        $(this).parent().children('.content').slideToggle('fast');
    });



    // Check or uncheck all checkboxes
    $('#check_all').click(function () {
        if ($(this).is(':checked')) {
            $('form#form_data input:checkbox').attr('checked', true);
        }
        else {
            $('form#form_data input:checkbox').attr('checked', false);
        }
    });



    // Setup notification badges for shortcut
    setNotifications();



    // Setup modal window link


    // Add tooltip to shortcut
    $('#shortcut li a').tipsy({ gravity: 's' });

    $('#btn_modal').fancybox({
        padding: 0,
        titleShow: false,
        overlayColor: '#333333',
        overlayOpacity: .5,
        href: 'modal_window.html'
    });



    // Setup tab contents

    // tab 1
    $('#tab1').click(function () {
        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');

        //show tab1 content
        $('.tab_content').addClass('hide');
        $('#tab1_content').removeClass('hide');
    });


    // tab 2
    $('#tab2').click(function () {
        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');

        //show tab2 content
        $('.tab_content').addClass('hide');
        $('#tab2_content').removeClass('hide');
    });
    $('#tab21').click(function () {
        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');

        //show tab2 content
        $('.tab_content').addClass('hide');
        $('#tab21_content').removeClass('hide');
    });
    $('#tab22').click(function () {
        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');

        //show tab2 content
        $('.tab_content').addClass('hide');
        $('#tab22_content').removeClass('hide');
    });
    $('#tab23').click(function () {
        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');

        //show tab2 content
        $('.tab_content').addClass('hide');
        $('#tab23_content').removeClass('hide');
    });
    $('#tab24').click(function () {
        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');

        //show tab2 content
        $('.tab_content').addClass('hide');
        $('#tab24_content').removeClass('hide');
    });
    $('#tab26').click(function () {
        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');

        //show tab2 content
        $('.tab_content').addClass('hide');
        $('#tab26_content').removeClass('hide');
    });
    $('#tab20').click(function () {
        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');

        //show tab2 content
        $('.tab_content').addClass('hide');
        $('#tab20_content').removeClass('hide');
    });

    // tab 3
    $('#tab3').click(function () {
        //switch menu
        $(this).parent().parent().find('td input').removeClass('active');
        $(this).addClass('active');

        //show tab3 content
        $('.tab_content').addClass('hide');
        $('#tab3_content').removeClass('hide');
    });



    // Add tooltip to edit and delete button
    $('.help').tipsy({ gravity: 's' });


    // Setup sortable to threecolumn div
    $("#threecolumn").sortable({
        opacity: 0.6,
        connectWith: '.threecolumn_each',
        items: 'div.threecolumn_each'
    });


});
function initMenu(mno, mnoc) {
    $('#main_menu ul').hide();
    $('#main_menu ul:eq(' + mno + ')').show();
    $('#main_menu li ul li a:eq(' + mnoc + ')').css("color", "black");
    $('#main_menu li a').click(
    function () {
        var checkElement = $(this).next();
        if ((checkElement.is('ul')) && (checkElement.is(':visible'))) {
            $('#main_menu ul:visible').slideUp('normal');
            return false;
        }
        if ((checkElement.is('ul')) && (!checkElement.is(':visible'))) {
            $('#main_menu ul:visible').slideUp('normal');
            checkElement.slideDown('normal');
            return false;
        }
    }
    );
}
(function ($) {
    $.QueryString = (function (a) {
        if (a == "") return {};
        var b = {};
        for (var i = 0; i < a.length; ++i) {
            var p = a[i].split('=');
            if (p.length != 2) continue;
            b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
        }
        return b;
    })(window.location.search.substr(1).split('?'))
})(jQuery);

$(document).ready(function () {
    //need to set timeout 0.5 sec for cross browser rendering
    setTimeout('setChart(\'table#graph_data_no_edit\', \'bar\', \'#chart_wrapper\')', 500);

    var a = $.QueryString["mno"];
    var b = ('' + a).split(".", 2);
    var d = b[1];
    a = b[0];
    if (a == null) {
        a = 10;
    }
    else {
        a = parseFloat(a);
    }
    if (d == null) {
        d = 50;
    }
    else {
        d = parseFloat(d);
    }
    initMenu(a, d);


    $('#main_menu').find('li a').click(function () {
        if ($(this).attr('href').length > 0) {
            location.href = $(this).attr('href');
        }
    });


});
