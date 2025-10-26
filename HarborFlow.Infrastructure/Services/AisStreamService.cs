using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure.DTOs.AisStream;
using Microsoft.Extensions.Configuration;

namespace HarborFlow.Infrastructure.Services
{
    public class AisStreamService : IAisStreamService
    {
        public event Action<VesselPosition> PositionReceived = delegate { };

        private readonly ClientWebSocket _webSocket = new ClientWebSocket();
        private readonly IConfiguration _configuration;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public AisStreamService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async void Start()
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            var apiKey = _configuration["ApiKeys:AisStream"];
            var uri = new Uri("wss://stream.aisstream.io/v0/stream");

            try
            {
                await _webSocket.ConnectAsync(uri, _cancellationTokenSource.Token);
                var subscriptionMessage = new
                {
                    APIkey = apiKey,
                    BoundingBoxes = new[] { new[] { new[] { -90, -180 }, new[] { 90, 180 } } }
                };
                var messageBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(subscriptionMessage)));
                await _webSocket.SendAsync(messageBuffer, WebSocketMessageType.Text, true, _cancellationTokenSource.Token);

                await ReceiveMessages(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket connection error: {ex.Message}");
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task ReceiveMessages(CancellationToken token)
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var aisMessage = JsonSerializer.Deserialize<AisStreamMessage>(messageJson);

                    if (aisMessage != null && aisMessage.MessageType == "PositionReport")
                    {
                        var vesselPosition = new VesselPosition
                        {
                            VesselImo = aisMessage.MetaData.Imo.ToString(),
                            Latitude = (decimal)aisMessage.Message.Latitude,
                            Longitude = (decimal)aisMessage.Message.Longitude,
                            PositionTimestamp = DateTime.UtcNow,
                            SpeedOverGround = (decimal)aisMessage.Message.SpeedOverGround,
                            CourseOverGround = (decimal)aisMessage.Message.CourseOverGround
                        };

                        PositionReceived?.Invoke(vesselPosition);
                    }
                }
            }
        }
    }
}