using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Strooptest
{
	public class Sockets
	{
		TcpListener server;
		TcpClient client;
		public Action<int> OnLebenReceived;

		int port = 5000;

		public string StarteHost() //nu´tzt letzte 2 Stellen der IP-Adresse als Code
		{
			server = new TcpListener(IPAddress.Any, port);
			server.Start();

			string ip = GetIP();
			string code = ip.Split('.')[3];

			return code;
		}

		public void WarteAufSpieler(Action onConnected)
		{
			client = server.AcceptTcpClient();

			NetworkStream stream = client.GetStream();

			byte[] data = Encoding.UTF8.GetBytes("START");
			stream.Write(data, 0, data.Length);

			onConnected?.Invoke();
		}

		public bool Beitreten(string code) //fügt Code zur IP-Adresse hinzu und verbindet sich
		{
			try
			{
				string ip = "192.168.0." + code; //IP für lokales Netzwerk

				client = new TcpClient();
				client.Connect(ip, port);

				NetworkStream stream = client.GetStream();

				byte[] buffer = new byte[1024];
				int length = stream.Read(buffer, 0, buffer.Length);

				string message = Encoding.UTF8.GetString(buffer, 0, length);

				if (message == "START")
					return true;
			}
			catch { }

			return false;
		}
		public void SendeLeben(int leben)
		{
			if (stream == null) return;

			string msg = leben.ToString();
			byte[] data = Encoding.UTF8.GetBytes(msg);

			stream.Write(data, 0, data.Length);
		}

		void StarteReceiveLoop()
		{
			Thread thread = new Thread(() =>
			{
				byte[] buffer = new byte[1024];

				while (true)
				{
					try
					{
						int length = stream.Read(buffer, 0, buffer.Length);

						if (length > 0)
						{
							string message = Encoding.UTF8.GetString(buffer, 0, length);

							int leben = int.Parse(message);

							OnLebenReceived?.Invoke(leben);
						}
					}
					catch
					{
						break;
					}
				}
			});

			thread.IsBackground = true;
			thread.Start();
		}

		string GetIP() //!!!
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());

			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
					return ip.ToString();
			}

			return "127.0.0.1"; //localhost
		}
	}
}