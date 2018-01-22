using System;
using System.Threading;
using MachineStreamApi.Models;
using Microsoft.Extensions.Logging;
using WebSocket4Net;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace MachineStreamApi
{
    public class MachineStreamListener : IDisposable
    {
        private readonly EventRepository _eventRepository;
        private readonly ILogger<MachineStreamListener> _logger;

        public MachineStreamListener(string webSocketUrl,  EventRepository eventRepository, ILogger<MachineStreamListener> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }
        private WebSocket _websocket;
        private Timer _timer;

        public void Start()
        {
            _websocket = new WebSocket("ws://machinestream.herokuapp.com/ws");
            _websocket.Opened += (sender, args) => { _logger.LogInformation("WebSocket opened"); };
            _websocket.Error += (sender, args) => { _logger.LogError("Error occurred" + args.Exception.Message); };
            _websocket.MessageReceived += OnMessageReceived;
            _websocket.Open();
            _timer = new Timer(_ => KeepAlive(), new object(), TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(50));
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            _logger.LogInformation("Received message");
            _logger.LogInformation(args.Message);
            var streamData = TryReadEventData(args.Message);
            if (streamData?.Payload != null)
            {
                _eventRepository.AddEventDataRecord(streamData.Payload);
            }
        }

        private MachineStreamData TryReadEventData(string eventPayload)
        {
            _logger.LogInformation("Trying to read the data");
            var data = JsonConvert.DeserializeObject<MachineStreamData>(eventPayload);
            _logger.LogInformation("Read the data");
            return data;
        }

        private void KeepAlive()
        {
            _logger.LogInformation("Keeping the socket connection alive...");
            _websocket.Send("");
        }

        public void Dispose()
        {
            if (_websocket == null)
                return;
            _timer?.Dispose();

            _websocket.MessageReceived -= OnMessageReceived;
            _websocket.Close();
            _websocket.Dispose();
        }
    }
}
