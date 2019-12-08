'use strict';

let roomMemberProgrammeTimers = [];
let roomMemberBreakTimers = [];

let roomHub = $.connection.roomHub;
$.connection.hub.logging = true; //todo: take this out when deployed

roomHub.client.updateRoomUsers = function (roomUsers) {
    $(".userList").empty();
    // removes intervals so old timers can be garbage collected when array reset
    for (let timer of roomMemberProgrammeTimers) {
        clearInterval(timer.interval);
    }
    for (let timer of roomMemberBreakTimers) {
        clearInterval(timer.interval);
    }

    let roomMembers = roomUsers.filter(function (value, index, arr) {
        return value.UserId !== roomData.UserId;
    }).sort((a, b) => { return a.UserId - b.UserId; });

    let roomMemberCount = roomMembers.length;
    roomMemberProgrammeTimers = [roomMemberCount];

    for (let i = 0; i < roomMemberCount; i++) {
        let member = roomMembers[i];

        let breakText = "";
        if (member.InBreak) {
            breakText = `<time id="room-member-break-timer-${member.UserId}"></time>`;

            if (member.BreakTimeSecs > 0) {
                roomMemberBreakTimers[i] = new RoomMemberBreakTimer(member.UserId, member.BreakTimeSecs, member.PlayStatus);
            }
        }
        $(".userList").append(`<p>${member.Username} - ${PlayStatusText(member.PlayStatus, member.InBreak)} ${breakText}<br>
                               Running Time: <time id="room-member-timer-${member.UserId}"></time> </p>`);

        let calculatedProgrammeTime = CurrentProgrammeTime(member.ProgrammeTimeSecs, member.LastUpdated, member.PlayStatus);

        let programmePlayStatus = member.InBreak ? PlayStatus.Paused : member.PlayStatus;
        roomMemberProgrammeTimers[i] = new RoomMemberProgrammeTimer(member.UserId, calculatedProgrammeTime, roomData.TotalRuntimeSeconds, programmePlayStatus);
    }
};

$.connection.hub.start()
    .done(function () {
        roomHub.server.joinRoom({ RoomId: roomData.RoomId, LastUpdated: new Date().getTime() });
    }).fail(function (error) {
        console.log('Failed to start hub: ' + error);
    });

let updateServerProgrammeTime = function (playStatus) {
    roomHub.server.updateServerProgrammeTime({
        RoomId: roomData.RoomId,
        ProgrammeTimeSecs: ProgrammeTimer.seconds(),
        LastUpdated: moment.utc().format(),
        PlayStatus: playStatus,
        InBreak: BreakTimer.inBreak(),
        BreakTimeSecs: BreakTimer.seconds()
    }).done(function () {
        console.log('Invocation of updateRoomMember succeeded');
    }).fail(function (error) {
        console.log('Invocation of updateRoomMember failed. Error: ' + error);
    });
};
    
class RoomMemberProgrammeTimer {
    constructor(memberId, seconds, maxSeconds, playStatus) {
        this.memberId = memberId;
        this.seconds = seconds;
        this.maxSeconds = maxSeconds;
        this.interval = null;   

        if (playStatus === PlayStatus.Playing) {
            this.play();
        }
        else {
            this.pause();
        }
        this.render();
    }

    play() {
        if (!this.interval) {
            this.interval = setInterval(() => this.update(), 1000);
        }
    }

    pause() {
        if (this.interval) {
            clearInterval(this.interval);
            this.interval = null;
        }
    }

    update() {
        if (this.seconds >= this.maxSeconds) {
            this.seconds = this.maxSeconds;
            this.pause();
        }
        else {
            this.seconds += 1;
        }
        this.render();
    }

    render() {
        $(`#room-member-timer-${this.memberId}`).html(moment().set({ h: 0, m: 0, s: 0 }).seconds(this.seconds).format('H:mm:ss'));
    }
}

class RoomMemberBreakTimer {
    constructor(memberId, seconds, playStatus) {
        this.memberId = memberId;
        this.seconds = seconds;
        this.interval = null;

        if (playStatus === PlayStatus.Playing) {
            this.play();
        }
        else {
            this.pause();
        }
        this.render();
    }

    play() {
        if (!this.interval) {
            this.interval = setInterval(() => this.update(), 1000);
        }
    }

    pause() {
        if (this.interval) {
            clearInterval(this.interval);
            this.interval = null;
        }
    }

    update() {
        if (this.seconds <= 0) {
            this.seconds = 0;
            this.pause();
        }
        else {
            this.seconds -= 1;
        }
        this.render();
    }

    render() {
        $(`#room-member-break-timer-${this.memberId}`).html(moment().set({ h: 0, m: 0, s: 0 }).seconds(this.seconds).format('H:mm:ss'));
    }
}

let ProgrammeTimer = (function () {
    let seconds,
        maxSeconds,
        interval,
        updateCallback;

    function init(initSeconds, initMaxSeconds, playStatus, inBreak, initupdateCallback) {
        seconds = initSeconds;
        maxSeconds = initMaxSeconds;
        updateCallback = initupdateCallback;

        $("#play-button").click(function () {
            BreakTimer.reset();
            play();
            updateCallback(PlayStatus.Playing);
        });

        $("#pause-button").click(function () {
            pause();
            updateCallback(PlayStatus.Paused);
        });

        $("#update-time-button").click(function () {
            let hours = $("#time-hours")[0].valueAsNumber || 0,
            mins = $("#time-mins")[0].valueAsNumber || 0,
            secs = $("#time-secs")[0].valueAsNumber || 0;

            updateSeconds((hours * 60 * 60) + (mins * 60) + secs);
        });

        if (playStatus === PlayStatus.Playing && !inBreak) {
            play();
        }
        else {
            pause();
        }
        render();
    }

    function play() {
        if (!interval) {
            interval = setInterval(update, 1000);
        }
    }

    function pause() {
        if (interval) {
            clearInterval(interval);
            interval = null;
        }
    }

    function update() {
        if (seconds >= maxSeconds) {
            seconds = maxSeconds;
            pause();
            updateCallback(PlayStatus.Finished);
        }
        else {
            seconds += 1;
        }
        render();
    }

    function render() {
        ProgressBar.render(seconds);
        $("#programme-time").html(moment().set({ h: 0, m: 0, s: 0 }).seconds(seconds).format('H:mm:ss'));
    }

    function updateSeconds(updateSeconds) {
        seconds = Math.round(updateSeconds);
        pause();
        update();
        updateCallback(PlayStatus.Paused);
    }

    return {
        init: init,
        seconds: () => { return seconds; },
        pause: pause,
        play: play,
        updateSeconds : updateSeconds
    };
})();

let BreakTimer = (function () {
    let seconds,
        interval,
        inBreak,
        updateCallback;

    function init(initSeconds, playStatus, initInBreak, initUpdateCallback) {
        seconds = initSeconds;
        updateCallback = initUpdateCallback;
        inBreak = initInBreak;

        $("#break-play-button").click(function () {
            if (!seconds) {
                setBreakTime();
            }
            inBreak = true;
            ProgrammeTimer.pause();
            countDown();
            updateCallback(PlayStatus.Playing);
        });
        $("#break-pause-button").click(function () {
            pause();
            updateCallback(PlayStatus.Paused);
        });
        $("#break-button").click(function () {
            if (!inBreak) {
                inBreak = true;
                setBreakTime();
                countDown();
                $("#break-time-mins")[0].value = null;
                $("#break-time-secs")[0].value = null;
                ProgrammeTimer.pause();
            }
            else {
                reset();
                ProgrammeTimer.play();
            }
            updateCallback(PlayStatus.Playing);
            render();
            
        });

        if (playStatus === PlayStatus.Playing && inBreak) {
            countDown();
        }
        else {
            pause();
        }
        render();
    }

    function setBreakTime() {
        let mins = $("#break-time-mins")[0].valueAsNumber || 0,
            secs = $("#break-time-secs")[0].valueAsNumber || 0;
        if (mins > 0 || secs > 0) {
            seconds = (mins * 60) + secs;
        }
    }

    function countDown() {
        inBreak = true;
        if (!interval) {
            interval = setInterval(update, 1000);
        }
    }

    function pause() {
        if (interval) {
            clearInterval(interval);
            interval = null;
        }
    }

    function update() {
        if (seconds !== null && seconds <= 0) {
            seconds = null;
            inBreak = false;
            pause();
            ProgrammeTimer.play();
            updateCallback(PlayStatus.Playing);
        }
        else if (seconds > 0){
            seconds -= 1;
        }
        render();
    }

    function render() {
        if (inBreak && !seconds) {
            $("#break-time").html("Enabled, duration not set");
        }
        else if (inBreak){
            $("#break-time").html(moment().set({ h: 0, m: 0, s: 0 }).seconds(seconds).format('H:mm:ss'));
        }
        else {
            $("#break-time").html("Disabled");
        }
    }

    function reset() {
        seconds = null;
        inBreak = false;
        render();
    }

    return {
        init: init,
        seconds: () => { return seconds; },
        inBreak: () => { return inBreak; },
        reset: reset
    };
})();

const PlayStatus = {
    NotStarted: 0,
    Playing: 1,
    Paused: 2,
    Finished: 3
};

function PlayStatusText(playStatus, inBreak) {
    let breakText = inBreak ? " Adverts" : "";

    switch (playStatus) {
        case PlayStatus.NotStarted:
            return "Not Started";
        case PlayStatus.Playing:
            return "Playing" + breakText;
        case PlayStatus.Paused:
            return "Paused" + breakText;
        case PlayStatus.Finished:
            return "Finished Watching";
        default:
            return "";
    }
}

function CurrentProgrammeTime(lastRecordedTimeSecs, lastUpdatedTime, playStatus) {
    let calculatedTime = 0;
    if (playStatus === PlayStatus.Playing) {
        let secondsSinceUpdated = moment.utc().diff(moment.utc(lastUpdatedTime), 'seconds');
        calculatedTime = lastRecordedTimeSecs + secondsSinceUpdated;
    }
    else {
        calculatedTime = lastRecordedTimeSecs;
    }
    return calculatedTime;
}

let ProgressBar = (function () {
    let progressBarClickZone,
        progressBar,
        progressBarFill,
        overlay,
        totalRuntimeSecs,
        updateRuntimeSecs,
        editMode;

    function init(initTotalRuntimeSecs) {
        progressBarClickZone = $("#progress-bar-click-zone");
        progressBar = $("#progress-bar");
        progressBarFill = $("#progress-bar-fill");
        overlay = $("#overlay");
        totalRuntimeSecs = initTotalRuntimeSecs;

        let mouseDown = false;

        progressBarClickZone.on('mousedown', function () {
            mouseDown = true;
        });

        progressBarClickZone.on('mouseup mousemove mouseleave',function () {
            if (progressBar.hasClass("selected")) {
                if (mouseDown) {
                    let updateRunningTimeMultiplier = (event.pageY - $(this).offset().top) / $(this).height();
                    updateRuntimeSecs = totalRuntimeSecs * updateRunningTimeMultiplier;
                    setFillBarHeight(updateRuntimeSecs);
                    editMode = true;
                }
            }

            progressBarClickZone.addClass("selected");
            progressBar.addClass("selected");
            overlay.show();
        });

        overlay.click(function () {
            progressBarClickZone.removeClass("selected");
            progressBar.removeClass("selected");

            if (updateRuntimeSecs) {
                ProgrammeTimer.updateSeconds(updateRuntimeSecs); 
            }

            updateRuntimeSecs = null;
            editMode = false;
            overlay.hide();
        });
    }

    function setFillBarHeight(programmeTimeSecs) {
        progressBarFill.height(`${programmeTimeSecs / totalRuntimeSecs * 100}%`);
    }

    function render(programmeTimeSecs)
    {
        if (!editMode) {
            setFillBarHeight(programmeTimeSecs);
        }
    }

    return {
        init: init,
        render : render
    };
})();

window.onload = function () {
    ProgressBar.init(roomData.TotalRuntimeSeconds);

    ProgrammeTimer.init(CurrentProgrammeTime(roomData.ProgrammeTimeSecs, roomData.LastUpdatedTime, roomData.PlayStatus), roomData.TotalRuntimeSeconds, roomData.PlayStatus, roomData.InBreak, updateServerProgrammeTime);

    let breakTimeSeconds = roomData.InBreak ? CurrentProgrammeTime(-roomData.BreakTimeSecs, roomData.LastUpdatedTime, roomData.PlayStatus) : null;
    BreakTimer.init(breakTimeSeconds, roomData.PlayStatus, roomData.InBreak, updateServerProgrammeTime);
};