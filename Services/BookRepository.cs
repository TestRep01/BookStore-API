using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _db;
        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Book endity)
        {
            await _db.Books.AddAsync(endity);
            return await Save();
        }

        public async Task<bool> Delete(Book endity)
        {
             _db.Books.Remove(endity);
            return await Save();
        }

        public async Task<IList<Book>> FindALL()
        {
          return  await _db.Books.ToListAsync();      
        }

        public async Task<Book> FindById(int id)
        {
            return await _db.Books.FindAsync(id);
        }

        public async Task<bool> isExists(int id)
        {
            return await _db.Books.AnyAsync(q=>q.Id == id);
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }


        public async Task<bool> Update(Book endity)
        {
            _db.Books.Update(endity);
            return await Save();
        }
    }
}
