﻿using BooksApp.MVC.Areas.Admin.Models.ViewModels;

namespace BooksApp.MVC.Models.ViewModels
{
    public class AuthorModel
    {
        public int Id { get; set; }
        public bool IsApproved { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string About { get; set; }
        public List<BookModel> Books { get; set; }
    }
}
