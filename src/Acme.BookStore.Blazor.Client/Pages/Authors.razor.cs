using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acme.BookStore.Authors;
using Acme.BookStore.Permissions;
using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Acme.BookStore.Blazor.Pages; 
//namespace Acme.BookStore.Authors;


public partial class Authors
{
    private IReadOnlyList<AuthorDto> AuthorList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private bool CanEditAuthor { get; set; }
    private bool CanDeleteAuthor { get; set; }
    private CreateAuthorDto NewAuthor { get; set; }
    private Guid EditingAuthorId { get; set; }
    private UpdateAuthorDto EditingAuthor { get; set; }
    private Modal CreateAuthorModal { get; set; }
    private Modal EditAuthorModal { get; set; }
    protected PageToolbar Toolbar { get; } = new();
    public Authors()
    {
        NewAuthor = new CreateAuthorDto();
        EditingAuthor = new UpdateAuthorDto();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetAuthorsAsync();
    }
    private async Task SetPermissionsAsync()
    {
        CanEditAuthor = await AuthorizationService
            .IsGrantedAsync(BookStorePermissions.Authors.Create);

        CanEditAuthor = await AuthorizationService
            .IsGrantedAsync(BookStorePermissions.Authors.Edit);

        CanDeleteAuthor = await AuthorizationService
            .IsGrantedAsync(BookStorePermissions.Authors.Delete);
    }
    private async Task GetAuthorsAsync()
    {
        var result = await AuthorAppService.GetListAsync(
            new GetAuthorListDto
            {
                MaxResultCount = PageSize,
                SkipCount = CurrentPage * PageSize,
                Sorting = CurrentSorting
            }
        );
        AuthorList = result.Items;
        TotalCount = (int)result.TotalCount;
    }
    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AuthorDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page-1;
        await GetAuthorsAsync();
        StateHasChanged();
    }
    private Task OpenCreateAuthorModal()
    {
        NewAuthor = new CreateAuthorDto();
        CreateAuthorModal.Show();

        return Task.CompletedTask;
    }
    private void CloseCreateAuthorModal()
    {
        CreateAuthorModal.Hide();
    }
    private void OpenEditAuthorModal(AuthorDto author)
    {
        EditingAuthorId = author.Id;
        EditingAuthor = ObjectMapper.Map<AuthorDto, UpdateAuthorDto>(author);
        EditAuthorModal.Show();
    }
    private async Task DeleteAuthorAsync(AuthorDto author)
    {
        var confirmMessage = L["AuthorDeletionConfirmationMessage", author.Name];
        if (!await Message.Confirm(confirmMessage))
        {
            return;
        }
        await AuthorAppService.DeleteAsync(author.Id);
        await GetAuthorsAsync();
    }
    private void CloseEditAuthorModal()
    {
        EditAuthorModal.Hide();
    }
    private async Task CreateAuthorAsync()
    {
        await AuthorAppService.CreateAsync(NewAuthor);
        await GetAuthorsAsync();
        CreateAuthorModal.Hide();
    }
    private async Task UpdateAuthorAsync()
    {
        await AuthorAppService.UpdateAsync(EditingAuthorId, EditingAuthor);
        await GetAuthorsAsync();
        EditAuthorModal.Hide();
    }
}
