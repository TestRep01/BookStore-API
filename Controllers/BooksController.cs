using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using BookStore_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Interacts to books table
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _booksRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository booksRepository, ILoggerService logger, IMapper mapper)
        {
            _booksRepository = booksRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        ///  Get all Books
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrators, Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var location = GetControloerActionNames();
            try
            {
                var books = await _booksRepository.FindALL();
                var response = _mapper.Map<IList<BookDTO>>(books);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError($"{location}- {ex.InnerException} - {ex.Message}");
            }

        }

        /// <summary>
        ///  Get all Book by ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Administrators, Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookById(int id)
        {
            var location = GetControloerActionNames();
            try
            {
                var book = await _booksRepository.FindById(id);
                var response = _mapper.Map<BookDTO>(book);

                if (book == null)
                {
                    _logger.Logwarn($"{location}- Book failed to be returvied- {id}");
                    return NotFound();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError($"{location}- {ex.InnerException} - {ex.Message}");
            }

        }


        /// <summary>
        /// Create book
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrators")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            var location = GetControloerActionNames();

            try
            {
                if (_booksRepository == null)
                    return BadRequest(ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _booksRepository.Create(book);
                if (!isSuccess)
                {
                    _logger.Logerror("Author creation failed");
                    return InterError("Author creation failed");
                }

                return Created("Create", new { book });
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError($"{location}- {ex.InnerException} - {ex.Message}"); 
            }
        }

        /// <summary>
        /// Update Book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="BookDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrators")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO BookDTO)
        {
            var location = GetControloerActionNames();

            try
            {
                if (id < 1 || _booksRepository == null || id != BookDTO.Id)
                    return BadRequest(ModelState);

                var isExists = await _booksRepository.isExists(id);

                if (!isExists)
                    return NotFound();

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var book = _mapper.Map<Book>(BookDTO);
                var isSuccess = await _booksRepository.Update(book);
                if (!isSuccess)
                {
                    _logger.Logerror("book update failed");
                    return InterError("book update failed");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError($"{location}- {ex.InnerException} - {ex.Message}"); 
            }
        }

        /// <summary>
        ///  book delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrators")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {

            var location = GetControloerActionNames();

            try
            {
                if (id < 1)
                    return BadRequest();


                var isExists = await _booksRepository.isExists(id);

                if (!isExists)
                    return NotFound();

                var book = await _booksRepository.FindById(id);

                var isSuccess = await _booksRepository.Delete(book);
                if (!isSuccess)
                {
                    _logger.Logerror("Book delete failed");
                    return InterError("Book delete failed");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError($"{location}- {ex.InnerException} - {ex.Message}");
            }
        }


        private ObjectResult InterError(string message)
        {
            _logger.Logerror(message);
            return StatusCode(500, "Something went wrong");
        }

        private string GetControloerActionNames()
        {
            var controllor = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controllor} - {action}";
        }
    }
}
