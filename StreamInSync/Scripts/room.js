(function () {
    var roomHub = $.connection.roomHub;
    $.connection.hub.logging = true; //todo: take this out when deployed
    $.connection.hub.start().done(function () {
        roomHub.server.joinRoom(room.Id);
    });

    roomHub.client.updateRoomUsers = function (roomUsers) {
        $(".userList").empty();
        for (user of roomUsers) {
            $(".userList").append("<p>" + user.Username + "</p>");
        }
    };
}());