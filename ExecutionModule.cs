using Discord.Commands;
using System.Threading.Tasks;
using WheatBot.WheatLang;

namespace WheatBot
{
	public class ExecutionModule : ModuleBase<SocketCommandContext>
	{
		[Command("execute")]
		[Summary("Executes any valid code in WheatLang")]
		public async Task ExecuteWheatCodeAsync([Remainder] [Summary("The code to execute")] string code)
		{
			code = code.Trim();

			if (code.StartsWith("```") && code.EndsWith("```"))
				code = code.Substring(3, code.Length - 6);

			code = code.Trim();

			LexerResult lexerResult = new Lexer(code).Tokenize();

			if (!lexerResult.Success)
			{
				await Context.Channel.SendMessageAsync("Error tokenizing code: " + lexerResult.ErrorMessage);
				return;
			}

			ParserResult parserResult = new Parser(lexerResult.Tokens).AssembleTokens();

			if (!parserResult.Success)
			{
				await Context.Channel.SendMessageAsync("Error parsing code: " + parserResult.ErrorMessage);
				return;
			}

			string output = "";

			Runtime runtime = new Runtime(
				parserResult.Statements,
				parserResult.Marks,
				200,
				str => output += "\n" + str,
				("a", 2),
				("b", 4),
				("c", 8),
				("d", 16),
				("e", 32),
				("g", 64),
				("h", 128),
				("i", 256)
				);

			string runtimeError = runtime.Execute();

			if (!string.IsNullOrWhiteSpace(output))
			{
				output = "```\n" + output + "\n```";
				await Context.Channel.SendMessageAsync(output);
			}
			else
				await Context.Channel.SendMessageAsync("no output");

			if (!string.IsNullOrWhiteSpace(runtimeError))
				await Context.Channel.SendMessageAsync("runtime terminated: " + runtimeError);
			return;
		}
	}
}
