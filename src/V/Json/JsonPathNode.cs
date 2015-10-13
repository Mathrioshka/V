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

		private readonly JsonPathContext FParser = new JsonPathContext { ValueSystem = new JsonNetValueSystem() };

		public void Evaluate(int spreadMax)
		{
			FDataOut.SliceCount = spreadMax;

			if(FParseIn[0] || FQueryIn.IsChanged)
			{
				for (var i = 0; i < spreadMax; i++)
				{
					try
					{
					    var tokens = FJObjectIn[i].SelectTokens(FQueryIn[i]);

                        var values = new List<string>();
                        foreach (var token in tokens)
                        {
                            //ToDO: Refactor to proper Dict-Func method
                            if (token is JProperty)
                            {
                                values.Add(token.ToString());
                            }
                            else if (token is JValue)
                            {
                                values.Add(token.Value<string>());
                            }
                            else if (token is JArray)
                            {
                                values.AddRange(token.Values<string>());
                            }
					    }

                        if(values.Any())
					    {
                            FDataOut[i].SliceCount = values.Count();
                            FDataOut[i].AssignFrom(values);
					    }
                        else
                        {
                            FDataOut[i].SliceCount = 0;
                        }
					    
					}
					catch (Exception ex)
					{
						FLogger.Log(LogType.Error, ex.ToString());
					}
				}
			}
		}
	}
}
