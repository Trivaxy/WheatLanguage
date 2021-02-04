using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WheatBot
{
	public static class Program
	{
		public static DiscordSocketClient Client { get; private set; }

		public static CommandService CommandService { get; private set; }

		static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

		public static async Task MainAsync()
		{
			string token = File.ReadAllText("token.txt").Trim();

			Client = new DiscordSocketClient();
			await Client.LoginAsync(Discord.TokenType.Bot, token);
			await Client.StartAsync();

			CommandService = new CommandService();
			await MessageHandler.RegisterCommandsAsync();

			await Task.Delay(-1);
		}
	}
}
