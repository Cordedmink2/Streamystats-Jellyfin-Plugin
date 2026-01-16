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

                var tasks = [];

                if (config.StreamystatsMovieRecommendations) {
                    tasks.push(getRecommendations('Movies').then(function (result) {
                        renderRecommendations(
                            wrapper,
                            'Recommended Movies',
                            result && result.Items ? result.Items : [],
                            'No movie recommendations yet.'
                        );
                    }));
                }

                if (config.StreamystatsSeriesRecommendations) {
                    tasks.push(getRecommendations('Series').then(function (result) {
                        renderRecommendations(
                            wrapper,
                            'Recommended Series',
                            result && result.Items ? result.Items : [],
                            'No series recommendations yet.'
                        );
                    }));
                }

                if (config.StreamystatsPromotedWatchlists) {
                    tasks.push(getPromotedWatchlists().then(function (result) {
                        renderWatchlists(wrapper, result);
                    }));
                }

                return Promise.all(tasks)
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
