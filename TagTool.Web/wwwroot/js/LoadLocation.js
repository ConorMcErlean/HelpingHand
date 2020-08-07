// Map JS
//This div will display Google map
const mapArea = document.getElementById('map');

//This button will set everything into motion when clicked
const actionBtn = document.getElementById('showMe');

//Let's bring in our API_KEY
const __KEY = document.getElementById('APIKey');

const inputLatitude = document.getElementById('LatitudePos'), 
    inputLongitude = document.getElementById('LongitudePos');
 
//Let's declare our Gmap and Gmarker variables that will hold the Map and Marker Objects later on
let Gmap;
let Gmarker;

//Now we listen for a click event on our button
actionBtn.addEventListener('click', e => {
  // hide the button 
  actionBtn.style.display = "none";
  // call Materialize toast to update user 
  loading = bootbox.dialog({
    message: '<div class="spinner-border text-pink" role="status"><span class="sr-only">Loading...</span> </div><p class="text-center mb-0"><i class="fa fa-spin fa-cog"></i> Please wait while we get your location</p>',
    closeButton: true
});
  // get the user's position
  getLocation();

  // Close box if not done
  setTimeout(function(){
    loading.modal('hide');
  }, 10000);
});

function getLocation(){
    // check if user's browser supports Navigator.geolocation
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(displayLocation, showError, options);
    } else {
        bootbox.alert("Sorry, your browser does not support this feature... Please Update your Browser to enjoy it")
    }
  }

// Displays the different error messages
function showError(error){
    mapArea.style.display = "block"
    switch (error.code) {
      case error.PERMISSION_DENIED:
        mapArea.innerHTML = "You denied the request for your location."
        break;
      case error.POSITION_UNAVAILABLE:
        mapArea.innerHTML = "Your Location information is unavailable."
        break;
      case error.TIMEOUT:
        mapArea.innerHTML = "Your request timed out. Please try again"
        break;
      case error.UNKNOWN_ERROR:
        mapArea.innerHTML = "An unknown error occurred please try again after some time."
        break;
    }
  }
  //Makes sure location accuracy is high
  const options = {
    enableHighAccuracy: true
  }

  function displayLocation(position){
    loading.modal('hide');
    const lat = position.coords.latitude;
    const lng = position.coords.longitude;
    updatePosition(lat, lng);
    const latlng = {lat, lng};
    showMap(latlng, lat, lng);
    createMarker(latlng);
    mapArea.style.display = "block";
  }

  function showMap(latlng, lat, lng){
    let mapOptions = {
      center: latlng,
      zoom: 17 // Street - Building level zoom
    };
    Gmap = new google.maps.Map(mapArea, mapOptions);
    Gmap.addListener('drag', function () {
        Gmarker.setPosition(this.getCenter()); // set marker position to map center
      });
      Gmap.addListener('dragend', function () {
        Gmarker.setPosition(this.getCenter()); // set marker position to map center
      });
      Gmap.addListener('idle', function () {
        Gmarker.setPosition(this.getCenter()); // set marker position to map center
        if (Gmarker.getPosition().lat() !== lat || Gmarker.getPosition().lng() !== lng) {
          setTimeout(() => {
            updatePosition(this.getCenter().lat(), this.getCenter().lng()); // update position display
          }, 2000);
        }
      });
  }

  function createMarker(latlng){
    let markerOptions = {
      position: latlng,
      map: Gmap,
      animation: google.maps.Animation.BOUNCE,
      clickable: true
    };
    Gmarker = new google.maps.Marker(markerOptions);
  }
  Gmap.addListener('drag', function () {
    Gmarker.setPosition(this.getCenter()); // set marker position to map center
  });
  Gmap.addListener('dragend', function () {
    Gmarker.setPosition(this.getCenter()); // set marker position to map center
  });
  Gmap.addListener('idle', function () {
    Gmarker.setPosition(this.getCenter()); // set marker position to map center
    if (Gmarker.getPosition().lat() !== lat || Gmarker.getPosition().lng() !== lng) {
      setTimeout(() => {
        updatePosition(this.getCenter().lat(), this.getCenter().lng()); // update position display
      }, 2000);
    }
  });

function updatePosition(lat, lng){
    // Where we will fill the form
    inputLatitude.value = lat;
    inputLongitude.value = lng;
  };