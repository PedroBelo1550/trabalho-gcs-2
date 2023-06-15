using WeBudgetWebAPI.Interfaces;
using WeBudgetWebAPI.Interfaces.Sevices;
using WeBudgetWebAPI.Models;
using WeBudgetWebAPI.Models.Enums;

namespace WeBudgetWebAPI.Services;

public class TransactionService : ITransactionService
{
    private readonly IBudget _iBudget;
    private readonly ITransaction _iTransaction;

    public TransactionService(IBudget iBudget, ITransaction iTransaction)
    {
        _iBudget = iBudget;
        _iTransaction = iTransaction;
    }

    public async Task<Transaction> Add(Transaction transaction)
    {
        //TO-DO validation
        //TO-DO account
        if (transaction.TansactionType == TansactionType.Expenses)
        {
            var budget = await _iBudget
                .ListByUserTimeAndCategory(transaction.UserId, transaction.TansactionDate, transaction.CategoryId);
            budget.BudgetValueUsed += transaction.PaymentValue;
            await _iBudget.Update(budget);
        }

        return await _iTransaction.Add(transaction);
    }

    public async Task<Transaction> Update(Transaction transaction)
    {
        //TO-DO validation
        //TO-DO account
        var savedTrasacton = await _iTransaction.GetEntityById(transaction.Id);

        if (transaction.TansactionType == TansactionType.Expenses)
        {
            var budget = await _iBudget
                .ListByUserTimeAndCategory(transaction.UserId, transaction.TansactionDate, transaction.CategoryId);
            budget.BudgetValueUsed += transaction.PaymentValue;
            budget.BudgetValueUsed -= savedTrasacton.PaymentValue;
            await _iBudget.Update(budget);
        }

        return await _iTransaction.Update(transaction);
    }

    public async Task Delete(Transaction transaction)
    {
        //TO-DO validation
        //TO-DO account
        if (transaction.TansactionType == TansactionType.Expenses)
        {
            var budget = await _iBudget
                .ListByUserTimeAndCategory(transaction.UserId, transaction.TansactionDate, transaction.CategoryId);
            budget.BudgetValueUsed -= transaction.PaymentValue;
            await _iBudget.Update(budget);
        }

        await _iTransaction.Delete(transaction);
    }
}