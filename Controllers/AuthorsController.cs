using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        ///  Get all Authors
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                var authors = await _authorRepository.FindALL();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return StatusCode(500, "Something went wrong");
            }

        }

        /// <summary>
        ///  Get  by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                var author = await _authorRepository.FindById(id);
                var response = _mapper.Map<AuthorDTO>(author);
                if(author == null)
                {
                    _logger.Logwarn("user was not found");
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError("Something went wrong");
            }
        }

        /// <summary>
        /// Create Author
        /// </summary>
        /// <param name="AuthorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO AuthorDTO)
        {
            try
            {
                if (_authorRepository == null)
                    return BadRequest(ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(AuthorDTO);
                var isSuccess =  await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    _logger.Logerror("Author creation failed");
                    return InterError("Author creation failed");
                }

                return Created("Create", new { author});
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError("Something went wrong");
            }
        }
        /// <summary>
        /// Update Author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="AuthorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO AuthorDTO)
        {
            try
            {
                if (id < 1 || _authorRepository == null || id != AuthorDTO.ID)
                    return BadRequest(ModelState);

                var isExists = await _authorRepository.isExists(id);

                if (!isExists)
                    return NotFound();

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var author = _mapper.Map<Author>(AuthorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    _logger.Logerror("Author update failed");
                    return InterError("Author update failed");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError("Something went wrong");
            }
        }

        /// <summary>
        ///  author delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1 )
                    return BadRequest();


                var isExists = await _authorRepository.isExists(id);

                if (!isExists)
                    return NotFound();

                var author = await _authorRepository.FindById(id);

                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    _logger.Logerror("Author delete failed");
                    return InterError("Author delete failed");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError("Something went wrong");
            }
        }

        private ObjectResult InterError(string message)
        {
            _logger.Logerror(message);
            return StatusCode(500, "Something went wrong");
        }
    }
           
 }
