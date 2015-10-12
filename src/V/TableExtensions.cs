using System.Data;

namespace VVVV.Nodes.V
{
	public static class TableExtensions
	{
		public static void CreateColumn(this DataTable table, string columnName, string type)
		{
			switch (type)
			{
				case "value":
					table.Columns.Add(columnName, typeof(double));
					break;
				case "string":
					table.Columns.Add(columnName, typeof (string));
					break;
			}
		}
	}
	
}
