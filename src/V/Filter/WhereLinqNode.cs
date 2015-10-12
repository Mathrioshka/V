using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V.Filter
{
	[PluginInfo(Name = "Where", Author = "alg")]
	public class WhereLinqNode : IPluginEvaluate
	{
		[Input("Data Table")]
		protected IDiffSpread<DataTable> FDataTableIn;

		[Input("Where Expression", DefaultString = "row => Convert.ToDouble(row[0]) >= 37")] 
		protected IDiffSpread<string> FExpressionIn;

		[Input("Refresh", IsBang = true, IsSingle = true)] 
		protected ISpread<bool> FRefreshIn;
		
		[Output("Rows")] 
		protected ISpread<ISpread<DataRow>> FRowsOut;
		
		[Import] 
		protected ILogger FLogger; 

		public void Evaluate(int spreadMax)
		{
			FRowsOut.SliceCount = FDataTableIn[0] == null ? 0 : FDataTableIn.SliceCount;

			if(!FDataTableIn.IsChanged && !FExpressionIn.IsChanged && !FRefreshIn[0]) return;
			
			for (var i = 0; i < spreadMax; i++)
			{
				if(FDataTableIn[i] == null) continue;

				IQueryable<DataRow> result = null;

				try
				{
					var query = FDataTableIn[i].AsEnumerable().AsParallel().AsQueryable();
					result = query.Where(FExpressionIn[i], new object());
				}
				catch (Exception ex)
				{
					FLogger.Log(LogType.Error, ex.Message);
				}

				FRowsOut[i].AssignFrom(result);
			}
		}
	}
}
