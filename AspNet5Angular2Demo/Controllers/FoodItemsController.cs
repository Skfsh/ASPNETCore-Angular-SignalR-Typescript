﻿using AutoMapper;

namespace Angular2Demo.Controllers
{
    using System;
    using System.Linq;
    using System.Net;

    using AspNet5Angular2Demo.Models;
    using AspNet5Angular2Demo.Repositories;
    using AspNet5Angular2Demo.ViewModels;

    using Microsoft.AspNet.Mvc;

    [Route("api/[controller]")]
    public class FoodItemsController : Controller
    {
        private readonly IFoodRepository _foodRepository;

        public FoodItemsController(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        [HttpGet]
        public IActionResult GetAllFoods()
        {
            var foods = _foodRepository.GetAll();
            return Ok(foods.Select(x => Mapper.Map<FoodItemViewModel>(x)));
        }

        [HttpGet]
        [Route("{foodItemId:int}", Name = "GetSingleFood")]
        public IActionResult GetSingleFood(int foodItemId)
        {
            FoodItem foodItem = _foodRepository.GetSingle(foodItemId);

            if (foodItem == null)
            {
                return HttpNotFound();
            }

            return Ok(Mapper.Map<FoodItemViewModel>(foodItem));
        }

        [HttpPost]
        public IActionResult AddFoodToList([FromBody] FoodItemViewModel viewModel)
        {

            if (viewModel == null)
            {
                return HttpBadRequest();
            }

            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }


            FoodItem item = Mapper.Map<FoodItem>(viewModel);
            item.Created = DateTime.Now;
            FoodItem newFoodItem = _foodRepository.Add(item);

            return CreatedAtRoute(
                "GetSingleFood",
                new { foodItemId = newFoodItem.Id },
                Mapper.Map<FoodItemViewModel>(newFoodItem));

        }

        [HttpPut]
        [Route("{foodItemId:int}")]
        public IActionResult UpdateFoodInList(int foodItemId, [FromBody] FoodItemViewModel viewModel)
        {
            if (viewModel == null)
            {
                return HttpBadRequest();
            }

            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }


            FoodItem singleById = _foodRepository.GetSingle(foodItemId);

            if (singleById == null)
            {
                return HttpNotFound();
            }

            singleById.ItemName = viewModel.ItemName;

            FoodItem newFoodItem = _foodRepository.Update(singleById);

            return Ok(Mapper.Map<FoodItemViewModel>(newFoodItem));
        }

        [HttpDelete]
        [Route("{foodItemId:int}")]
        public IActionResult DeleteFoodFromList(int foodItemId)
        {

            FoodItem singleById = _foodRepository.GetSingle(foodItemId);

            if (singleById == null)
            {
                return HttpNotFound();
            }

            _foodRepository.Delete(foodItemId);

            return new NoContentResult();
        }
    }
}
