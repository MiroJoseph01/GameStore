document.getElementById('filter-form').onsubmit = function (event) {
    event.preventDefault();

    let form = document.forms[0];

    let values = {
        platforms: [],
        genres: [],
        publishers: [],
        date_filter: '',
        order_filter: '',
        page_size: '',
        from: form.elements.From.value,
        to: form.elements.To.value,
        game: form.elements.Game.value,
        page: form.elements.page.value,
        is_filtered: form.elements.IsFiltered.value
    };

    let platforms = form.elements.platform;
    let genres = form.elements.genre;
    let publishers = form.elements.publisher;
    let dates = form.elements.DateFilter;
    let filter = form.elements.OrderFilter;
    let pageSize = form.elements.PageSize;

    platforms.forEach(function (p) {
        if (p.checked) {
            values.platforms.push(p.getAttribute("value"));
        }
    })

    genres.forEach(function (p) {
        if (p.checked) {
            values.genres.push(p.getAttribute("value"));
        }
    })

    publishers.forEach(function (p) {
        if (p.checked) {
            values.publishers.push(p.getAttribute("value"));
        }
    });

    dates.forEach(function (p) {
        if (p.checked) {
            values.date_filter = p.getAttribute("value");
        }
    });

    values.order_filter = filter.options[filter.selectedIndex].value;
    values.page_size = pageSize.options[pageSize.selectedIndex].value;

    let queryString = new URLSearchParams();

    for (const [key, val] of Object.entries(values)) {
        if (Array.isArray(val)) {
            if (val.length) {
                queryString.append(key, val.join('-'));
            }
        }
        else {
            if (val) {
                queryString.append(key, val);
            }
        }

    }

    window.location.search = queryString.toString();
}

function updatePageURLParameter(paramVal) {
    var searchParams = new URLSearchParams(window.location.search);
    searchParams.set('page', paramVal);
    searchParams.set('is_filtered', false);
    window.location.search = searchParams.toString();
}

function updateItemNumberURLParameter(paramVal) {
    let pageSize = document.forms[0].elements.PageSize;

    let searchParams = new URLSearchParams(window.location.search);
    searchParams.set('page_size', pageSize.options[pageSize.selectedIndex].value);
    searchParams.set('is_filtered', true);
    window.location.search = searchParams.toString();
}
