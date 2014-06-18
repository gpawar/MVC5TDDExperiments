﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC5TDDExperiments.Models
{
    public interface IRepository
    {
        List<Book> GetAll();

        void CreateBook(Book bookToCreate);
    }
}
