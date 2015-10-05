using System;
using System.Linq;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V
{
	[PluginInfo(Name = "AsId", Category = "String", Author = "alg", Tags = "data visualization", Help = "Converts string to lower case letters and digits only, so it can be used as ID")]
	public class AsIdNode : IPluginEvaluate
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
				FOutput[i] = new String(FInput[i].Where(Char.IsLetterOrDigit).ToArray()).ToLower();
			}
		}
	}
}
