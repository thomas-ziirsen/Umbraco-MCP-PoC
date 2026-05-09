(function () {
    function formatCountdownText(diffMs) {
        if (diffMs <= 0) {
            return "Now live";
        }

        const totalSeconds = Math.floor(diffMs / 1000);
        const days = Math.floor(totalSeconds / 86400);
        const hours = Math.floor((totalSeconds % 86400) / 3600);
        const minutes = Math.floor((totalSeconds % 3600) / 60);

        if (days > 0) {
            return `${days}d ${hours}h ${minutes}m`;
        }

        if (hours > 0) {
            return `${hours}h ${minutes}m`;
        }

        return `${minutes}m`;
    }

    function initializeCountdowns() {
        const countdownItems = Array.from(document.querySelectorAll("[data-countdown][data-event-date]"));
        if (countdownItems.length === 0) {
            return;
        }

        function updateCountdowns() {
            const nowMs = Date.now();

            countdownItems.forEach((item) => {
                const eventDateRaw = item.getAttribute("data-event-date");
                const eventMs = eventDateRaw ? Date.parse(eventDateRaw) : NaN;

                if (Number.isNaN(eventMs)) {
                    item.textContent = "Date unavailable";
                    return;
                }

                const diffMs = eventMs - nowMs;
                const prefix = diffMs > 0 ? "Starts in " : "Status: ";
                item.textContent = `${prefix}${formatCountdownText(diffMs)}`;
            });
        }

        updateCountdowns();
        window.setInterval(updateCountdowns, 30000);
    }

    initializeCountdowns();

    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    const revealItems = Array.from(document.querySelectorAll("[data-reveal]"));

    if (revealItems.length === 0) {
        return;
    }

    if (prefersReducedMotion || !("IntersectionObserver" in window)) {
        revealItems.forEach((item) => item.classList.add("is-visible"));
        return;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (!entry.isIntersecting) {
                return;
            }

            entry.target.classList.add("is-visible");
            observer.unobserve(entry.target);
        });
    }, {
        threshold: 0.2,
        rootMargin: "0px 0px -8% 0px"
    });

    revealItems.forEach((item) => observer.observe(item));
})();