function initAudio(dotNetRef, elementRef) {
    window.audioApi = createAudioApi(dotNetRef);

    function createAudioApi(ref) {
        const audio = document.getElementById('audio');
        let interval;

        audio.onplay = () => ref.invokeMethodAsync('PlayStarted');
        audio.onpause = () => ref.invokeMethodAsync('PlayPaused');
        audio.onloadstart = (e) => {
            if (interval) { clearInterval(interval); }
            ref.invokeMethodAsync('LoadStarted');
        };
        audio.oncanplay = (e) => ref.invokeMethodAsync('CanPlayTriggered', audio.duration);

        elementRef.addEventListener('click', (e) => {
            const seek = (e.pageX - elementRef.offsetLeft) / elementRef.offsetWidth;
            ref.invokeMethodAsync('Seeked', seek);
        });

        function pause() {
            audio.pause();
            clearInterval(interval);
        }

        function play() {
            audio.play();

            if (interval) { clearInterval(interval); }

            interval = setInterval(() => {
                let end = 0;
                for (let i = 0; i < audio.buffered.length; i++) {
                    let value = audio.buffered.end(i);
                    if (value > end) {
                        end = value;
                    }
                }

                ref.invokeMethodAsync('Progressed', audio.currentTime, end);
            }, 500);
        }

        function seek(time) {
            audio.currentTime = time;
        }

        return {
            pause,
            play,
            seek
        };
    }
}