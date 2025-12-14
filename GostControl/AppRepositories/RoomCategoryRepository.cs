using GostControl.AppData;
using GostControl.AppModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GostControl.AppRepositories
{
    public interface IRoomCategoryRepository : IDisposable
    {
        List<RoomCategory> GetAllCategories();
        RoomCategory GetCategoryById(int categoryId);
        int AddCategory(RoomCategory category);
        void UpdateCategory(RoomCategory category);
        void DeleteCategory(int categoryId);
        RoomCategory GetCategoryByName(string categoryName);
    }

    public class RoomCategoryRepository : IRoomCategoryRepository
    {
        private readonly HotelDbContext _context;
        private bool _disposed = false;

        public RoomCategoryRepository()
        {
            _context = new HotelDbContext();
        }

        public RoomCategoryRepository(HotelDbContext context)
        {
            _context = context;
        }

        public List<RoomCategory> GetAllCategories()
        {
            return _context.RoomCategories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .ToList();
        }

        public RoomCategory GetCategoryById(int categoryId)
        {
            return _context.RoomCategories
                .AsNoTracking()
                .FirstOrDefault(c => c.CategoryID == categoryId);
        }

        public int AddCategory(RoomCategory category)
        {
            _context.RoomCategories.Add(category);
            _context.SaveChanges();
            return category.CategoryID;
        }

        public void UpdateCategory(RoomCategory category)
        {
            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteCategory(int categoryId)
        {
            var category = _context.RoomCategories.Find(categoryId);
            if (category != null)
            {
                // Есть ли номера с этой категорией
                var roomsWithCategory = _context.Rooms.Any(r => r.CategoryID == categoryId);
                if (roomsWithCategory)
                {
                    throw new InvalidOperationException("Нельзя удалить категорию, так как существуют номера с этой категорией");
                }

                _context.RoomCategories.Remove(category);
                _context.SaveChanges();
            }
        }

        public RoomCategory GetCategoryByName(string categoryName)
        {
            return _context.RoomCategories
                .AsNoTracking()
                .FirstOrDefault(c => c.CategoryName.ToLower() == categoryName.ToLower());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}