namespace Julep

open Types

module AggregateTypes =

  type IncomeCategoryRow = {
    Category : IncomeCategory
    Projected : decimal<dollars>
    Actual : decimal<dollars>
  }
