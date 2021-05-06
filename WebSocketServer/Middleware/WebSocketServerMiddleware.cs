using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebSocketServer.ocpp.classes;

namespace WebSocketServer.MiddleWare
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        int c = 0;
        public WebSocketServerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public void WriteRequestParam(HttpContext context)
        {
            Console.WriteLine("Request Method: " + context.Request.Method);
            Console.WriteLine("Request Protocol: " + context.Request.Protocol);
            
            
            if(context.Request.Headers != null)
            {
                foreach(var h in context.Request.Headers)
                {
                    Console.WriteLine("--> : " + h.Key + " : " + h.Value);
                }
            }

        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
                {
                    
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    
                    Console.WriteLine("WebSocket Connected");
                    // Console.WriteLine(webSocket.SubProtocol);
                    WriteRequestParam(context);
                    await ReceiveMessageAsync(webSocket, async(result, buffer) => 
                    {
                        
                        if(result.MessageType == WebSocketMessageType.Text)
                        {
                            c++;
                            Console.WriteLine("Message Received");
                            Console.WriteLine($"Message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                            Console.WriteLine("------------------------------------------------------------------------------");
                            if(c == 1)
                            {
                                var buffer2 = new byte[1024 *4];
                            
                                var hej123 = "[3,\n \"BE8BMlFOqvbxuzT11sSMAngSvbDaPzQE5zSa\",\n {\n \"currentTime\": \"2021-05-05T20:53:32.486Z\",\n \"interval\": 5,\n \"status\": \"Accepted\"\n }\n]\n";
                        
                                var json = JsonConvert.SerializeObject(hej123);                        
                                buffer2 = Encoding.UTF8.GetBytes(hej123);

                                await webSocket.SendAsync(buffer2, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            
                            if(c == 3)
                            {
                                var buffer2 = new byte[1024 *4];
                            
                                var hej123 = "[2,\n \"BE8BMlFOqvbxuzT11sSMAngSvbDaPzQE5zSa\",\n \"RequestStartTransaction\"\n]\n";
                        
                                var json = JsonConvert.SerializeObject(hej123);                        
                                buffer2 = Encoding.UTF8.GetBytes(hej123);

                                await webSocket.SendAsync(buffer2, WebSocketMessageType.Text, true, CancellationToken.None);
                            }

                            if(c == 7)
                            {
                                var buffer2 = new byte[1024 *4];
                            
                                var hej123 = "[2,\n \"BE8BMlFOqvbxuzT11sSMAngSvbDaPzQE5zSa\",\n \"RequestStopTransaction\",\n \"123123\"\n]\n\n\n";
                        
                                var json = JsonConvert.SerializeObject(hej123);                        
                                buffer2 = Encoding.UTF8.GetBytes(hej123);

                                await webSocket.SendAsync(buffer2, WebSocketMessageType.Text, true, CancellationToken.None);
                            }

                            return;
                        }   
                        else if(result.MessageType == WebSocketMessageType.Close)
                        {
                            Console.WriteLine("Received Close message");
                            return;
                        }


                    });
                }
                else
                {
                    Console.WriteLine("Hello from await next");
                    await _next(context);
                }
        }

        private async Task ReceiveMessageAsync(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 *4];

            while(socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);

            }
        }


    }
}