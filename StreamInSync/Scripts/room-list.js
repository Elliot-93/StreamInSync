'use strict';

$(document).ready(function () {
    $('#room-table').DataTable({
        "ajax": {
            "url": "/room/list",
            "dataSrc": '',
            "type": "POST"
        },
        "responsive": true,
        columns: [
            { data: 'Name' },
            { data: 'ProgrammeName' },
            { data: 'ProgrammeStartTime' },
            { data: 'Username' },
            { data: 'Link' }
        ],
        columnDefs: [
            {
                targets: 0,
                responsivePriority: 1
            },
            {
                targets: 1,
                responsivePriority: 3
            },
            {
                targets: 2,
                responsivePriority: 2,
                render: (d) => moment().utc(d).format("HH:mm DD/MM")
            },
            {
                targets: 3,
                responsivePriority: 4
            },
            {
                targets: 4,
                responsivePriority: 5,
                render: (d) => `<a href="${d}">Enter</a>`
            }
        ]
    });
});