using System.Collections.Generic;
using System.ComponentModel.Composition;
using Newtonsoft.Json.Linq;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V.Json
{
	[PluginInfo(Name = "Parse", Category = "JSON", Help = "Parse JSON using JSONPath Query", Tags = "json")]
	public class ParseNode : IPluginEvaluate
	{
		[Input("JSON")]
		protected IDiffSpread<string> FJsonIn;

		[Input("Reload", IsSingle = true, IsBang = true)] 
		protected ISpread<bool> FReload;

		[Output("Output")]
		protected ISpread<JObject> FObjectsOut;

		private List<JObject> FObjects = new List<JObject>();

		[Import]
		protected ILogger FLogger;

		public void Evaluate(int spreadMax)
		{
			FObjectsOut.SliceCount = spreadMax;

			if (FJsonIn.IsChanged || FReload[0])
			{
				FObjects.Clear();

				for (var i = 0; i < spreadMax; i++)
				{
					try
					{
						var jObject = JObject.Parse(FJsonIn[i]);
						if (jObject != null) FObjects.Add(jObject);
					}
					catch
					{
						FLogger.Log(LogType.Error, "Can't parse JSON at slice " + i);
					}
				}
			}

			FObjectsOut.AssignFrom(FObjects);
		}
	}
}
