using System.Data;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V.DTable
{
	[PluginInfo(Name = "AsRows", Author = "alg", Category = "DataTable", Tags = "csv", Help = "Converts DataTale to spread of rows")]
	public class AsRowsNode : IPluginEvaluate
	{
		[Input("Data Table")] 
		protected IDiffSpread<DataTable> FTableIn;

		[Output("Data Row")] 
		protected ISpread<ISpread<DataRow>> FRowsOut;

		public void Evaluate(int spreadMax)
		{
			FRowsOut.SliceCount = FTableIn[0] == null ? 0 : FTableIn.SliceCount;

			if(!FTableIn.IsChanged) return;

			for (var i = 0; i < spreadMax; i++)
			{
				if (FTableIn[i] == null) continue;

				FRowsOut[i].AssignFrom(FTableIn[i].AsEnumerable());	
			}
		}
	}
}
