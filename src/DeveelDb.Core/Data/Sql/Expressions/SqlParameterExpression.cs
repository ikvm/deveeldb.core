﻿using System;

namespace Deveel.Data.Sql.Expressions {
	public sealed class SqlParameterExpression : SqlExpression {
		internal SqlParameterExpression()
			: base(SqlExpressionType.Parameter) {
		}

		public override SqlExpression Accept(SqlExpressionVisitor visitor) {
			return visitor.VisitParameter(this);
		}

		protected override void AppendTo(SqlStringBuilder builder) {
			builder.Append("?");
		}
	}
}