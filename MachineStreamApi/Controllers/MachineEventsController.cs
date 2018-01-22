using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MachineStreamApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MachineStreamApi.Controllers
{
    [Route("/api/machines")]
    [Produces("application/json")]
    public class MachineEventsController : Controller
    {
        private readonly EventRepository _eventRepository;

        public MachineEventsController(EventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        //GET latest event for each machine
        [HttpGet]
        public async Task<IEnumerable<MachineStreamEventData>> Get()
        {
            var res = await _eventRepository.GetLatestEventsAsync();
            return res;
        }

        //GET latest {count} events for a given machine
        [HttpGet("{machineId}")]
        public async Task<IActionResult> GetEventsForMachine(Guid machineId, int count = 10)
        {
            var eventsForMachine = await _eventRepository.GetLatestEventsForMachineAsync(machineId, count);
            if (!eventsForMachine.Any())
            {
                return NotFound();
            }
            var projection = eventsForMachine
                .GroupBy(x => x.Machine_Id)
                .Select(g => new 
                {
                    machineId = g.Key,
                    events = g.AsEnumerable()
                }).ToList();
            return Ok(projection);
        }
    }
}
