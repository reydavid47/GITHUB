/* Copyright (C) 2012 Sylvain Hamel
Project: https://github.com/redhotsly/simple-expand
MIT Licence: https://raw.github.com/redhotsly/simple-expand/master/licence-mit.txt */
(function(){"use strict";function e(){var e=this;e.defaults={hideMode:"fadeToggle",defaultSearchMode:"parent",defaultTarget:".content",throwOnMissingTarget:!0,keepStateInCookie:!1,cookieName:"simple-expand"},e.settings={},$.extend(e.settings,e.defaults),e.findLevelOneDeep=function(e,t,n){return e.find(t).filter(function(){return!$(this).parentsUntil(e,n).length})},e.setInitialState=function(t,n){var r=e.readState(t);r?(t.removeClass("collapsed").addClass("expanded"),e.show(n)):(t.removeClass("expanded").addClass("collapsed"),e.hide(n))},e.hide=function(t){e.settings.hideMode==="fadeToggle"?t.hide():e.settings.hideMode==="basic"&&t.hide()},e.show=function(t){e.settings.hideMode==="fadeToggle"?t.show():e.settings.hideMode==="basic"&&t.show()},e.checkKeepStateInCookiePreconditions=function(){if(e.settings.keepStateInCookie&&$.cookie===undefined)throw new Error("simple-expand: keepStateInCookie option requires $.cookie to be defined.")},e.readCookie=function(){var t=$.cookie(e.settings.cookieName);return t===null||t===""?{}:JSON.parse(t)},e.readState=function(t){if(!e.settings.keepStateInCookie)return!1;var n=t.attr("Id");if(n===undefined)return;var r=e.readCookie(),i=r[n]===!0||!1;return i},e.saveState=function(t,n){if(!e.settings.keepStateInCookie)return;var r=t.attr("Id");if(r===undefined)return;var i=e.readCookie();i[r]=n,$.cookie(e.settings.cookieName,JSON.stringify(i),{raw:!0,path:window.location.pathname})},e.toggle=function(t,n){var r=e.toggleCss(t);return e.settings.hideMode==="fadeToggle"?n.fadeToggle(150):e.settings.hideMode==="basic"?n.toggle():$.isFunction(e.settings.hideMode)&&e.settings.hideMode(t,n,r),e.saveState(t,r),!1},e.toggleCss=function(e){return e.hasClass("expanded")?(e.toggleClass("collapsed expanded"),!1):(e.toggleClass("expanded collapsed"),!0)},e.findTargets=function(t,n,r){var i=[];if(n==="absolute")i=$(r);else if(n==="relative")i=e.findLevelOneDeep(t,r,r);else if(n==="parent"){var s=t.parent();do i=e.findLevelOneDeep(s,r,r),i.length===0&&(s=s.parent());while(i.length===0&&s.length!==0)}return i},e.activate=function(t,n){$.extend(e.settings,n),e.checkKeepStateInCookiePreconditions(),t.each(function(){var t=$(this),n=t.attr("data-expander-target")||e.settings.defaultTarget,r=t.attr("data-expander-target-search")||e.settings.defaultSearchMode,i=e.findTargets(t,r,n);if(i.length===0){if(e.settings.throwOnMissingTarget)throw"simple-expand: Targets not found";return this}e.setInitialState(t,i),t.click(function(){return e.toggle(t,i)})})}}window.SimpleExpand=e,$.fn.simpleexpand=function(t){var n=new e;return n.activate(this,t),this}})();