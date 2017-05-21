$(document).ready(function () {
    $(".collect").colorbox({ scrolling: false, width: "100%", maxWidth: "650px" });
    $(".select").colorbox({ width: "100%", maxWidth: "900px", height: "100%" });
    renderWorkingPanel();
    startRefreshWorkingPanelInterval();
});

function startRefreshWorkingPanelInterval() {
    setInterval(function () {
        if (sessionStorage.getItem("currentIntervalID")) {
            clearInterval(sessionStorage.getItem("currentIntervalID"));
        }
        renderWorkingPanel();
    }, 5 * 60 * 1000);
}

function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
}

$.urlParam = function (name) {
    var results = new RegExp('[\\?&amp;]' + name + '=([^&amp;#]*)').exec(window.location.href);
    return (results != undefined && results.length > 0 && results[1]) || 0;
}
if ($.urlParam('collecting') == 1) {
    $.colorbox({ scrolling: false, width: "100%", maxWidth: "650px", href: "/CollectedThing/CreateLight" });
}
function jump(h){
    var url = location.href;               //Save down the URL without hash.
    location.href = "#"+h;                 //Go to the target element.
    history.replaceState(null,null,url);   //Don't like hashes. Changing it back.
}
function selectChildrenTab() {
    $('#bottom .nav-tabs li').removeClass('active');
    $($('#bottom .nav-tabs li')[0]).addClass('active');
}

function renderWorkingPanel() {
    var workingPanel = $("#working-panel");
    $('#working-panel').html("<img src='/Content/img/indicator.white.gif' /> Loading...")

    // remove time from window title
    var regex = /^\(\d{2,}:\d{2}\)/
    document.title = document.title.replace(regex, "");

    workingPanel.load();
    $.ajax({
        url: workingPanel.data("url"),
        cache: false,
        dataType: "html",
        success: function (data) {
            $('#working-panel').html(data);
        }
    });
}

function renderPartialViews() {
    $(".partial-content").each(function (index, item) {
        var url = $(item).data("url");
        if (url && url.length > 0) {
            $(item).load(url);
        }
    })
}

function sortSelectListByValue(selectElements) {
    var selectList;
    var selectedItem;
    $(selectElements).each(function () {
        selectedItem = $(this).val();
        selectList = $(this).find('option').sort(function (a, b) {
            a = a.value;
            b = b.value;
            return a - b;
        });
        $(this).html(selectList);
        $(this).val(selectedItem);
    })
}

function loadTab(name) {
    if (name) {
        $('.nav-tabs a[href="#tab-' + name + '"]').tab('show');
    }
}