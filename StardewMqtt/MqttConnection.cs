using System;
using System.Text;
using System.Threading;
using StardewModdingAPI;
using uPLibrary.Networking.M2Mqtt;

namespace StardewMqtt
{
	public class MqttConnection
	{
		readonly MqttClient _mqttClient;
		readonly IMonitor _monitor;
		bool _closing = false;
		Timer timer = null;

		public MqttConnection(IMonitor monitor, string host)
		{
			_monitor = monitor;
			_mqttClient = new MqttClient(host);
			//_mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
			_mqttClient.ConnectionClosed += (sender, e) =>
			{
				_monitor.Log("Disconnected from MQTT");
				if (!_closing)
				{
					TryConnect();
				}
			};
		}

		public void Open()
		{
			_closing = false;
			TryConnect();
		}

		public void Close()
		{
			_closing = true;
		}

		private void TryConnect()
		{
			if (timer == null)
			{
				timer = new Timer(OnTimerTick, new object(), TimeSpan.Zero, TimeSpan.FromMinutes(1));
			}
			else
			{
				timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));
			}
		}

		private void OnTimerTick(object stateInfo)
		{
			try
			{
				_monitor.Log("Connecting to MQTT...");

				byte code = _mqttClient.Connect(Guid.NewGuid().ToString());
				if (code == 0)
				{
					if (timer != null)
					{
						Console.WriteLine("Connected to MQTT");
						timer.Change(Timeout.Infinite, Timeout.Infinite);
					}
				}

			}
			catch (Exception ex)
			{
				_monitor.Log("Failed to connect to MQTT.");
			}

		}

		public void Send(string topic, string value)
		{
			Send(topic, value, false);
		}

		public void Send(string topic, string value, bool retain)
		{
			Send(topic, value, 1, retain);
		}

		public void Send(string topic, string value, byte qosLevel, bool retain)
		{
			_mqttClient.Publish(topic, Encoding.UTF8.GetBytes(value), qosLevel, retain);
		}
	}
}

