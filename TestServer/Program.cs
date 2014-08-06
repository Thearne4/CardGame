using System;
using Server;
using Server.Broadcast;
using Shared.Broadcast;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var lanLobby = new LanLobby(1608, new Broadcastpackage(Broadcaster.GetMyIp(), 1608, "TestServer", "Description"));
            new LanLobby(1608, new Broadcastpackage(Broadcaster.GetMyIp(), 1608, "TestServer2", "Description2"));
            new LanLobby(1608, new Broadcastpackage(Broadcaster.GetMyIp(), 1608, "TestServer3", "Description3"));
            new LanLobby(1608, new Broadcastpackage(Broadcaster.GetMyIp(), 1608, "TestServer4", "Description4"));
            new LanLobby(1608, new Broadcastpackage(Broadcaster.GetMyIp(), 1608, "TestServer5", "Description5"));
            new LanLobby(1608, new Broadcastpackage(Broadcaster.GetMyIp(), 1608, "TestServer6", "Description6"));
            new LanLobby(1608, new Broadcastpackage(Broadcaster.GetMyIp(), 1608, "TestServer7", "Description7"));

            Console.ReadLine();
        }
    }
}
