using System.Linq;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V.String
{
	[PluginInfo(Name = "AsId", Category = "String", Author = "alg", Tags = "V", Help = "Converts string to lower case letters and digits only, so it can be used as ID")]
	public class AsIdNode : IPluginEvaluate
	{
		[Input("Input", DefaultString = "VvVv 1. ,, 8")] 
		protected IDiffSpread<string> FInput;

		[Output("Output")]
		protected ISpread<string> FOutput; 
		
		public void Evaluate(int spreadMax)
		{
			FOutput.SliceCount = spreadMax;

			if(!FInput.IsChanged) return;

			for (var i = 0; i < spreadMax; i++)
			{
				FOutput[i] = new string(FInput[i].Where(char.IsLetterOrDigit).ToArray()).ToLower();
			}
		}
	}
}
