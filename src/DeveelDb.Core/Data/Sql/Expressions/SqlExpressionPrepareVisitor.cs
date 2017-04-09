﻿using System;

namespace Deveel.Data.Sql.Expressions {
	class SqlExpressionPrepareVisitor : SqlExpressionVisitor {
		private readonly ISqlExpressionPreparer preparer;

		public SqlExpressionPrepareVisitor(ISqlExpressionPreparer preparer) {
			this.preparer = preparer;
		}

		public override SqlExpression Visit(SqlExpression expression) {
			if (preparer.CanPrepare(expression))
				expression = preparer.Prepare(expression);

			return base.Visit(expression);
		}
	}
}