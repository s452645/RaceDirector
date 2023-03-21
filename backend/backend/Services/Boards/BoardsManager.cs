﻿using backend.Models;
using backend.Models.Hardware;
using backend.Services.Boards.Comms;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Net.WebSockets;

namespace backend.Services.Boards
{
    public class BoardsManager
    {
        private readonly TimeSyncService _timeSyncService;
        private readonly BoardEventsService _boardEventsService;

        private readonly List<PicoWBoard> _picoWBoards = new();

        public BoardsManager(TimeSyncService timeSyncService, BoardEventsService boardEventsService) 
        {
            _timeSyncService = timeSyncService;
            _boardEventsService = boardEventsService;
        }

        public async Task<bool> AddPicoWBoard(PicoWBoard picoWBoard)
        {
            var result = await picoWBoard.ConnectSockets();

            if (result) {
                this._picoWBoards.Add(picoWBoard);
            }

            return result;
        }

        public List<PicoWBoardDto> GetAllBoards()
        {
            return this._picoWBoards.Select(board =>
                new PicoWBoardDto(board.Id, board.SyncSocket.Address, board.IsConnected())
            ).ToList();
        }

        public void LaunchSyncAllBoards(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            _timeSyncService.StartSyncingAll(this._picoWBoards, webSocket, socketFinishedTcs);
        }

        public void LaunchEventHandlingAllBoards(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            _boardEventsService.StartListeningForAll(this._picoWBoards, webSocket, socketFinishedTcs);
        }
    }
}