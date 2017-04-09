﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Deveel.Data.Sql.Methods {
	public abstract class SqlMethodInfo : IDbObjectInfo, ISqlFormattable {
		internal SqlMethodInfo(ObjectName methodName, MethodType type) {
			if (methodName == null)
				throw new ArgumentNullException(nameof(methodName));

			MethodName = methodName;
			Type = type;
			Parameters = new ParameterCollection(this);
		}

		public MethodType Type { get; }

		public bool IsFunction => Type == MethodType.Function;

		public bool IsProcedure => Type == MethodType.Procedure;

		DbObjectType IDbObjectInfo.ObjectType => DbObjectType.Method;

		public ObjectName MethodName { get; }

		ObjectName IDbObjectInfo.FullName => MethodName;

		public IList<SqlMethodParameterInfo> Parameters { get; }

		public SqlMethodBody Body { get; set; }

		internal bool TryGetParameter(string name, bool ignoreCase, out SqlMethodParameterInfo paramInfo) {
			var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
			var dictionary = Parameters.ToDictionary(x => x.Name, y => y, comparer);
			return dictionary.TryGetValue(name, out paramInfo);
		}

		internal virtual void AppendTo(SqlStringBuilder builder) {
		}

		void ISqlFormattable.AppendTo(SqlStringBuilder builder) {
			AppendTo(builder);
		}

		internal void AppendParametersTo(SqlStringBuilder builder) {
			builder.Append("(");

			if (Parameters == null) {
				for (int i = 0; i < Parameters.Count; i++) {
					Parameters[i].AppendTo(builder);

					if (i < Parameters.Count - 1)
						builder.Append(", ");
				}
			}

			builder.Append(")");
		}

		public override string ToString() {
			return this.ToSqlString();
		}

		#region ParameterCollection

		class ParameterCollection : Collection<SqlMethodParameterInfo> {
			private readonly SqlMethodInfo methodInfo;

			public ParameterCollection(SqlMethodInfo methodInfo) {
				this.methodInfo = methodInfo;
			}

			private void AssertNotContains(string name) {
				if (Items.Any(x => String.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
					throw new ArgumentException($"A parameter named {name} was already specified in method '{methodInfo.MethodName}'.");
			}

			private void AssetNotOutputInFunction(SqlMethodParameterInfo parameter) {
				if (parameter.IsOutput && methodInfo.IsFunction)
					throw new ArgumentException($"Trying to add the OUT parameter {parameter.Name} to the function {methodInfo.MethodName}");
			}

			protected override void InsertItem(int index, SqlMethodParameterInfo item) {
				AssertNotContains(item.Name);
				AssetNotOutputInFunction(item);
				item.Offset = index;
				base.InsertItem(index, item);
			}

			protected override void SetItem(int index, SqlMethodParameterInfo item) {
				AssetNotOutputInFunction(item);
				item.Offset = index;
				base.SetItem(index, item);
			}
		}

		#endregion
	}
}