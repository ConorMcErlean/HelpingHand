const mapArea = document.getElementById('mapR');
const actionBtn = document.getElementById('showMe');
const Long = Number(document.getElementById('Longitude').value);
const Lat = Number(document.getElementById('Latitude').value);

const latlng = {lat: Lat, lng: Long};
let gmap;
let gmarker;

// Initialize and add the map
function initMap() {
    // The location of for map
    
    // Options for map
    let mapOptions = {
        center: latlng,
        zoom: 17 // Street - Building level zoom
      };

    // The map, centered on location
    var gmap = new google.maps.Map(mapArea, mapOptions);
    // The marker
    let markerOptions = {
        position: latlng,
        map: gmap,
        clickable: true
      };
    var gmarker = new google.maps.Marker(markerOptions);
    mapArea.style.display = "block"
}