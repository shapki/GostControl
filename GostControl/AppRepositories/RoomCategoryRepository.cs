using GostControl.AppModels;
using GostControl.AppServices;
using System.Collections.Generic;
using System.Linq;

namespace GostControl.AppRepositories
{
    public class RoomCategoryRepository
    {
        private readonly LocalDataService _dataService;

        public RoomCategoryRepository()
        {
            _dataService = LocalDataService.Instance;
        }

        public List<RoomCategory> GetAllCategories()
        {
            return _dataService.RoomCategories.ToList();
        }

        public RoomCategory GetCategoryById(int categoryId)
        {
            return _dataService.GetCategoryById(categoryId);
        }

        public int AddCategory(RoomCategory category)
        {
            category.CategoryID = _dataService.RoomCategories.Any() ?
                _dataService.RoomCategories.Max(c => c.CategoryID) + 1 : 1;
            _dataService.RoomCategories.Add(category);
            return category.CategoryID;
        }

        public void UpdateCategory(RoomCategory category)
        {
            var existingCategory = _dataService.GetCategoryById(category.CategoryID);
            if (existingCategory != null)
            {
                int index = _dataService.RoomCategories.IndexOf(existingCategory);
                _dataService.RoomCategories[index] = category;
            }
        }

        public void DeleteCategory(int categoryId)
        {
            var category = _dataService.GetCategoryById(categoryId);
            if (category != null)
            {
                var roomsWithCategory = _dataService.Rooms.Where(r => r.CategoryID == categoryId).ToList();
                if (roomsWithCategory.Any())
                {
                    throw new System.Exception("Нельзя удалить категорию, так как существуют номера с этой категорией");
                }

                _dataService.RoomCategories.Remove(category);
            }
        }

        public RoomCategory GetCategoryByName(string categoryName)
        {
            return _dataService.RoomCategories
                .FirstOrDefault(c => c.CategoryName.ToLower() == categoryName.ToLower());
        }
    }
}