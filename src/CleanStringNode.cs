using System;
using System.Linq;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V
{
	[PluginInfo(Name = "CleanSymbols", Category = "String", Author = "alg", Tags = "data visualization")]
	public class CleanStringNode : IPluginEvaluate
	{
		[Input("Input", DefaultString = "text")] 
		public IDiffSpread<string> FInput;

		[Output("Output")]
		public ISpread<string> FOutput; 
		
		public void Evaluate(int spreadMax)
		{
			FOutput.SliceCount = spreadMax;

			if(!FInput.IsChanged) return;

			for (var i = 0; i < spreadMax; i++)
			{
				FOutput[i] = new String(FInput[i].Where((c => Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || Char.IsWhiteSpace(c) && c != '"')).ToArray());
			}
		}
	}
}
