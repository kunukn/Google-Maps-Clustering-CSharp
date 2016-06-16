Frequently Asked Questions

Q. How do I get an overview of how this Google Maps Server-side clustering works?

A. Read my blog http://kunuk.wordpress.com/2011/11/05/google-map-server-side-clustering-with-asp-net/


-----------------------------------------------


Q. How many points can I use?

A. With current implementation, the upper limit should be around 300.000 where the delay is below 1 sec. 
The server running time with an average laptop was about 300 msec. for 300.000 points.

It should be possible to increase the number of points and keep a delay below 1 sec. 
by implementing a spatial data structure such as range search but it would only help if you are zoomed in. 
If you are zoomed all the way out you will have to iterate all the points then you would have to rely on pre-clustering for time complexity improvement.


-----------------------------------------------


Q. How do I control the grid size?

A. In the file GmcGlobalKeySettings.config Gridx and Gridy


-----------------------------------------------


Q. How do I load the dataset?

A. By default the dataset are loaded from a csv file to the Application session. 
Look how it is done in Kunukn.GooglemapsClustering.Web.Application_Start

The csv file is located at App_Data/Points.csv

The format of the csv file is longitude; latitude; point id in GUID format; type id.

The point id and type id data in this example are dummy data. 
The longitude and latitude are parsed to double values. 
point id and type id are parsed as int values.


-----------------------------------------------


Q. How do I set the initial zoom-level and position?

A. In the file mapclustering.js those values are set in the settings area.


-----------------------------------------------


Q. When does the clustering stops?

A. You can define at which zoom-level the clustering should stop. 
In the file mapclustering.js in the settings set a value for zoomlevelClusterStop.


-----------------------------------------------


Q. What are the red grid lines used for?

A. I made them for debugging purpose, i.e. to make sure the clustering behaved as intented.
By default there can only be either one mark inside a grid. Either a cluster-point or a single point.


-----------------------------------------------


Q. How do I enable/disable the red gridlines?

A. In the file mapclustering.js set the showGridLines to false or disable it server-side setting GmcGlobalKeySettings.config DoShowGridLinesInGoogleMap to false. 
I recommend to disable it server-side which will reduce the json-data size for the webservice call.


-----------------------------------------------


Q. How do I extend the viewport data send to the client?

A. Increase or decrease the value in GmcGlobalKeySettings.config OuterGridExtend


-----------------------------------------------


Q. How do I control the cluster merging point?

A. Increase or decrease the value in GmcGlobalKeySettings.config MergeWithin


-----------------------------------------------


Q. How do I set the minimum count of points level before a cluster is made?

A. Increase or decrease the value in GmcGlobalKeySettings.config MinClusterSize


-----------------------------------------------


Q. Is the clustering broken if I change the window size?

A. The clustering might behave buggy if you increase the window size. 
A quick fix is to edit the property Kunukn.GooglemapsClustering.Data.ClusterInfo.IsFilterData increase to higher value e.g. ZoomLevel>=7. 
This property is also used for filtering data returned to client. 
If set to never filter, then too much data will be send back to the client when zoomed far in.


-----------------------------------------------


Q. How did you get the idea for the clustering part?

A. Inspiration from http://www.crunchpanorama.com/ and https://developers.google.com/maps/articles/toomanymarkers


-----------------------------------------------


Q. Where did you get the points?

A. The points are taken from the file cities1000 at http://download.geonames.org/export/dump/


-----------------------------------------------


Q. How can I customize this to use most points and keep fast performance?

A. In the file: GmcGlobalKeySettings.config. Set OuterGridExtend to 0. 
Set DoMergeGridIfCentroidsAreCloseToEachOther to false. Set DoShowGridLinesInGoogleMap to false.

In the mapclustering.js in the settings. set gridx and gridy to low numbers, e.g. 2 and 2.


-----------------------------------------------


Q. What license type is this?

A. MIT License.


-----------------------------------------------


Q. How much does it cost to use Google Maps?

A. Google knows the answer. Try this link https://developers.google.com/maps/faq


-----------------------------------------------


Q. What is the complexity of time and space?

A. Time complexity is on average(m*n) and space complexity is O(m*n) where n is the number of points used in total and m is the number of grids returned to the client. 
You are welcome to analyse it yourself :) I use hashset and hashmap to minimize the time complexity.

Time complexity is ~ O(n^2) on worst case but extremely unlikely, 
happens if most centroids are merged with neighbor-centroids (not a likely scenario for common dataset, where data only exists at grid borderlines).


-----------------------------------------------


Q. Why do the red gridlines look wierd when zoomed far out?

A. Because the world wraps and multiple areas are displayed for same latlon values. 
Some more logic can be applied to draw the grid lines properly for those conditions but has been left out. 
This happens only on zoom level below 3.

