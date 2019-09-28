const PlayStatus = {
    NotStarted: 0,
    Playing: 1,
    Paused: 2,
    Finished: 3
}

var roomHub = $.connection.roomHub;
$.connection.hub.logging = true; //todo: take this out when deployed
$.connection.hub.start().done(function () {
    roomHub.server.joinRoom({ RoomId: roomData.RoomId, LastUpdated: new Date().getTime() });
});

roomHub.client.updateRoomUsers = function (roomUsers) {
    $(".userList").empty();
    var membersExceptCurrentUser = roomUsers.filter(function (value, index, arr) {
        return value.UserId !== roomData.UserId;
    });

    var roomMemberCount = membersExceptCurrentUser.length;
    roomMemberTimers = [roomMemberCount];
    
    for (var i = 0; i < roomMemberCount; i++) {
        var member = membersExceptCurrentUser[i];
        $(".userList").append(`<p>${member.Username}  <time id="room-member-timer-${i}"></time>  ${member.InBreak}</p>`);
        roomMemberTimers[i] = roomMemberTimer.init($(`#room-member-timer-${i}`), member.ProgrammeTimeSecs, member.PlayStatus);
    }
};

var updateRoomMembers = function (seconds, playStatus, InBreak) {
    roomHub.server.updateRoomMember({
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

var roomMemberTimer = (function () {

    var seconds,
        interval,
        timeElement;

    function init(initTimeElement, initSeconds, playStatus) {
        timeElement = initTimeElement;
        seconds = initSeconds;
        if (playStatus === PlayStatus.Playing) {
            play();
        }
        render();
    }

    function play() {
        if (!interval) {
            interval = setInterval(function () {
                seconds += 1;
                render();
            },
            1000);
        }
    }

    function pause() {
        if (interval) {
            clearInterval(interval);
            interval = null;
        }
    }

    function render() {
        timeElement.html(moment().set({ h: 0, m: 0, s: 0 }).seconds(seconds).format('H:mm:ss'));
    }

    return {
        init: init
    }
})();
//var roomMemberTimer 
//$("#programme-time").html(moment().set({ h: 0, m: 0, s: 0 }).seconds(seconds).format('H:mm:ss'));

var ProgrammeTimer = (function () {

    var seconds,
        interval,
        timeElement;

    function init(initTimeElement, initSeconds, playCallback, pauseCallback, updateTimeCallback) {
        $("#play-button").click(function () {
            play();
            playCallback(seconds, PlayStatus.Playing);
        });
        $("#pause-button").click(function () {
            pause();
            pauseCallback(seconds, PlayStatus.Paused);
        });
        $("#update-time-button").click(function () {
            var hours = $("#time-hours")[0].valueAsNumber || 0
            var mins = $("#time-mins")[0].valueAsNumber || 0
            var secs = $("#time-secs")[0].valueAsNumber || 0

            seconds = (hours * 60 * 60) + (mins * 60) + secs;
            render();
            pause();
            updateTimeCallback(seconds, PlayStatus.Paused);
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
    ProgrammeTimer.init($("#programme-time"), 0, updateRoomMembers, updateRoomMembers, updateRoomMembers);
};