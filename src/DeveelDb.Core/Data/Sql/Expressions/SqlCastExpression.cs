﻿// 
//  Copyright 2010-2017 Deveel
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//


using System;
using System.Threading.Tasks;

using Deveel.Data.Serialization;

namespace Deveel.Data.Sql.Expressions {
	public sealed class SqlCastExpression : SqlExpression {
		internal SqlCastExpression(SqlExpression value, SqlType targetType)
			: base(SqlExpressionType.Cast) {
			if (value == null)
				throw new ArgumentException(nameof(value));
			if (targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			Value = value;
			TargetType = targetType;
		}

		private SqlCastExpression(SerializationInfo info)
			: base(info) {
			Value = info.GetValue<SqlExpression>("value");
			TargetType = info.GetValue<SqlType>("targetType");
		}

		public SqlExpression Value { get; }

		public SqlType TargetType { get; }

		public override bool CanReduce => true;

		public override SqlType GetSqlType(IContext context) {
			return TargetType;
		}

		protected override void GetObjectData(SerializationInfo info) {
			info.SetValue("value", Value);
			info.SetValue("targetType", TargetType);
		}

		public override SqlExpression Accept(SqlExpressionVisitor visitor) {
			return visitor.VisitCast(this);
		}

		public override async Task<SqlExpression> ReduceAsync(IContext context) {
			var value = await Value.ReduceAsync(context);

			if (!(value is SqlConstantExpression))
				throw new SqlExpressionException("The value of the cast could not be reduced to constant");

			var obj = ((SqlConstantExpression) value).Value;
			var result = obj.CastTo(TargetType);

			return Constant(result);
		}

		protected override void AppendTo(SqlStringBuilder builder) {
			builder.Append("CAST(");
			Value.AppendTo(builder);
			builder.Append(" AS ");
			TargetType.AppendTo(builder);
			builder.Append(")");
		}
	}
}