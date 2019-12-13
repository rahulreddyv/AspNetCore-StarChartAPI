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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute(
                "GetById",
                new
                {
                    Id = celestialObject.Id
                }, celestialObject
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]CelestialObject celestialObject)
        {
            var celestialObjectFromDb = _context.CelestialObjects.Find(id);
            if (celestialObjectFromDb == null)
            {
                return NotFound();
            }

            celestialObjectFromDb.Name = celestialObject.Name;
            celestialObjectFromDb.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialObjectFromDb.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialObjectFromDb);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObjectFromDb = _context.CelestialObjects.Find(id);
            if (celestialObjectFromDb == null)
            {
                return NotFound();
            }

            celestialObjectFromDb.Name = name;

            _context.CelestialObjects.Update(celestialObjectFromDb);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(obj => (obj.OrbitedObjectId == id || obj.Id == id)).ToList();
            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
