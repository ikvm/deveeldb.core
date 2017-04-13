﻿using System;
using System.Collections.Generic;

using Deveel.Data.Services;
using Deveel.Data.Sql.Expressions;

using Moq;

using Xunit;

namespace Deveel.Data.Sql.Tables {
	public class RowTests {
		private IContext context;
		private ITable left;

		public RowTests() {
			var leftInfo = new TableInfo(ObjectName.Parse("tab1"));
			leftInfo.Columns.Add(new ColumnInfo("a", PrimitiveTypes.Integer()));
			leftInfo.Columns.Add(new ColumnInfo("b", PrimitiveTypes.Boolean()) {
				DefaultValue = SqlExpression.GreaterThan(SqlExpression.Reference(new ObjectName("a")),
					SqlExpression.Constant(SqlObject.Integer(5)))
			});
			leftInfo.Columns.Add(new ColumnInfo("c", PrimitiveTypes.Double()));

			left = new TestTable(leftInfo, new List<SqlObject[]> {
				new[] {SqlObject.Integer(23), SqlObject.Boolean(true), SqlObject.Double(5563.22)},
				new[] {SqlObject.Integer(54), SqlObject.Boolean(null), SqlObject.Double(921.001)},
				new[] {SqlObject.Integer(23), SqlObject.Boolean(true), SqlObject.Double(2010.221)}
			});

			var mock = new Mock<IContext>();
			mock.SetupGet(x => x.Scope)
				.Returns(new ServiceContainer());
			context = mock.Object;
		}

		[Fact]
		public void GetValueFromRow() {
			var row = left.GetRow(0);

			Assert.Equal(-1, row.Id.TableId);
			Assert.Equal(0, row.Id.Number);

			var value1 = row.GetValue("a");
			var value2 = row.GetValue("b");

			Assert.Equal(SqlObject.Integer(23), value1);
			Assert.Equal(SqlObject.Boolean(true), value2);
		}

		[Fact]
		public void SetValueInNewRow() {
			var row = left.NewRow();

			row["a"] = SqlObject.Integer(345);
			row["b"] = SqlObject.Boolean(false);

			var value1 = row["a"];
			var value2 = row["b"];

			Assert.Equal(SqlObject.Integer(345), value1);
			Assert.Equal(SqlObject.Boolean(false), value2);
		}

		[Fact]
		public void ResolveReference() {
			var resolver = new RowReferenceResolver(left, 0);

			var type1 = resolver.ResolveType(ObjectName.Parse("tab1.a"));
			var type2 = resolver.ResolveType(ObjectName.Parse("tab1.b"));
			var type3 = resolver.ResolveType(ObjectName.Parse("tab1.c"));

			Assert.Equal(PrimitiveTypes.Integer(), type1);
			Assert.Equal(PrimitiveTypes.Boolean(), type2);
			Assert.Equal(PrimitiveTypes.Double(), type3);

			var value1 = resolver.ResolveReference(ObjectName.Parse("tab1.a"));
			var value2 = resolver.ResolveReference(ObjectName.Parse("tab1.b"));
			var value3 = resolver.ResolveReference(ObjectName.Parse("tab1.c"));

			Assert.Equal(SqlObject.Integer(23), value1);
			Assert.Equal(SqlObject.Boolean(true), value2);
			Assert.Equal(SqlObject.Double(5563.22), value3);
		}

		[Fact]
		public void SetDefaultValues() {
			var row = left.NewRow();

			row["a"] = SqlObject.Integer(345);
			row.SetDefault(context);

			var value = row["b"];
			Assert.Equal(SqlObject.Boolean(true), value);
		}
	}
}