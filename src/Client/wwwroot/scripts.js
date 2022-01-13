var _stream;

function startVideo() {
    var video = document.getElementById('video');

    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        navigator.mediaDevices.getUserMedia({ video: true }).then(function (stream) {
            _stream = stream;

            _stream.stop = function () {
                this.getAudioTracks().forEach(function (track) {
                    track.stop();
                });
                this.getVideoTracks().forEach(function (track) { //in case... :)
                    track.stop();
                });
            };

            try {
                video.srcObject = stream;
            } catch (error) {
                video.src = window.URL.createObjectURL(stream);
            }
            video.play();
        });
    }
}

function stopVideo(stream) {
    _stream.stop();
}

window.Snap = async (src, dest) => {
    let video = document.getElementById(src);
    let ctx = get2DContext(dest);
    ctx.drawImage(video, 0, 0, 480, 360);
}

// Get image as base64 string
window.GetImageData = async (el, format) => {
    let canvas = document.getElementById(el);
    let dataUrl = canvas.toDataURL(format);
    return dataUrl.split(',')[1];
}

// Helper functions
function get2DContext(el) {
    return document.getElementById(el).getContext('2d');
}
