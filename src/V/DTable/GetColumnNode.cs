using System;
using System.ComponentModel.Composition;
using System.Data;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V.DTable
{
	public abstract class GetColumn<TOut, TIndex> : IPluginEvaluate
	{
		[Input("Data Row")]
		protected IDiffSpread<DataRow> FRowIn;

		[Input("Column", MinValue = 0)]
		protected IDiffSpread<TIndex> FColumnIndexIn;
		
		[Output("Row Data")]
		protected ISpread<TOut> FRowDataOut;

		[Import]
		protected ILogger FLogger;

		public void Evaluate(int spreadMax)
		{
			if(spreadMax == 0) return;

			FRowDataOut.SliceCount = FRowIn[0] == null ? 0 : spreadMax;
			
			if(FRowDataOut.SliceCount == 0) return;

			if(!FRowIn.IsChanged && !FColumnIndexIn.IsChanged) return;
			
			for (var i = 0; i < spreadMax; i++)
			{
				if (FRowIn[i] == null) continue;
				try
				{
                    AssignValue(i);
				}
				catch (Exception ex)
				{
					FLogger.Log(LogType.Error, ex.Message);
				}
			}
		}

	    protected abstract void AssignValue(int sliceIndex);
	}

    [PluginInfo(Name = "GetColumn", Category = "String", Version = "Index", Author = "alg")]
    public class GetColumnByIndexString : GetColumn<string, int>
    {
        protected override void AssignValue(int sliceIndex)
        {
            var columnsCount = FRowIn[sliceIndex].Table.Columns.Count;

            FRowDataOut[sliceIndex] = FRowIn[sliceIndex].Field<string>(FColumnIndexIn[sliceIndex] % columnsCount);
        }
    }

//    [PluginInfo(Name = "GetColumn", Category = "Value", Version = "Index", Author = "alg")]
//    public class GetColumnByIndexDouble : GetColumn<double, int>
//    {
//        protected override void AssignValue(int sliceIndex)
//        {
//            var columnsCount = FRowIn[sliceIndex].Table.Columns.Count;
//
//            FRowDataOut[sliceIndex] = FRowIn[sliceIndex].Field<double>(FColumnIndexIn[sliceIndex] % columnsCount);
//        }
//    }

//    [PluginInfo(Name = "GetColumn", Category = "Value", Version = "Name", Author = "alg")]
//    public class GetColumnByNameDouble : GetColumn<double, string>
//    {
//        protected override void AssignValue(int sliceIndex)
//        {
//            FRowDataOut[sliceIndex] = (double) FRowIn[sliceIndex][FColumnIndexIn[sliceIndex]];
//        }
//    }

    [PluginInfo(Name = "GetColumn", Category = "String", Version = "Name", Author = "alg")]
    public class GetColumnByNameString : GetColumn<string, string>
    {
        protected override void AssignValue(int sliceIndex)
        {
            FRowDataOut[sliceIndex] = (string) FRowIn[sliceIndex][FColumnIndexIn[sliceIndex]];
        }
    }
}
