using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Text;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V
{
	public abstract class TableReader : IPluginEvaluate
	{
		[Input("Filename", StringType = StringType.Filename)]
		public IDiffSpread<string> FFileNameIn;

		[Input("Has Headers", IsToggle = true)]
		public IDiffSpread<bool> FHasHeadersIn;

		[Input("Column Type", DefaultString = "string")]
		public IDiffSpread<ISpread<string>> FColumnTypeIn;

		[Input("Read", IsBang = true, IsSingle = true)]
		public ISpread<bool> FReadIn;

		[Output("Data Table")]
		public ISpread<DataTable> FTableOut;

		[Output("Columns Count")]
		public ISpread<int> FColumnsCountOut;

		[Output("Loaded")]
		public ISpread<bool> FLoaded;

		[Import]
		public ILogger FLogger;

		public virtual void Evaluate(int spreadMax)
		{
			throw new NotImplementedException();
		}
	}
}
