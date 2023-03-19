using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Common.Network;


namespace Session
{
    class Program
    {
        static public async void Main(string[] args)
        {
            var tcpSocket = new Server();
            var SM = new Service.SessionManager();

            tcpSocket.Start();
            SM.SendAsync("하이~");

            //var webSocket = new Service.SessionService();
            //webSocket.Start();
        }
    }
}
