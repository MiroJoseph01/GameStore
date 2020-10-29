using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.MongoEntities;

namespace GameStore.DAL.Repositories.Mongo.Util
{
    [ExcludeFromCodeCoverage]
    public static class RepositoryHelper
    {
        public static string GetDescription(Type type)
        {
            var descriptions = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptions.Length == 0)
            {
                return null;
            }

            return descriptions[0].Description;
        }

        public static Genre ConvertCategoryToGenre(Category category)
        {
            Genre genre = null;

            if (category != null)
            {
                genre = new Genre
                {
                    GenreId = category.CategoryID.ToString(),
                    GenreName = category.CategoryName,
                };
            }

            return genre;
        }
    }
}
