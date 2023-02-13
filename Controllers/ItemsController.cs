using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Entitiesusing;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IEnumerable<ItemDto> GetItems()
        {
            var items = repository.GetItemsAsync().Select( item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetItem(Guid id)
        {
            var item = repository.GetItemAsync(id);

            if(item is null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPost]
        public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto)
        {
            Item item = new(){
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow,
            };

            repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = repository.GetItemAsync(id);
            
            if(existingItem is null)
            {
                return NotFound();
            }

            Item updatedItem = existingItem with {
                Name = itemDto.Name,
                Price = itemDto.Price,
            };

            repository.UpdateItemAsync(updatedItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id)
        {
            var existingItem = repository.GetItemAsync(id);

            if(existingItem is null)
            {
                return NotFound();
            }

            repository.DeleteItemAsync(id);

            return NoContent();
        }

    }

    
}