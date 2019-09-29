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

let updateServerProgrammeTime = function (seconds, playStatus) {
    roomHub.server.updateServerProgrammeTime({
        RoomId: roomData.RoomId,
        ProgrammeTimeSecs: seconds,
        LastUpdated: moment.utc().format(),
        PlayStatus: playStatus,
        InBreak: InBreak,
        BreakTimeSecs: null
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
        timeElement,
        updateCallback;

    function init(initTimeElement, initSeconds, initMaxSeconds, playStatus, initupdateCallback) {
        timeElement = initTimeElement;
        seconds = initSeconds;
        maxSeconds = initMaxSeconds;
        updateCallback = initupdateCallback;

        $("#play-button").click(function () {
            play();
            updateCallback(seconds, PlayStatus.Playing);
        });
        $("#pause-button").click(function () {
            pause();
            updateCallback(seconds, PlayStatus.Paused);
        });
        $("#update-time-button").click(function () {
            let hours = $("#time-hours")[0].valueAsNumber || 0
            let mins = $("#time-mins")[0].valueAsNumber || 0
            let secs = $("#time-secs")[0].valueAsNumber || 0

            seconds = (hours * 60 * 60) + (mins * 60) + secs;
            render();
            pause();
            updateCallback(seconds, PlayStatus.Paused);
        });

        if (playStatus === PlayStatus.Playing) {
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
            updateCallback(seconds, PlayStatus.Finished);
        }
        else {
            seconds += 1;
        }
        render();
    }

    function render() {
        timeElement.html(moment().set({h: 0, m: 0, s: 0}).seconds(seconds).format('H:mm:ss'));
    }

    return {
        init: init
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
    ProgrammeTimer.init($("#programme-time"), CurrentProgrammeTime(roomData.ProgrammeTimeSecs, roomData.LastUpdatedTime, roomData.PlayStatus), roomData.TotalRuntimeSeconds, roomData.PlayStatus, updateServerProgrammeTime);
};