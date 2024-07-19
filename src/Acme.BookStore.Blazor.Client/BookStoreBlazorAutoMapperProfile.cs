using AutoMapper;
using Acme.BookStore.Books;
using Acme.BookStore.Authors;
namespace Acme.BookStore.Blazor.Client;

public class BookStoreBlazorAutoMapperProfile : Profile
{
    public BookStoreBlazorAutoMapperProfile()
    {
        CreateMap<BookDto, CreateUpdateBookDto>();
        CreateMap<AuthorDto, UpdateAuthorDto>();

        //Define your AutoMapper configuration here for the Blazor project.
    }
}
