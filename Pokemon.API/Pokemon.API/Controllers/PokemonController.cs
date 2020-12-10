using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pokemon.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class PokemonController : ControllerBase
    {
        private static readonly HttpClient httpClient_ = new HttpClient();

        public PokemonController()
        {
        }

        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetItems(int id)
        {
            var response = await httpClient_.GetAsync($"https://pokeapi.co/api/v2/item/{id}/");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Bad: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }

        [HttpGet("location/{id}")]
        public async Task<IActionResult> GetLocations(int id)
        {
            var response = await httpClient_.GetAsync($"https://pokeapi.co/api/v2/location/{id}/");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Bad: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }

        [HttpGet("pokemon/{id}")]
        public async Task<IActionResult> GetPokemon(int id)
        {
            var response = await httpClient_.GetAsync($"https://pokeapi.co/api/v2/pokemon/{id}/");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Bad: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }
    }
}
