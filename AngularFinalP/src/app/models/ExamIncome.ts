// exam-income-expense.dto.ts

export interface ExamIncomeCreateDto {
  examId: number;
  typesOfExpense?: string;
  amount: number;
}

export interface ExamIncomeUpdateDto extends ExamIncomeCreateDto {
  incomeExpenseId: number;
}

export interface ExamIncomeReadDto {
  incomeExpenseId: number;
  examId: number;
  examName: string;
  typesOfExpense?: string;
  amount: number;
}
