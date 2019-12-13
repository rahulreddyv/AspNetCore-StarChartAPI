using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            List<CelestialObject> satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == id).ToList();
            celestialObject.Satellites = satellites;
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(obj => obj.Name == name).ToList();
            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                List<CelestialObject> satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == celestialObject.Id).ToList();
                celestialObject.Satellites = satellites;
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celestialObjects)
            {
                List<CelestialObject> satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == celestialObject.Id).ToList();
                celestialObject.Satellites = satellites;
            }

            return Ok(celestialObjects);
        }
    }
}
