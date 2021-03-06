﻿using System;
using System.Collections.Generic;
using System.IO;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using BTIG.Cats.Web.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Web.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    [ApiController]
    public class CatsController : ControllerBase
    {
        private readonly ICatService _catService;

        private readonly IFileService _fileService;

        private ApplicationOptions ApplicationSettings { get; set; }

        public CatsController(ICatService catService, IFileService fileService, IOptions<ApplicationOptions> settings)
        {
            _catService = catService;
            _fileService = fileService;
            ApplicationSettings = settings.Value;
        }

        /// <summary>
        /// Get List of Cats 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<CatModel>))]
        public IActionResult Get()
        {
            var results = _catService.GetCats();
            return Ok(results);
        }

        /// <summary>
        /// Get Cat 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(CatModel))]
        public IActionResult Get(int id)
        {
            var result = _catService.GetCat(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Delete Cat
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _catService.GetCat(id);
            if (result == null)
                return NotFound();

            _catService.DeleteCat(id);
            _fileService.DeleteFile(result.Photo);

            return NoContent();
        }

        /// <summary>
        ///  Create Cat 
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [Consumes("application/json", "multipart/form-data")]
        public IActionResult Post([FromBody] CatCreateModel value, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            $"Bytes Exist:{value.Bytes != null}".ConsoleRed();
            $"File Exists:{file != null}".ConsoleRed();

            string fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine($"{Path.GetFullPath(ApplicationSettings.FileStorage.RequestPath)}/{fileName}");

            var catModel = new CatModel
            {
                Name = value.Name,
                Description = value.Description,
                Photo = fileName,
            };

            //_fileService.SaveFile(filePath, value.Bytes);
            // _catService.CreateCat(catModel);

            return Ok();
        }

        /// <summary>
        /// Update Cat Properties 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CatUpdateModel value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            _catService.EditCat(id, value);

            return NoContent();
        }
    }
}
