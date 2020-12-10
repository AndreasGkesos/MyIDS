using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarWars.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Operator,Admin")]
    public class StarWarsController : ControllerBase
    {
        private static readonly HttpClient httpClient_ = new HttpClient();

        public StarWarsController()
        {
        }

        [HttpGet("people/{id}")]
        public async Task<IActionResult> GetPeople(int id)
        {
            var response = await httpClient_.GetAsync($"https://swapi.dev/api/people/{id}/");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Bad: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();
            
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("species/{id}")] 
        public async Task<IActionResult> GetSpecies(int id)
        {
            var response = await httpClient_.GetAsync($"https://swapi.dev/api/species/{id}/");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Bad: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }

        [HttpGet("starships/{id}")]
        public async Task<IActionResult> GetStarships(int id)
        {
            var response = await httpClient_.GetAsync($"https://swapi.dev/api/starships/{id}/");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Bad: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }
    }
}
