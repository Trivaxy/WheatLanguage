using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;

namespace WheatBot
{
	public static class MessageHandler
	{
		public static readonly string Prefix = "w!";

		public static async Task RegisterCommandsAsync()
		{
			Program.Client.MessageReceived += HandleMessageAsync;

			await Program.CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
		}

		private static async Task HandleMessageAsync(SocketMessage socketMessage)
		{
			if (!(socketMessage is SocketUserMessage userMessage))
				return;

			int argPos = 0;

			if (!(userMessage.HasStringPrefix(Prefix, ref argPos) || userMessage.HasMentionPrefix(Program.Client.CurrentUser, ref argPos)) || socketMessage.Author.IsBot)
				return;

			SocketCommandContext context = new SocketCommandContext(Program.Client, userMessage);

			await Program.CommandService.ExecuteAsync(context, argPos, null);
		}
	}
}
