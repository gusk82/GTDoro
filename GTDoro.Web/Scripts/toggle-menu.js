$(document).ready(function () {
    $(".menu li a").each(function () {
        if ($(this).next().length > 0) {
            $(this).addClass("parent");
        };
    })
    var menua = $('.nav-dropdown.menu li a.parent');
    $('<div class="more"><img src="/Content/img/greater-than-symbol.png" alt=""></div>').insertBefore(menua);
    var menub = $('nav .menu li a.parent');
    $('<div class="more"><img src="/Content/img/btn-hamburger.png" alt=""></div>').insertBefore(menub);
    $('nav .more').click(function () {
        $(this).parent('li').toggleClass('open');
    });
    $('.menu-btn').click(function () {
        $('nav').toggleClass('menu-open');
    });
});