﻿using Newtonsoft.Json;
using StreamInSync.Contexts;
using StreamInSync.Models;
using StreamInSync.Services;
using System.Web.Mvc;
using System.Linq;
using StreamInSync.Provider;
using StreamInSync.Enums;
using System;

namespace StreamInSync.Controllers
{
    [Authorize]
    [RoutePrefix("room")]
    public class RoomController : Controller
    {
        private readonly ISessionContext sessionContext;
        private readonly IRoomService roomService;
        private readonly IRoomSummaryVmProvider roomSummaryVmProvider;

        public RoomController()
        {
            sessionContext = new SessionContext();
            roomService = new RoomService();
            roomSummaryVmProvider = new RoomSummaryVmProvider();
        }

        public ActionResult Index(int roomId)
        {
            //todo: get room if public fine. If private check on auth token - authenticatedPrivateRoomIds

            var room = roomService.Get(roomId);

            if (room != null)
            {
                var userId = sessionContext.GetUser().UserId;
                var existingUserRoomData = room.Members.FirstOrDefault(m => m.UserId == userId);
                var roomJson = new
                {
                    room.RoomId,
                    UserId = userId,
                    TotalRuntimeSeconds = room.Runtime.TotalSeconds,    
                    ProgrammeTimeSecs = existingUserRoomData?.ProgrammeTimeSecs ?? 0,
                    PlayStatus = existingUserRoomData?.PlayStatus ?? PlayStatus.Paused,
                    LastUpdatedTime = existingUserRoomData?.LastUpdated ?? DateTime.UtcNow,
                    InBreak = existingUserRoomData?.InBreak ?? false,
                    existingUserRoomData?.BreakTimeSecs
                };

                return View(new RoomVM(room, JsonConvert.SerializeObject(roomJson)));
            }

            return View("~/Views/Shared/Error");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateRoomVM newRoom)
        {
            if (ModelState.IsValid)
            {
                var room = roomService.Create(newRoom, sessionContext.GetUser());

                if (room != null)
                {
                    // ToDo: do this in routing intialisation?
                    return RedirectToAction("Index", new { roomId = room.RoomId });
                }
            }

            return View(newRoom);
        }

        public ActionResult Join()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Join(JoinRoomVM joinRoom)
        {
            if (ModelState.IsValid)
            {
                var room = roomService.Get(joinRoom.InviteCode, joinRoom.Password);

                if (room != null)
                {
                    return RedirectToAction("Index", new { roomId = room.RoomId });
                }
            }

            return View(joinRoom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete/{roomId:int}")]
        public ActionResult Delete(int roomId)
        {
            return Content(roomService.Delete(roomId, sessionContext.GetUser().UserId).ToString());
        }

        public ActionResult List()
        {
            var rooms = roomSummaryVmProvider.BuildRoomViewModels(roomService.GetAllPublicRooms());
            
            return View(rooms);
        }

        [OverrideAuthorization]
        [HttpPost]
        public ActionResult List(string queryParams)
        {
            var roomData = roomSummaryVmProvider
                .BuildRoomViewModels(roomService.GetAllPublicRooms())
                .Select(r =>
                new
                {
                    r.Room.Name,
                    r.Room.ProgrammeName,
                    r.Room.ProgrammeStartTime,
                    r.Room.Owner.Username,
                    r.JoinRoomLink
                });

            //todo: serialise just values then dont need columns defined in .js
            return Content(JsonConvert.SerializeObject(roomData));
        }
    }
}