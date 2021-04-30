using Microsoft.AspNetCore.Components;
using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Pages
{
    public partial class PokemonDetails
    {
        //PokemonName is passed as a parameter to this component when it is created
        [Parameter]
        public string PokemonName { get; set; }

        //This injects the PokeClient from the dependency injection container
        [Inject]
        PokeApiClient PokeClient { get; set; }
        
        //This injects the NavigationManager class which allows us to navigate to a different page
        [Inject]
        NavigationManager NavigationManager { get; set; }

        //This serves as the main data model for the page
        private Pokemon Pokemon;

        //This is also used as data for the page and contains a set of descriptive paragraphs.
        private HashSet<string> PokemonDescriptions = new HashSet<string>();

        protected override async Task OnInitializedAsync()
        {
            //Create a task for retrieving the basic Pokemon details
            var pokemonTask = PokeClient.GetResourceAsync<Pokemon>(PokemonName);
            //Retrieve additional details about the Pokemon from the API including descriptions
            var pokemonSpecies = await PokeClient.GetResourceAsync<PokemonSpecies>(PokemonName);
            Pokemon = await pokemonTask;
            //We filter out just the english descriptions
            //Also we replace a special character appearing in the descriptions with a space.
            pokemonSpecies.FlavorTextEntries.Where(f => f.Language.Name == "en")
                .ToList().ForEach(f => PokemonDescriptions.Add(f.FlavorText.Replace("\f", " ")));
        }

        //This method is used to navigate back to the Pokemon list page after clicking a button
        void GoBackToListPage()
        {
            NavigationManager.NavigateTo("pokemon");
        }
    }
}
