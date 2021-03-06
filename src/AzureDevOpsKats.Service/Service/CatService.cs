﻿using System.Collections.Generic;
using AutoMapper;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;

namespace AzureDevOpsKats.Service.Service
{
    public class CatService : ICatService
    {
        private readonly ICatRepository _catRepository;
        public CatService(ICatRepository catRepository)
        {
            this._catRepository = catRepository;
        }

        public IEnumerable<CatModel> GetCats()
        {
            var cats = _catRepository.GetCats();
            var results = Mapper.Map<IEnumerable<CatModel>>(cats);
            foreach (var result in results)
            {
                result.Photo = @"\images\" + result.Photo;
            }

            return results;
        }

        public CatModel GetCat(int id)
        {
            var cat = _catRepository.GetCat(id);
            var result = Mapper.Map<CatModel>(cat);

            return result;
        }

        public void EditCat(int id, CatUpdateModel cat)
        {
            var result = new Cat
            {
                Id = id,
                Name = cat.Name,
                Description = cat.Description,
            };

            _catRepository.EditCat(result);
        }

        public void CreateCat(CatModel cat)
        {
            var result = Mapper.Map<Cat>(cat);
            _catRepository.CreateCat(result);
        }

        public void DeleteCat(int id)
        {
            _catRepository.DeleteCat(id);
        }
    }
}
