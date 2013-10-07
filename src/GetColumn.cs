using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Text;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V
{
	public abstract class GetColumn<T> : IPluginEvaluate, IPartImportsSatisfiedNotification
	{
		[Input("Data Table")] 
		public Pin<DataTable> FTableIn;

		[Input("Column Index")] 
		public IDiffSpread<int> FColumnIndexIn;

		[Output("Row Data")]
		public ISpread<ISpread<T>> FRowData;

		protected bool FConnected;

		public void OnImportsSatisfied()
		{
			FTableIn.Connected += OnConnected;
			FTableIn.Disconnected += OnDisconnected;
		}

		private void OnDisconnected(object sender, PinConnectionEventArgs args)
		{
			FConnected = false;
		}

		private void OnConnected(object sender, PinConnectionEventArgs args)
		{
			FConnected = true;
		}

		public void Evaluate(int spreadMax)
		{
			FRowData.SliceCount = FConnected ? spreadMax : 0;

			if ((FTableIn.IsChanged || FColumnIndexIn.IsChanged) && FConnected)
			{
				for (var i = 0; i < spreadMax; i++)
				{
					if(FTableIn[i] == null) return;

					var rows = FTableIn[i].Rows;
					var rowsCount = rows.Count;

					FRowData[i].SliceCount = rowsCount;

					for (var j = 0; j < rowsCount; j++)
					{
						FRowData[i][j] = rows[j].Field<T>(FColumnIndexIn[i]);
					}
				}
			}
		}

		
	}

	[PluginInfo(Name = "GetColumn", Category = "String", Author = "alg")]
	public class GetColumnString : GetColumn<string>{}

	[PluginInfo(Name = "GetColumn", Category = "Value", Author = "alg")]
	public class GetColumnDouble : GetColumn<double> {}
}
