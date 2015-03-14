// Author: Kunuk Nykjaer et al.
// jQuery and google library

// only search related js here
// source: http://tech.cibul.net/geocode-with-google-maps-api-v3/


gmcKN.searchInfo = {
    searchMarker: null,
    zoomLevel: 12,
    round: 6,
    prefix: 4
};

gmcKN.mymap.latlonsearch = function() {
    var lat = $('#gmcKN_latitude').val() + "";
    var lon = $('#gmcKN_longitude').val() + "";
    if (lat.length > gmcKN.searchInfo.round + gmcKN.searchInfo.prefix) {
        lat = lat.substring(0, gmcKN.searchInfo.round + 2 + gmcKN.searchInfo.prefix);
    }

    if (lon.length > gmcKN.searchInfo.round + gmcKN.searchInfo.prefix) {
        lon = lon.substring(0, gmcKN.searchInfo.round + 2 + gmcKN.searchInfo.prefix);
    }

    lat = parseFloat(lat).toFixed(gmcKN.searchInfo.round);
    lon = parseFloat(lon).toFixed(gmcKN.searchInfo.round);
    $('#gmcKN_latitude').val(lat); //update
    $('#gmcKN_longitude').val(lon);
    $('#gmcKN_lonlat').val(lon + ';' + lat);
    var latlon = new google.maps.LatLng(lat, lon);

    gmcKN.geocoder.geocode({ 'latLng': latlon }, function(results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            if (results[0]) {
                $('#gmcKN_search').val(results[0].formatted_address);
            }
        } else {
            $('#gmcKN_search').val("");
        }

        gmcKN.searchInfo.searchMarker.setPosition(latlon);
        gmcKN.map.setOptions({
        //zoom: gmcKN.searchInfo.zoomLevel,
            center: latlon
        });
    });
};

gmcKN.mymap.initializeSearch = function () {

    $('#gmcKN_search').focus();
    $('#gmcKN_latitude').keypress(function (e) {
        if (e.which === 13) {
            gmcKN.mymap.latlonsearch();
        }
    });
    $('#gmcKN_longitude').keypress(function (e) {
        if (e.which === 13) {
            gmcKN.mymap.latlonsearch();
        }
    });

    gmcKN.searchInfo.searchMarker = new google.maps.Marker({
        //init
        map: gmcKN.map,
        draggable: true,
        zIndex: 1
    });

    gmcKN.searchInfo.searchMarker.setPosition(new google.maps.LatLng(gmcKN.mymap.settings.mapCenterLat, gmcKN.mymap.settings.mapCenterLon));
    gmcKN.searchInfo.searchMarker.setVisible(true);

    //Add listener to marker for reverse geocoding
    google.maps.event.addListener(gmcKN.searchInfo.searchMarker, 'drag', function () {

        gmcKN.geocoder.geocode({ 'latLng': gmcKN.searchInfo.searchMarker.getPosition() }, function (results, status) {

            gmcKN.mymap.events.loadKnn(); // reload knn
            
            if (status === google.maps.GeocoderStatus.OK) {
                
                if (results[0]) {
                    var addr = results[0].formatted_address;
                    $('#gmcKN_search').val(addr);
                    var lat = gmcKN.searchInfo.searchMarker.getPosition().lat() + "";
                    var lon = gmcKN.searchInfo.searchMarker.getPosition().lng() + "";
                    if (lat.length > gmcKN.searchInfo.round + gmcKN.searchInfo.prefix)
                        lat = lat.substring(0, gmcKN.searchInfo.round + 2 + gmcKN.searchInfo.prefix);

                    if (lon.length > gmcKN.searchInfo.round + gmcKN.searchInfo.prefix)
                        lon = lon.substring(0, gmcKN.searchInfo.round + 2 + gmcKN.searchInfo.prefix);

                    lat = parseFloat(lat).toFixed(gmcKN.searchInfo.round);
                    lon = parseFloat(lon).toFixed(gmcKN.searchInfo.round);
                    $('#gmcKN_latitude').val(lat);
                    $('#gmcKN_longitude').val(lon);
                    $('#gmcKN_lonlat').val(lon + ';' + lat);
                }
            }
        });
    });

    $(function () {
        $("#gmcKN_search").autocomplete({
            //This uses the geocoder to fetch address values
            source: function (request, response) {
                gmcKN.geocoder.geocode({ 'address': request.term }, function (results, status) { //WORLD
                    response($.map(results, function (item) {

                        return {
                            label: item.formatted_address,
                            value: item.formatted_address,
                            latitude: item.geometry.location.lat(),
                            longitude: item.geometry.location.lng()
                        };

                    }));
                });
            },
            //This is executed upon selection of an address
            select: function (event, ui) {
                //parseFloat()   
                var lat = ui.item.latitude + "";
                var lon = ui.item.longitude + "";
                if (lat.length > gmcKN.searchInfo.round + gmcKN.searchInfo.prefix)
                    lat = lat.substring(0, gmcKN.searchInfo.round +
                        2 + gmcKN.searchInfo.prefix);
                if (lon.length > gmcKN.searchInfo.round + gmcKN.searchInfo.prefix)
                    lon = lon.substring(0, gmcKN.searchInfo.round +
                        2 + gmcKN.searchInfo.prefix);
                lat = parseFloat(lat).toFixed(gmcKN.searchInfo.round);
                lon = parseFloat(lon).toFixed(gmcKN.searchInfo.round);

                $("#gmcKN_latitude").val(lat);
                $("#gmcKN_longitude").val(lon);
                $('#gmcKN_lonlat').val(lon + ';' + lat);
                var location = new google.maps.LatLng(lat, lon);

                gmcKN.searchInfo.searchMarker.setPosition(location);
                gmcKN.searchInfo.searchMarker.setVisible(true);

                gmcKN.map.setOptions({
                    zoom: gmcKN.searchInfo.zoomLevel,
                    center: location
                });
            }
        });
    });
};
// end search -------------

google.maps.event.addDomListener(window, 'load', gmcKN.mymap.initializeSearch); // init search