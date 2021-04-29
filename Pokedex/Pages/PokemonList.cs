using Microsoft.AspNetCore.Components;
using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Pages
{
    public partial class PokemonList
    {
        //This injects the PokeClient from the dependency injection container
        [Inject]
        PokeApiClient PokeClient { get; set; }

        //This value gets overwritten after the first API call
        int TotalPokemon = 932; 
        //Determines whether loading spinner for next page is displayed
        bool LoadingNextPage = false;
        //Determines whether loading spinner for previous page is displayed
        bool LoadingPreviousPage = false;
        int CurrentPage = 1;
        //Evaluates whether we're on the last page of Pokemon
        bool OnLastPage() => (CurrentPage * ItemsPerPage) >= TotalPokemon;

        private async Task LoadNextPage()
        {
            LoadingNextPage = true;
            CurrentPage++;
            await LoadPage();
            LoadingNextPage = false;
        }
        private async Task LoadPreviousPage()
        {
            CurrentPage--;
            await LoadPage();
        }

        private List<Pokemon> Pokemon = new List<Pokemon>();
        const int ItemsPerPage = 12;
        protected override async Task OnInitializedAsync()
        {
            await LoadPage();
        }

        //Helper method to retrieve an individual pokemon from the list of pokemon
        private Pokemon GetPokemon(int index)
        {
            if (Pokemon.Count > index)
                return Pokemon[index];
            else
                return null;
        }

        private async Task LoadPage()
        {
            //Get the list of pokemon resources 
            var pageResponse = (await PokeClient.GetNamedResourcePageAsync<PokeApiNet.Pokemon>(
                ItemsPerPage, (CurrentPage - 1) * ItemsPerPage));
            TotalPokemon = pageResponse.Count;

            //Create a list of tasks for calling getting the details of each pokemon from the list above
            var tasks = pageResponse.Results
                .Select(p => PokeClient.GetResourceAsync<PokeApiNet.Pokemon>(p.Name));

            //Await all the tasks and set the data model for the page
            Pokemon = (await Task.WhenAll(tasks)).ToList();
        }
    }
}
