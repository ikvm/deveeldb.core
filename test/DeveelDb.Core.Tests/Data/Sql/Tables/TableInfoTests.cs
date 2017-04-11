﻿using System;
using System.Text;

using Xunit;

namespace Deveel.Data.Sql.Tables {
	public static class TableInfoTests {
		[Fact]
		public static void NewSimpleTableInfo() {
			var tableName = new ObjectName("tab1");
			var tableInfo = new TableInfo(tableName);
			tableInfo.Columns.Add(new ColumnInfo("a", PrimitiveTypes.BigInt()));
			tableInfo.Columns.Add(new ColumnInfo("b", PrimitiveTypes.VarChar(22)));

			Assert.Equal(tableName, tableInfo.TableName);
			Assert.Equal(2, tableInfo.Columns.Count);
			Assert.Equal(0, tableInfo.Columns.IndexOf("a"));
			Assert.Equal(1, tableInfo.Columns.IndexOf("b"));
			Assert.Equal(0, tableInfo.Columns.IndexOf(ObjectName.Parse("tab1.a")));
			Assert.Equal(1, tableInfo.Columns.IndexOf(ObjectName.Parse("tab1.b")));
			Assert.Equal(ObjectName.Parse("tab1.a"), tableInfo.Columns.GetColumnName(0));
			Assert.Equal(ObjectName.Parse("tab1.b"), tableInfo.Columns.GetColumnName(1));
		}

		[Fact]
		public static void MutateReadOnlyTableInfo() {
			var tableName = new ObjectName("tab1");
			var tableInfo = new TableInfo(tableName);
			tableInfo.Columns.Add(new ColumnInfo("a", PrimitiveTypes.BigInt()));
			tableInfo.Columns.Add(new ColumnInfo("b", PrimitiveTypes.VarChar(22)));

			tableInfo = TableInfo.ReadOnly(tableInfo);

			Assert.Throws<InvalidOperationException>(() => tableInfo.Columns.RemoveAt(0));
		}

		[Fact]
		public static void AliasTableInfo() {
			var tableName = new ObjectName("tab1");
			var tableInfo = new TableInfo(tableName);
			tableInfo.Columns.Add(new ColumnInfo("a", PrimitiveTypes.BigInt()));
			tableInfo.Columns.Add(new ColumnInfo("b", PrimitiveTypes.VarChar(22)));

			var alias = new ObjectName("alias1");
			var aliased = TableInfo.Alias(tableInfo, alias);

			Assert.NotEqual(tableName, aliased.TableName);
			Assert.Equal(aliased.Columns.Count, tableInfo.Columns.Count);
		}

		[Fact]
		public static void GetString() {
			var tableName = new ObjectName("tab1");
			var tableInfo = new TableInfo(tableName);
			tableInfo.Columns.Add(new ColumnInfo("a", PrimitiveTypes.BigInt()));
			tableInfo.Columns.Add(new ColumnInfo("b", PrimitiveTypes.VarChar(22)));

			var sb = new StringBuilder();
			sb.AppendLine("tab1 (");
			sb.AppendLine("  a BIGINT,");
			sb.AppendLine("  b VARCHAR(22)");
			sb.Append(")");

			var expected = sb.ToString();
			var sql = tableInfo.ToString();

			Assert.Equal(expected, sql);
		}

		[Fact]
		public static void NewJoinedTableInfo() {
			var tableName1 = new ObjectName("tab1");
			var tableInfo1 = new TableInfo(tableName1);
			tableInfo1.Columns.Add(new ColumnInfo("a", PrimitiveTypes.BigInt()));
			tableInfo1.Columns.Add(new ColumnInfo("b", PrimitiveTypes.VarChar(22)));

			var tableName2 = new ObjectName("tab2");
			var tableInfo2 = new TableInfo(tableName2);
			tableInfo2.Columns.Add(new ColumnInfo("a", PrimitiveTypes.BigInt()));
			tableInfo2.Columns.Add(new ColumnInfo("b", PrimitiveTypes.VarChar(22)));

			var joinedInfo = new JoinedTableInfo(new TableInfo[]{tableInfo1, tableInfo2});

			Assert.Equal(4, joinedInfo.Columns.Count);

			Assert.Equal(0, joinedInfo.Columns.IndexOf(ObjectName.Parse("tab1.a")));
			Assert.Equal(1, joinedInfo.Columns.IndexOf(ObjectName.Parse("tab1.b")));
			Assert.Equal(2, joinedInfo.Columns.IndexOf(ObjectName.Parse("tab2.a")));
			Assert.Equal(3, joinedInfo.Columns.IndexOf(ObjectName.Parse("tab2.b")));

			Assert.Equal(0, joinedInfo.GetTableOffset(0));
			Assert.Equal(0, joinedInfo.GetTableOffset(1));
			Assert.Equal(1, joinedInfo.GetTableOffset(2));
			Assert.Equal(1, joinedInfo.GetTableOffset(3));

			Assert.Equal(ObjectName.Parse("tab1.a"), joinedInfo.Columns.GetColumnName(0));
			Assert.Equal(ObjectName.Parse("tab1.b"), joinedInfo.Columns.GetColumnName(1));
			Assert.Equal(ObjectName.Parse("tab2.a"), joinedInfo.Columns.GetColumnName(2));
			Assert.Equal(ObjectName.Parse("tab2.b"), joinedInfo.Columns.GetColumnName(3));
		}
	}
}