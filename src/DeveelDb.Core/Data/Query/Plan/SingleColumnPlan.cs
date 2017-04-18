﻿using System;

using Deveel.Data.Sql.Expressions;

namespace Deveel.Data.Query.Plan {
	class SingleColumnPlan {
		public ObjectName SingleColumn { get; set; }

		public ObjectName Column { get; set; }

		public SqlExpression Expression { get; set; }

		public TablePlan Table { get; set; }
	}
}