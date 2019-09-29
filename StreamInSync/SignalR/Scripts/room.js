'use strict';

let roomMemberTimers = [];

let roomHub = $.connection.roomHub;
$.connection.hub.logging = true; //todo: take this out when deployed

roomHub.client.updateRoomUsers = function (roomUsers) {
    $(".userList").empty();
    // removes intervals so old timers can be garbage collected when array reset
    for (let member of roomMemberTimers) {
        clearInterval(member.interval);
    }

    let roomMembers = roomUsers.filter(function (value, index, arr) {
        return value.UserId !== roomData.UserId;
    }).sort((a, b) => { return a.UserId - b.UserId });

    let roomMemberCount = roomMembers.length;
    roomMemberTimers = [roomMemberCount];

    for (let i = 0; i < roomMemberCount; i++) {
        let member = roomMembers[i];
        $(".userList").append(`<p>${member.Username}  Running Time: <time id="room-member-timer-${member.UserId}"></time>  ${PlayStatusText(member.PlayStatus, member.InBreak)}</p>`);

        let calculatedProgrammeTime = CurrentProgrammeTime(member.ProgrammeTimeSecs, member.LastUpdated, member.PlayStatus);

        roomMemberTimers[i] = new RoomMemberTimer(member.UserId, calculatedProgrammeTime, roomData.TotalRuntimeSeconds, member.PlayStatus);
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
    
class RoomMemberTimer {
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
};


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
            let hours = $("#time-hours")[0].valueAsNumber || 0
            let mins = $("#time-mins")[0].valueAsNumber || 0
            let secs = $("#time-secs")[0].valueAsNumber || 0

            seconds = (hours * 60 * 60) + (mins * 60) + secs;
            render();
            pause();
            updateCallback(PlayStatus.Paused);
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
        $("#programme-time").html(moment().set({ h: 0, m: 0, s: 0 }).seconds(seconds).format('H:mm:ss'));
    }

    function getSeconds() {
        return seconds;
    }

    return {
        init: init,
        seconds: () => { return seconds; },
        pause: pause,
        play: play
    }
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
        if (seconds != null && seconds <= 0) {
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
            $("#break-time").html("Disabled")
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
    }
})();

const PlayStatus = {
    NotStarted: 0,
    Playing: 1,
    Paused: 2,
    Finished: 3
}

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
            return "Finished Watching"
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

window.onload = function () {
    ProgrammeTimer.init(CurrentProgrammeTime(roomData.ProgrammeTimeSecs, roomData.LastUpdatedTime, roomData.PlayStatus), roomData.TotalRuntimeSeconds, roomData.PlayStatus, roomData.InBreak, updateServerProgrammeTime);

    let breakTimeSeconds = roomData.InBreak ? CurrentProgrammeTime(-roomData.BreakTimeSecs, roomData.LastUpdatedTime, roomData.PlayStatus) : null;
    BreakTimer.init(breakTimeSeconds, roomData.PlayStatus, roomData.InBreak, updateServerProgrammeTime);
};