using System;
using System.ComponentModel.Composition;
using System.Linq.Dynamic;
using System.Data;
using System.Linq;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V
{
	[PluginInfo(Name = "Where", Author = "alg")]
	public class WhereLinqNode : IPluginEvaluate
	{
		[Input("Data Table")]
		public IDiffSpread<DataTable> FDataTableIn;

		[Input("Where Expression", DefaultString = "row => Convert.ToDouble(row[0]) >= 37")] 
		public IDiffSpread<string> FExpressionIn;

		[Output("Rows")] 
		public ISpread<ISpread<DataRow>> FRows;
		
		[Import] 
		public ILogger FLogger; 

		public void Evaluate(int spreadMax)
		{
			FRows.SliceCount = FDataTableIn[0] == null ? 0 : FDataTableIn.SliceCount;

			if(!FDataTableIn.IsChanged && !FExpressionIn.IsChanged) return;
			
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

				FRows[i].AssignFrom(result);
			}
		}
	}
}
