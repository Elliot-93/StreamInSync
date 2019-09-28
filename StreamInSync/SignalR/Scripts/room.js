'use strict';

const PlayStatus = {
    NotStarted: 0,
    Playing: 1,
    Paused: 2,
    Finished: 3
}

var roomMemberTimers = [];

var roomHub = $.connection.roomHub;
$.connection.hub.logging = true; //todo: take this out when deployed
$.connection.hub.start()
    .done(function () {
        roomHub.server.joinRoom({ RoomId: roomData.RoomId, LastUpdated: new Date().getTime() });
    }).fail(function (error) {
        console.log('Failed to start hub: ' + error);
    });

roomHub.client.updateRoomUsers = function (roomUsers) {
    $(".userList").empty();
    // removes intervals so old timers can be garbage collected when array reset
    for (var member of roomMemberTimers) {
        clearInterval(member.interval);
    }

    let roomMembers = roomUsers.filter(function (value, index, arr) {
        return value.UserId !== roomData.UserId;
    }).sort((a, b) => { return a.UserId - b.UserId });

    let roomMemberCount = roomMembers.length;
    roomMemberTimers = [roomMemberCount];
    
    for (var i = 0; i < roomMemberCount; i++) {
        let member = roomMembers[i];
        $(".userList").append(`<p>${member.Username}  <time id="room-member-timer-${member.UserId}"></time>  ${member.InBreak}</p>`);

        let inferredProgrammeTime;
        if (member.playStatus === PlayStatus.Playing) {
            var secondsSinceUpdated = moment.utc().diff(moment.utc(member.LastUpdated), 'seconds');
            inferredProgrammeTime = member.ProgrammeTimeSecs + secondsSinceUpdated;
        }
        else {
            inferredProgrammeTime = member.ProgrammeTimeSecs;
        }

        roomMemberTimers[i] = new RoomMemberTimer(member.UserId, inferredProgrammeTime , member.PlayStatus);
    }
};

var updateServerProgrammeTime = function (seconds, playStatus, InBreak) {
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
    constructor(memberId, seconds, playStatus) {
        this.memberId = memberId;
        this.seconds = seconds;
        this.interval = null;
        this.id = timerId;

        timerId++;

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

    update() {
        this.seconds += 1;
        this.render();
    }

    pause() {
        if (this.interval) {
            clearInterval(this.interval);
            this.interval = null;
        }
    }

    render() {
        $(`#room-member-timer-${this.memberId}`).html(moment().set({ h: 0, m: 0, s: 0 }).seconds(this.seconds).format('H:mm:ss'));
    }
};


var ProgrammeTimer = (function () {

    var seconds,
        interval,
        timeElement;

    function init(initTimeElement, initSeconds, updateCallback) {
        $("#play-button").click(function () {
            play();
            updateCallback(seconds, PlayStatus.Playing);
        });
        $("#pause-button").click(function () {
            pause();
            updateCallback(seconds, PlayStatus.Paused);
        });
        $("#update-time-button").click(function () {
            var hours = $("#time-hours")[0].valueAsNumber || 0
            var mins = $("#time-mins")[0].valueAsNumber || 0
            var secs = $("#time-secs")[0].valueAsNumber || 0

            seconds = (hours * 60 * 60) + (mins * 60) + secs;
            render();
            pause();
            updateCallback(seconds, PlayStatus.Paused);
        });

        timeElement = initTimeElement;
        seconds = initSeconds;
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
        seconds += 1;
        render();
    }

    function render() {
        timeElement.html(moment().set({h: 0, m: 0, s: 0}).seconds(seconds).format('H:mm:ss'));
    }

    return {
        init: init
    }
})();

window.onload = function () {
    ProgrammeTimer.init($("#programme-time"), 0, updateServerProgrammeTime);
};