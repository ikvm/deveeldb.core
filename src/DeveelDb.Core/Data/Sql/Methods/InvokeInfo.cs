﻿using System;
using System.Collections.Generic;
using System.Linq;

using Deveel.Data.Sql.Expressions;

namespace Deveel.Data.Sql.Methods {
	public sealed class InvokeInfo {
		private readonly Dictionary<string, SqlType> argumentTypes;

		internal InvokeInfo(SqlMethodInfo methodInfo, Dictionary<string, SqlType> argumentTypes) {
			MethodInfo = methodInfo;
			this.argumentTypes = argumentTypes;
		}

		public SqlMethodInfo MethodInfo { get; }

		public IEnumerable<string> ArgumentNames => argumentTypes.Keys;

		public SqlType GetArgumentType(string parameterName) {
			SqlType argType;
			if (!argumentTypes.TryGetValue(parameterName, out argType))
				return null;

			return argType;
		}
	}
}