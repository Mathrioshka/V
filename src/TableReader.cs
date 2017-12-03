using System;
using System.ComponentModel.Composition;
using System.Data;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V
{
	public abstract class TableReader : IPluginEvaluate
	{
		[Input("Filename", StringType = StringType.Filename)]
		protected IDiffSpread<string> FFileNameIn;

		[Input("Has Headers", IsToggle = true)]
		protected IDiffSpread<bool> FHasHeadersIn;

		[Input("Column Type", DefaultString = "string")]
		protected IDiffSpread<ISpread<string>> FColumnTypeIn;

		[Input("Read", IsBang = true, IsSingle = true)]
		protected ISpread<bool> FReadIn;

		[Output("Data Table")]
		protected ISpread<DataTable> FTableOut;

		[Output("Headers")]
		protected ISpread<ISpread<string>> FHeadersOut;

		[Output("Loaded")]
		protected ISpread<bool> FLoaded;

		[Import]
		protected ILogger FLogger;

		public virtual void Evaluate(int spreadMax)
		{
			throw new NotImplementedException();
		}
	}
}
