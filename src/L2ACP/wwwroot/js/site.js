/*
 *
 *   INSPINIA - Responsive Admin Theme
 *   version 2.7
 *
 */

$(document).ready(function () {


    // Add body-small class if window less than 768px
    if ($(this).width() < 769) {
        $('body').addClass('body-small')
    } else {
        $('body').removeClass('body-small')
    }

    // MetisMenu
    $('#side-menu').metisMenu();

    // Collapse ibox function
    $('.collapse-link').on('click', function () {
        var ibox = $(this).closest('div.ibox');
        var button = $(this).find('i');
        var content = ibox.children('.ibox-content');
        content.slideToggle(200);
        button.toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');
        ibox.toggleClass('').toggleClass('border-bottom');
        setTimeout(function () {
            ibox.resize();
            ibox.find('[id^=map-]').resize();
        }, 50);
    });

    // Close ibox function
    $('.close-link').on('click', function () {
        var content = $(this).closest('div.ibox');
        content.remove();
    });

    // Fullscreen ibox function
    $('.fullscreen-link').on('click', function () {
        var ibox = $(this).closest('div.ibox');
        var button = $(this).find('i');
        $('body').toggleClass('fullscreen-ibox-mode');
        button.toggleClass('fa-expand').toggleClass('fa-compress');
        ibox.toggleClass('fullscreen');
        setTimeout(function () {
            $(window).trigger('resize');
        }, 100);
    });

    // Close menu in canvas mode
    $('.close-canvas-menu').on('click', function () {
        $("body").toggleClass("mini-navbar");
        SmoothlyMenu();
    });

    // Run menu of canvas
    $('body.canvas-menu .sidebar-collapse').slimScroll({
        height: '100%',
        railOpacity: 0.9
    });

    // Open close right sidebar
    $('.right-sidebar-toggle').on('click', function () {
        $('#right-sidebar').toggleClass('sidebar-open');
    });

    // Initialize slimscroll for right sidebar
    $('.sidebar-container').slimScroll({
        height: '100%',
        railOpacity: 0.4,
        wheelStep: 10
    });

    // Open close small chat
    $('.open-small-chat').on('click', function () {
        $(this).children().toggleClass('fa-comments').toggleClass('fa-remove');
        $('.small-chat-box').toggleClass('active');
    });

    // Initialize slimscroll for small chat
    $('.small-chat-box .content').slimScroll({
        height: '234px',
        railOpacity: 0.4
    });

    // Small todo handler
    $('.check-link').on('click', function () {
        var button = $(this).find('i');
        var label = $(this).next('span');
        button.toggleClass('fa-check-square').toggleClass('fa-square-o');
        label.toggleClass('todo-completed');
        return false;
    });

    // Append config box / Only for demo purpose
    // Uncomment on server mode to enable XHR calls
    //$.get("skin-config.html", function (data) {
    //    if (!$('body').hasClass('no-skin-config'))
    //        $('body').append(data);
    //});

    // Minimalize menu
    $('.navbar-minimalize').on('click', function () {
        $("body").toggleClass("mini-navbar");
        SmoothlyMenu();

    });

    // Tooltips demo
    $('.tooltip-demo').tooltip({
        selector: "[data-toggle=tooltip]",
        container: "body"
    });


    // Full height of sidebar
    function fix_height() {
        var heightWithoutNavbar = $("body > #wrapper").height() - 61;
        $(".sidebar-panel").css("min-height", heightWithoutNavbar + "px");

        var navbarheight = $('nav.navbar-default').height();
        var wrapperHeight = $('#page-wrapper').height();

        if (navbarheight > wrapperHeight) {
            $('#page-wrapper').css("min-height", navbarheight + "px");
        }

        if (navbarheight < wrapperHeight) {
            $('#page-wrapper').css("min-height", $(window).height() + "px");
        }

        if ($('body').hasClass('fixed-nav')) {
            if (navbarheight > wrapperHeight) {
                $('#page-wrapper').css("min-height", navbarheight + "px");
            } else {
                $('#page-wrapper').css("min-height", $(window).height() - 60 + "px");
            }
        }

    }

    fix_height();

    // Fixed Sidebar
    $(window).bind("load", function () {
        if ($("body").hasClass('fixed-sidebar')) {
            $('.sidebar-collapse').slimScroll({
                height: '100%',
                railOpacity: 0.9
            });
        }
    });

    // Move right sidebar top after scroll
    $(window).scroll(function () {
        if ($(window).scrollTop() > 0 && !$('body').hasClass('fixed-nav')) {
            $('#right-sidebar').addClass('sidebar-top');
        } else {
            $('#right-sidebar').removeClass('sidebar-top');
        }
    });

    $(window).bind("load resize scroll", function () {
        if (!$("body").hasClass('body-small')) {
            fix_height();
        }
    });

    $("[data-toggle=popover]")
        .popover();

    // Add slimscroll to element
    $('.full-height-scroll').slimscroll({
        height: '100%'
    })
});


// Minimalize menu when screen is less than 768px
$(window).bind("resize", function () {
    if ($(this).width() < 769) {
        $('body').addClass('body-small')
    } else {
        $('body').removeClass('body-small')
    }
});

// Local Storage functions
// Set proper body class and plugins based on user configuration
$(document).ready(function () {
    if (localStorageSupport()) {

        var collapse = localStorage.getItem("collapse_menu");
        var fixedsidebar = localStorage.getItem("fixedsidebar");
        var fixednavbar = localStorage.getItem("fixednavbar");
        var boxedlayout = localStorage.getItem("boxedlayout");
        var fixedfooter = localStorage.getItem("fixedfooter");

        var body = $('body');

        if (fixedsidebar == 'on') {
            body.addClass('fixed-sidebar');
            $('.sidebar-collapse').slimScroll({
                height: '100%',
                railOpacity: 0.9
            });
        }

        if (collapse == 'on') {
            if (body.hasClass('fixed-sidebar')) {
                if (!body.hasClass('body-small')) {
                    body.addClass('mini-navbar');
                }
            } else {
                if (!body.hasClass('body-small')) {
                    body.addClass('mini-navbar');
                }

            }
        }

        if (fixednavbar == 'on') {
            $(".navbar-static-top").removeClass('navbar-static-top').addClass('navbar-fixed-top');
            body.addClass('fixed-nav');
        }

        if (boxedlayout == 'on') {
            body.addClass('boxed-layout');
        }

        if (fixedfooter == 'on') {
            $(".footer").addClass('fixed');
        }
    }
});

// check if browser support HTML5 local storage
function localStorageSupport() {
    return (('localStorage' in window) && window['localStorage'] !== null)
}

// For demo purpose - animation css script
function animationHover(element, animation) {
    element = $(element);
    element.hover(
        function () {
            element.addClass('animated ' + animation);
        },
        function () {
            //wait for animation to finish before removing classes
            window.setTimeout(function () {
                element.removeClass('animated ' + animation);
            }, 2000);
        });
}

function SmoothlyMenu() {
    if (!$('body').hasClass('mini-navbar') || $('body').hasClass('body-small')) {
        // Hide menu in order to smoothly turn on when maximize menu
        $('#side-menu').hide();
        // For smoothly turn on menu
        setTimeout(
            function () {
                $('#side-menu').fadeIn(400);
            }, 200);
    } else if ($('body').hasClass('fixed-sidebar')) {
        $('#side-menu').hide();
        setTimeout(
            function () {
                $('#side-menu').fadeIn(400);
            }, 100);
    } else {
        // Remove all inline style from jquery fadeIn function to reset menu state
        $('#side-menu').removeAttr('style');
    }
}

// Dragable panels
function WinMove() {
    var element = "[class*=col]";
    var handle = ".ibox-title";
    var connect = "[class*=col]";
    $(element).sortable(
        {
            handle: handle,
            connectWith: connect,
            tolerance: 'pointer',
            forcePlaceholderSize: true,
            opacity: 0.8
        })
        .disableSelection();
}

function numberWithDots(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

function removeDots(s) {
    return s.replace(/[.]/g, '');
}

/*
 * metismenu - v2.0.2
 * A jQuery menu plugin
 * https://github.com/onokumus/metisMenu
 *
 * Made by Osman Nuri Okumus
 * Under MIT License
 */

!function (a) { "use strict"; function b() { var a = document.createElement("mm"), b = { WebkitTransition: "webkitTransitionEnd", MozTransition: "transitionend", OTransition: "oTransitionEnd otransitionend", transition: "transitionend" }; for (var c in b) if (void 0 !== a.style[c]) return { end: b[c] }; return !1 } function c(b) { return this.each(function () { var c = a(this), d = c.data("mm"), f = a.extend({}, e.DEFAULTS, c.data(), "object" == typeof b && b); d || c.data("mm", d = new e(this, f)), "string" == typeof b && d[b]() }) } a.fn.emulateTransitionEnd = function (b) { var c = !1, e = this; a(this).one("mmTransitionEnd", function () { c = !0 }); var f = function () { c || a(e).trigger(d.end) }; return setTimeout(f, b), this }; var d = b(); d && (a.event.special.mmTransitionEnd = { bindType: d.end, delegateType: d.end, handle: function (b) { return a(b.target).is(this) ? b.handleObj.handler.apply(this, arguments) : void 0 } }); var e = function (b, c) { this.$element = a(b), this.options = a.extend({}, e.DEFAULTS, c), this.transitioning = null, this.init() }; e.TRANSITION_DURATION = 350, e.DEFAULTS = { toggle: !0, doubleTapToGo: !1, activeClass: "active" }, e.prototype.init = function () { var b = this, c = this.options.activeClass; this.$element.find("li." + c).has("ul").children("ul").addClass("collapse in"), this.$element.find("li").not("." + c).has("ul").children("ul").addClass("collapse"), this.options.doubleTapToGo && this.$element.find("li." + c).has("ul").children("a").addClass("doubleTapToGo"), this.$element.find("li").has("ul").children("a").on("click.metisMenu", function (d) { var e = a(this), f = e.parent("li"), g = f.children("ul"); return d.preventDefault(), f.hasClass(c) ? b.hide(g) : b.show(g), b.options.doubleTapToGo && b.doubleTapToGo(e) && "#" !== e.attr("href") && "" !== e.attr("href") ? (d.stopPropagation(), void (document.location = e.attr("href"))) : void 0 }) }, e.prototype.doubleTapToGo = function (a) { var b = this.$element; return a.hasClass("doubleTapToGo") ? (a.removeClass("doubleTapToGo"), !0) : a.parent().children("ul").length ? (b.find(".doubleTapToGo").removeClass("doubleTapToGo"), a.addClass("doubleTapToGo"), !1) : void 0 }, e.prototype.show = function (b) { var c = this.options.activeClass, f = a(b), g = f.parent("li"); if (!this.transitioning && !f.hasClass("in")) { g.addClass(c), this.options.toggle && this.hide(g.siblings().children("ul.in")), f.removeClass("collapse").addClass("collapsing").height(0), this.transitioning = 1; var h = function () { f.removeClass("collapsing").addClass("collapse in").height(""), this.transitioning = 0 }; return d ? void f.one("mmTransitionEnd", a.proxy(h, this)).emulateTransitionEnd(e.TRANSITION_DURATION).height(f[0].scrollHeight) : h.call(this) } }, e.prototype.hide = function (b) { var c = this.options.activeClass, f = a(b); if (!this.transitioning && f.hasClass("in")) { f.parent("li").removeClass(c), f.height(f.height())[0].offsetHeight, f.addClass("collapsing").removeClass("collapse").removeClass("in"), this.transitioning = 1; var g = function () { this.transitioning = 0, f.removeClass("collapsing").addClass("collapse") }; return d ? void f.height(0).one("mmTransitionEnd", a.proxy(g, this)).emulateTransitionEnd(e.TRANSITION_DURATION) : g.call(this) } }; var f = a.fn.metisMenu; a.fn.metisMenu = c, a.fn.metisMenu.Constructor = e, a.fn.metisMenu.noConflict = function () { return a.fn.metisMenu = f, this } }(jQuery);


/*! Copyright (c) 2011 Piotr Rochala (http://rocha.la)
 * Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
 * and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
 *
 * Version: 1.3.6
 *
 */
(function (e) {
    e.fn.extend({
        slimScroll: function (g) {
            var a = e.extend({ width: "auto", height: "250px", size: "7px", color: "#000", position: "right", distance: "1px", start: "top", opacity: .4, alwaysVisible: !1, disableFadeOut: !1, railVisible: !1, railColor: "#333", railOpacity: .2, railDraggable: !0, railClass: "slimScrollRail", barClass: "slimScrollBar", wrapperClass: "slimScrollDiv", allowPageScroll: !1, wheelStep: 20, touchScrollStep: 200, borderRadius: "7px", railBorderRadius: "7px" }, g); this.each(function () {
                function v(d) {
                    if (r) {
                        d = d || window.event;
                        var c = 0; d.wheelDelta && (c = -d.wheelDelta / 120); d.detail && (c = d.detail / 3); e(d.target || d.srcTarget || d.srcElement).closest("." + a.wrapperClass).is(b.parent()) && m(c, !0); d.preventDefault && !k && d.preventDefault(); k || (d.returnValue = !1)
                    }
                } function m(d, e, g) {
                    k = !1; var f = d, h = b.outerHeight() - c.outerHeight(); e && (f = parseInt(c.css("top")) + d * parseInt(a.wheelStep) / 100 * c.outerHeight(), f = Math.min(Math.max(f, 0), h), f = 0 < d ? Math.ceil(f) : Math.floor(f), c.css({ top: f + "px" })); l = parseInt(c.css("top")) / (b.outerHeight() - c.outerHeight());
                    f = l * (b[0].scrollHeight - b.outerHeight()); g && (f = d, d = f / b[0].scrollHeight * b.outerHeight(), d = Math.min(Math.max(d, 0), h), c.css({ top: d + "px" })); b.scrollTop(f); b.trigger("slimscrolling", ~~f); w(); p()
                } function x() { u = Math.max(b.outerHeight() / b[0].scrollHeight * b.outerHeight(), 30); c.css({ height: u + "px" }); var a = u == b.outerHeight() ? "none" : "block"; c.css({ display: a }) } function w() {
                    x(); clearTimeout(B); l == ~~l ? (k = a.allowPageScroll, C != l && b.trigger("slimscroll", 0 == ~~l ? "top" : "bottom")) : k = !1; C = l; u >= b.outerHeight() ? k = !0 : (c.stop(!0,
                    !0).fadeIn("fast"), a.railVisible && h.stop(!0, !0).fadeIn("fast"))
                } function p() { a.alwaysVisible || (B = setTimeout(function () { a.disableFadeOut && r || y || z || (c.fadeOut("slow"), h.fadeOut("slow")) }, 1E3)) } var r, y, z, B, A, u, l, C, k = !1, b = e(this); if (b.parent().hasClass(a.wrapperClass)) {
                    var n = b.scrollTop(), c = b.closest("." + a.barClass), h = b.closest("." + a.railClass); x(); if (e.isPlainObject(g)) {
                        if ("height" in g && "auto" == g.height) {
                            b.parent().css("height", "auto"); b.css("height", "auto"); var q = b.parent().parent().height(); b.parent().css("height",
                            q); b.css("height", q)
                        } if ("scrollTo" in g) n = parseInt(a.scrollTo); else if ("scrollBy" in g) n += parseInt(a.scrollBy); else if ("destroy" in g) { c.remove(); h.remove(); b.unwrap(); return } m(n, !1, !0)
                    }
                } else if (!(e.isPlainObject(g) && "destroy" in g)) {
                    a.height = "auto" == a.height ? b.parent().height() : a.height; n = e("<div></div>").addClass(a.wrapperClass).css({ position: "relative", overflow: "hidden", width: a.width, height: a.height }); b.css({ overflow: "hidden", width: a.width, height: a.height }); var h = e("<div></div>").addClass(a.railClass).css({
                        width: a.size,
                        height: "100%", position: "absolute", top: 0, display: a.alwaysVisible && a.railVisible ? "block" : "none", "border-radius": a.railBorderRadius, background: a.railColor, opacity: a.railOpacity, zIndex: 90
                    }), c = e("<div></div>").addClass(a.barClass).css({ background: a.color, width: a.size, position: "absolute", top: 0, opacity: a.opacity, display: a.alwaysVisible ? "block" : "none", "border-radius": a.borderRadius, BorderRadius: a.borderRadius, MozBorderRadius: a.borderRadius, WebkitBorderRadius: a.borderRadius, zIndex: 99 }), q = "right" == a.position ?
{ right: a.distance } : { left: a.distance }; h.css(q); c.css(q); b.wrap(n); b.parent().append(c); b.parent().append(h); a.railDraggable && c.bind("mousedown", function (a) { var b = e(document); z = !0; t = parseFloat(c.css("top")); pageY = a.pageY; b.bind("mousemove.slimscroll", function (a) { currTop = t + a.pageY - pageY; c.css("top", currTop); m(0, c.position().top, !1) }); b.bind("mouseup.slimscroll", function (a) { z = !1; p(); b.unbind(".slimscroll") }); return !1 }).bind("selectstart.slimscroll", function (a) { a.stopPropagation(); a.preventDefault(); return !1 });
                    h.hover(function () { w() }, function () { p() }); c.hover(function () { y = !0 }, function () { y = !1 }); b.hover(function () { r = !0; w(); p() }, function () { r = !1; p() }); b.bind("touchstart", function (a, b) { a.originalEvent.touches.length && (A = a.originalEvent.touches[0].pageY) }); b.bind("touchmove", function (b) { k || b.originalEvent.preventDefault(); b.originalEvent.touches.length && (m((A - b.originalEvent.touches[0].pageY) / a.touchScrollStep, !0), A = b.originalEvent.touches[0].pageY) }); x(); "bottom" === a.start ? (c.css({ top: b.outerHeight() - c.outerHeight() }),
                        m(0, !0)) : "top" !== a.start && (m(e(a.start).position().top, null, !0), a.alwaysVisible || c.hide()); window.addEventListener ? (this.addEventListener("DOMMouseScroll", v, !1), this.addEventListener("mousewheel", v, !1)) : document.attachEvent("onmousewheel", v)
                }
            }); return this
        }
    }); e.fn.extend({ slimscroll: e.fn.slimScroll })
})(jQuery);


!function (e) { e(["jquery"], function (e) { return function () { function t(e, t, n) { return f({ type: O.error, iconClass: g().iconClasses.error, message: e, optionsOverride: n, title: t }) } function n(t, n) { return t || (t = g()), v = e("#" + t.containerId), v.length ? v : (n && (v = c(t)), v) } function i(e, t, n) { return f({ type: O.info, iconClass: g().iconClasses.info, message: e, optionsOverride: n, title: t }) } function o(e) { w = e } function s(e, t, n) { return f({ type: O.success, iconClass: g().iconClasses.success, message: e, optionsOverride: n, title: t }) } function a(e, t, n) { return f({ type: O.warning, iconClass: g().iconClasses.warning, message: e, optionsOverride: n, title: t }) } function r(e) { var t = g(); v || n(t), l(e, t) || u(t) } function d(t) { var i = g(); return v || n(i), t && 0 === e(":focus", t).length ? void h(t) : void (v.children().length && v.remove()) } function u(t) { for (var n = v.children(), i = n.length - 1; i >= 0; i--) l(e(n[i]), t) } function l(t, n) { return t && 0 === e(":focus", t).length ? (t[n.hideMethod]({ duration: n.hideDuration, easing: n.hideEasing, complete: function () { h(t) } }), !0) : !1 } function c(t) { return v = e("<div/>").attr("id", t.containerId).addClass(t.positionClass).attr("aria-live", "polite").attr("role", "alert"), v.appendTo(e(t.target)), v } function p() { return { tapToDismiss: !0, toastClass: "toast", containerId: "toast-container", debug: !1, showMethod: "fadeIn", showDuration: 300, showEasing: "swing", onShown: void 0, hideMethod: "fadeOut", hideDuration: 1e3, hideEasing: "swing", onHidden: void 0, extendedTimeOut: 1e3, iconClasses: { error: "toast-error", info: "toast-info", success: "toast-success", warning: "toast-warning" }, iconClass: "toast-info", positionClass: "toast-top-right", timeOut: 5e3, titleClass: "toast-title", messageClass: "toast-message", target: "body", closeHtml: '<button type="button">&times;</button>', newestOnTop: !0, preventDuplicates: !1, progressBar: !1 } } function m(e) { w && w(e) } function f(t) { function i(t) { return !e(":focus", l).length || t ? (clearTimeout(O.intervalId), l[r.hideMethod]({ duration: r.hideDuration, easing: r.hideEasing, complete: function () { h(l), r.onHidden && "hidden" !== b.state && r.onHidden(), b.state = "hidden", b.endTime = new Date, m(b) } })) : void 0 } function o() { (r.timeOut > 0 || r.extendedTimeOut > 0) && (u = setTimeout(i, r.extendedTimeOut), O.maxHideTime = parseFloat(r.extendedTimeOut), O.hideEta = (new Date).getTime() + O.maxHideTime) } function s() { clearTimeout(u), O.hideEta = 0, l.stop(!0, !0)[r.showMethod]({ duration: r.showDuration, easing: r.showEasing }) } function a() { var e = (O.hideEta - (new Date).getTime()) / O.maxHideTime * 100; f.width(e + "%") } var r = g(), d = t.iconClass || r.iconClass; if ("undefined" != typeof t.optionsOverride && (r = e.extend(r, t.optionsOverride), d = t.optionsOverride.iconClass || d), r.preventDuplicates) { if (t.message === C) return; C = t.message } T++, v = n(r, !0); var u = null, l = e("<div/>"), c = e("<div/>"), p = e("<div/>"), f = e("<div/>"), w = e(r.closeHtml), O = { intervalId: null, hideEta: null, maxHideTime: null }, b = { toastId: T, state: "visible", startTime: new Date, options: r, map: t }; return t.iconClass && l.addClass(r.toastClass).addClass(d), t.title && (c.append(t.title).addClass(r.titleClass), l.append(c)), t.message && (p.append(t.message).addClass(r.messageClass), l.append(p)), r.closeButton && (w.addClass("toast-close-button").attr("role", "button"), l.prepend(w)), r.progressBar && (f.addClass("toast-progress"), l.prepend(f)), l.hide(), r.newestOnTop ? v.prepend(l) : v.append(l), l[r.showMethod]({ duration: r.showDuration, easing: r.showEasing, complete: r.onShown }), r.timeOut > 0 && (u = setTimeout(i, r.timeOut), O.maxHideTime = parseFloat(r.timeOut), O.hideEta = (new Date).getTime() + O.maxHideTime, r.progressBar && (O.intervalId = setInterval(a, 10))), l.hover(s, o), !r.onclick && r.tapToDismiss && l.click(i), r.closeButton && w && w.click(function (e) { e.stopPropagation ? e.stopPropagation() : void 0 !== e.cancelBubble && e.cancelBubble !== !0 && (e.cancelBubble = !0), i(!0) }), r.onclick && l.click(function () { r.onclick(), i() }), m(b), r.debug && console && console.log(b), l } function g() { return e.extend({}, p(), b.options) } function h(e) { v || (v = n()), e.is(":visible") || (e.remove(), e = null, 0 === v.children().length && (v.remove(), C = void 0)) } var v, w, C, T = 0, O = { error: "error", info: "info", success: "success", warning: "warning" }, b = { clear: r, remove: d, error: t, getContainer: n, info: i, options: {}, subscribe: o, success: s, version: "2.1.0", warning: a }; return b }() }) }("function" == typeof define && define.amd ? define : function (e, t) { "undefined" != typeof module && module.exports ? module.exports = t(require("jquery")) : window.toastr = t(window.jQuery) });
//# sourceMappingURL=/toastr.js.map