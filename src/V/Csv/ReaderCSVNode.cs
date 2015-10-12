using System;
using System.Data;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V.Csv
{
	[PluginInfo(Name = "Reader", Category = "CSV", Author = "alg", Tags = "data visualization")]
	public class ReaderCsvNode : TableReader
	{		
		[Input("Delimiter", DefaultString = ";", Visibility = PinVisibility.OnlyInspector)]
		protected IDiffSpread<string> FDelimiterIn;

		[Input("Quote Character", DefaultString = "\"", Visibility = PinVisibility.OnlyInspector)]
		protected IDiffSpread<string> FQuoteCharIn;

		[Input("Escape Chararacter", DefaultString = "\\", Visibility = PinVisibility.OnlyInspector)]
		protected IDiffSpread<string> FEcapeCharIn;

		[Input("Comment Character", DefaultString = "#", Visibility = PinVisibility.OnlyInspector)]
		protected IDiffSpread<string> FCommentCharIn;

		public override void Evaluate(int spreadMax)
		{
			var maxTables = FFileNameIn.SliceCount;
			FTableOut.SliceCount = FHeadersOut.SliceCount = FLoaded.SliceCount = maxTables;

			if (!FFileNameIn.IsChanged && !FHasHeadersIn.IsChanged && !FColumnTypeIn.IsChanged && !FDelimiterIn.IsChanged 
					&& !FQuoteCharIn.IsChanged && !FEcapeCharIn.IsChanged && !FCommentCharIn.IsChanged && !FReadIn[0]) return;

			for (var i = 0; i < maxTables; i++)
			{	
				var hasHeaders = FHasHeadersIn[i];

				var table = new DataTable();
				FLoaded[i] = false;
				FHeadersOut[i].SliceCount = 0;

				try
				{
					using (var csv = new CsvReader(new StreamReader(FFileNameIn[i]), hasHeaders, FDelimiterIn[i][0], FQuoteCharIn[i][0], 
						FEcapeCharIn[i][0], FCommentCharIn[i][0], ValueTrimmingOptions.UnquotedOnly))
					{
						var fieldCount = csv.FieldCount;
						var headers = new string[fieldCount];

						if (hasHeaders)
						{
							headers = csv.GetFieldHeaders();
						}
						else
						{
							for (var j = 0; j < fieldCount; j++)
							{
								headers[j] = "Column " + j;
							}
						}

					    FHeadersOut[i].SliceCount = headers.Length;
                        FHeadersOut[i].AssignFrom(headers);

						for (var j = 0; j < fieldCount; j++)
						{
							table.CreateColumn(headers[j], FColumnTypeIn[i][j]);
						}

						while (csv.ReadNextRecord())
						{
							var row = table.NewRow();

							for (var j = 0; j < fieldCount; j++)
							{
								row[j] = csv[j];
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
	}
}
