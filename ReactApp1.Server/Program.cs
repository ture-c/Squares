using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BoxesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddSingleton<BoxService>();
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader());
            });

            var app = builder.Build();

            
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            
            app.Run();
        }
    }

    // Model 
    public class Box
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int X { get; set; }
        public int Y { get; set; }
        public string Color { get; set; } = string.Empty;
        
    }

    // Service
    public class BoxService
    {
        private readonly string _filePath = "boxes.json";
        private readonly ILogger<BoxService> _logger;

        public BoxService(ILogger<BoxService> logger)
        {
            _logger = logger;

            // Create file if it doesn't exist
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, JsonSerializer.Serialize(new List<Box>()));
            }
        }

        public async Task<List<Box>> GetAllBoxesAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Box>();

                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<Box>>(json) ?? new List<Box>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading boxes from file");
                return new List<Box>();
            }
        }

        public async Task AddBoxAsync(Box box)
        {
            try
            {
                var boxes = await GetAllBoxesAsync();
                boxes.Add(box);
                var json = JsonSerializer.Serialize(boxes, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving box to file");
                throw;
            }
        }
    }

    // API Controller
    [ApiController]
    [Route("api/[controller]")]
    public class BoxesController : ControllerBase
    {
        private readonly BoxService _boxService;
        private readonly ILogger<BoxesController> _logger;

        public BoxesController(BoxService boxService, ILogger<BoxesController> logger)
        {
            _boxService = boxService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Box>>> GetBoxes()
        {
            var boxes = await _boxService.GetAllBoxesAsync();
            return Ok(boxes);
        }

        [HttpPost]
        public async Task<ActionResult<Box>> AddBox(Box box)
        {
            await _boxService.AddBoxAsync(box);
            return CreatedAtAction(nameof(GetBoxes), null, box);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllBoxes()
        {
            try
            {
               
                await System.IO.File.WriteAllTextAsync("boxes.json", "[]");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}