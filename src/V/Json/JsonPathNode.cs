using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using JsonPath;
using Newtonsoft.Json.Linq;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V.Json
{
	[PluginInfo(Name = "JSONPath", Category = "JSON", Help = "Get values by JSONPath query", Tags = "")]
	public class JsonPathNode : IPluginEvaluate
	{
		[Input("Objects")] 
		protected IDiffSpread<JObject> FJObjectIn;

		[Input("JSONPath Query")] 
		protected IDiffSpread<string> FQueryIn;
		
		[Input("Parse", IsBang = true, IsSingle = true)] 
		protected ISpread<bool> FParseIn;
		
		[Output("Output")] 
		protected ISpread<ISpread<string>> FDataOut;

		[Import]
		protected ILogger FLogger;

		private readonly List<List<string>> FData = new List<List<string>>(1);
		private readonly JsonPathContext FParser = new JsonPathContext { ValueSystem = new JsonNetValueSystem() };

		public void Evaluate(int spreadMax)
		{
			FDataOut.SliceCount = spreadMax;

			if(FParseIn[0] || FQueryIn.IsChanged)
			{
				FData.Clear();
				for (var i = 0; i < spreadMax; i++)
				{
					try
					{
						var values = FParser.SelectNodes(FJObjectIn[i], FQueryIn[i]).Select(node => node.Value);
						var list = values.ToList();

						FData.Add(new List<string>());

						for (var j = 0; j < values.Count(); j++)
						{
							FData[i].Add(list[j].ToString());
						}
					}
					catch (Exception ex)
					{
						//FLogger.Log(LogType.Error, "Query is wrong at slice " + i);
						
						FLogger.Log(LogType.Error, ex.ToString());
					}
				}
			}

			if(FData.Count == 0) return;

			for (var i = 0; i < spreadMax; i++)
			{
				FDataOut[i].AssignFrom(FData[i]);
			}
		}
	}
}
