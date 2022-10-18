var test = null;

var myVal = ""; // Drop down selected value of reader 
var disabled = true;
var startEnroll = false;
var currentFormat = Fingerprint.SampleFormat.PngImage;
var FingerprintSdkTest = (function () {
    function FingerprintSdkTest() {
        var _instance = this;
        this.operationToRestart = null;
        this.acquisitionStarted = false;
        this.sdk = new Fingerprint.WebApi;
        this.sdk.onDeviceConnected = function (e) {
            // Detects if the deveice is connected for which acquisition started
            showMessage("Scan your finger");
        };
        this.sdk.onDeviceDisconnected = function (e) {
            // Detects if device gets disconnected - provides deviceUid of disconnected device
            showMessage("Device disconnected");
        };
        this.sdk.onCommunicationFailed = function (e) {
            // Detects if there is a failure in communicating with U.R.U web SDK
            showMessage("Communinication Failed")
        };
        this.sdk.onSamplesAcquired = function (s) {
            // Sample acquired event triggers this function
            sampleAcquired(s);
        };
        this.sdk.onQualityReported = function (e) {
            // Quality of sample aquired - Function triggered on every sample acquired
            showMessage(Fingerprint.QualityCode[(e.quality)]);
            var qualityElem = document.getElementById('quality');
            qualityElem.innerText = Fingerprint.QualityCode[(e.quality)];
            
        }

    }

    FingerprintSdkTest.prototype.startCapture = function () {
        if (this.acquisitionStarted) // Monitoring if already started capturing
            return;
        var _instance = this;
        showMessage("");
        this.operationToRestart = this.startCapture;
        this.sdk.startAcquisition(currentFormat, myVal).then(function () {
            _instance.acquisitionStarted = true;

            //Disabling start once started

        }, function (error) {
            showMessage(error.message);
        });
    };
    FingerprintSdkTest.prototype.stopCapture = function () {
        if (!this.acquisitionStarted) //Monitor if already stopped capturing
            return;
        var _instance = this;
        showMessage("");
        this.sdk.stopAcquisition().then(function () {
            _instance.acquisitionStarted = false;


        }, function (error) {
            showMessage(error.message);
        });
    };

    FingerprintSdkTest.prototype.getInfo = function () {
        var _instance = this;
        return this.sdk.enumerateDevices();
    };

    FingerprintSdkTest.prototype.getDeviceInfoWithID = function (uid) {
        var _instance = this;
        return this.sdk.getDeviceInfo(uid);
    };


    return FingerprintSdkTest;
})();

function showMessage(message) {
    console.log(message);
}

window.onload = function () {

    test = new FingerprintSdkTest();

};


function onStart() {
    test.startCapture();

}

function onStop() {
    test.stopCapture();
}

function onGetInfo() {
    var allReaders = test.getInfo();
    allReaders.then(function (sucessObj) {
        populateReaders(sucessObj);
    }, function (error) {
        showMessage(error.message);
    });
}


function sampleAcquired(s) {
    // If sample acquired format is PNG- perform following call on object recieved 
    // Get samples from the object - get 0th element of samples as base 64 encoded PNG image         
    // localStorage.setItem("imageSrc", "");                
    var samples = JSON.parse(s.samples);
    var base64 = Fingerprint.b64UrlTo64(samples[0]);
    var image = "data:image/png;base64," + base64;
    var fpdata = document.getElementById('fpdata1');
    fpdata.value = base64;
    var img = document.getElementById('finger1');
    img.src = image;
    if (image) {
            document.getElementById('verifyBtn').click();
           /*  setTimeout(() => {
        }, 3000); */
    }
    // if(state == document.getElementById("content-capture")){ 
    //    
    // }
}