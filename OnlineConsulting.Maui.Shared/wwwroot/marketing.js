// Vanilla JS for static-SSR marketing pages - re-runs on Blazor's enhanced-navigation updates.

// Guarded clipboard write - navigator.clipboard is undefined on non-HTTPS/non-localhost origins.
window.copyToClipboard = function (text) {
    if (!navigator.clipboard) {
        return false;
    }
    navigator.clipboard.writeText(text);
    return true;
};

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
        init: function (publishableKey, clientSecret) {
            const unavailable = stripeUnavailableReason(publishableKey);
            if (unavailable) {
                return unavailable;
            }
            stripe = Stripe(publishableKey);
            elements = stripe.elements({ clientSecret: clientSecret });
            elements.create('payment').mount('#payment-element');
            return null;
        },
        confirmPayment: async function () {
            if (!stripe || !elements) {
                return { error: 'Payment form is not ready yet.' };
            }

            const { error } = await stripe.confirmPayment({
                elements: elements,
                confirmParams: { return_url: window.location.origin + '/checkout' },
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
        init: function (publishableKey, elementId) {
            const unavailable = stripeUnavailableReason(publishableKey);
            if (unavailable) {
                return unavailable;
            }
            stripe = Stripe(publishableKey);
            cardElement = stripe.elements().create('card');
            cardElement.mount('#' + elementId);
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

    // Hovering a "Why Choose Us" row fades the right-side image to that row's own photo, reverting
    // to the section's default cover once the mouse leaves the whole row. Delegated on document
    // (mouseover/mouseout bubble, mouseenter/mouseleave don't) rather than dataset.bound
    // per-element listeners, for the same reason as the other delegated handlers here.
    function initFeatureShowcase() {
        if (document.__featureShowcaseBound) {
            return;
        }
        document.__featureShowcaseBound = true;

        var swap = function (image, url) {
            if (!url || image.dataset.pendingSrc === url) {
                return;
            }
            image.dataset.pendingSrc = url;
            image.style.opacity = '0';
            window.setTimeout(function () {
                image.src = url;
                image.style.opacity = '1';
            }, 150);
        };

        document.addEventListener('mouseover', function (event) {
            var row = event.target.closest('.marketing-feature-row');
            var image = document.getElementById('marketing-feature-intro-image');
            if (row && image) {
                swap(image, row.dataset.featureImage);
            }
        });

        document.addEventListener('mouseout', function (event) {
            var row = event.target.closest('.marketing-feature-row');
            var image = document.getElementById('marketing-feature-intro-image');
            if (row && image && !row.contains(event.relatedTarget)) {
                swap(image, image.dataset.defaultSrc);
            }
        });
    }

    function init() {
        initStickyHeader();
        initScrollReveal();
        initServiceGallery();
        initQuoteCta();
        initBrandMarkHome();
        initFeatureShowcase();
    }

    // DOMContentLoaded/enhancedload never fire in time for BlazorWebView (no static-SSR shell, no
    // enhanced-nav pipeline) - MarketingLayout also calls this directly via JS interop after render.
    window.initMarketingNav = init;

    document.addEventListener('DOMContentLoaded', init);
    document.addEventListener('enhancedload', init);
})();

// Ctrl+K / Cmd+K opens the admin command palette (CommandPaletteButton registers itself after first render).
// Brings a section into view after Blazor renders it (e.g. the booking times under the calendar). Phones align it
// to the top, since it sits below the calendar; wider screens use "nearest", a no-op when it is already visible.
window.comfortProScrollIntoView = function (id) {
    var element = document.getElementById(id);
    if (element) {
        var isPhone = window.matchMedia('(max-width: 759.98px)').matches;
        element.scrollIntoView({ behavior: 'smooth', block: isPhone ? 'start' : 'nearest' });
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
