using System;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.V
{
	[PluginInfo(Name = "Reader", Category = "CSV", Author = "alg", Tags = "data visualization")]
	public class ReaderCsvNode : IPluginEvaluate
	{
		[Input("Filename", StringType = StringType.Filename)] 
		public IDiffSpread<string> FFileNameIn;

		[Input("Has Headers", IsToggle = true)] 
		public IDiffSpread<bool> FHasHeadersIn;

		[Input("Column Type", DefaultString = "string")] 
		public IDiffSpread<ISpread<string>> FColumnTypeIn;

		[Input("Delimiter", DefaultString = ";", Visibility = PinVisibility.OnlyInspector)] 
		public IDiffSpread<string> FDelimiterIn;

		[Input("Quote Character", DefaultString = "\"", Visibility = PinVisibility.OnlyInspector)] 
		public IDiffSpread<string> FQuoteCharIn;

		[Input("Escape Chararacter", DefaultString = "\\", Visibility = PinVisibility.OnlyInspector)] 
		public IDiffSpread<string> FEcapeCharIn;

		[Input("Comment Character", DefaultString = "#", Visibility = PinVisibility.OnlyInspector)]
		public IDiffSpread<string> FCommentCharIn;
		
		[Output("Data Table")] 
		public ISpread<DataTable> FTableOut;

		[Import] 
		public ILogger FLogger;

		public void Evaluate(int spreadMax)
		{
			var maxTables = FFileNameIn.SliceCount;
			FTableOut.SliceCount = maxTables;

			if (!FFileNameIn.IsChanged && !FHasHeadersIn.IsChanged && !FColumnTypeIn.IsChanged && !FDelimiterIn.IsChanged 
					&& !FQuoteCharIn.IsChanged && !FEcapeCharIn.IsChanged && !FCommentCharIn.IsChanged) return;

			for (var i = 0; i < maxTables; i++)
			{	
				var hasHeaders = FHasHeadersIn[i];

				var table = new DataTable();

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
