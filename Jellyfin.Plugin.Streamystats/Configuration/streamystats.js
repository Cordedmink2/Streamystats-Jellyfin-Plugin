(function () {
    'use strict';

    var baseUrl;

    function getConfig() {
        return ApiClient.getJSON(ApiClient.getUrl('Streamystats/Config'));
    }

    function getRecommendations(type) {
        var url = ApiClient.getUrl('Streamystats/Recommendations/' + type + '/Items');
        return ApiClient.getJSON(url);
    }

    function getPromotedWatchlists() {
        var url = ApiClient.getUrl('Streamystats/Watchlists/Promoted');
        return ApiClient.getJSON(url);
    }

    function getImageUrl(item) {
        if (!item || !item.Id) {
            return '';
        }

        var primary = item.PrimaryImageTag || (item.ImageTags && item.ImageTags.Primary);
        if (!primary) {
            return '';
        }

        return ApiClient.getUrl('Items/' + item.Id + '/Images/Primary', {
            tag: primary,
            quality: 90,
            maxWidth: 240
        });
    }

    function createCard(item) {
        var card = document.createElement('a');
        card.className = 'streamystats-card';
        card.href = '#/details?id=' + item.Id;

        var image = document.createElement('div');
        image.className = 'streamystats-card-image';
        var url = getImageUrl(item);
        if (url) {
            image.style.backgroundImage = 'url("' + url + '")';
        } else {
            image.classList.add('streamystats-card-image--empty');
        }

        var title = document.createElement('div');
        title.className = 'streamystats-card-title';
        title.textContent = item.Name || item.Title || 'Unknown';

        card.appendChild(image);
        card.appendChild(title);

        return card;
    }

    function createSection(titleText, items, emptyMessage) {
        var section = document.createElement('div');
        section.className = 'streamystats-section';

        var header = document.createElement('div');
        header.className = 'streamystats-section-header';

        var title = document.createElement('div');
        title.className = 'streamystats-section-title';
        title.textContent = titleText;

        header.appendChild(title);

        var list = document.createElement('div');
        list.className = 'streamystats-section-list';

        if (!items || !items.length) {
            var empty = document.createElement('div');
            empty.className = 'streamystats-empty';
            empty.textContent = emptyMessage || 'No items available.';
            list.appendChild(empty);
        } else {
            items.forEach(function (item) {
                list.appendChild(createCard(item));
            });
        }

        section.appendChild(header);
        section.appendChild(list);

        return section;
    }

    function ensureContainer() {
        var container = document.querySelector('.homeSectionsContainer');
        if (!container) {
            return null;
        }

        var existing = document.getElementById('streamystats-section-root');
        if (existing) {
            existing.remove();
        }

        var wrapper = document.createElement('div');
        wrapper.id = 'streamystats-section-root';
        wrapper.className = 'streamystats-root';
        container.prepend(wrapper);
        return wrapper;
    }

    function renderRecommendations(wrapper, label, items, emptyMessage) {
        wrapper.appendChild(createSection(label, items, emptyMessage));
    }

    function renderWatchlists(wrapper, data) {
        var watchlists = data && data.data ? data.data : [];

        var section = document.createElement('div');
        section.className = 'streamystats-section';

        var header = document.createElement('div');
        header.className = 'streamystats-section-header';

        var title = document.createElement('div');
        title.className = 'streamystats-section-title';
        title.textContent = 'Promoted Watchlists';

        header.appendChild(title);
        section.appendChild(header);

        var list = document.createElement('div');
        list.className = 'streamystats-watchlists';

        if (!watchlists.length) {
            var empty = document.createElement('div');
            empty.className = 'streamystats-empty';
            empty.textContent = 'No promoted watchlists found.';
            list.appendChild(empty);
        } else {
            watchlists.forEach(function (watchlist) {
                var card = document.createElement('div');
                card.className = 'streamystats-watchlist-card';

                var name = document.createElement('div');
                name.className = 'streamystats-watchlist-title';
                name.textContent = watchlist.name || 'Watchlist';

                var description = document.createElement('div');
                description.className = 'streamystats-watchlist-description';
                description.textContent = watchlist.description || '';

                var link = document.createElement('a');
                link.className = 'streamystats-watchlist-link';
                link.target = '_blank';
                link.rel = 'noopener noreferrer';
                link.textContent = 'Open in Streamystats';
                link.href = baseUrl;

                card.appendChild(name);
                if (description.textContent) {
                    card.appendChild(description);
                }
                card.appendChild(link);
                list.appendChild(card);
            });
        }

        section.appendChild(list);
        wrapper.appendChild(section);
    }

    function setLoading(wrapper, message) {
        var loading = document.createElement('div');
        loading.className = 'streamystats-loading';
        loading.textContent = message || 'Loading Streamystats recommendations...';
        wrapper.appendChild(loading);
        return loading;
    }

    function clearLoading(loadingElement) {
        if (loadingElement && loadingElement.parentNode) {
            loadingElement.parentNode.removeChild(loadingElement);
        }
    }

    function normalizeItems(result) {
        return result && result.Items ? result.Items : [];
    }

    function normalizeWatchlists(result) {
        return result && result.data ? result.data : [];
    }

    function normalizeSectionOrder(orderValue, enabledSections) {
        var defaultOrder = ['movies', 'series', 'watchlists'];
        var normalized = [];

        if (orderValue) {
            orderValue.split(',').forEach(function (entry) {
                var key = entry.trim().toLowerCase();
                if (!key) {
                    return;
                }

                if (key === 'movie' || key === 'movies') {
                    key = 'movies';
                } else if (key === 'series' || key === 'shows' || key === 'tv') {
                    key = 'series';
                } else if (key === 'watchlists' || key === 'watchlist') {
                    key = 'watchlists';
                } else {
                    return;
                }

                if (normalized.indexOf(key) === -1) {
                    normalized.push(key);
                }
            });
        }

        if (!normalized.length) {
            normalized = defaultOrder.slice();
        }

        defaultOrder.forEach(function (key) {
            if (normalized.indexOf(key) === -1) {
                normalized.push(key);
            }
        });

        return normalized.filter(function (key) {
            return enabledSections.indexOf(key) !== -1;
        });
    }

    function init() {
        getConfig()
            .then(function (config) {
                if (!config || !config.Enabled) {
                    return;
                }

                if (!config.StreamystatsBaseUrl) {
                    return;
                }

                baseUrl = config.StreamystatsBaseUrl;

                var wrapper = ensureContainer();
                if (!wrapper) {
                    return;
                }

                var loading = setLoading(wrapper);

                var enabledSections = [];
                if (config.StreamystatsMovieRecommendations) {
                    enabledSections.push('movies');
                }
                if (config.StreamystatsSeriesRecommendations) {
                    enabledSections.push('series');
                }
                if (config.StreamystatsPromotedWatchlists) {
                    enabledSections.push('watchlists');
                }

                if (!enabledSections.length) {
                    clearLoading(loading);
                    return;
                }

                var sectionOrder = normalizeSectionOrder(config.StreamystatsSectionOrder, enabledSections);
                var sectionRequests = {};
                var tasks = [];

                if (enabledSections.indexOf('movies') !== -1) {
                    tasks.push(getRecommendations('Movies').then(function (result) {
                        sectionRequests.movies = normalizeItems(result);
                    }));
                }

                if (enabledSections.indexOf('series') !== -1) {
                    tasks.push(getRecommendations('Series').then(function (result) {
                        sectionRequests.series = normalizeItems(result);
                    }));
                }

                if (enabledSections.indexOf('watchlists') !== -1) {
                    tasks.push(getPromotedWatchlists().then(function (result) {
                        sectionRequests.watchlists = normalizeWatchlists(result);
                    }));
                }

                return Promise.allSettled(tasks)
                    .then(function () {
                        sectionOrder.forEach(function (sectionKey) {
                            if (sectionKey === 'movies') {
                                renderRecommendations(
                                    wrapper,
                                    'Recommended Movies',
                                    sectionRequests.movies || [],
                                    'No movie recommendations yet.'
                                );
                            } else if (sectionKey === 'series') {
                                renderRecommendations(
                                    wrapper,
                                    'Recommended Series',
                                    sectionRequests.series || [],
                                    'No series recommendations yet.'
                                );
                            } else if (sectionKey === 'watchlists') {
                                renderWatchlists(wrapper, { data: sectionRequests.watchlists || [] });
                            }
                        });
                    })
                    .catch(function () {
                        var error = document.createElement('div');
                        error.className = 'streamystats-error';
                        error.textContent = 'Streamystats integration failed to load.';
                        wrapper.appendChild(error);
                    })
                    .finally(function () {
                        clearLoading(loading);
                    });
            })
            .catch(function () {
                // fail silently for UI injection
            });
    }

    document.addEventListener('viewshow', function (event) {
        var view = event.detail && event.detail.view;
        if (!view) {
            return;
        }

        if (view.id === 'home') {
            init();
        }
    });
})();
