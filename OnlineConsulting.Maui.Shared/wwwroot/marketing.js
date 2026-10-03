// Vanilla JS for static-SSR marketing pages - re-runs on Blazor's enhanced-navigation updates.

// Guarded clipboard write - navigator.clipboard is undefined on non-HTTPS/non-localhost origins.
window.copyToClipboard = function (text) {
    if (!navigator.clipboard) {
        return false;
    }
    navigator.clipboard.writeText(text);
    return true;
};

// Stripe.js is loaded only when a payment form opens, never with the page: a slow or unreachable js.stripe.com
// would otherwise hold back the page's load event, and the app build only starts Blazor once that event fires.
var stripeScriptPromise = null;
function loadStripe() {
    if (typeof Stripe !== 'undefined') {
        return Promise.resolve(true);
    }
    if (!stripeScriptPromise) {
        stripeScriptPromise = new Promise(function (resolve) {
            var script = document.createElement('script');
            var settle = function (loaded) {
                clearTimeout(timer);
                if (!loaded) {
                    stripeScriptPromise = null;
                    script.remove();
                }
                resolve(loaded);
            };
            var timer = setTimeout(function () { settle(false); }, 15000);
            script.src = 'https://js.stripe.com/v3/';
            script.async = true;
            script.onload = function () { settle(true); };
            script.onerror = function () { settle(false); };
            document.head.appendChild(script);
        });
    }
    return stripeScriptPromise;
}

// Returns why the Stripe form can't mount (script blocked/offline, key not configured), or null when it can.
function stripeUnavailableReason(publishableKey) {
    if (typeof Stripe === 'undefined') {
        return 'The payment form could not load. Check your internet connection and try again.';
    }
    return publishableKey ? null : 'Online payments are not configured yet.';
}

// In-page Stripe Payment Element for Checkout.razor - stripe/elements are module-scoped since
// only one checkout flow is active per tab. init returns an error message, or null once mounted.
window.checkoutStripe = (function () {
    let stripe = null;
    let elements = null;

    return {
        init: async function (publishableKey, clientSecret) {
            await loadStripe();
            const unavailable = stripeUnavailableReason(publishableKey);
            if (unavailable) {
                return unavailable;
            }
            stripe = Stripe(publishableKey);
            elements = stripe.elements({ clientSecret: clientSecret });
            elements.create('payment').mount('#payment-element');
            return null;
        },
        confirmPayment: async function (returnPath) {
            if (!stripe || !elements) {
                return { error: 'Payment form is not ready yet.' };
            }

            const { error } = await stripe.confirmPayment({
                elements: elements,
                confirmParams: { return_url: window.location.origin + (returnPath || '/checkout') },
                redirect: 'if_required'
            });

            return { error: error ? (error.message || 'Payment could not be confirmed. Please try again.') : null };
        }
    };
})();

// In-page Stripe Card Element for MembershipSubscribe.razor/Tenancy Signup.razor - tokenizes a card into a
// PaymentMethodId before the subscribe/signup API call, unlike checkoutStripe which confirms an
// already-created PaymentIntent. init returns an error message, or null once mounted.
window.subscribeStripe = (function () {
    let stripe = null;
    let cardElement = null;

    return {
        init: async function (publishableKey, elementId, dotNetRef) {
            await loadStripe();
            const unavailable = stripeUnavailableReason(publishableKey);
            if (unavailable) {
                return unavailable;
            }
            stripe = Stripe(publishableKey);
            var palette = getComputedStyle(document.documentElement);
            var themeColor = function (name, fallback) {
                return (palette.getPropertyValue(name) || '').trim() || fallback;
            };
            cardElement = stripe.elements().create('card', {
                hidePostalCode: false,
                style: {
                    base: {
                        fontSize: '16px',
                        fontFamily: 'Inter, "Segoe UI", Roboto, Arial, sans-serif',
                        color: themeColor('--mud-palette-text-primary', '#242424'),
                        iconColor: themeColor('--mud-palette-primary', '#0F6CBD'),
                        '::placeholder': { color: themeColor('--mud-palette-text-disabled', '#8a8a8a') }
                    },
                    invalid: {
                        color: themeColor('--mud-palette-error', '#C50F1F'),
                        iconColor: themeColor('--mud-palette-error', '#C50F1F')
                    }
                }
            });
            cardElement.mount('#' + elementId);
            var lastState = null;
            cardElement.on('change', function (event) {
                var error = event.error ? event.error.message : null;
                var state = (error || '') + '|' + event.complete;
                if (dotNetRef && state !== lastState) {
                    lastState = state;
                    dotNetRef.invokeMethodAsync('OnCardChanged', error, event.complete === true);
                }
            });
            return null;
        },
        createPaymentMethod: async function () {
            if (!stripe || !cardElement) {
                return { error: 'Payment form is not ready yet.' };
            }

            const { paymentMethod, error } = await stripe.createPaymentMethod('card', cardElement);
            return error
                ? { error: error.message || 'Could not process card details.' }
                : { paymentMethodId: paymentMethod.id };
        }
    };
})();

(function () {
    // Bound on window (not the header node) and re-queried each time - survives the header
    // element getting swapped out by an interactive render or enhanced nav.
    function initStickyHeader() {
        var stickyThreshold = 8;
        var applyState = function () {
            var header = document.querySelector('.marketing-sticky-header');
            var spacer = document.querySelector('.marketing-sticky-header-spacer');
            if (!header || !spacer) {
                return;
            }

            var shouldStick = window.scrollY > stickyThreshold;
            header.classList.toggle('is-sticky', shouldStick);
            spacer.classList.toggle('is-active', shouldStick);

            var isPhone = window.matchMedia('(max-width: 959.98px)').matches;
            var scrollingDown = window.scrollY > (window.__lastScrollY || 0);
            header.classList.toggle('is-hidden', isPhone && scrollingDown && window.scrollY > 160);
            window.__lastScrollY = window.scrollY;
        };

        if (!window.__stickyHeaderBound) {
            window.__stickyHeaderBound = true;
            window.addEventListener('scroll', applyState, { passive: true });
        }

        applyState();
    }

    // Apple-style subtle fade+rise as sections enter the viewport. No-op for
    // prefers-reduced-motion, for browsers without IntersectionObserver, and on phone-sized
    // screens, where a fast flick would briefly show empty space and read as slow loading.
    function initScrollReveal() {
        if (!('IntersectionObserver' in window)
            || window.matchMedia('(prefers-reduced-motion: reduce)').matches
            || window.matchMedia('(max-width: 959.98px)').matches) {
            return;
        }

        var targets = document.querySelectorAll(
            '.marketing-section, .marketing-section-tight, .marketing-dark-section, .marketing-quote-section'
        );
        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('marketing-revealed');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12 });

        targets.forEach(function (target) {
            if (!target.dataset.revealBound) {
                target.dataset.revealBound = 'true';
                target.classList.add('marketing-reveal');
                observer.observe(target);
            }
        });
    }

    // Thumbnail click swaps it with the main image (not a copy). Delegated on document - same
    // reason as initQuoteCta below, the interactive island further down the page can replace
    // nodes on first circuit-attach, which silently detaches a per-element listener.
    function initServiceGallery() {
        if (document.__serviceGalleryBound) {
            return;
        }
        document.__serviceGalleryBound = true;

        document.addEventListener('click', function (event) {
            var thumb = event.target.closest('.service-gallery-thumb');
            var main = document.getElementById('service-gallery-main');
            if (!thumb || !main) {
                return;
            }

            var clickedSrc = thumb.src;
            thumb.src = main.src;
            main.src = clickedSrc;
        });
    }

    // Scrolls the Ask panel into view once "Get a Quote" checks its radio. Delegated on document -
    // per-element listeners go stale when the interactive island further down replaces this node.
    function initQuoteCta() {
        if (document.__quoteCtaBound) {
            return;
        }
        document.__quoteCtaBound = true;

        document.addEventListener('click', function (event) {
            if (!event.target.closest('.service-quote-cta')) {
                return;
            }

            // Double rAF - waits for the panel's display:block to actually apply before measuring.
            requestAnimationFrame(function () {
                requestAnimationFrame(function () {
                    var tabs = document.querySelector('.marketing-tabs');
                    if (tabs) {
                        tabs.scrollIntoView({ behavior: 'smooth', block: 'start' });
                    }
                });
            });
        });
    }

    // Clicking the logo while already on "/" is an enhanced-nav request to the same URL - Blazor
    // patches the page but (unlike a real navigation) never resets scroll, so the sticky header
    // loses its is-sticky state while the viewport stays scrolled down, looking like it vanished.
    // Scrolling to top ourselves for the same-page case sidesteps that entirely.
    function initBrandMarkHome() {
        if (document.__brandMarkBound) {
            return;
        }
        document.__brandMarkBound = true;

        document.addEventListener('click', function (event) {
            var link = event.target.closest('.marketing-brand-mark');
            if (!link || location.pathname !== '/') {
                return;
            }

            event.preventDefault();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // "What We Provide" showcase ([data-provide]): clicking a tab (or arrow keys on the tab list) picks the photo panel; nothing
    // changes on its own.
    function initProvide() {
        var activate = function (root, index, focus) {
            root.querySelectorAll('[data-provide-tab]').forEach(function (tab) {
                var isActive = Number(tab.getAttribute('data-provide-tab')) === index;
                tab.classList.toggle('active', isActive);
                tab.setAttribute('aria-selected', isActive ? 'true' : 'false');
                tab.setAttribute('tabindex', isActive ? '0' : '-1');
                if (isActive && focus) {
                    tab.focus();
                }
            });
            root.querySelectorAll('[data-provide-panel]').forEach(function (panel) {
                panel.classList.toggle('active', Number(panel.getAttribute('data-provide-panel')) === index);
            });
        };

        if (document.__provideBound) {
            return;
        }
        document.__provideBound = true;

        document.addEventListener('click', function (event) {
            var tab = event.target.closest && event.target.closest('[data-provide-tab]');
            var root = tab && tab.closest('[data-provide]');
            if (!root) {
                return;
            }
            activate(root, Number(tab.getAttribute('data-provide-tab')), false);
        });

        document.addEventListener('keydown', function (event) {
            var tab = event.target.closest && event.target.closest('[data-provide-tab]');
            var root = tab && tab.closest('[data-provide]');
            if (!root) {
                return;
            }
            var count = root.querySelectorAll('[data-provide-tab]').length;
            var current = Number(tab.getAttribute('data-provide-tab'));
            var next = { ArrowDown: current + 1, ArrowRight: current + 1, ArrowUp: current - 1, ArrowLeft: current - 1, Home: 0, End: count - 1 }[event.key];
            if (next === undefined) {
                return;
            }
            event.preventDefault();
            activate(root, (next + count) % count, true);
        });
    }

    // Horizontal snap carousels ([data-carousel] > [data-carousel-track]): arrows page by one viewport, dots jump to a card,
    // and scrolling (swipe, trackpad, arrows) keeps the active dot and arrow disabled states in sync. Delegated on document,
    // scroll via capture since it doesn't bubble - carousels rendered later by Blazor work without re-binding.
    function initCarousels() {
        if (document.__carouselsBound) {
            return;
        }
        document.__carouselsBound = true;

        var cardStep = function (track) {
            var first = track.firstElementChild;
            if (!first) {
                return track.clientWidth;
            }
            var gap = parseFloat(getComputedStyle(track).columnGap) || 0;
            return first.getBoundingClientRect().width + gap;
        };

        var sync = function (carousel) {
            var track = carousel.querySelector('[data-carousel-track]');
            if (!track) {
                return;
            }
            var index = Math.round(track.scrollLeft / cardStep(track));
            carousel.querySelectorAll('[data-carousel-dot]').forEach(function (dot, i) {
                dot.classList.toggle('active', i === index);
                dot.setAttribute('aria-current', i === index ? 'true' : 'false');
            });
            var atStart = track.scrollLeft <= 4;
            var atEnd = track.scrollLeft + track.clientWidth >= track.scrollWidth - 4;
            carousel.querySelectorAll('[data-carousel-prev]').forEach(function (b) { b.disabled = atStart; });
            carousel.querySelectorAll('[data-carousel-next]').forEach(function (b) { b.disabled = atEnd; });
        };

        document.addEventListener('click', function (event) {
            var more = event.target.closest('[data-carousel-more]');
            if (more) {
                var card = more.closest('[data-carousel-card]');
                if (card) {
                    var expanded = card.classList.toggle('expanded');
                    more.textContent = expanded ? 'Show less' : 'Read more';
                    more.setAttribute('aria-expanded', expanded ? 'true' : 'false');
                }
                return;
            }

            var control = event.target.closest('[data-carousel-prev], [data-carousel-next], [data-carousel-dot]');
            var carousel = control && control.closest('[data-carousel]');
            var track = carousel && carousel.querySelector('[data-carousel-track]');
            if (!track) {
                return;
            }

            if (control.hasAttribute('data-carousel-dot')) {
                track.scrollTo({ left: Number(control.getAttribute('data-carousel-dot')) * cardStep(track), behavior: 'smooth' });
            } else {
                var direction = control.hasAttribute('data-carousel-next') ? 1 : -1;
                var perPage = Math.max(1, Math.floor((track.clientWidth + 1) / cardStep(track)));
                track.scrollBy({ left: direction * perPage * cardStep(track), behavior: 'smooth' });
            }
        });

        document.addEventListener('scroll', function (event) {
            var track = event.target;
            if (track && track.matches && track.matches('[data-carousel-track]')) {
                var carousel = track.closest('[data-carousel]');
                if (carousel) {
                    sync(carousel);
                }
            }
        }, true);

        window.addEventListener('resize', function () {
            document.querySelectorAll('[data-carousel]').forEach(sync);
        });
    }

    // Service area finder ([data-area-finder]): the search box and state chips filter the area cards in place, and the
    // empty-state message appears when nothing matches. Delegated like the carousels, so static and interactive renders both work.
    function initAreaFinder() {
        if (document.__areaFinderBound) {
            return;
        }
        document.__areaFinderBound = true;

        var apply = function (finder) {
            var input = finder.querySelector('[data-area-search]');
            var query = (input ? input.value : '').trim().toLowerCase();
            var activeChip = finder.querySelector('[data-area-state].active');
            var state = activeChip ? activeChip.getAttribute('data-area-state') : '';
            var shown = 0;
            finder.querySelectorAll('[data-area-card]').forEach(function (card) {
                var matchesState = !state || card.getAttribute('data-state') === state;
                var matchesQuery = !query || card.getAttribute('data-search').indexOf(query) !== -1;
                var visible = matchesState && matchesQuery;
                card.hidden = !visible;
                if (visible) {
                    shown++;
                }
            });
            var empty = finder.querySelector('[data-area-empty]');
            if (empty) {
                empty.hidden = shown > 0;
                var echo = empty.querySelector('[data-area-query]');
                if (echo) {
                    echo.textContent = input ? input.value.trim() : '';
                }
            }
            var count = finder.querySelector('[data-area-count]');
            if (count) {
                count.textContent = shown === 1 ? '1 area' : shown + ' areas';
            }
            var clear = finder.querySelector('[data-area-clear]');
            if (clear) {
                clear.hidden = !query;
            }
            syncAreaMap(finder);
        };

        document.addEventListener('input', function (event) {
            var finder = event.target.closest && event.target.closest('[data-area-finder]');
            if (finder && event.target.matches('[data-area-search]')) {
                apply(finder);
            }
        });

        document.addEventListener('click', function (event) {
            var finder = event.target.closest('[data-area-finder]');
            if (!finder) {
                return;
            }
            var chip = event.target.closest('[data-area-state]');
            if (chip) {
                finder.querySelectorAll('[data-area-state]').forEach(function (c) {
                    c.classList.toggle('active', c === chip);
                    c.setAttribute('aria-pressed', c === chip ? 'true' : 'false');
                });
                apply(finder);
                return;
            }
            if (event.target.closest('[data-area-clear]')) {
                var input = finder.querySelector('[data-area-search]');
                if (input) {
                    input.value = '';
                    input.focus();
                }
                apply(finder);
            }
        });
    }

    // Service area map ([data-area-map] inside a [data-area-finder]): MapLibre GL JS draws OpenFreeMap's vector "liberty" style (no API
    // key). The library is loaded only when a map is on the page; pins come from the cards' data-lat/data-lng, and a pin and its card
    // highlight each other. Touch devices pan with two fingers (cooperative gestures) so the page still scrolls. A MutationObserver picks
    // up maps that Blazor renders after load. If MapLibre can't load or WebGL is missing, the map box is hidden and the list keeps working.
    var MAPLIBRE_VERSION = '5.24.0';
    var AREA_MAP_STYLE = 'https://tiles.openfreemap.org/styles/liberty';
    var mapLibrePromise = null;
    function loadMapLibre() {
        if (window.maplibregl) {
            return Promise.resolve(true);
        }
        if (!mapLibrePromise) {
            mapLibrePromise = new Promise(function (resolve) {
                var base = 'https://cdn.jsdelivr.net/npm/maplibre-gl@' + MAPLIBRE_VERSION + '/dist/';
                var css = document.createElement('link');
                css.rel = 'stylesheet';
                css.href = base + 'maplibre-gl.css';
                document.head.appendChild(css);
                var script = document.createElement('script');
                var timer = null;
                var finish = function (ok) {
                    clearTimeout(timer);
                    if (!ok) {
                        mapLibrePromise = null;
                    }
                    resolve(ok);
                };
                timer = setTimeout(function () { finish(false); }, 15000);
                script.src = base + 'maplibre-gl.js';
                script.async = true;
                script.onload = function () { finish(!!window.maplibregl); };
                script.onerror = function () { finish(false); };
                document.head.appendChild(script);
            });
        }
        return mapLibrePromise;
    }

    function escapeHtml(text) {
        var map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };
        return String(text).replace(/[&<>"']/g, function (c) { return map[c]; });
    }

    function prefersReducedMotion() {
        return window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    }

    function highlightArea(finder, id, scrollCard) {
        finder.querySelectorAll('[data-area-card]').forEach(function (card) {
            var match = card.getAttribute('data-area-id') === id;
            card.classList.toggle('highlight', match);
            if (match && scrollCard) {
                card.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }
        });
        var mapElement = finder.querySelector('[data-area-map]');
        var pins = mapElement && mapElement.__areaMap ? mapElement.__areaMap.markers : {};
        Object.keys(pins).forEach(function (key) {
            var element = pins[key].getElement();
            if (element) {
                element.classList.toggle('active', key === id);
            }
        });
    }

    function showAreaPopup(state, id) {
        Object.keys(state.markers).forEach(function (key) {
            var popup = state.markers[key].getPopup();
            if (key !== id && popup && popup.isOpen()) {
                popup.remove();
            }
        });
        var marker = state.markers[id];
        var popup = marker && marker.getPopup();
        if (popup && !popup.isOpen() && marker.__onMap) {
            popup.setLngLat(marker.getLngLat()).addTo(state.map);
        }
    }

    function syncAreaMap(finder) {
        var mapElement = finder.querySelector('[data-area-map]');
        var state = mapElement && mapElement.__areaMap;
        if (!state) {
            return;
        }
        var bounds = new window.maplibregl.LngLatBounds();
        var visible = 0;
        var last = null;
        finder.querySelectorAll('[data-area-card]').forEach(function (card) {
            var marker = state.markers[card.getAttribute('data-area-id')];
            if (!marker) {
                return;
            }
            if (card.hidden) {
                marker.remove();
                marker.__onMap = false;
            } else {
                if (!marker.__onMap) {
                    marker.addTo(state.map);
                    marker.__onMap = true;
                }
                bounds.extend(marker.getLngLat());
                last = marker.getLngLat();
                visible++;
            }
        });
        var duration = prefersReducedMotion() ? 0 : 700;
        if (visible === 1) {
            state.map.easeTo({ center: last, zoom: 10, duration: duration });
        } else if (visible > 1) {
            state.map.fitBounds(bounds, { padding: 48, maxZoom: 10, duration: duration });
        }
    }

    function setupAreaMap(mapElement) {
        if (mapElement.__areaMapState) {
            return;
        }
        mapElement.__areaMapState = 'loading';
        loadMapLibre().then(function (ok) {
            var finder = mapElement.closest('[data-area-finder]');
            var unavailable = function () {
                mapElement.__areaMapState = null;
                mapElement.classList.add('map-unavailable');
            };
            if (!ok || !finder || !document.body.contains(mapElement)) {
                unavailable();
                return;
            }
            var maplibregl = window.maplibregl;
            var map;
            try {
                map = new maplibregl.Map({
                    container: mapElement,
                    style: AREA_MAP_STYLE,
                    center: [-96, 38],
                    zoom: 3,
                    attributionControl: { compact: true },
                    cooperativeGestures: true,
                    dragRotate: false,
                    pitchWithRotate: false
                });
            } catch (error) {
                unavailable();
                return;
            }
            map.touchZoomRotate.disableRotation();
            map.addControl(new maplibregl.NavigationControl({ showCompass: false }), 'top-right');
            map.on('error', function (event) {
                if (event && event.error && /webgl/i.test(String(event.error.message))) {
                    map.remove();
                    unavailable();
                }
            });

            var markers = {};
            finder.querySelectorAll('[data-area-card]').forEach(function (card) {
                var lat = parseFloat(card.getAttribute('data-lat'));
                var lng = parseFloat(card.getAttribute('data-lng'));
                if (isNaN(lat) || isNaN(lng)) {
                    return;
                }
                var id = card.getAttribute('data-area-id');
                var name = card.getAttribute('data-name') || '';
                var element = document.createElement('button');
                element.type = 'button';
                element.className = 'area-pin';
                element.setAttribute('aria-label', name);
                element.innerHTML = '<span></span>';
                var popup = new maplibregl.Popup({ offset: 30, closeButton: false, className: 'area-popup' })
                    .setHTML('<strong>' + escapeHtml(name) + '</strong><a href="/appointment" class="area-pin-link">Book a visit &rarr;</a>');
                var marker = new maplibregl.Marker({ element: element, anchor: 'bottom' })
                    .setLngLat([lng, lat])
                    .setPopup(popup);
                element.addEventListener('click', function () { highlightArea(finder, id, true); });
                element.addEventListener('mouseenter', function () {
                    highlightArea(finder, id, false);
                    showAreaPopup(mapElement.__areaMap, id);
                });
                marker.__onMap = false;
                markers[id] = marker;
            });
            mapElement.__areaMap = { map: map, markers: markers };
            mapElement.__areaMapState = 'ready';
            syncAreaMap(finder);
            map.once('load', function () {
                map.resize();
                syncAreaMap(finder);
                var attribution = mapElement.querySelector('.maplibregl-ctrl-attrib.maplibregl-compact-show');
                if (attribution) {
                    attribution.classList.remove('maplibregl-compact-show');
                    attribution.removeAttribute('open');
                }
            });
        });
    }

    // The map (about 1 MB of script plus WebGL) is only built when its box comes near the viewport.
    var areaMapObserver = 'IntersectionObserver' in window
        ? new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    areaMapObserver.unobserve(entry.target);
                    setupAreaMap(entry.target);
                }
            });
        }, { rootMargin: '300px 0px' })
        : null;

    function initAreaMaps() {
        var scan = function () {
            document.querySelectorAll('[data-area-map]').forEach(function (mapElement) {
                if (mapElement.__areaMapState || mapElement.__areaMapWatched) {
                    return;
                }
                if (!areaMapObserver) {
                    setupAreaMap(mapElement);
                    return;
                }
                mapElement.__areaMapWatched = true;
                areaMapObserver.observe(mapElement);
            });
        };
        scan();
        if (document.__areaMapsBound) {
            return;
        }
        document.__areaMapsBound = true;

        var pending = false;
        new MutationObserver(function () {
            if (!pending) {
                pending = true;
                requestAnimationFrame(function () {
                    pending = false;
                    scan();
                });
            }
        }).observe(document.body, { childList: true, subtree: true });

        document.addEventListener('mouseover', function (event) {
            var card = event.target.closest && event.target.closest('[data-area-card]');
            var finder = card && card.closest('[data-area-finder]');
            var mapElement = finder && finder.querySelector('[data-area-map]');
            if (!card || !mapElement || !mapElement.__areaMap) {
                return;
            }
            var id = card.getAttribute('data-area-id');
            highlightArea(finder, id, false);
            showAreaPopup(mapElement.__areaMap, id);
        });
    }


    function init() {
        initAreaMaps();
        initAreaFinder();
        initCarousels();
        initStickyHeader();
        initScrollReveal();
        initServiceGallery();
        initQuoteCta();
        initBrandMarkHome();
        initProvide();
    }

    // DOMContentLoaded/enhancedload never fire in time for BlazorWebView (no static-SSR shell, no
    // enhanced-nav pipeline) - MarketingLayout also calls this directly via JS interop after render.
    window.initMarketingNav = init;

    document.addEventListener('DOMContentLoaded', init);
    document.addEventListener('enhancedload', init);
})();

// Ctrl+K / Cmd+K opens the admin command palette (CommandPaletteButton registers itself after first render).
// App-level refresh triggers for LiveUpdatesHost: the page becoming visible again (tab switch, app back from the
// background) and, in the native app only, a pull-down gesture at the top of the page with a spinner indicator.
window.comfortProLiveUpdates = (function () {
    var PULL_THRESHOLD = 72;
    var PULL_MAX = 110;
    var dotnet = null;
    var indicator = null;
    var startX = 0;
    var startY = 0;
    var distance = 0;
    var tracking = false;
    var refreshing = false;
    var wasHidden = false;

    function onVisibilityChange() {
        if (document.visibilityState === 'hidden') {
            wasHidden = true;
        } else if (wasHidden && dotnet) {
            wasHidden = false;
            dotnet.invokeMethodAsync('OnPageVisible');
        }
    }

    function ensureIndicator() {
        if (!indicator) {
            indicator = document.createElement('div');
            indicator.className = 'pull-refresh-indicator';
            indicator.setAttribute('aria-hidden', 'true');
            indicator.innerHTML = '<span class="pull-refresh-spinner"></span>';
            document.body.appendChild(indicator);
        }
        return indicator;
    }

    function setPull(value) {
        var element = ensureIndicator();
        var progress = Math.min(value / PULL_THRESHOLD, 1);
        element.style.transform = 'translate(-50%, ' + (value - 48) + 'px) rotate(' + (progress * 270) + 'deg)';
        element.style.opacity = String(progress);
        element.classList.toggle('ready', value >= PULL_THRESHOLD);
    }

    function reset() {
        if (!indicator) {
            return;
        }
        indicator.classList.remove('ready', 'refreshing');
        indicator.style.transform = 'translate(-50%, -48px)';
        indicator.style.opacity = '0';
    }

    function startsInsideControl(target) {
        return !!(target && target.closest && target.closest('textarea, input, select, .mud-dialog, .mud-popover, [data-no-pull]'));
    }

    function onTouchStart(event) {
        if (refreshing || window.scrollY > 0 || event.touches.length !== 1 || startsInsideControl(event.target)) {
            tracking = false;
            return;
        }
        tracking = true;
        distance = 0;
        startX = event.touches[0].clientX;
        startY = event.touches[0].clientY;
    }

    function onTouchMove(event) {
        if (!tracking) {
            return;
        }
        var dx = event.touches[0].clientX - startX;
        var dy = event.touches[0].clientY - startY;
        if (dy <= 0 || Math.abs(dx) > Math.abs(dy)) {
            distance = 0;
            tracking = false;
            reset();
            return;
        }
        distance = Math.min(dy * 0.5, PULL_MAX);
        setPull(distance);
    }

    function onTouchEnd() {
        if (!tracking) {
            return;
        }
        tracking = false;
        if (distance < PULL_THRESHOLD || !dotnet) {
            reset();
            return;
        }
        refreshing = true;
        var element = ensureIndicator();
        element.classList.add('refreshing');
        element.style.opacity = '1';
        element.style.transform = 'translate(-50%, ' + (PULL_THRESHOLD - 40) + 'px)';
        dotnet.invokeMethodAsync('OnPullToRefresh').finally(function () {
            refreshing = false;
            reset();
        });
    }

    return {
        init: function (dotnetReference, enablePull) {
            this.dispose();
            dotnet = dotnetReference;
            document.addEventListener('visibilitychange', onVisibilityChange);
            if (enablePull) {
                window.addEventListener('touchstart', onTouchStart, { passive: true });
                window.addEventListener('touchmove', onTouchMove, { passive: true });
                window.addEventListener('touchend', onTouchEnd, { passive: true });
                window.addEventListener('touchcancel', onTouchEnd, { passive: true });
            }
        },
        dispose: function () {
            document.removeEventListener('visibilitychange', onVisibilityChange);
            window.removeEventListener('touchstart', onTouchStart);
            window.removeEventListener('touchmove', onTouchMove);
            window.removeEventListener('touchend', onTouchEnd);
            window.removeEventListener('touchcancel', onTouchEnd);
            dotnet = null;
            reset();
        }
    };
})();

// Brings a section into view after Blazor renders it (e.g. the booking times under the calendar). Phones align it
// to the top, since it sits below the calendar; wider screens use "nearest", a no-op when it is already visible.
window.comfortProScrollIntoView = function (id) {
    var element = document.getElementById(id);
    if (element) {
        var isPhone = window.matchMedia('(max-width: 759.98px)').matches;
        element.scrollIntoView({ behavior: 'smooth', block: isPhone ? 'start' : 'nearest' });
    }
};

// Browser download for a generated file (invoice PDF) - the app build hands files to the device viewer instead.
window.comfortProSaveFile = function (fileName, base64, contentType) {
    var bytes = Uint8Array.from(atob(base64), function (c) { return c.charCodeAt(0); });
    var url = URL.createObjectURL(new Blob([bytes], { type: contentType }));
    var link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    link.remove();
    setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
};

window.comfortProScrollToSection = function (id) {
    var element = document.getElementById(id);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};

window.comfortProShortcuts = (function () {
    let handler = null;

    return {
        registerCommandPalette(dotNetReference) {
            if (handler) {
                document.removeEventListener('keydown', handler);
            }

            handler = function (event) {
                if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
                    event.preventDefault();
                    dotNetReference.invokeMethodAsync('OpenAsync');
                }
            };
            document.addEventListener('keydown', handler);
        },
        unregisterCommandPalette() {
            if (handler) {
                document.removeEventListener('keydown', handler);
                handler = null;
            }
        },
    };
})();
