﻿using System;
using System.Collections.Generic;

namespace Deveel.Data.Sql.Tables {
	public class VirtualTable : JoinedTable {
		private long rowCount;

		public VirtualTable(ITable[] tables, IEnumerable<long>[] rows)
			: base(tables) {
			InitRows(rows);
		}

		public VirtualTable(ITable table, IEnumerable<long> rows)
			: this(new[] {table}, new []{ rows}) {
		}

		public VirtualTable(ObjectName tableName, ITable[] tables, IEnumerable<long>[] rows)
			: base(tableName, tables) {
			InitRows(rows);
		}

		public VirtualTable(ObjectName tableName, ITable table, IEnumerable<long> rows)
			: this(tableName, new[] {table}, new[]{ rows}) {
		}

		public override long RowCount => rowCount;

		protected BigArray<long>[] Rows { get; private set; }

		private void InitRows(IEnumerable<long>[] rowEnum) {
			Rows = new BigArray<long>[Tables.Length];

			for (int i = 0; i < Tables.Length; ++i) {
				Rows[i] = rowEnum[i].ToBigArray();
			}

			if (Rows.Length > 0) {
				rowCount = Rows[0].Length;
			}

		}

		protected override IEnumerable<long> ResolveTableRows(IEnumerable<long> rowSet, int tableNum) {
			var result = rowSet.ToBigArray();
			var curRowList = Rows[tableNum];
			for (long n = result.Length - 1; n >= 0; --n) {
				var row1 = result[n];
				long row2;

				if (row1 == Int64.MaxValue - 1) {
					row2 = 0;
				} else if (row1 < 0 || row1 >= curRowList.Length) {
					throw new InvalidOperationException("Invalid row reference");
				} else {
					row2 = curRowList[row1];
				}

				result[n] = row2;
			}

			return result;
		}
	}
}