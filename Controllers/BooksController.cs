using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.Data;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;
using System.Text.Json;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BooksController> _logger;
        public BooksController(AppDbContext dbContext, IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        [HttpGet("get-all-books")]
        [Authorize(Roles = "Read")]
        public IActionResult GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
          [FromQuery] string? sortBy, [FromQuery] bool isAscending,
          [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            _logger.LogInformation("GetAll Book Action method was invoked");
            _logger.LogWarning("This is a warning log");
            _logger.LogError("This is a error log");
            // su dung reposity pattern
            var allBooks = _bookRepository.GetAllBooks(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            _logger.LogInformation($"Finished GetAllBook request with data {JsonSerializer.Serialize(allBooks)}");
            return Ok(allBooks);
        }

        [HttpGet]
        [Route("get-book-by-id/{id}")]
        [Authorize(Roles = "Read")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);
            if (bookWithIdDTO == null)
            {
                return NotFound(new { message = "Không tìm thấy sách" });
            }
            return Ok(bookWithIdDTO);
        }

        [HttpPost("add-book")]
        [Authorize(Roles = "Write")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var bookAdd = _bookRepository.AddBook(addBookRequestDTO);
                return Ok(bookAdd);
            }
            else return BadRequest(ModelState);
        }

        [HttpPut("update-book-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var updateBook = _bookRepository.UpdateBookById(id, bookDTO);
            if (updateBook == null)
            {
                return NotFound(new { message = "Không tìm thấy sách" });
            }
            return Ok(updateBook);
        }

        [HttpDelete("delete-book-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult DeleteBookById(int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);
            if (deleteBook == null)
            {
                return NotFound(new { message = "Không tìm thấy sách" });
            }
            return Ok(deleteBook);
        }


    }
}