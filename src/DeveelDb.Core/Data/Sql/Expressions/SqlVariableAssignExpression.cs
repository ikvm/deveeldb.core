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

using Deveel.Data.Sql.Variables;

namespace Deveel.Data.Sql.Expressions {
	public sealed class SqlVariableAssignExpression : SqlExpression {
		public string VariableName { get; }

		public SqlExpression Value { get; }

		internal SqlVariableAssignExpression(string variableName, SqlExpression value)
			: base(SqlExpressionType.VariableAssign) {
			if (String.IsNullOrWhiteSpace(variableName))
				throw new ArgumentNullException(nameof(variableName));
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			if (!Variables.Variable.IsValidName(variableName))
				throw new ArgumentException($"Variable name '{variableName}' is invalid.");

			VariableName = variableName;
			Value = value;
		}

		public override bool CanReduce => true;

		public override SqlExpression Reduce(IContext context) {
			if (context == null)
				throw new SqlExpressionException("A context is required to reduce a variable expression");

			var manager = context.ResolveVariableManager();
			if (manager == null)
				throw new SqlExpressionException("No variable manager was found in the context hierarchy");

			return manager.AssignVariable(VariableName, Value, context);
		}

		protected override void AppendTo(SqlStringBuilder builder) {
			builder.AppendFormat(":{0}", VariableName);
			builder.Append(" := ");
			Value.AppendTo(builder);
		}
	}
}