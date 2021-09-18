using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}",Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();
            
            if(celestialObject == null)
            {
                return NotFound();
            }

            // set the satellites property
            celestialObject.Satellites = _context.CelestialObjects.Where(i => i.OrbitedObjectId == celestialObject.Id).ToList();

            // set the Satellites props to any CO 
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(n => n.Name == name).ToList();

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var c in celestialObjects)
            {
                c.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == c.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var c in celestialObjects)
            {
                c.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == c.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { Id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingCelestialObject = _context.CelestialObjects.Where(i => i.Id == id).FirstOrDefault();

            if (existingCelestialObject == null)
            {
                return NotFound();
            }

            existingCelestialObject.Name = celestialObject.Name;
            existingCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.Update(existingCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var existingCelestialObject = _context.CelestialObjects.Where(i => i.Id == id).FirstOrDefault();

            if (existingCelestialObject == null)
            {
                return NotFound();
            }

            existingCelestialObject.Name = name;

            _context.Update(existingCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
