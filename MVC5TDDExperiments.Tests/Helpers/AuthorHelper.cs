using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVC5TDDExperiments.Models;

namespace MVC5TDDExperiments.Tests.Helpers
{
    public static class AuthorHelper
    {
        private const int NO_ID = -1;

        public static Author RoyOsherove(int id = NO_ID)
        {
            return CreateAuthor(id, "Roy", "Osherove");
        }

        public static Author RobertMartin(int id = NO_ID)
        {
            return CreateAuthor(id, "Robert C.", "Martin");
        }

        public static Author JRRTolkien(int id = NO_ID)
        {
            return CreateAuthor(id, "J. R. R.", "Tolkien");
        }

        private static Author CreateAuthor(int id, string firstName, string lastName)
        {
            if (id == -1)
            {
                return new Author() { FirstName = firstName, LastName = lastName };
            }
            return new Author() {AuthorId = id, FirstName = firstName, LastName = lastName };
        }
    }
}
