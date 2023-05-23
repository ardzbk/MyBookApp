﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BooksApp.MVC.Models;

using BooksApp.Entity.Concrete;
using BooksApp.MVC.Models.ViewModels;
using BooksApp.Business.Abstract;
using BooksApp.MVC.Areas.Admin.Models.ViewModels;

namespace BooksApp.MVC.Controllers;

public class HomeController : Controller
{
    private readonly IBookService _bookService;
    private readonly ICategoryService _categoryService;
    private readonly IAuthorService _authorService;

    public HomeController(IBookService bookService, ICategoryService categoryService, IAuthorService authorService)
    {
        _bookService = bookService;
        _categoryService = categoryService;
        _authorService = authorService;
    }

    public async Task<IActionResult> Index(string categoryurl)
    {
        List<Book> books = await _bookService.GetAllBooksFullDataAsync(true, categoryurl);

        List<BookModel> bookModelList = new List<BookModel>();
        bookModelList = books.Select(b => new BookModel
        {
            Id = b.Id,
            Name = b.Name,
            Stock = b.Stock,
            Price = b.Price,
            PageCount = b.PageCount,
            EditionYear = b.EditionYear,
            EditionNumber = b.EditionNumber,
            CreatedDate = b.CreatedDate,
            ModifiedDate = b.ModifiedDate,
            IsApproved = b.IsApproved,
            Categories = b.BookCategories.Select(bc => bc.Category).ToList(),
            Authors = b.BookAuthors.Select(ba => ba.Author).ToList(),
            Images = b.Images,
            Url = b.Url,
            Summary = b.Summary
        }).ToList();
        if (RouteData.Values["categoryurl"] != null)
        {
            ViewBag.SelectedCategoryName = await _categoryService.GetCategoryNameByUrlAsync(RouteData.Values["categoryurl"].ToString());
        }
        return View(bookModelList);
    }
    public async Task<IActionResult> BookDetails(string url)
    {
        var bookId = await _bookService.GetByUrlAsync(url);
        Book book = await _bookService.GetBookFullDataAsync(bookId);
        BookModel bookModel = new BookModel()
        {
            Id = book.Id,
            Name = book.Name,
            Stock = book.Stock,
            PageCount = book.PageCount,
            EditionNumber = book.EditionNumber,
            EditionYear = book.EditionYear,
            Price = book.Price,
            ModifiedDate = book.ModifiedDate,
            IsApproved = book.IsApproved,
            Summary = book.Summary,
            Images = book.Images.Select(i => new Image
            {
                Id = i.Id,
                Url = i.Url
            }).ToList(),
            Categories = book.BookCategories.Select(bc => bc.Category).ToList(),
            Authors = book.BookAuthors.Select(ba => ba.Author).ToList()
        };
        return View(bookModel);
    }
    public async Task<IActionResult> AuthorDetails(string url)
    {
        var author = await _authorService.GetAuthorWithBooksAsync(url);
        //AutoMapper paketini araştırın!
        var authorModel = new AuthorModel()
        {
            Id = author.Id,
            Name = author.Name,
            BirthDate = author.BirthDate,
            Gender = author.Gender,
            ImageUrl = author.ImageUrl,
            IsApproved = author.IsApproved,
            Url = author.Url,
            About=author.About,
            Books = author.BookAuthors.Select(ba => new BookModel
            {
                Id = ba.Book.Id,
                Url = ba.Book.Url,
                CreatedDate = ba.Book.CreatedDate,
                ModifiedDate = ba.Book.ModifiedDate,
                EditionNumber = ba.Book.EditionNumber,
                EditionYear = ba.Book.EditionYear,
                IsApproved = ba.Book.IsApproved,
                Name = ba.Book.Name,
                PageCount = ba.Book.PageCount,
                Price = ba.Book.Price,
                Stock = ba.Book.Stock,
                Summary = ba.Book.Summary,
                Images = ba.Book.Images,
                Categories = ba.Book.BookCategories.Select(bc=>bc.Category).ToList()
            }).ToList()
        };
        return View(authorModel);
    }
}
