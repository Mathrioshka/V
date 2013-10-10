using System;
using System.ComponentModel.Composition;
using System.Data;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V
{
	public abstract class GetColumn<T> : IPluginEvaluate
	{
		[Input("Data Table")]
		public IDiffSpread<DataRow> FRowIn;

		[Input("Column Index")]
		public IDiffSpread<int> FColumnIndexIn;

		[Input("Columns Count")] 
		public IDiffSpread<int> FColumnsCountIn;
		
		[Output("Row Data")]
		public ISpread<T> FRowData;

		[Import]
		public ILogger FLogger;

		public void Evaluate(int spreadMax)
		{
			FRowData.SliceCount = FRowIn[0] == null ? 0 : spreadMax;
			
			if(FRowData.SliceCount == 0) return;
			if(!FRowIn.IsChanged && !FColumnIndexIn.IsChanged && !FColumnsCountIn.IsChanged) return;
			
			for (var i = 0; i < spreadMax; i++)
			{
				if (FRowIn[i] == null) continue;
				try
				{
					FRowData[i] = FRowIn[i].Field<T>(FColumnIndexIn[i] % FColumnsCountIn[i]);
				}
				catch (Exception ex)
				{
					FLogger.Log(LogType.Error, ex.Message);
				}
			}
		}
	}

	[PluginInfo(Name = "GetColumn", Category = "String", Author = "alg")]
	public class GetColumnString : GetColumn<string>{}

	[PluginInfo(Name = "GetColumn", Category = "Value", Author = "alg")]
	public class GetColumnDouble : GetColumn<double> {}
}
