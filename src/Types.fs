namespace Julep

open System

module Types =

  [<Measure>] type dollars

  type Account = Account of string

  type Month = Month of int

  type MonthRange = {
    From : Month
    To : Month option
  }

  type TransactionCategory = {
    Name : string
    ValidRange : MonthRange
  }

  type Transaction = {
    Month : Month
    Date : DateTime
    Category : TransactionCategory
    Amount : decimal<dollars>
    Account : Account
    Description : string
  }

  type IncomeCategory = {
    Name : string
    ValidRange : MonthRange
  }

  type IncomeSource = {
    IncomeCategory : IncomeCategory
    TransactionCategory : TransactionCategory
    ValidRange : MonthRange
  }

  type IncomeProjection = {
    Category : IncomeCategory
    Projected : decimal<dollars>
    Month : Month
  }

  type DueDate = DueDate of DateTime

  type FrequentExpenseType =
    | Fixed
    | Variabled
    | Budgeted

  type InfrequentExpenseType =
    | Bill
    | Expense

  type FrequentExpenseCategory = {
    Name : string
    Type : FrequentExpenseType
    ValidRange : MonthRange
    Due : DueDate
  }

  type FrequentExpenseSource = {
    FrequentExpenseCategory : FrequentExpenseCategory
    TransactionCategory : TransactionCategory
    ValidRange : MonthRange
  }

  type InfrequentExpenseCategory = {
    Name : string
    Type : InfrequentExpenseType
    ValidRange : MonthRange
    Due : DueDate
    Account : Account
  }

  type InfrequentExpenseSource = {
    InfrequentExpenseCategory : InfrequentExpenseCategory
    TransactionCategory : TransactionCategory
    ValidRange : MonthRange
  }

  type Goal = {
    Name : string
    Account : Account
    ValidRange : MonthRange
  }

  type GoalSource = {
    Goal : Goal
    TransactionCategory : TransactionCategory
    ValidRange : MonthRange
  }

