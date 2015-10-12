using System;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V.Xlsx
{
	[PluginInfo(Name = "Reader", Category = "XLSX", Author = "alg", Tags = "data visualization")]
	public class ReaderXlsxNode : TableReader
	{
		[Input("Skip Rows", Visibility = PinVisibility.OnlyInspector)] 
		protected ISpread<int> FSkipRowsIn;

		public override void Evaluate(int spreadMax)
		{
			var maxTables = FFileNameIn.SliceCount;
			FTableOut.SliceCount = FHeadersOut.SliceCount = FLoaded.SliceCount = maxTables;

			if (!FFileNameIn.IsChanged && !FHasHeadersIn.IsChanged && !FColumnTypeIn.IsChanged && !FReadIn[0]) return;

			for (var i = 0; i < maxTables; i++)
			{
				var fileInfo = new FileInfo(FFileNameIn[i]);
				var hasHeaders = FHasHeadersIn[i];
			    
                FHeadersOut[i].SliceCount = 0;

				var table = new DataTable();

				FLoaded[i] = false;

				try
				{
					using (var package = new ExcelPackage(fileInfo))
					{
						var workBook = package.Workbook;
						var worksheet = workBook.Worksheets.First();
						
						var cells = worksheet.Cells;

						var rowsCount = worksheet.Dimension.End.Row;
						var columnsCount = worksheet.Dimension.End.Column;

						var headers = new string[columnsCount];
						
						var skipRows = FSkipRowsIn[i];

						
						if (hasHeaders)
						{
							headers = GetHeaders(columnsCount, cells);
							skipRows++;
						}
						else
						{
							for (var j = 0; j < columnsCount; j++)
							{
								headers[j] = "Column " + j;
							}
						}

					    FHeadersOut[i].SliceCount = headers.Length;
                        FHeadersOut[i].AssignFrom(headers);

						for (var j = 0; j < columnsCount; j++)
						{
							table.CreateColumn(headers[j], FColumnTypeIn[i][j]);
						}

						for (var j = 0 + skipRows; j < rowsCount; j++)
						{
							var row = table.NewRow();

							for (var k = 0; k < columnsCount; k++)
							{
								row[k] = cells[j + 1, k + 1].Text;
							}

							table.Rows.Add(row);
						}

						FTableOut[i] = table;
						FLoaded[i] = true;
					}
				}
				catch (Exception ex)
				{
					FLogger.Log(LogType.Error, ex.Message);
				}
			}
		}

		private string[] GetHeaders(int columnsCount, ExcelRange cells)
		{
			var headers = new string[columnsCount];

			for (var i = 0; i < columnsCount; i++)
			{
				headers[0] = cells[1, i + 1].Text;
			}

			return headers;
		}
	}
}
