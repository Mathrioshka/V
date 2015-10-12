using System.Linq;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V.String
{
	[PluginInfo(Name = "CleanSymbols", Category = "String", Author = "alg", Tags = "data visualization")]
	public class CleanSymbolsNode : IPluginEvaluate
	{
		[Input("Input", DefaultString = "vvvv")] 
		protected IDiffSpread<string> FInput;

		[Output("Output")]
		protected ISpread<string> FOutput; 
		
		public void Evaluate(int spreadMax)
		{
			FOutput.SliceCount = spreadMax;

			if(!FInput.IsChanged) return;

			for (var i = 0; i < spreadMax; i++)
			{
				FOutput[i] = new string(FInput[i].Where((c => char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c) && c != '"')).ToArray());
			}
		}
	}
}
