using AutoMapper;
using LibraryManagement.Dtos;
using LibraryManagement.Entities;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public TransactionsController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpPost("borrow")]
        public async Task<ActionResult> BorrowBook([FromBody] CreateTransactionDto createTransactionDto)
        {
            try
            {
                await _transactionService.BorrowBookAsync(createTransactionDto.BookId, createTransactionDto.MemberId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("return")]
        public async Task<ActionResult> ReturnBook([FromBody] CreateTransactionDto createTransactionDto)
        {
            try
            {
                await _transactionService.ReturnBookAsync(createTransactionDto.BookId, createTransactionDto.MemberId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetOverdueTransactions()
        {
            var transactions = await _transactionService.GetOverdueTransactionsAsync();
            var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);
            return Ok(transactionDtos);
        }
    }
}