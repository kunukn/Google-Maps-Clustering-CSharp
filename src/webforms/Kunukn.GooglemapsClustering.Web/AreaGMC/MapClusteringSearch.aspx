<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Google Maps Server-side Clustering</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <link rel="shortcut icon" href="/favicon.ico" type="image/x-icon" />
    <link type="text/css" rel="stylesheet" href="Styles/Gmc.css?v=1" />
    <link type="text/css" rel="stylesheet" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/themes/excite-bike/jquery-ui.css" />
</head>
<body>
    <div id="gmcKN_map_container">
        <div id="gmcKN_topbar">
            <label>
                Address:
            </label>
            <input id="gmcKN_search" size="18" type="text" />
            <label>
                latitude:</label>
            <input id="gmcKN_latitude" size="10" class="gmcKN_mono" maxlength="19" type="text" onclick="this.select();" />
            <label>
                longitude:</label>
            <input id="gmcKN_longitude" class="gmcKN_mono" size="10" maxlength="19" type="text" onclick="this.select();" />
            <input id="gmcKN_btnLatLonSearch" type="button" value="search" onclick="gmcKN.mymap.latlonsearch(); return false;" />            
            <input id="gmcKN_lonlat" maxlength="19" onclick="this.select();" value="Drag the marker"/>
            <!--
            <a href="http://kunuk.wordpress.com/2011/11/05/google-map-server-side-clustering-with-asp-net/"
                                              target="_blank">Server-side clustering blog&nbsp;</a> -->
        </div>

        <div id="gmcKN_map">
        </div>

        <div id="gmcKN_checkboxContainer">
                <!-- 
                <input type="button" value="ZoomIn" onclick="gmcKN.zoomIn();" class="inputZoom" />&nbsp;&nbsp;
                <input type="button" value="ZoomOut" onclick="gmcKN.zoomOut();"  class="inputZoom" />
                <br />
                -->

            <input type="checkbox" id="gmcKN_Type1" name="Type" value="Type1" onclick="gmcKN.checkboxClicked('gmc_type1',this.checked+'');" checked />Type1<br />
            <input type="checkbox" id="gmcKN_Type2" name="Type" value="Type2" onclick="gmcKN.checkboxClicked('gmc_type2',this.checked+'');" checked />Type2<br />
            <input type="checkbox" id="gmcKN_Type3" name="Type" value="Type3" onclick="gmcKN.checkboxClicked('gmc_type3',this.checked+'');" checked />Type3<br />
            <input type="checkbox" id="gmcKN_Knn"  title="drag the search marker" name="Type" value="Knn"  onclick="gmcKN.checkboxClicked('gmc_meta_knn',this.checked+'');" /> K-NN<br />
            <input type="checkbox" id="gmcKN_Lines" name="Type" value="Lines" onclick="gmcKN.checkboxClicked('gmc_meta_lines',this.checked+'');" /> Lines<br />
            <span id="gmcKN_Clustering_span">
                <input type="checkbox" id="gmcKN_Clustering" name="Type" value="Clustering" onclick="gmcKN.checkboxClicked('gmc_clustering',this.checked+'');"
                       checked="checked" />
                       Clustering</span>
            <br />
            <span id="gmcKN_zoomInfo" class="gmcKN_mono"></span><br />
            <span id="gmcKN_markersCount" class="gmcKN_mono"></span>
        </div>
    </div>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
    <script>var $ = jQuery.noConflict();</script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/jquery-ui.min.js"></script>
    <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false"></script>
    <script type="text/javascript" src="Scripts/mapclustering.js?v=1"></script>
    <script type="text/javascript" src="Scripts/mapclustering_search.js?v=1"></script>
</body>
</html>
